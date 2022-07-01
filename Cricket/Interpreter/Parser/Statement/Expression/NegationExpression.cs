namespace Cricket.Interpreter.Parser.Statement.Expression;

/**
 * Klasa wyrażenia negacji.
 */
public class NegationExpression : IExpression {
    private readonly IExpression _expression;

    /**
     * Konstruktor klasy NegationExpression.
     * <param name="expression">Negowane wyrażenie</param>
     */
    public NegationExpression(IExpression expression) {
        _expression = expression;
    }

    /**
     * Metoda interpretacji wyrażenia negacji.
     * <param name="environment">Środowisko pracy</param>
     * <returns>Zanegowana wartość</returns>
     */
    public object Interpret(Environment.Environment environment) {
        var value = _expression.Interpret(environment);
        return value switch {
            int i => -i,
            bool b => !b,
            _ => null
        };
    }

    /**
     * Metoda rozwiązania wyrażenia negacji.
     * <param name="environment">Środowisko rozwiązania</param>
     * <returns>Wartość rozwiązania</returns>
     */
    public object Resolve(Resolver.ResolverEnvironment environment) {
        return _expression.Returns(environment);
    }

    /**
     * Metoda rozwiązania zwracająca typ wyrażenia negacji.
     * <param name="environment">Środowisko rozwiązania</param>
     * <returns>Typ zanegowanej wartości</returns>
     */
    public DataType Returns(Resolver.ResolverEnvironment environment) {
        return _expression.Returns(environment);
    }
}