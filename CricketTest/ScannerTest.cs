using System;
using System.Collections.Generic;
using Cricket.Interpreter;
using Cricket.Interpreter.Error;
using Cricket.Interpreter.Scanner;
using NUnit.Framework;

namespace CricketTest;

public class Tests
{
    [Test]
    public void MathExprTest()
    {
        var scanner = new Scanner(new[] {"2 + 2 / 1012 * 31"});
        List<Token>? tokens = null;
        try
        {
            tokens = scanner.Tokenize();
            foreach (var token in tokens)
            {
                Console.Out.WriteLine($@"{token.Line}: {token.Type} {token.Lexeme}");
            }
        }
        catch (UnexpectedSyntaxError e)
        {
            Interpreter.HandleException(e);
            Assert.Fail();
        }

        Assert.AreEqual(new List<Token>()
            {
                new(TokenType.NUMERIC, "2", 0), new(TokenType.PLUS, "+", 0), new(TokenType.NUMERIC, "2", 0),
                new(TokenType.SLASH, "/", 0), new(TokenType.NUMERIC, "1012", 0),
                new(TokenType.ASTERISK, "*", 0), new(TokenType.NUMERIC, "31", 0)
            }, tokens);
    }
}