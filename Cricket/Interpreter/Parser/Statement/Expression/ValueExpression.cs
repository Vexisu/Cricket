namespace Cricket.Interpreter.Parser.Statement.Expression;

public class ValueExpression : IExpression
{
    private object _value;
    private ValueType _valueType;

    public ValueExpression(object value, ValueType valueType)
    {
        _value = value;
    }

    public object Interpreter()
    {
        return _value;
    }

    public enum ValueType
    {
        Numeric, String, Identifier
    }
}