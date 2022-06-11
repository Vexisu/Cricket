using System;
using System.Linq;
using Cricket.Interpreter.Error;

namespace Cricket.Interpreter.Parser.Statement.Expression;

public class BinaryExpression : IExpression {
    public enum ExpressionType {
        Addition,
        Subtraction,
        Multiplication,
        Division,
        Equal,
        Greater,
        Less,
        GreaterEqual,
        LessEqual
    }

    private readonly IExpression _left, _right;
    private readonly ExpressionType _type;

    public BinaryExpression(IExpression left, ExpressionType type, IExpression right) {
        _left = left;
        _type = type;
        _right = right;
    }

    public object Interpret(Environment.Environment environment) {
        var leftValue = (dynamic) _left.Interpret(environment);
        var rightValue = (dynamic) _right.Interpret(environment);
        return _type switch {
            ExpressionType.Addition => leftValue + rightValue,
            ExpressionType.Subtraction => leftValue - rightValue,
            ExpressionType.Multiplication => leftValue * rightValue,
            ExpressionType.Division => leftValue / rightValue,
            ExpressionType.Equal => leftValue == rightValue,
            ExpressionType.Greater => leftValue > rightValue,
            ExpressionType.Less => leftValue < rightValue,
            ExpressionType.GreaterEqual => leftValue >= rightValue,
            ExpressionType.LessEqual => leftValue <= rightValue,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public object Resolve(Resolver.ResolverEnvironment environment) {
        _left.Resolve(environment);
        _right.Resolve(environment);
        return null;
    }

    // TODO: Better log for invalid binary expressions.
    public DataType Returns(Resolver.ResolverEnvironment environment) {
        if (new[] {
                ExpressionType.Equal, ExpressionType.Greater, ExpressionType.Less, ExpressionType.Greater,
                ExpressionType.GreaterEqual, ExpressionType.LessEqual
            }.Contains(_type)) {
            return DataType.Boolean;
        }
        if (_left.Returns(environment) == DataType.Boolean || _right.Returns(environment) == DataType.Boolean) {
            throw new ResolverError("Arithmetical operations are not allowed on Boolean.");
        }
        if (_left.Returns(environment) == DataType.Float || _right.Returns(environment) == DataType.Float) {
            return DataType.Float;
        }
        if (_left.Returns(environment) == DataType.Integer && _right.Returns(environment) == DataType.Integer) {
            return DataType.Integer;
        }
        throw new ResolverError("Unresolvable data types in expression.");
    }
}