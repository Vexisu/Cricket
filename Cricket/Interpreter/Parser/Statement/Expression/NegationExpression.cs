using System;

namespace Cricket.Interpreter.Parser.Statement.Expression;

public class NegationExpression : IExpression {
    private readonly IExpression _expression;

    public NegationExpression(IExpression expression) {
        _expression = expression;
    }

    public object Interpret(Environment.Environment environment) {
        var value = _expression.Interpret(environment);
        return value switch {
            int i => -i,
            bool b => !b,
            _ => null
        };
    }

    public object Resolve(Resolver.ResolverEnvironment environment) {
        return _expression.Returns(environment);
    }

    public DataType Returns(Resolver.ResolverEnvironment environment) {
        return _expression.Returns(environment);
    }
}