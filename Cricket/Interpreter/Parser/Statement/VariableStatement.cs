using Cricket.Interpreter.Parser.Statement.Expression;

namespace Cricket.Interpreter.Parser.Statement;

public class VariableStatement : IStatement {
    private readonly IExpression _expression;
    private readonly string _name;
    private readonly DataType _type;

    public VariableStatement(string name, DataType type, IExpression expression) {
        _name = name;
        _type = type;
        _expression = expression;
    }

    public object Interpreter(Environment.Environment environment) {
        environment.CreateVariable(_name, _type, _expression);
        return null;
    }
}