using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using Cricket.Interpreter.Error;
using Cricket.Interpreter.Parser;
using Cricket.Interpreter.Parser.Statement;

namespace Cricket.Interpreter;

public class Interpreter {
    public static bool Debug = false;
    public readonly Environment.Environment Environment;
    private readonly string _path;
    private readonly List<IStatement> _externalFunctions;

    /**
     * Konstruktor klasy Interpreter.
     * <param name="path">Ścieżka pliku źródłowego</param>
     */
    public Interpreter(string path) {
        _path = path;
        _externalFunctions = new List<IStatement>();
        Environment = new Environment.Environment();
    }

    /**
     * Metoda uruchamiająca interpreter.
     */
    public void StartInterpreter() {
        HackDecimals();
        try {
            var source = File.ReadAllLines(_path);
            var scanner = new Scanner.Scanner(source);
            var tokens = scanner.Tokenize();
            var parser = new Parser.Parser(tokens);
            var statements = parser.ParseStatements();
            _externalFunctions.AddRange(statements);
            statements = _externalFunctions;
            var resolver = new Resolver(statements);
            resolver.Resolve();
            foreach (var statement in statements) statement.Interpret(Environment);
        }
        catch (Exception e) {
            HandleException(e);
        }
    }

    /**
     * Metoda API dodająca zewnętrzne funkcje do środowika interpretera.
     * <param name="statement">Obiekt funkcji</param>
     */
    public void AddExternalFunction(FunctionStatement statement) {
        _externalFunctions.Add(statement);
    }

    /**
     * Metoda API wywołująca funkcje ze środowiska intepretera.
     * <param name="name">Nazwa wywoływanej funkcji</param>
     * <returns>Wartość zwórcona przez wywołaną funkcję</returns>
     */
    public object CallFunction(string name) {
        Environment.CallFunction(name, new List<DataType>());
        var expression = Environment.PopFromStack();
        return expression.Interpret(Environment);
    }

    /**
     * Funkcja przechwytująca wyjątki.
     * <param name="exception">Przechwytywany wyjątek</param>
     */
    public static void HandleException(Exception exception) {
        switch (exception) {
            case UnrecognizedSyntaxError error:
                HandleUnrecognizedSyntaxError(error);
                break;
            case UnexpectedSyntaxError error:
                HandleUnexpectedSyntaxError(error);
                break;
            case UnexpectedEndOfFileError:
                Console.Out.WriteLine("Unexpected end of file error.");
                break;
            case DisallowedSyntaxError error:
                Console.Out.WriteLine(error.Message);
                break;
            case ResolverError error:
                Console.Out.WriteLine(error.Message);
                break;
            case InvalidReturnStatementError error:
                Console.Out.WriteLine(error.Message);
                break;
            case MissingReturnStatementError error:
                Console.Out.WriteLine(error.Message);
                break;
            default:
                Console.Out.WriteLine(exception.ToString());
                break;
        }
        System.Environment.Exit(102);
    }

    /**
     * Funkcja formatująca wyjście dla wyjątku syntaktycznego.
     * <param name="error">Wyjątek wyntaktyczny</param>
     */
    private static void HandleUnexpectedSyntaxError(UnexpectedSyntaxError error) {
        Console.Out.WriteLine(error.Message);
        Console.Out.WriteLine($@"{error.Line + 1}: present: {error.Present}, expected: {error.Expected}.");
    }
    
    /**
     * Funkcja formatująca wyjście dla wyjątku nierozpoznanej syntaktyki.
     * <param name="error">Wyjątek nierozpoznanej syntaktyki</param>
     */
    private static void HandleUnrecognizedSyntaxError(UnrecognizedSyntaxError error) {
        Console.Out.WriteLine(error.Message);
        Console.Out.WriteLine($@"{error.Line + 1}:{error.Index}: {error.SourceCode}");
        Console.Out.WriteLine($@"{new string(' ', error.Line / 10 + error.Index / 10 + error.Index + 4)}^");
    }

    // This dirty hack fixes decimal separator in CSharp
    /**
     * Funkcja naprawiająca błąd związany z wykorzystaniem przecinków w typie float w niektórych regionach świata.
     */
    private static void HackDecimals() {
        var cultureInfo = new CultureInfo("en-US");
        cultureInfo.NumberFormat = NumberFormatInfo.InvariantInfo;
        Thread.CurrentThread.CurrentCulture = cultureInfo;
    }
}