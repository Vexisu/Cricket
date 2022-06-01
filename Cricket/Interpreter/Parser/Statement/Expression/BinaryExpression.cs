using System;

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

    public object Interpreter(Environment.Environment environment) {
        var leftValue = (dynamic) _left.Interpreter(environment);
        var rightValue = (dynamic) _right.Interpreter(environment);
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
}