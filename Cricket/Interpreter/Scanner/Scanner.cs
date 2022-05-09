using System.Collections.Generic;
using Cricket.Interpreter.Error;

namespace Cricket.Interpreter.Scanner;

public class Scanner
{
    private readonly string[] _source;
    private int _index, _line;

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
            switch (current)
            {
                case '+':
                    tokens.Add(new Token(TokenType.PLUS, current.ToString(), _line));
                    break;
                case '-':
                    tokens.Add(new Token(TokenType.MINUS, current.ToString(), _line));
                    break;
                case '*':
                    tokens.Add(new Token(TokenType.ASTERISK, current.ToString(), _line));
                    break;
                case '/':
                    tokens.Add(new Token(TokenType.SLASH, current.ToString(), _line));
                    break;
                default:
                    throw (_index != 0
                        ? new UnexpectedSyntaxError(_source[_line], _line, _index)
                        : new UnexpectedSyntaxError(_source[_line - 1], _line - 1, _source[_line - 1].Length));
            }
        }

        return null;
    }

    private char Consume()
    {
        var character = _source[_line][_index];
        if (_source[_line].Length == ++_index)
        {
            _index = 0;
            _line++;
        }

        return character;
    }

    private bool EndOfFile()
    {
        return _line == _source.Length;
    }
}