using System;
using System.Collections.Generic;

namespace Cricket.Interpreter.Parser.Statement;

public class FunctionStatement : IStatement {
    
    private string _name;
    private Dictionary<string, DataType> _arguments;
    private List<IStatement> _statements;

    public FunctionStatement(string name, Dictionary<string, DataType> arguments, List<IStatement> statements) {
        _name = name;
        _arguments = arguments;
        _statements = statements;
    }

    public object Interpret(Environment.Environment environment) {
        throw new NotImplementedException();
    }

    public object Resolve(Resolver.ResolverEnvironment environment) {
        throw new NotImplementedException();
    }
}