using System;

namespace Cricket.Interpreter.Error;

public class DisallowedSyntaxError : Exception {
    public DisallowedSyntaxError(string message) : base(message) { }
}