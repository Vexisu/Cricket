using System;

namespace Cricket.Interpreter.Error;

public class UnexpectedSyntaxError : Exception
{
    public string SourceCode { get; }
    public int Line { get; }
    public int Index { get; }

    public UnexpectedSyntaxError(string sourceCode, int line, int index) : base("Unexpected syntax error")
    {
        SourceCode = sourceCode;
        Line = line;
        Index = index;
    }

    public UnexpectedSyntaxError()
    {
    }

    public UnexpectedSyntaxError(string message) : base(message)
    {
    }

    public UnexpectedSyntaxError(string message, Exception innerException) : base(message, innerException)
    {
    }
}