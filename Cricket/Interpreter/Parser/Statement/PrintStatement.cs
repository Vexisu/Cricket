using System;
using Cricket.Interpreter.Parser.Statement.Expression;

namespace Cricket.Interpreter.Parser.Statement;

public class PrintStatement : IStatement {
    private readonly IExpression _expression;

    public PrintStatement(IExpression expression) {
        _expression = expression;
    }

    public object Interpret(Environment.Environment environment) {
        Console.Out.WriteLine(_expression.Interpret(environment));
        return null;
    }

    public object Resolve(Resolver.ResolverEnvironment environment) {
        return _expression.Resolve(environment);
    }
}