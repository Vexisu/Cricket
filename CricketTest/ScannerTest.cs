using System;
using System.Collections.Generic;
using Cricket.Interpreter;
using Cricket.Interpreter.Error;
using Cricket.Interpreter.Scanner;
using NUnit.Framework;

namespace CricketTest;

public class Tests {
    [Test]
    public void MathExprTest() {
        var scanner = new Scanner(new[] {"2 + 2 / 1012 * 31"});
        List<Token>? tokens = null;
        try {
            tokens = scanner.Tokenize();
            foreach (var token in tokens) Console.Out.WriteLine($@"{token.Line}: {token.Type} {token.Lexeme}");
        }
        catch (UnrecognizedSyntaxError e) {
            Interpreter.HandleException(e);
            Assert.Fail();
        }

        Assert.AreEqual(new List<Token> {
            new(TokenType.Numeric, "2", 0), new(TokenType.Plus, "+", 0), new(TokenType.Numeric, "2", 0),
            new(TokenType.Slash, "/", 0), new(TokenType.Numeric, "1012", 0),
            new(TokenType.Asterisk, "*", 0), new(TokenType.Numeric, "31", 0)
        }, tokens);
    }
}