namespace Cricket.Interpreter.Parser.Statement.Expression;

public class ValueExpression : IExpression {
    public enum ValueType {
        Numeric,
        String,
        Identifier
    }

    private readonly object _value;
    public ValueType _valueType { get; }

    public ValueExpression(object value, ValueType valueType) {
        _value = value;
    }

    public object Interpreter() {
        return _value;
    }
}