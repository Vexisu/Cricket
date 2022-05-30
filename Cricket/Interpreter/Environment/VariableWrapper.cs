using Cricket.Interpreter.Parser.Statement.Expression;

namespace Cricket.Interpreter.Environment;

public class VariableWrapper {
    public IExpression Expression { get; set; }
    public DataType DataType { get; }

    public VariableWrapper(IExpression expression, DataType dataType) {
        Expression = expression;
        DataType = dataType;
    }
}