using System;
using Cricket.Interpreter.Error;
using Cricket.Interpreter.Parser.Statement.Expression;

namespace Cricket.Interpreter.Parser.Statement;

public class AssignmentStatement : IStatement {
    private readonly IExpression _expression;
    private readonly string _name;

    public AssignmentStatement(IExpression expression, string name) {
        _expression = expression;
        _name = name;
    }

    public object Interpret(Environment.Environment environment) {
        environment.UpdateVariable(_name,
            new ValueExpression(_expression.Interpret(environment), environment.GetVariableType(_name)));
        return null;
    }

    public object Resolve(Resolver.ResolverEnvironment environment) {
        if (Interpreter.Debug) Console.Out.WriteLine($"Resolver: Assigning {_name}.");
        if (!environment.VariableExists(_name)) {
            throw new ResolverError($"Variable {_name} does not exists.");
        }
        _expression.Resolve(environment);
        Resolver.CheckTypeIntegrity(_name, _expression.Returns(environment), environment.VariableReturns(_name));
        return null;
    }
}