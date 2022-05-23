namespace Cricket.Interpreter.Parser.Statement.Expression;

public class BinaryExpression : IExpression
{
    private IExpression _left, _right;
    private ExpressionType _type;

    public BinaryExpression(IExpression left, ExpressionType type, IExpression right)
    {
        _left = left;
        _type = type;
        _right = right;
    }

    public object Interpreter()
    {
        return null;
    }

    public enum ExpressionType
    {
        Addition, Subtraction, Multiplication, Division 
    }
}