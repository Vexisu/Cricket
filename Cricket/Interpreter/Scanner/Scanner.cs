using System.Collections.Generic;
using System.Text;
using Cricket.Interpreter.Error;

namespace Cricket.Interpreter.Scanner;

public class Scanner {
    private readonly string[] _source;
    private readonly List<Token> _tokens;
    private int _index, _line;

    public Scanner(string[] source) {
        _source = source;
        _tokens = new List<Token>();
    }

    // TODO: Add support for multi-character syntax
    public List<Token> Tokenize() {
        while (!EndOfFile()) {
            var current = Consume();
            if (char.IsDigit(current)) {
                ConsumeNumeric(current);
                continue;
            }

            if (char.IsLetter(current)) {
                var consumedString = ConsumeString(current);
                switch (consumedString) {
                    case "var":
                        NewToken(TokenType.Var, consumedString);
                        break;
                    case "if":
                        NewToken(TokenType.If, consumedString);
                        break;
                    case "print":
                        NewToken(TokenType.Print, consumedString);
                        break;
                    case "true":
                        NewToken(TokenType.True, consumedString);
                        break;
                    case "false":
                        NewToken(TokenType.False, consumedString);
                        break;
                    default:
                        NewToken(TokenType.Identifier, consumedString);
                        break;
                }
                continue;
            }
            switch (current) {
                case ' ':
                    break;
                case '\u0009':
                    break;
                case '+':
                    NewToken(TokenType.Plus, current.ToString());
                    break;
                case '-':
                    NewToken(TokenType.Minus, current.ToString());
                    break;
                case '*':
                    NewToken(TokenType.Asterisk, current.ToString());
                    break;
                case '/':
                    NewToken(TokenType.Slash, current.ToString());
                    break;
                case ';':
                    NewToken(TokenType.Semicolon, current.ToString());
                    break;
                case '(':
                    NewToken(TokenType.LeftParenthesis, current.ToString());
                    break;
                case ')':
                    NewToken(TokenType.RightParenthesis, current.ToString());
                    break;
                case '=':
                    if (Peek() == '=') {
                        NewToken(TokenType.EqualEqual, new string(new[] {current, Consume()}));
                    }
                    else {
                        NewToken(TokenType.Equal, current.ToString());
                    }
                    break;
                case '<':
                    NewToken(TokenType.Less, current.ToString());
                    break;
                case '>':
                    NewToken(TokenType.Greater, current.ToString());
                    break;
                case '{':
                    NewToken(TokenType.LeftBrace, current.ToString());
                    break;
                case '}':
                    NewToken(TokenType.RightBrace, current.ToString());
                    break;
                default:
                    throw _index != 0
                        ? new UnrecognizedSyntaxError(_source[_line], _line, _index)
                        : new UnrecognizedSyntaxError(_source[_line - 1], _line - 1, _source[_line - 1].Length);
            }
        }
        return _tokens;
    }

    private char Consume() {
        if (_source[_line].Length == _index) {
            _index = 0;
            _line++;
        }
        var character = _source[_line][_index];
        _index++;
        return character;
    }

    private void ConsumeNumeric(char current) {
        var tokenType = TokenType.Integer;
        var numeric = current.ToString();
        while (!EndOfFile()) {
            if (!(char.IsDigit(Peek()) || Peek() == '.')) break;
            if (Peek() == '.') {
                if (tokenType == TokenType.Integer) {
                    tokenType = TokenType.Float;
                }
                else if (tokenType == TokenType.Float) break;
            }
            numeric += Consume();
        }
        NewToken(tokenType, numeric);
    }

    private string ConsumeString(char current) {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(current);
        while (!EndOfFile()) {
            if (!char.IsLetterOrDigit(Peek())) break;
            stringBuilder.Append(Consume());
        }
        return stringBuilder.ToString();
    }

    private void NewToken(TokenType tokenType, string lexeme) {
        _tokens.Add(new Token(tokenType, lexeme, _line));
    }

    private char Peek() {
        return _index < _source[_line].Length ? _source[_line][_index] : _source[_line + 1][0];
    }

    private bool EndOfFile() {
        return _line == _source.Length - 1 && _index == _source[_line].Length;
    }
}