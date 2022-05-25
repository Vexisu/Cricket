using System;
using System.Collections.Generic;
using System.Linq;
using Cricket.Interpreter.Error;
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
        while (!EndOfFile()) {
            statements.Add(ParseExpression());
        }
        return statements;
    }

    private IExpression ParseExpression() {
        if (Match(Peek(), TokenType.Numeric, TokenType.LeftParenthesis)) {
            var expression = ParseTerm();
            if (Peek() != null && !Match(Peek(), TokenType.Semicolon)) {
                throw new UnexpectedSyntaxError(Peek().Line, Peek().Lexeme, ";");
            }
            if (Peek() == null) {
                throw new UnexpectedSyntaxError(_tokens[^1].Line, _tokens[^1].Lexeme, ";");
            }
            Consume();
            return expression;
        }
        return null;
    }

    private IExpression ParseTerm() {
        var expression = ParseFactor();
        while (Match(Peek(), TokenType.Plus, TokenType.Minus)) {
            switch (Peek().Type) {
                case TokenType.Plus:
                    Consume();
                    expression = new BinaryExpression(expression, BinaryExpression.ExpressionType.Addition,
                        ParseFactor());
                    break;
                case TokenType.Minus:
                    Consume();
                    expression = new BinaryExpression(expression, BinaryExpression.ExpressionType.Subtraction,
                        ParseFactor());
                    break;
            }
        }
        return expression;
    }

    private IExpression ParseFactor() {
        var expression = ParseParenthesis();
        while (Match(Peek(), TokenType.Asterisk, TokenType.Slash)) {
            switch (Peek().Type) {
                case TokenType.Asterisk:
                    Consume();
                    expression = new BinaryExpression(expression, BinaryExpression.ExpressionType.Multiplication,
                        ParseParenthesis());
                    break;
                case TokenType.Slash:
                    Consume();
                    expression = new BinaryExpression(expression, BinaryExpression.ExpressionType.Division,
                        ParseParenthesis());
                    break;
            }
        }
        return expression;
    }

    private IExpression ParseParenthesis() {
        if (!Match(Peek(), TokenType.LeftParenthesis, TokenType.RightParenthesis)) {
            return ParseValue();
        }
        if (!Match(Peek(), TokenType.LeftParenthesis)) {
            throw new UnexpectedSyntaxError(Peek().Line, Peek().Lexeme, "(");
        }
        Consume();
        var expression = ParseTerm();
        if (!Match(Peek(), TokenType.RightParenthesis)) {
            throw new UnexpectedSyntaxError(Peek().Line, Peek().Lexeme, ")");
        }
        Consume();
        return expression;
    }

    private IExpression ParseValue() {
        var current = Consume();
        return new ValueExpression(Int32.Parse(current.Lexeme), ValueExpression.ValueType.Numeric);
    }

    private bool Match(Token token, params TokenType[] tokenTypes) {
        return token != null && tokenTypes.Any(tokenType => token.Type == tokenType);
    }

    private Token Peek(int i = 1) {
        i--;
        return _index + i < _tokens.Count ? _tokens[_index + i] : null;
    }

    private Token Previous(int i = 1) {
        return _index - i > 0 ? _tokens[_index - i] : null;
    }

    private Token Consume() {
        return !EndOfFile() ? _tokens[_index++] : null;
    }

    private bool EndOfFile() {
        return _index >= _tokens.Count;
    }
}