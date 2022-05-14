using System;

namespace Cricket.Interpreter.Scanner;

public class Token
{
    public Token(TokenType type, string lexeme, int line)
    {
        Type = type;
        Lexeme = lexeme;
        Line = line;
    }

    public TokenType Type { get; }
    public string Lexeme { get; }
    public int Line { get; }

    public override bool Equals(object obj)
    {
        if (obj is not Token other) return false;
        return Lexeme == other.Lexeme && Type == other.Type && Line == other.Line;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int) Type, Lexeme, Line);
    }
}