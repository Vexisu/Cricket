using System;

namespace Cricket.Interpreter.Error; 

public class InvalidReturnStatementError : Exception{
    public InvalidReturnStatementError(string message) : base(message) { }
}