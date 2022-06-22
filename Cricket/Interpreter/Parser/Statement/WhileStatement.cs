using System;
using System.Collections.Generic;
using Cricket.Interpreter.Error;
using Cricket.Interpreter.Parser.Statement.Expression;

namespace Cricket.Interpreter.Parser.Statement;

public class WhileStatement : IStatement {
    private IExpression _condition;
    private List<IStatement> _statements;

    public WhileStatement(IExpression condition, List<IStatement> statements) {
        _condition = condition;
        _statements = statements;
    }

    public object Interpret(Environment.Environment environment) {
        while ((bool) _condition.Interpret(environment)) {
            var localEnvironment = new Environment.Environment(environment);
            foreach (var statement in _statements) {
                statement.Interpret(localEnvironment);
            }
        }
        return null;
    }
    
    

    public object Resolve(Resolver.ResolverEnvironment environment) {
        _condition.Resolve(environment);
        var conditionReturns = _condition.Returns(environment);
        if (conditionReturns != DataType.Boolean) {
            throw new ResolverError(
                $"Condition in while loop does not return Boolean. Present: {Enum.GetName(conditionReturns)}.");
        }
        foreach (var statement in _statements) {
            var localEnvironment = new Resolver.ResolverEnvironment(environment);
            statement.Resolve(localEnvironment);
        }
        return null;
    }
}