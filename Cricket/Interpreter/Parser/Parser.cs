using System.Collections.Generic;
using Cricket.Interpreter.Parser.Statement;
using Cricket.Interpreter.Parser.Statement.Expression;
using Cricket.Interpreter.Scanner;

namespace Cricket.Interpreter.Parser;

public class Parser {
    private int _index;
    private readonly List<Token> _tokens;

    public Parser(List<Token> tokens) {
        _tokens = tokens;
    }

    public List<IStatement> Parse() {
        var statements = new List<IStatement>();
        while (!EndOfFile()) { }
        return statements;
    }

    private IExpression ParseExpression(List<Token> current) {
        return null;
    }

    private bool Match(Token token, params TokenType[] tokenTypes) {
        foreach (var tokenType in tokenTypes)
            if (token.Type == tokenType)
                return true;
        return false;
    }

    private Token Peek(int i) {
        return _index + i < _tokens.Count ? _tokens[_index + i] : null;
    }

    private Token Previous(int i) {
        return _index - i > 0 ? _tokens[_index - i] : null;
    }

    private Token Consume() {
        return EndOfFile() ? _tokens[_index++] : null;
    }

    private bool EndOfFile() {
        return _index < _tokens.Count;
    }
}