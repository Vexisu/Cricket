using System.Collections.Generic;
using System.Text;
using Cricket.Interpreter.Error;

namespace Cricket.Interpreter.Scanner;

public class Scanner {
    private readonly string[] _source;
    private readonly List<Token> _tokens;
    private int _index, _line;

    /**
     * Konstruktor klasy Scanner.
     * <param name="source">Kod źródłowy</param>
     */
    public Scanner(string[] source) {
        _source = source;
        _tokens = new List<Token>();
    }

    /**
     * Metoda rozpoczynający proces tokenizacji.
     */
    public List<Token> Tokenize() {
        while (!EndOfFile()) {
            var current = Consume();
            if (char.IsDigit(current)) {
                ConsumeNumeric(current);
                continue;
            }

            if (char.IsLetter(current)) {
                var consumeIdentifier = ConsumeIdentifier(current);
                switch (consumeIdentifier) {
                    case "var":
                        NewToken(TokenType.Var, consumeIdentifier);
                        break;
                    case "if":
                        NewToken(TokenType.If, consumeIdentifier);
                        break;
                    case "print":
                        NewToken(TokenType.Print, consumeIdentifier);
                        break;
                    case "true":
                        NewToken(TokenType.True, consumeIdentifier);
                        break;
                    case "false":
                        NewToken(TokenType.False, consumeIdentifier);
                        break;
                    case "func":
                        NewToken(TokenType.Func, consumeIdentifier);
                        break;
                    case "return":
                        NewToken(TokenType.Return, consumeIdentifier);
                        break;
                    case "while":
                        NewToken(TokenType.While, consumeIdentifier);
                        break;
                    default:
                        NewToken(TokenType.Identifier, consumeIdentifier);
                        break;
                }
                continue;
            }
            switch (current) {
                case ' ':
                    break;
                case '\u0009':
                    break;
                case ',':
                    NewToken(TokenType.Comma, current.ToString());
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
                case '"':
                    ConsumeString();
                    break;
                default:
                    throw _index != 0
                        ? new UnrecognizedSyntaxError(_source[_line], _line, _index)
                        : new UnrecognizedSyntaxError(_source[_line - 1], _line - 1, _source[_line - 1].Length);
            }
        }
        return _tokens;
    }

    /**
     * Metoda konsumująca znaki kodu źródłowego.
     * <returns>Skonsumowany znak</returns>
     */
    private char Consume() {
        if (_index >= _source[_line].Length) {
            _index = 0;
            _line++;
        }
        var character = _source[_line].Length > 0 ? _source[_line][_index] : ' ';
        _index++;
        return character;
    }

    /**
     * Metoda konsumująca ciąg numeryczny.
     * <param name="current">Skonsumowany poprzednio znak</param> 
     */
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

    /**
     * Metoda konsumująca ciąg znaków rozpoznany jako typ String.
     */
    private void ConsumeString() {
        var stringBuilder = new StringBuilder();
        while (!EndOfFile()) {
            if (Peek() == '"') {
                Consume();
                break;
            }
            stringBuilder.Append(Consume());
        }
        NewToken(TokenType.String, stringBuilder.ToString());
    }
    
    /**
     * Metoda konsumująca ciąg znaków rozpoznany jako identyfikator.
     * <param name="current">Skonsumowany poprzednio znak</param>
     */
    private string ConsumeIdentifier(char current) {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(current);
        while (!EndOfFile()) {
            if (!char.IsLetterOrDigit(Peek())) break;
            stringBuilder.Append(Consume());
        }
        return stringBuilder.ToString();
    }

    /**
     * Metoda tworząca nowy token.
     * <param name="tokenType">Typ tokenu</param>
     * <param name="lexeme">Skonsumowany ciąg identyfikujący</param>
     */
    private void NewToken(TokenType tokenType, string lexeme) {
        _tokens.Add(new Token(tokenType, lexeme, _line));
    }

    /**
     * Metoda zaglądająca w przód.
     * <returns>Kolejny znak</returns>
     */
    private char Peek() {
        return _index < _source[_line].Length ? _source[_line][_index] :
            _source[_line + 1].Length > 0 ? _source[_line + 1][0] : ' ';
    }

    /**
     * Metoda zwracająca stan końca pliku.
     * <returns>Koniec pliku</returns>
     */
    private bool EndOfFile() {
        return _line == _source.Length - 1 && _index >= _source[_line].Length;
    }
}