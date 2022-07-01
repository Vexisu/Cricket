using Cricket.Interpreter.Parser.Statement.Expression;

namespace Cricket.Interpreter.Environment;

/**
 * Klasa opakowująca wyrażenie zmiennej.
 */
public class VariableWrapper {
    /**
     * Konstruktor klasy VariableWrapper.
     * <param name="expression">Opakowane wyrażenie</param>
     * <param name="dataType">Typ zwracany przez wyrażenie</param>
     */
    public VariableWrapper(IExpression expression, DataType dataType) {
        Expression = expression;
        DataType = dataType;
    }

    public IExpression Expression { get; set; }
    public DataType DataType { get; }
}