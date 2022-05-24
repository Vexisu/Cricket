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

    public object Interpreter() {
        switch (_type) {
            case ExpressionType.Addition:
                return (int) _left.Interpreter() + (int) _right.Interpreter();
            case ExpressionType.Subtraction:
                return (int) _left.Interpreter() - (int) _right.Interpreter();
            case ExpressionType.Multiplication:
                return (int) _left.Interpreter() * (int) _right.Interpreter();
            case ExpressionType.Division:
                return (int) _left.Interpreter() / (int) _right.Interpreter();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}