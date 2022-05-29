namespace Cricket.Interpreter.Scanner;

public enum TokenType {
    // One character tokens
    Plus,
    Minus,
    Asterisk,
    Slash,
    LeftBrace,
    RightBrace,
    LeftParenthesis,
    RightParenthesis,
    Semicolon,
    Dot,
    Comma,
    Exclamation,
    Equal,
    Less,
    Greater,

    // Two character tokens
    LessEqual,
    GreaterEqual,
    ExclamationEqual,
    EqualEqual,

    // Literals
    Identifier,
    String,
    Integer,
    Float,

    // Keywords
    Import,
    Func,
    Var,
    If,
    While
}