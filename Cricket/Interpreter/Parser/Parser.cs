using System;
using System.Collections.Generic;
using System.Linq;
using Cricket.Interpreter.Error;
using Cricket.Interpreter.Parser.Statement;
using Cricket.Interpreter.Parser.Statement.Expression;
using Cricket.Interpreter.Scanner;

namespace Cricket.Interpreter.Parser;

/**
 * Klasa parsera.
 */
public class Parser {
    private readonly List<Token> _tokens;
    private int _index;

    /**
     * Konstruktor klasy parsera.
     * <param name="tokens">Lista tokenów</param>
     */
    public Parser(List<Token> tokens) {
        _tokens = tokens;
    }

    /**
     * Metoda parsująca deklaracje.
     * <param name="inner">Czy deklaracje są zawarte w deklaracji nadrzędnej</param>
     * <returns>Lista deklaracji</returns>
     */
    public List<IStatement> ParseStatements(bool inner = false) {
        var statements = new List<IStatement>();
        while (!EndOfFile()) {
            if (inner && Peek().Type == TokenType.RightBrace) {
                break;
            }
            switch (Peek().Type) {
                case TokenType.Identifier:
                    statements.Add(ParseIdentifierPrecededStatement());
                    ExpectSemicolon();
                    break;
                case TokenType.Func:
                    if (inner) {
                        throw new DisallowedSyntaxError(
                            $"{Peek().Line}: Function definition is not allowed in this scope.");
                    }
                    statements.Add(ParseFunctionStatement());
                    break;
                case TokenType.Var:
                    statements.Add(ParseVariableStatement());
                    ExpectSemicolon();
                    break;
                case TokenType.If:
                    statements.Add(ParseIfStatement());
                    break;
                case TokenType.Return:
                    if (!inner) {
                        throw new DisallowedSyntaxError(
                            $"{Peek().Line}: Return statement is not allowed outside a function.");
                    }
                    statements.Add(ParseReturnStatement());
                    ExpectSemicolon();
                    break;
                case TokenType.Print:
                    statements.Add(ParsePrintStatement());
                    ExpectSemicolon();
                    break;
                case TokenType.While:
                    statements.Add(ParseWhileStatement());
                    break;
                default:
                    throw new UnexpectedSyntaxError(Peek().Line, Peek().Lexeme, "statement");
            }
        }
        return statements;
    }

    /* Statement identification helper */
    /**
     * Metoda parsująca deklarację poprzedzoną identyfikatorem.
     * <returns>Deklaracja</returns>
     */
    private IStatement ParseIdentifierPrecededStatement() {
        Log("IdentifierPreceded");
        var identifier = Consume();
        switch (Peek().Type) {
            case TokenType.Equal:
                return ParseAssignmentStatement(identifier.Lexeme);
            case TokenType.LeftParenthesis:
                return ParseFunctionCall(identifier.Lexeme);
            default:
                var current = Consume();
                throw new UnexpectedSyntaxError(current.Line, current.Lexeme, "operand");
        }
    }

    /* Statements */
    /**
     * Metoda parsująca deklarację funkcji drukującej.
     * <returns>Deklaracja</returns>
     */
    private IStatement ParsePrintStatement() {
        Log("PrintStatement");
        Consume();
        return new PrintStatement(ParseExpression());
    }

    /**
     * Metoda parsująca deklarację funkcji warunkowej.
     * <returns>Deklaracja</returns>
     */
    private IStatement ParseIfStatement() {
        Log("IfStatement");
        Consume();
        ExpectAndConsume(TokenType.LeftParenthesis, "(");
        var condition = ParseExpression();
        ExpectAndConsume(TokenType.RightParenthesis, ")");
        ExpectAndConsume(TokenType.LeftBrace, "{");
        var statements = ParseStatements(true);
        ExpectAndConsume(TokenType.RightBrace, "}");
        return new IfStatement(condition, statements);
    }

    /**
     * Metoda parsująca deklarację zmiennej.
     * <returns>Deklaracja</returns>
     */
    private IStatement ParseVariableStatement() {
        Log("VariableStatement");
        Consume();
        ExpectAndConsume(TokenType.Less, "<");
        var dataType = ExpectDataTypeAndReturn();
        ExpectAndConsume(TokenType.Greater, ">");
        Expect(TokenType.Identifier, "variable name");
        var variableName = Consume().Lexeme;
        ExpectAndConsume(TokenType.Equal, "=");
        return new VariableStatement(variableName, dataType, ParseExpression());
    }

    /**
     * Metoda parsująca deklarację przypisania do zmiennej.
     * <returns>Deklaracja</returns>
     */
    private IStatement ParseAssignmentStatement(string identifier) {
        Log("AssignmentStatement");
        Consume();
        return new AssignmentStatement(ParseExpression(), identifier);
    }

