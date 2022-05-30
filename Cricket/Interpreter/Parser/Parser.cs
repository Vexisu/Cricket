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

    public List<IStatement> ParseStatements(bool inner = false) {
        var statements = new List<IStatement>();
        while (!EndOfFile()) {
            if (inner && Peek().Type == TokenType.RightBrace)
                break;
            switch (Peek().Type) {
                case TokenType.Var:
                    statements.Add(ParseVariableStatement());
                    ExpectSemicolon();
                    break;
                case TokenType.If:
                    statements.Add(ParseIfStatement());
                    break;
                case TokenType.Print:
                    statements.Add(ParsePrintStatement());
                    ExpectSemicolon();
                    break;
                default:
                    throw new UnexpectedSyntaxError(Peek().Line, Peek().Lexeme, "statement");
            }
        }
        return statements;
    }
    
    /* Statements */

    private IStatement ParsePrintStatement() {
        Consume();
        return new PrintStatement(ParseExpression());
    }

    private IStatement ParseIfStatement() {
        Consume();
        if (Peek().Type != TokenType.LeftParenthesis)
            throw new UnexpectedSyntaxError(Peek().Line, Peek().Lexeme, "(");
        Consume();
        var condition = ParseExpression();
        if (Peek().Type != TokenType.RightParenthesis)
            throw new UnexpectedSyntaxError(Peek().Line, Peek().Lexeme, ")");
        Consume();
        if (Peek().Type != TokenType.LeftBrace)
            throw new UnexpectedSyntaxError(Peek().Line, Peek().Lexeme, "{");
        Consume();
        var statements = ParseStatements(true);
        if (Peek().Type != TokenType.RightBrace)
            throw new UnexpectedSyntaxError(Peek().Line, Peek().Lexeme, "}");
        Consume();
        return new IfStatement(condition, statements);
    }

    private IStatement ParseVariableStatement() {
        Consume();
        if (Peek().Type != TokenType.Less)
            throw new UnexpectedSyntaxError(Peek().Line, Peek().Lexeme, "<");
        Consume();
        if (Peek().Type != TokenType.Identifier || !Enum.IsDefined(typeof(DataType), Peek().Lexeme))
            throw new UnexpectedSyntaxError(Peek().Line, Peek().Lexeme, "data type");
        var dataType = Enum.Parse<DataType>(Consume().Lexeme);
        if (Peek().Type != TokenType.Greater)
            throw new UnexpectedSyntaxError(Peek().Line, Peek().Lexeme, ">");
        Consume();
        if (Peek().Type != TokenType.Identifier)
            throw new UnexpectedSyntaxError(Peek().Line, Peek().Lexeme, "variable name");
        var variableName = Consume().Lexeme;
        if (Consume().Type != TokenType.Equal)
            throw new UnexpectedSyntaxError(Peek().Line, Peek().Lexeme, "=");
        return new VariableStatement(variableName, dataType, ParseExpression());
    }
    
    /* Expressions */

    private IExpression ParseExpression() {
        return Match(Peek(), TokenType.Integer, TokenType.Identifier, TokenType.LeftParenthesis, TokenType.Minus) ? ParseTerm() : null;
    }

    private IExpression ParseTerm() {
        var expression = ParseFactor();
        while (Match(Peek(), TokenType.Plus, TokenType.Minus))
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
        return expression;
    }

    private IExpression ParseFactor() {
        var expression = ParseParenthesis();
        while (Match(Peek(), TokenType.Asterisk, TokenType.Slash))
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
        return expression;
    }

    private IExpression ParseParenthesis() {
        if (!Match(Peek(), TokenType.LeftParenthesis, TokenType.RightParenthesis)) return ParseUnary();
        if (!Match(Peek(), TokenType.LeftParenthesis))
            throw new UnexpectedSyntaxError(Peek().Line, Peek().Lexeme, "(");
        Consume();
        var expression = ParseTerm();
        if (!Match(Peek(), TokenType.RightParenthesis))
            throw new UnexpectedSyntaxError(Peek().Line, Peek().Lexeme, ")");
        Consume();
        return expression;
    }

    private IExpression ParseUnary() {
        if (Peek().Type == TokenType.Minus) {
            Consume();
            return new NegationExpression(ParseParenthesis());
        }
        return ParseValue();
    }

    private IExpression ParseValue() {
        switch (Peek().Type) {
            case TokenType.Integer:
                return new ValueExpression(int.Parse(Consume().Lexeme), DataType.Integer);
            case TokenType.Identifier:
                return ParseIdentifier();
            case TokenType.String:
                return new ValueExpression(Consume().Lexeme, DataType.String);
            case TokenType.True:
                Consume();
                return new ValueExpression(true, DataType.Boolean);
            case TokenType.False:
                Consume();
                return new ValueExpression(false, DataType.Boolean);
            default:
                var current = Consume();
                throw new UnexpectedSyntaxError(current.Line, current.Lexeme, "Value");
        }
    }

    private IExpression ParseIdentifier() {
        var current = Consume();
        return new VariableExpression(current.Lexeme);
    }

    private void ExpectSemicolon() {
        if (Peek() != null && !Match(Peek(), TokenType.Semicolon))
            throw new UnexpectedSyntaxError(Peek().Line, Peek().Lexeme, ";");
        if (Peek() == null)
            throw new UnexpectedSyntaxError(_tokens[^1].Line, _tokens[^1].Lexeme, ";");
        Consume();
    }

    private static bool Match(Token token, params TokenType[] tokenTypes) {
        return token != null && tokenTypes.Any(tokenType => token.Type == tokenType);
    }

    private Token Peek(int i = 1) {
        i--;
        if (_index + i >= _tokens.Count)
            throw new UnexpectedEndOfFileError();
        return _tokens[_index + i];
    }

    private Token Previous(int i = 1) {
        return _index - i > 0 ? _tokens[_index - i] : null;
    }

    private Token Consume() {
        if (EndOfFile())
            throw new UnexpectedEndOfFileError();
        return _tokens[_index++];
    }

    private bool EndOfFile() {
        return _index >= _tokens.Count;
    }
}