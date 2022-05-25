using System;
using Cricket.Interpreter;
using Cricket.Interpreter.Parser;
using Cricket.Interpreter.Scanner;
using NUnit.Framework;

namespace CricketTest; 

public class ParserTest {
    [Test]
    public void TestValue() {
        try {
            var scanner = new Scanner(new[] {"(5*(2+3*4));"});
            var parser = new Parser(scanner.Tokenize());
            var statements = parser.Parse();
            var output = statements[0].Interpreter();
            Console.Out.WriteLine(output);
        }
        catch (Exception e) {
            Interpreter.HandleException(e);
        }
    }
}