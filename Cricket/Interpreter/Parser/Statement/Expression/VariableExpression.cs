using System;
using Cricket.Interpreter.Error;

namespace Cricket.Interpreter.Parser.Statement.Expression;

public class VariableExpression : IExpression {
    private readonly string _name;

    public VariableExpression(string name) {
        _name = name;
    }

    public object Interpreter(Environment.Environment environment) {
        return environment.GetVariable(_name).Interpreter(environment);
    }

    public object Resolve(Resolver.ResolverEnvironment environment) {
        if (Program.Debug) Console.Out.WriteLine($"Resolver: Resolving for variable {_name}.");
        if (!environment.Variables.ContainsKey(_name)) {
            throw new ResolverError($"Variable {_name} does not exists.");
        }
        return null;
    }

    public DataType Returns(Resolver.ResolverEnvironment environment) {
        return environment.Variables[_name];
    }
}