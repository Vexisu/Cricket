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
            if (IsDigit(current))
            {
                tokens.Add(new Token(TokenType.Numeric, ConsumeNumeric(current), _line));
                continue;
            }

            switch (current)
            {
                case ' ':
                    break;
                case '+':
                    tokens.Add(new Token(TokenType.Plus, current.ToString(), _line));
                    break;
                case '-':
                    tokens.Add(new Token(TokenType.Minus, current.ToString(), _line));
                    break;
                case '*':
                    tokens.Add(new Token(TokenType.Asterisk, current.ToString(), _line));
                    break;
                case '/':
                    tokens.Add(new Token(TokenType.Slash, current.ToString(), _line));
                    break;
                default:
                    throw (_index != 0
                        ? new UnexpectedSyntaxError(_source[_line], _line, _index)
                        : new UnexpectedSyntaxError(_source[_line - 1], _line - 1, _source[_line - 1].Length));
            }
        }

        return tokens;
    }

    private char Consume()
    {
        if (_source[_line].Length == _index)
        {
            _index = 0;
            _line++;
        }
        var character = _source[_line][_index];
        _index++;
        return character;
    }

    private string ConsumeNumeric(char current)
    {
        var numeric = current.ToString();
        while (!EndOfFile())
        {
            if (!IsDigit(Peek()))
            {
                break;
            }

            numeric += Consume();
        }

        return numeric;
    }

    private static bool IsDigit(char character)
    {
        return character is >= '0' and <= '9';
    }

    private char Peek()
    {
        return _source[_line][_index];
    }

    private bool EndOfFile()
    {
        return _line == _source.Length - 1 && _index == _source[_line].Length;
    }
}