using System;
using Cricket.Interpreter;
using Cricket.Interpreter.Parser;
using Cricket.Interpreter.Scanner;
using NUnit.Framework;
using Environment = Cricket.Interpreter.Environment.Environment;

namespace CricketTest;

public class ParserTest {
    [Test]
    public void TestValue() {
        try {
            Environment environment = new();
            var scanner = new Scanner(new[] {"var x = 5 + 22; var y = 12 + x; x + y;"});
            var parser = new Parser(scanner.Tokenize());
            var statements = parser.Parse();
            foreach (var statement in statements) Console.Out.WriteLine(statement.Interpreter(environment));
        }
        catch (Exception e) {
            Interpreter.HandleException(e);
        }
    }
}