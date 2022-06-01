using Cricket.Interpreter.Parser.Statement.Expression;

namespace Cricket.Interpreter.Environment;

public class VariableWrapper {
    public VariableWrapper(IExpression expression, DataType dataType) {
        Expression = expression;
        DataType = dataType;
    }

    public IExpression Expression { get; set; }
    public DataType DataType { get; }
}