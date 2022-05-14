using System;
using System.IO;
using Cricket.Interpreter.Error;

namespace Cricket.Interpreter;

public class Interpreter
{
    private readonly string _path;

    public Interpreter(string path)
    {
        _path = path;
    }

    public void StartInterpreter()
    {
        try
        {
            var source = File.ReadAllLines(_path);
            var scanner = new Scanner.Scanner(source);
            scanner.Tokenize();
        }
        catch (Exception e)
        {
            HandleException(e);
        }
    }

    public static void HandleException(Exception exception)
    {
        if (exception is UnexpectedSyntaxError error)
        {
            HandleUnexpectedSyntaxError(error);
        }
        else
        {
            Console.Out.WriteLine(exception.Message);
        }
    }

    private static void HandleUnexpectedSyntaxError(UnexpectedSyntaxError error)
    {
        Console.Out.WriteLine(error.Message);
        Console.Out.WriteLine($@"{error.Line + 1}:{error.Index}: {error.SourceCode}");
        Console.Out.WriteLine($@"{new string(' ', (error.Line / 10) + (error.Index / 10) + error.Index + 4)}^");
    }
}