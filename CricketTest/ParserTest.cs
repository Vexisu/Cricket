using System;
using System.Globalization;
using System.Threading;
using Cricket.Interpreter;
using Cricket.Interpreter.Parser;
using Cricket.Interpreter.Scanner;
using NUnit.Framework;
using Environment = Cricket.Interpreter.Environment.Environment;

namespace CricketTest;

public class ParserTest {
    [SetUp]
    public void SetCultureInfo() {
        var cultureInfo = new CultureInfo("en-US");
        cultureInfo.NumberFormat = NumberFormatInfo.InvariantInfo;
        Thread.CurrentThread.CurrentCulture = cultureInfo;
    }
    
    [Test]
    public void TestValue() {
        try {
            Environment environment = new();
            var scanner = new Scanner(new[] {
                "var<Integer> x = 2 + 18;",
                "var<Integer> y = (12 + x) * 2;",
                "print x;",
                "if (x == 20) { print y; }"
            });
            var tokens = scanner.Tokenize();
            var parser = new Parser(tokens);
            var statements = parser.ParseStatements();
            foreach (var statement in statements) {
                statement.Interpreter(environment);
            }
        }
        catch (Exception e) {
            Interpreter.HandleException(e);
            Console.Out.WriteLine(e);
        }
    }
}