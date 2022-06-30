using System;
using Cricket.Interpreter.Parser.Statement.Expression;

namespace Cricket.Interpreter.Parser.Statement;

/**
 * Klasa deklaracji instrukcji zwracania.
 */
public class ReturnStatement : IStatement {
    private readonly IExpression _expression;
    private DataType _returnedType;

    /**
     * Konstruktor klasy ReturnStatement.
     * <param name="expression">Zwracane wyrażenie.</param>
     */
    public ReturnStatement(IExpression expression) {
        _expression = expression;
    }

    /**
     * Metoda interpretacji instrukcji zwracania
     * <param name="environment">Środowisko pracy</param>
     */
    public object Interpret(Environment.Environment environment) {
        throw new HackyReturnException(_expression.Interpret(environment), _returnedType);
    }

    /**
     * Metoda rozwiązania instrukcji zwracania.
     * <param name="environment">Środowisko rozwiązania</param>
     * <returns>Wartość rozwiązania</returns>
     */
    public object Resolve(Resolver.ResolverEnvironment environment) {
        _expression.Resolve(environment);
        _returnedType = _expression.Returns(environment);
        return null;
    }

    /**
     * Wyjątek wykorzystywany do kaskadowego zwracania wartości.
     */
    public class HackyReturnException : Exception {
        public HackyReturnException(object value, DataType type) {
            Value = value;
            Type = type;
        }

        public object Value { get; }
        public DataType Type { get; }
    }
}