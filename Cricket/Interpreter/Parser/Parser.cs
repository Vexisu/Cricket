using System;
using System.Collections.Generic;
using System.Linq;
using Cricket.Interpreter.Error;
using Cricket.Interpreter.Parser.Statement;
using Cricket.Interpreter.Parser.Statement.Expression;
using Cricket.Interpreter.Scanner;

namespace Cricket.Interpreter.Parser;

public class Parser {
    private readonly List<Token> _tokens;
    private int _index;

    public Parser(List<Token> tokens) {
        _tokens = tokens;
    }

    public List<IStatement> ParseStatements(bool inner = false) {
        var statements = new List<IStatement>();
        while (!EndOfFile()) {
            if (inner && Peek().Type == TokenType.RightBrace) {
                break;
            }
            switch (Peek().Type) {
                case TokenType.Identifier:
                    statements.Add(ParseIdentifierPrecededStatement());
                    break;
                case TokenType.Func:
                    statements.Add(ParseFunctionStatement());
                    break;
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

    /* Statement identification helper */

    private IStatement ParseIdentifierPrecededStatement() {
        var identifier = Consume();
        switch (Peek().Type) {
            case TokenType.Equal:
                return ParseAssignmentStatement(identifier);
            default:
                var current = Consume();
                throw new UnexpectedSyntaxError(current.Line, current.Lexeme, "operand");
        }
    }

    /* Statements */

    private IStatement ParsePrintStatement() {
        Consume();
        return new PrintStatement(ParseExpression());
    }

    private IStatement ParseIfStatement() {
        Consume();
        Expect(TokenType.LeftParenthesis, "(");
        Consume();
        var condition = ParseExpression();
        Expect(TokenType.RightParenthesis, ")");
        Consume();
        Expect(TokenType.LeftBrace, "{");
        Consume();
        var statements = ParseStatements(true);
        Expect(TokenType.RightBrace, "}");
        Consume();
        return new IfStatement(condition, statements);
    }

    private IStatement ParseVariableStatement() {
        Consume();
        Expect(TokenType.Less, "<");
        Consume();
        if (CheckIfDataType(Peek())) {
            throw new UnexpectedSyntaxError(Peek().Line, Peek().Lexeme, "data type");
        }
        var dataType = Enum.Parse<DataType>(Consume().Lexeme);
        Expect(TokenType.Greater, ">");
        Consume();
        Expect(TokenType.Identifier, "variable name");
        var variableName = Consume().Lexeme;
        Expect(TokenType.Equal, "=");
        return new VariableStatement(variableName, dataType, ParseExpression());
    }

    private IStatement ParseAssignmentStatement(Token identifier) {
        Consume();
        var statement = new AssignmentStatement(ParseExpression(), identifier.Lexeme);
        ExpectSemicolon();
        return statement;
    }

    private IStatement ParseFunctionStatement() {
        throw new NotImplementedException("Parse function statement");
    }

    /* Expressions */

    private IExpression ParseExpression() {
        return Match(Peek(), TokenType.Integer, TokenType.Float, TokenType.True, TokenType.False, TokenType.Identifier,
            TokenType.LeftParenthesis, TokenType.Minus)
            ? ParseComparison()
            : null;
    }

    private IExpression ParseComparison() {
        var expression = ParseTerm();
        while (Match(Peek(), TokenType.EqualEqual, TokenType.Greater, TokenType.Less, TokenType.GreaterEqual,
                   TokenType.LessEqual)) {
            var tokenType = Consume().Type;
            switch (tokenType) {
                case TokenType.EqualEqual:
                    expression = new BinaryExpression(expression, BinaryExpression.ExpressionType.Equal, ParseTerm());
                    break;
                case TokenType.Greater:
                    expression = new BinaryExpression(expression, BinaryExpression.ExpressionType.Greater, ParseTerm());
                    break;
                case TokenType.Less:
                    expression = new BinaryExpression(expression, BinaryExpression.ExpressionType.Less, ParseTerm());
                    break;
                case TokenType.GreaterEqual:
                    expression =
                        new BinaryExpression(expression, BinaryExpression.ExpressionType.GreaterEqual, ParseTerm());
                    break;
                case TokenType.LessEqual:
                    expression = new BinaryExpression(expression, BinaryExpression.ExpressionType.LessEqual,
                        ParseTerm());
                    break;
            }
        }
        return expression;
    }

    private IExpression ParseTerm() {
        var expression = ParseFactor();
        while (PeekMatch(TokenType.Plus, TokenType.Minus)) {
            var tokenType = Consume().Type;
            switch (tokenType) {
                case TokenType.Plus:
                    expression = new BinaryExpression(expression, BinaryExpression.ExpressionType.Addition,
                        ParseFactor());
                    break;
                case TokenType.Minus:
                    expression = new BinaryExpression(expression, BinaryExpression.ExpressionType.Subtraction,
                        ParseFactor());
                    break;
            }
        }
        return expression;
    }

    private IExpression ParseFactor() {
        var expression = ParseParenthesis();
        while (PeekMatch(TokenType.Asterisk, TokenType.Slash)) {
            var tokenType = Consume().Type;
            switch (tokenType) {
                case TokenType.Asterisk:
                    expression = new BinaryExpression(expression, BinaryExpression.ExpressionType.Multiplication,
                        ParseParenthesis());
                    break;
                case TokenType.Slash:
                    expression = new BinaryExpression(expression, BinaryExpression.ExpressionType.Division,
                        ParseParenthesis());
                    break;
            }
        }
        return expression;
    }

    private IExpression ParseParenthesis() {
        if (!PeekMatch(TokenType.LeftParenthesis, TokenType.RightParenthesis)) return ParseUnary();
        Expect(TokenType.LeftParenthesis, "(");
        Consume();
        var expression = ParseTerm();
        Expect(TokenType.RightParenthesis, ")");
        Consume();
        return expression;
    }

    private IExpression ParseUnary() {
        if (PeekMatch(TokenType.Minus)) {
            Consume();
            return new NegationExpression(ParseParenthesis());
        }
        return ParseValue();
    }

    private IExpression ParseValue() {
        switch (Peek().Type) {
            case TokenType.Integer:
                return new ValueExpression(int.Parse(Consume().Lexeme), DataType.Integer);
            case TokenType.Float:
                return new ValueExpression(float.Parse(Consume().Lexeme), DataType.Float);
            case TokenType.Identifier:
                return ParseVariableIdentifier();
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
                throw new UnexpectedSyntaxError(current.Line, current.Lexeme, "value");
        }
    }

    private IExpression ParseVariableIdentifier() {
        var current = Consume();
        return new VariableExpression(current.Lexeme);
    }

    /* Helpers */

    private bool CheckIfDataType(Token token) {
        return token.Type != TokenType.Identifier || !Enum.IsDefined(typeof(DataType), token.Lexeme);
    }

    private void ExpectSemicolon() {
        if (Peek() == null) {
            throw new UnexpectedSyntaxError(_tokens[^1].Line, _tokens[^1].Lexeme, ";");
        }
        if (!Match(Peek(), TokenType.Semicolon)) {
            throw new UnexpectedSyntaxError(Peek().Line, Peek().Lexeme, ";");
        }
        Consume();
    }

    private void Expect(TokenType type, string lexeme) {
        if (Peek().Type != type) {
            throw new UnexpectedSyntaxError(Peek().Line, Peek().Lexeme, lexeme);
        }
    }

    private bool PeekMatch(params TokenType[] tokenTypes) {
        return Peek() != null && tokenTypes.Any(tokenType => Peek().Type == tokenType);
    }

    private static bool Match(Token token, params TokenType[] tokenTypes) {
        return token != null && tokenTypes.Any(tokenType => token.Type == tokenType);
    }

    private Token Peek(int i = 1) {
        i--;
        if (_index + i >= _tokens.Count) {
            throw new UnexpectedEndOfFileError();
        }
        return _tokens[_index + i];
    }

    private Token Previous(int i = 1) {
        return _index - i > 0 ? _tokens[_index - i] : null;
    }

    private Token Consume() {
        if (EndOfFile()) {
            throw new UnexpectedEndOfFileError();
        }
        return _tokens[_index++];
    }

    private bool EndOfFile() {
        return _index >= _tokens.Count;
    }
}