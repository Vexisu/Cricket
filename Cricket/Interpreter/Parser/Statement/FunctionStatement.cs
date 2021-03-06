using System;
using System.Collections.Generic;
using Cricket.Interpreter.Error;
using Cricket.Interpreter.Parser.Statement.Expression;

namespace Cricket.Interpreter.Parser.Statement;

/**
 * Klasa deklaracji funkcji.
 */
public class FunctionStatement : IStatement {
    private readonly DataType _returns;
    private readonly List<IStatement> _statements;

    /**
     * Konstruktor klasy FunctionStatement.
     * <param name="name">Nazwa funkcji</param>
     * <param name="arguments">Lista argumentów</param>
     * <param name="statements">Lista deklaracji</param>
     * <param name="returns">Typ zwracany</param>
     */
    public FunctionStatement(string name, List<FunctionArgument> arguments, List<IStatement> statements,
        DataType returns) {
        Name = name;
        Arguments = arguments;
        _returns = returns;
        _statements = statements;
    }

    public string Name { get; }
    public List<FunctionArgument> Arguments { get; }

    /**
     * Metoda interpretacji deklaracji funkcji.
     * <param name="environment">Środowisko pracy</param>
     * <returns>Wartość operacji</returns>
     */
    public object Interpret(Environment.Environment environment) {
        environment.CreateFunction(this);
        if (Interpreter.Debug) {
            Console.Out.WriteLine($"Interpreter: {Name}() has been registered.");
        }
        return null;
    }

    /**
     * Metoda rozwiązania deklaracji funkcji.
     * <param name="environment">Środowisko rozwiązania</param>
     * <returns>Wartość rozwiązania</returns>
     */
    public object Resolve(Resolver.ResolverEnvironment environment) {
        var argumentsType = new List<DataType>();
        var argumentsTypeName = new List<string>();
        var localEnvironment = new Resolver.ResolverEnvironment(environment.GetGlobal());
        Arguments.ForEach(argument => {
            argumentsType.Add(argument.Type);
            localEnvironment.AddVariable(argument.Name, argument.Type);
            argumentsTypeName.Add(Enum.GetName(argument.Type));
        });
        if (Interpreter.Debug)
            Console.Out.WriteLine($"Resolver: Defining {Name}({string.Join(", ", argumentsTypeName)}).");
        environment.GetGlobal().AddFunction(Name, argumentsType, _returns);
        foreach (var statement in _statements) statement.Resolve(localEnvironment);
        return null;
    }

    /**
     * Metoda wywołania funkcji.
     * <param name="environment">Środowisko pracy</param>
     */
    public void Call(Environment.Environment environment) {
        var localEnvironment = new Environment.Environment(environment.GetGlobal());
        for (var i = Arguments.Count - 1; i >= 0; i--) {
            var argumentExpression = environment.PopFromStack();
            localEnvironment.CreateVariable(Arguments[i].Name, Arguments[i].Type, argumentExpression);
        }
        try {
            foreach (var statement in _statements) statement.Interpret(localEnvironment);
        }
        catch (ReturnStatement.HackyReturnException returned) {
            OnCallPutOnStack(environment, returned);
            return;
        }
        if (_returns != DataType.Null) {
            var argumentsTypeName = new List<string>();
            Arguments.ForEach(argument => argumentsTypeName.Add(Enum.GetName(argument.Type)));
            throw new MissingReturnStatementError(
                $"Called function {Name}({string.Join(", ", argumentsTypeName)}) does not return any value. Required: {_returns}.");
        }
    }

    /**
     * Metoda odkładająca na stos.
     * <param name="environment">Środowisko pracy</param>
     * <param name="returned">Wartość zwracana</param>
     */
    private void OnCallPutOnStack(Environment.Environment environment, ReturnStatement.HackyReturnException returned) {
        if (returned.Type != _returns) {
            var argumentsTypeName = new List<string>();
            Arguments.ForEach(argument => argumentsTypeName.Add(Enum.GetName(argument.Type)));
            throw new InvalidReturnStatementError(
                $"Called function {Name}({string.Join(", ", argumentsTypeName)}) returns invalid value type. Required: {_returns}, Present: {returned.Type}");
        }
        environment.PutOnStack(new ValueExpression(returned.Value, returned.Type));
    }

    /**
     * Klasa argumentów funkcji.
     */
    public class FunctionArgument {
        public FunctionArgument(string name, DataType type) {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public DataType Type { get; }
    }
}