using System.Collections.Generic;
using Cricket.Interpreter.Error;
using Cricket.Interpreter.Parser.Statement.Expression;

namespace Cricket.Interpreter.Parser.Statement;

/**
 * Klasa deklaracji funkcji warunkowej
 */
public class IfStatement : IStatement {
    private readonly IExpression _condition;
    private readonly List<IStatement> _statements;

    /**
     * Konstruktor klasy IfStatement.
     * <param name="condition">Warunek</param>
     * <param name="statements">Lista deklaracji</param>
     */
    public IfStatement(IExpression condition, List<IStatement> statements) {
        _condition = condition;
        _statements = statements;
    }

    /**
     * Metoda interpretacji deklaracji funkcji warunkowej.
     * <param name="environment">Środowisko pracy</param>
     * <returns>Wartość operacji</returns>
     */
    public object Interpret(Environment.Environment environment) {
        if (_condition.Interpret(environment) is bool condition) {
            var scope = new Environment.Environment(environment);
            if (!condition) return null;
            foreach (var statement in _statements) {
                var returned = statement.Interpret(scope);
                if (returned != null) {
                    return returned;
                }
            }
        }
        return null;
    }

    /**
     * Metoda rozwiązania deklaracji funkcji warunkowej.
     * <param name="environment">Środowisko rozwiązania</param>
     * <returns>Wartość rozwiązania</returns>
     */
    public object Resolve(Resolver.ResolverEnvironment environment) {
        _condition.Resolve(environment);
        var returnedType = _condition.Returns(environment);
        if (returnedType == DataType.Boolean) {
            var scope = new Resolver.ResolverEnvironment(environment);
            foreach (var statement in _statements) statement.Resolve(scope);
        }
        else {
            throw new ResolverError(
                $"The expression of if's condition does not return Boolean. Present: {returnedType}.");
        }
        return null;
    }
}