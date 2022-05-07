using System.Collections.Generic;

namespace Cricket.Interpreter.Scanner;

public class Scanner
{
    private readonly string[] _source;
    private int index, line;

    public Scanner(string[] source)
    {
        _source = source;
    }

    public List<Token> Tokenize()
    {
        var tokens = new List<Token>();
        while (!EndOfFile())
        {
            var current = Consume();
        }

        return null;
    }

    private char Consume()
    {
        var character = _source[line][index];
        if (_source[line].Length == ++index)
        {
            index = 0;
            line++;
        }

        return character;
    }

    private bool EndOfFile()
    {
        return line == _source.Length;
    }
}