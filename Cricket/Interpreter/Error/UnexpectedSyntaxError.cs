using System;

namespace Cricket.Interpreter.Error;

public class UnexpectedSyntaxError : Exception {
    public int Line { get; }
    public string Expected { get; }
    public string Present { get; }

    public UnexpectedSyntaxError(int line, string present, string expected) : base("Unexpected syntax error") {
        Line = line;
        Present = present;
        Expected = expected;
    }
}