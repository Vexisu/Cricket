using System;
using Cricket.Interpreter;
using Cricket.Interpreter.Scanner;
using NUnit.Framework;

namespace CricketTest;

public class Tests
{
    private Scanner? _scanner;
    private string[]? _testSource;

    [SetUp]
    public void Setup()
    {
        _testSource = new[] {"2 + 2 == 4;", "var test;"};
        _scanner = new Scanner(_testSource);
    }

    [Test]
    public void TokenizerTest()
    {
        try
        {
            _scanner?.Tokenize();
        }
        catch (Exception e)
        {
            Interpreter.HandleException(e);
        }
        Assert.Pass();
    }
}