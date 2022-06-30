using System;
using Cricket.Interpreter.Parser.Statement.Expression;

namespace Cricket.Interpreter.Parser.Statement;

/**
 * Klasa deklaracji drukowania.
 */
public class PrintStatement : IStatement {
    private readonly IExpression _expression;

    /**
     * Konstruktor klasy PrintStatement.
     * <param name="expression">Drukowane wyrażenie</param>
     */
    public PrintStatement(IExpression expression) {
        _expression = expression;
    }

    /**
     * Metoda interpretacji deklaracji drukowania.
     * <param name="environment">Środowisko pracy</param>
     * <returns>Wartość operacji</returns>
     */
    public object Interpret(Environment.Environment environment) {
        Console.Out.WriteLine(_expression.Interpret(environment));
        return null;
    }

    /**
     * Metoda rozwiązania deklaracji drukowania.
     * <param name="environment">Środowisko rozwiązania</param>
     * <returns>Wartość rozwiązania</returns>
     */
    public object Resolve(Resolver.ResolverEnvironment environment) {
        return _expression.Resolve(environment);
    }
}