    /**
     * Metoda parsująca deklarację funkcji.
     * <returns>Deklaracjaa</returns>
     */
    private IStatement ParseFunctionStatement() {
        Log("FunctionStatement");
        Consume();
        var returnedType = DataType.Null;
        if (PeekMatch(TokenType.Less)) {
            Consume();
            returnedType = ExpectDataTypeAndReturn();
            ExpectAndConsume(TokenType.Greater, ">");
        }
        Expect(TokenType.Identifier, "function name");
        var functionName = Consume().Lexeme;
        ExpectAndConsume(TokenType.LeftParenthesis, "(");
        var arguments = new List<FunctionStatement.FunctionArgument>();
        if (PeekMatch(TokenType.Identifier)) {
            do {
                var argumentType = ExpectDataTypeAndReturn();
                Expect(TokenType.Identifier, "argument name");
                var argumentName = Consume().Lexeme;
                arguments.Add(new FunctionStatement.FunctionArgument(argumentName, argumentType));
            } while (PeekMatchAndConsume(TokenType.Comma));
        }
        ExpectAndConsume(TokenType.RightParenthesis, ")");
        ExpectAndConsume(TokenType.LeftBrace, "{");
        var statements = ParseStatements(true);
        ExpectAndConsume(TokenType.RightBrace, "}");
        return new FunctionStatement(functionName, arguments, statements, returnedType);
    }

    /**
     * Metoda parsująca deklarację pętli while.
     * <returns>Deklaracja</returns>
     */
    private IStatement ParseWhileStatement() {
        Consume();
        ExpectAndConsume(TokenType.LeftParenthesis, ("("));
        var condition = ParseExpression();
        ExpectAndConsume(TokenType.RightParenthesis, ")");
        ExpectAndConsume(TokenType.LeftBrace, "{");
        var statements = ParseStatements(true);
        ExpectAndConsume(TokenType.RightBrace, "}");
        return new WhileStatement(condition, statements);
    }

    /**
     * Metoda parsująca deklarację zwrócenia wartości.
     * <returns>Deklaracja</returns>
     */
    private IStatement ParseReturnStatement() {
        Log("ReturnStatement");
        Consume();
        return new ReturnStatement(ParseExpression());
    }

    /* Expressions */

    /**
     * Metoda rozpoczynająca parsowanie wyrażenia.
     * <returns>Wyrażenie</returns>
     */
    private IExpression ParseExpression() {
        Log("Expression");
        return Match(Peek(), TokenType.Integer, TokenType.Float, TokenType.True, TokenType.False, TokenType.Identifier,
            TokenType.LeftParenthesis, TokenType.Minus, TokenType.String)
            ? ParseComparison()
            : null;
    }

    /**
     * Metoda parsująca wyrażenie porównania.
     * <returns>Wyrażenie</returns>
     */
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

    /**
     * Metoda parsująca wyrażenie sumy/różnicy.
     * <returns>Wyrażenie</returns>
     */
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

    /**
     * Metoda parsująca wyrażenie iloczynu/ilorazu.
     * <returns>Wyrażenie</returns>
     */
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

    /**
     * Metoda parsująca wyrażenie nawiasów priorytetu.
     * <returns>Wyrażenie</returns>
     */
    private IExpression ParseParenthesis() {
        if (!PeekMatch(TokenType.LeftParenthesis, TokenType.RightParenthesis)) return ParseUnary();
        ExpectAndConsume(TokenType.LeftParenthesis, "(");
        var expression = ParseTerm();
        ExpectAndConsume(TokenType.RightParenthesis, ")");
        return expression;
    }

    /**
     * Metoda parsująca wyrażenie pojedyncze.
     * <returns>Wyrażenie</returns>
     */
    private IExpression ParseUnary() {
        if (PeekMatch(TokenType.Minus)) {
            Consume();
            return new NegationExpression(ParseParenthesis());
        }
        return ParseValue();
    }

