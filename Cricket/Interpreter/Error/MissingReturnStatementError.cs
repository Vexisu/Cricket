using System;

namespace Cricket.Interpreter.Error;

public class MissingReturnStatementError : Exception {
    public MissingReturnStatementError(string message) : base(message) { }
}