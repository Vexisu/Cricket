using System;
using Cricket.Interpreter.Parser.Statement.Expression;

namespace Cricket.Interpreter.Parser.Statement;

/**
 * Klasa deklaracji zmiennej.
 */
public class VariableStatement : IStatement {
    private readonly IExpression _expression;
    private readonly string _name;
    private readonly DataType _type;
    
    /**
     * Konstruktor klasy VariableStatement.
     * <param name="name">Nazwa zmiennej</param>
     * <param name="type">Typ zmiennej</param>
     * <param name="expression">Wyrażenie zmiennej</param>
     */
    public VariableStatement(string name, DataType type, IExpression expression) {
        _name = name;
        _type = type;
        _expression = expression;
    }

    /**
     * Metoda interpretacji deklaracji zmiennej.
     * <param name="environment">Środowisko pracy</param>
     * <returns>Wartość operacji</returns>
     */
    public object Interpret(Environment.Environment environment) {
        environment.CreateVariable(_name, _type, new ValueExpression(_expression.Interpret(environment), _type));
        return null;
    }

    /**
     * Metoda rozwiązania deklaracji zmiennej.
     * <param name="environment">Środowisko rozwiązania</param>
     * <returns>Wartość rozwiązania</returns>
     */
    public object Resolve(Resolver.ResolverEnvironment environment) {
        if (Interpreter.Debug) Console.Out.WriteLine($"Resolver: Defining {_name} ({_type}).");
        _expression.Resolve(environment);
        Resolver.CheckTypeIntegrity(_name, _expression.Returns(environment), _type);
        environment.AddVariable(_name, _type);
        return null;
    }
}