    /**
     * Metoda parsująca wyrażenie wartości.
     * <returns>Wyrażenie</returns>
     */
    private IExpression ParseValue() {
        switch (Peek().Type) {
            case TokenType.Integer:
                return new ValueExpression(int.Parse(Consume().Lexeme), DataType.Integer);
            case TokenType.Float:
                return new ValueExpression(float.Parse(Consume().Lexeme), DataType.Float);
            case TokenType.Identifier:
                return ParseIdentifierPrecededExpression();
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

    /**
     * Metoda parsująca wyrażenie poprzedzone identyfikatorem.
     * <returns>Wyrażenie</returns>
     */
    private IExpression ParseIdentifierPrecededExpression() {
        if (Match(Peek(2), TokenType.LeftParenthesis)) {
            return ParseFunctionCall();
        }
        return ParseVariableIdentifier();
    }

    /**
     * Metoda parsująca wyrażenie wywołania funkcji.
     * <param name="functionName">Nazwa funkcji</param>
     * <returns>Wyrażenie</returns>
     */
    private IExpression ParseFunctionCall(string functionName = null) {
        if (functionName == null) {
            Expect(TokenType.Identifier, "called function name");
            functionName = Consume().Lexeme;
        }
        ExpectAndConsume(TokenType.LeftParenthesis, "(");
        var arguments = new List<IExpression>();
        if (!PeekMatch(TokenType.RightParenthesis)) {
            do {
                arguments.Add(ParseExpression());
            } while (PeekMatchAndConsume(TokenType.Comma));
        }
        ExpectAndConsume(TokenType.RightParenthesis, ")");
        return new CallExpression(functionName, arguments);
    }

    private IExpression ParseVariableIdentifier() {
        return new VariableExpression(Consume().Lexeme);
    }

    /* Helpers */

    /**
     * Metoda oczekiwania identyfikatora typu danych.
     * <returns>Typ danych</returns>
     * <exception cref="UnexpectedSyntaxError">Błąd nieoczekiwanej składni</exception>
     */
    private DataType ExpectDataTypeAndReturn() {
        var token = Consume();
        if (token.Type == TokenType.Identifier && Enum.IsDefined(typeof(DataType), token.Lexeme)) {
            return Enum.Parse<DataType>(token.Lexeme);
        }
        throw new UnexpectedSyntaxError(token.Line, token.Lexeme, "data type");
    }

    /**
     * Metoda oczekująca średnik.
     * <exception cref="UnexpectedSyntaxError">Błąd nieoczekiwanej składni</exception>
     */
    private void ExpectSemicolon() {
        if (Peek() == null) {
            throw new UnexpectedSyntaxError(_tokens[^1].Line, _tokens[^1].Lexeme, ";");
        }
        if (!Match(Peek(), TokenType.Semicolon)) {
            throw new UnexpectedSyntaxError(Peek().Line, Peek().Lexeme, ";");
        }
        Consume();
    }

    /**
     * Metoda oczekująca na dany token, następnie go konsumująca.
     * <param name="type">Typ tokenu</param>
     * <param name="lexeme">Ciąg identyfikujący</param>
     * <exception cref="UnexpectedSyntaxError">Błąd nieoczekiwanej składni</exception>
     */
    private void ExpectAndConsume(TokenType type, string lexeme) {
        var token = Consume();
        if (token.Type != type) {
            throw new UnexpectedSyntaxError(token.Line, token.Lexeme, lexeme);
        }
    }

    /**
     * Metoda oczekująca dany token.
     * <param name="type">Typ tokenu</param>
     * <param name="lexeme">Ciąg identyfikujący</param>
     * <exception cref="UnexpectedSyntaxError">Błąd nieoczekiwanej składni</exception>
     */
    private void Expect(TokenType type, string lexeme) {
        if (Peek().Type != type) {
            throw new UnexpectedSyntaxError(Peek().Line, Peek().Lexeme, lexeme);
        }
    }
    
    /**
     * Metoda porównaniu do przodu, a następnie konsumująca token.
     *  <param name="tokenTypes">Lista typu tokenów</param>
     */
    private bool PeekMatchAndConsume(params TokenType[] tokenTypes) {
        if (Peek() == null || tokenTypes.All(tokenType => Peek().Type != tokenType)) return false;
        Consume();
        return true;
    }
    
    /**
     * Metoda porównująca kolejny typ tokenu do przekazanych typów tokenu.
     * <param name="tokenTypes">Lista tokenów</param>
     * <returns>Czy jeden z tokenów jest taki sam jak kolejny</returns>
     */
    private bool PeekMatch(params TokenType[] tokenTypes) {
        return Peek() != null && tokenTypes.Any(tokenType => Peek().Type == tokenType);
    }

    /**
     * Funkcja sprawdzająca, czy istnieje typ tokenu taki, jak w przekazanym tokenie.
     * <param name="token">Token porównywany</param>
     * <param name="tokenTypes">Lista typów tokenów</param>
     */
    private static bool Match(Token token, params TokenType[] tokenTypes) {
        return token != null && tokenTypes.Any(tokenType => token.Type == tokenType);
    }

    /**
     * Metoda patrząca w przód.
     * <param name="i">Ilość kroków w przód</param>
     * <returns>Kolejny token</returns>
     */
    private Token Peek(int i = 1) {
        i--;
        if (_index + i >= _tokens.Count) {
            throw new UnexpectedEndOfFileError();
        }
        return _tokens[_index + i];
    }

    /**
     * Metoda konsumujące i zwracająca kolejny token.
     * <returns>Skonsumowany token</returns>
     */
    private Token Consume() {
        if (EndOfFile()) {
            throw new UnexpectedEndOfFileError();
        }
        return _tokens[_index++];
    }

    /**
     * Metoda informująca o kończu pliku.
     * <returns>Czy koniec pliku</returns>
     */
    private bool EndOfFile() {
        return _index >= _tokens.Count;
    }

    /**
     * Metoda drukująca loggera.
     * <param name="logType">Informacja dla loggera</param>
     */
    private void Log(string logType) {
        if (!Interpreter.Debug) return;
        var current = _tokens[_index];
        Console.Out.WriteLine($"Parser: {current.Line + 1}:{logType} around {current.Lexeme} ");
    }
}