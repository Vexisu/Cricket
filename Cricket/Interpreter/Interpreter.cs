using System;
using System.IO;
using Cricket.Interpreter.Error;

namespace Cricket.Interpreter;

public class Interpreter
{
    private readonly string _path;
    public Environment.Environment Environment { get; }

    public Interpreter(string path)
    {
        _path = path;
        Environment = new Environment.Environment();
    }

    public void StartInterpreter()
    {
        try
        {
            var source = File.ReadAllLines(_path);
            var scanner = new Scanner.Scanner(source);
            var tokens = scanner.Tokenize();
            var parser = new Parser.Parser(tokens);
            var statements = parser.Parse();
            foreach (var statement in statements)
            {
                Console.Out.WriteLine(statement.Interpreter());
            }
        }
        catch (Exception e)
        {
            HandleException(e);
        }
    }

    public static void HandleException(Exception exception)
    {
        switch (exception)
        {
            case UnrecognizedSyntaxError error:
                HandleUnrecognizedSyntaxError(error);
                break;
            case UnexpectedSyntaxError error:
                HandleUnexpectedSyntaxError(error);
                break;
            default:
                Console.Out.WriteLine(exception.Message);
                break;
        }
    }

    private static void HandleUnexpectedSyntaxError(UnexpectedSyntaxError error)
    {
        Console.Out.WriteLine(error.Message);
        Console.Out.Write($@"{error.Line + 1}: present: {error.Present}, expected {error.Expected}");
    }

    private static void HandleUnrecognizedSyntaxError(UnrecognizedSyntaxError error)
    {
        Console.Out.WriteLine(error.Message);
        Console.Out.WriteLine($@"{error.Line + 1}:{error.Index}: {error.SourceCode}");
        Console.Out.WriteLine($@"{new string(' ', error.Line / 10 + error.Index / 10 + error.Index + 4)}^");
    }
}