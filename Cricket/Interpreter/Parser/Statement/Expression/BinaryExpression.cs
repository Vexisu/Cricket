using System;

namespace Cricket.Interpreter.Parser.Statement.Expression;

public class BinaryExpression : IExpression {
    public enum ExpressionType {
        Addition,
        Subtraction,
        Multiplication,
        Division
    }

    private IExpression _left, _right;
    private ExpressionType _type;

    public BinaryExpression(IExpression left, ExpressionType type, IExpression right) {
        _left = left;
        _type = type;
        _right = right;
    }

    public object Interpreter(Environment.Environment environment) {
        switch (_type) {
            case ExpressionType.Addition:
                return (int) _left.Interpreter(environment) + (int) _right.Interpreter(environment);
            case ExpressionType.Subtraction:
                return (int) _left.Interpreter(environment) - (int) _right.Interpreter(environment);
            case ExpressionType.Multiplication:
                return (int) _left.Interpreter(environment) * (int) _right.Interpreter(environment);
            case ExpressionType.Division:
                return (int) _left.Interpreter(environment) / (int) _right.Interpreter(environment);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}