using System;

namespace Cricket.Interpreter.Error; 

public class ResolverError : Exception {
    public ResolverError(string message) : base(message) { }
}