using System;
using System.Collections.Generic;

namespace Cricket.Interpreter.Parser.Statement;

public class FunctionStatement : IStatement {
    private Dictionary<string, DataType> _arguments;

    public object Interpret(Environment.Environment environment) {
        throw new NotImplementedException();
    }

    public object Resolve(Resolver.ResolverEnvironment environment) {
        throw new NotImplementedException();
    }
}