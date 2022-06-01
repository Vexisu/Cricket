namespace Cricket.Interpreter.Parser.Statement.Expression;

public class NegationExpression : IExpression {
    private readonly IExpression _expression;

    public NegationExpression(IExpression expression) {
        _expression = expression;
    }

    public object Interpreter(Environment.Environment environment) {
        var value = _expression.Interpreter(environment);
        return value switch {
            int i => -i,
            bool b => !b,
            _ => null
        };
    }
}