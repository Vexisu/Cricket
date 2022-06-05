namespace Cricket.Interpreter.Parser.Statement.Expression;

public class ValueExpression : IExpression {
    private readonly object _value;
    private readonly DataType _dataType;

    public ValueExpression(object value, DataType dataType) {
        _value = value;
        _dataType = dataType;
    }

    public object Interpreter(Environment.Environment environment) {
        return _value;
    }

    public object Resolve(Resolver.ResolverEnvironment environment) {
        return null;
    }

    public DataType Returns(Resolver.ResolverEnvironment environment) {
        return _dataType;
    }
}