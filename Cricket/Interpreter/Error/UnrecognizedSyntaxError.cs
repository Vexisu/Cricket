using System;

namespace Cricket.Interpreter.Error;

public class UnrecognizedSyntaxError : Exception {
    public UnrecognizedSyntaxError(string sourceCode, int line, int index) : base("Unrecognized syntax error") {
        SourceCode = sourceCode;
        Line = line;
        Index = index;
    }

    public UnrecognizedSyntaxError() { }

    public UnrecognizedSyntaxError(string message) : base(message) { }

    public UnrecognizedSyntaxError(string message, Exception innerException) : base(message, innerException) { }

    public string SourceCode { get; }
    public int Line { get; }
    public int Index { get; }
}