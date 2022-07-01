namespace Cricket.Interpreter.Parser.Statement.Expression;

/**
 * Klasa wyrażenia wartości.
 */
public class ValueExpression : IExpression {
    private readonly DataType _dataType;
    private readonly object _value;

    /**
     * Konstruktor klasy ValueExpression.
     * <param name="value">Wartość</param>
     * <param name="dataType">Typ wartości</param>
     */
    public ValueExpression(object value, DataType dataType) {
        _value = value;
        _dataType = dataType;
    }

    /**
     * Metoda interpretacji wyrażenia wartości.
     * <param name="environment">Środowisko pracy</param>
     * <returns>Wartość</returns>
     */
    public object Interpret(Environment.Environment environment) {
        return _value;
    }

    /**
     * Metoda rozwiązania wyrażenia wartości.
     * <param name="environment">Środowisko rozwiązania</param>
     * <returns>Wartość rozwiązania</returns>
     */
    public object Resolve(Resolver.ResolverEnvironment environment) {
        return null;
    }

    /**
     * Metoda rozwiązania zwracająca typ wartości.
     * <param name="environment">Środowisko rozwiązania</param>
     * <returns>Typ wartości</returns>
     */
    public DataType Returns(Resolver.ResolverEnvironment environment) {
        return _dataType;
    }
}