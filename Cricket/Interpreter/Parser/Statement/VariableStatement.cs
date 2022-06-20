using System;
using Cricket.Interpreter.Parser.Statement.Expression;

namespace Cricket.Interpreter.Parser.Statement;

public class VariableStatement : IStatement {
    private readonly IExpression _expression;
    private readonly string _name;
    private readonly DataType _type;

    public VariableStatement(string name, DataType type, IExpression expression) {
        _name = name;
        _type = type;
        _expression = expression;
    }

    public object Interpret(Environment.Environment environment) {
        environment.CreateVariable(_name, _type, new ValueExpression(_expression.Interpret(environment), _type));
        return null;
    }

    public object Resolve(Resolver.ResolverEnvironment environment) {
        if (Interpreter.Debug) Console.Out.WriteLine($"Resolver: Defining {_name} ({_type}).");
        _expression.Resolve(environment);
        Resolver.CheckTypeIntegrity(_name, _expression.Returns(environment), _type);
        environment.AddVariable(_name, _type);
        return null;
    }
}