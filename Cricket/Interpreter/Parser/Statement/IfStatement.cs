using System;
using System.Collections.Generic;
using Cricket.Interpreter.Error;
using Cricket.Interpreter.Parser.Statement.Expression;

namespace Cricket.Interpreter.Parser.Statement;

public class IfStatement : IStatement {
    private readonly IExpression _condition;
    private readonly List<IStatement> _statements;

    public IfStatement(IExpression condition, List<IStatement> statements) {
        _condition = condition;
        _statements = statements;
    }

    public object Interpreter(Environment.Environment environment) {
        if (_condition.Interpreter(environment) is bool condition) {
            if (!condition) return null;
            foreach (var statement in _statements) statement.Interpreter(environment);
        }
        return null;
    }

    public object Resolve(Resolver.ResolverEnvironment environment) {
        _condition.Resolve(environment);
        var returnedType = _condition.Returns(environment);
        if (returnedType == DataType.Boolean) {
            foreach (var statement in _statements) {
                statement.Resolve(environment);
            }
        }
        else {
            throw new ResolverError($"The expression of if's condition does not return Boolean. Present: {returnedType}");
        }
        return null;
    }
}