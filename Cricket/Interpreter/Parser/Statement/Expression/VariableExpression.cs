using System;
using Cricket.Interpreter.Error;

namespace Cricket.Interpreter.Parser.Statement.Expression;
/**
 * Klasa wyrażenia zmiennej.
 */
public class VariableExpression : IExpression {
    private readonly string _name;

    /**
     * Konstruktor klasy VariableExpression.
     * <param name="name">Nazwa zmiennej</param>
     */
    public VariableExpression(string name) {
        _name = name;
    }

    /**
     * Metoda interpretacji wyrażenia zmiennej.
     * <param name="environment">Środowisko pracy</param>
     * <returns>Wartość zwracana przez wyrażenie</returns>
     */
    public object Interpret(Environment.Environment environment) {
        return environment.GetVariable(_name).Interpret(environment);
    }

    /**
     * Metoda rozwiązania wyrażenia zmiennej.
     * <param name="environment">Środowisko rozwiązania</param>
     * <returns>Wartość rozwiązania</returns>
     */
    public object Resolve(Resolver.ResolverEnvironment environment) {
        if (Interpreter.Debug) Console.Out.WriteLine($"Resolver: Resolving for variable {_name}.");
        if (!environment.VariableExists(_name)) {
            throw new ResolverError($"Variable {_name} does not exists.");
        }
        return null;
    }

    /**
     * Metoda rozwiązania zwracająca typ wartości zwracanej przez wyrażenie zmiennej.
     * <param name="environment">Środowisko rozwiązania</param>
     * <returns>Typ zmiennej</returns>
     */
    public DataType Returns(Resolver.ResolverEnvironment environment) {
        return environment.VariableReturns(_name);
    }
}