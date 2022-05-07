using Cricket.Interpreter.Scanner;
using NUnit.Framework;

namespace CricketTest;

public class Tests
{
    private Scanner _scanner;
    private string[] _testSource;

    [SetUp]
    public void Setup()
    {
        _testSource = new[] {"2 + 2 == 4;", "var test;"};
        _scanner = new Scanner(_testSource);
    }

    [Test]
    public void TokenizerTest()
    {
        _scanner.Tokenize();
        Assert.Pass();
    }
}