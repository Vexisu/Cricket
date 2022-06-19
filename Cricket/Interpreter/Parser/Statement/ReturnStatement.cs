using System;
using Cricket.Interpreter.Parser.Statement.Expression;

namespace Cricket.Interpreter.Parser.Statement;

public class ReturnStatement : IStatement {
    private readonly IExpression _expression;
    private DataType _returnedType;

    public ReturnStatement(IExpression expression) {
        _expression = expression;
    }

    public object Interpret(Environment.Environment environment) {
        throw new HackyReturnException(_expression.Interpret(environment), _returnedType);
    }

    public object Resolve(Resolver.ResolverEnvironment environment) {
        _expression.Resolve(environment);
        _returnedType = _expression.Returns(environment);
        return null;
    }

    public class HackyReturnException : Exception {
        public object Value { get; }
        public DataType Type { get; }

        public HackyReturnException(object value, DataType type) {
            Value = value;
            Type = type;
        }
    }
}