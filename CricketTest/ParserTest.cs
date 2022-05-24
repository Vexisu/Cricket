using System;
using Cricket.Interpreter.Parser;
using Cricket.Interpreter.Scanner;
using NUnit.Framework;

namespace CricketTest; 

public class ParserTest {
    [Test]
    public void TestValue() {
        var scanner = new Scanner(new []{"4*82+18-2/2;"});
        var parser = new Parser(scanner.Tokenize());
        var statements = parser.Parse();
        var output = statements[0].Interpreter();
        Console.Out.WriteLine(output);
    }
}