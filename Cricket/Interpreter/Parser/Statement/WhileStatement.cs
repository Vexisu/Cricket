using System;
using System.Collections.Generic;
using Cricket.Interpreter.Error;
using Cricket.Interpreter.Parser.Statement.Expression;

namespace Cricket.Interpreter.Parser.Statement;

/**
 * Klasa deklaracji pętli while.
 */
public class WhileStatement : IStatement {
    private IExpression _condition;
    private List<IStatement> _statements;

    /**
     * Konstruktor klasy WhileStatement.
     * <param name="condition">Warunek</param>
     * <param name="statements">Lista deklaracji</param>
     */
    public WhileStatement(IExpression condition, List<IStatement> statements) {
        _condition = condition;
        _statements = statements;
    }

    /**
     * Metoda interpretacji deklaracji pętli while.
     * <param name="environment">Środowisko pracy</param>
     * <returns>Wartość operacji</returns>
     */
    public object Interpret(Environment.Environment environment) {
        while ((bool) _condition.Interpret(environment)) {
            var localEnvironment = new Environment.Environment(environment);
            foreach (var statement in _statements) {
                statement.Interpret(localEnvironment);
            }
        }
        return null;
    }

    /**
     * Metoda rozwiązania deklaracji pętli while.
     * <param name="environment">Środowisko rozwiązania</param>
     * <returns>Wartość rozwiązania</returns>
     */
    public object Resolve(Resolver.ResolverEnvironment environment) {
        _condition.Resolve(environment);
        var conditionReturns = _condition.Returns(environment);
        if (conditionReturns != DataType.Boolean) {
            throw new ResolverError(
                $"Condition in while loop does not return Boolean. Present: {Enum.GetName(conditionReturns)}.");
        }
        foreach (var statement in _statements) {
            var localEnvironment = new Resolver.ResolverEnvironment(environment);
            statement.Resolve(localEnvironment);
        }
        return null;
    }
}