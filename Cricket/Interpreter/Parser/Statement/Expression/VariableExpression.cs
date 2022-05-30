namespace Cricket.Interpreter.Parser.Statement.Expression;

public class VariableExpression : IExpression {
    private readonly string _name;

    public VariableExpression(string name) {
        _name = name;
    }

    public object Interpreter(Environment.Environment environment) {
        return environment.GetVariable(_name).Interpreter(environment);
    }
}