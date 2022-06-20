using System;
using System.Globalization;
using System.IO;
using System.Threading;
using Cricket.Interpreter.Error;
using Cricket.Interpreter.Parser;

namespace Cricket.Interpreter;

public class Interpreter {
    public static bool Debug = false;
    private readonly Environment.Environment _environment;
    private readonly string _path;

    public Interpreter(string path) {
        _path = path;
        _environment = new Environment.Environment();
    }

    public void StartInterpreter() {
        HackDecimals();
        try {
            var source = File.ReadAllLines(_path);
            var scanner = new Scanner.Scanner(source);
            var tokens = scanner.Tokenize();
            var parser = new Parser.Parser(tokens);
            var statements = parser.ParseStatements();
            var resolver = new Resolver(statements);
            resolver.Resolve();
            foreach (var statement in statements) statement.Interpret(_environment);
        }
        catch (Exception e) {
            HandleException(e);
        }
    }

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
    }

    private static void HandleUnexpectedSyntaxError(UnexpectedSyntaxError error) {
        Console.Out.WriteLine(error.Message);
        Console.Out.WriteLine($@"{error.Line + 1}: present: {error.Present}, expected: {error.Expected}.");
    }

    private static void HandleUnrecognizedSyntaxError(UnrecognizedSyntaxError error) {
        Console.Out.WriteLine(error.Message);
        Console.Out.WriteLine($@"{error.Line + 1}:{error.Index}: {error.SourceCode}");
        Console.Out.WriteLine($@"{new string(' ', error.Line / 10 + error.Index / 10 + error.Index + 4)}^");
    }

    // This dirty hack fixes decimal separator in CSharp
    private static void HackDecimals() {
        var cultureInfo = new CultureInfo("en-US");
        cultureInfo.NumberFormat = NumberFormatInfo.InvariantInfo;
        Thread.CurrentThread.CurrentCulture = cultureInfo;
    }
}