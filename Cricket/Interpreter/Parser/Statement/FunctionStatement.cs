using System.Collections.Generic;

namespace Cricket.Interpreter.Parser.Statement;

public class FunctionStatement : IStatement {
    private readonly DataType _returns;
    private readonly List<IStatement> _statements;

    public FunctionStatement(string name, List<FunctionArgument> arguments, List<IStatement> statements,
        DataType returns) {
        Name = name;
        Arguments = arguments;
        _returns = returns;
        _statements = statements;
    }

    public string Name { get; }
    public List<FunctionArgument> Arguments { get; }

    public object Interpret(Environment.Environment environment) {
        return null;
    }

    //TODO: Implement resolver for function statement.
    public object Resolve(Resolver.ResolverEnvironment environment) {
        return null;
    }

    public void Call(Environment.Environment environment) {
        var localEnvironment = new Environment.Environment(environment.GetGlobal());
        for (var i = Arguments.Count - 1; i >= 0; i--) {
            var argumentExpression = environment.PopFromStack();
            localEnvironment.CreateVariable(Arguments[i].Name, Arguments[i].Type, argumentExpression);
        }
        foreach (var statement in _statements) statement.Interpret(localEnvironment);
    }

    public class FunctionArgument {
        public FunctionArgument(string name, DataType type) {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public DataType Type { get; }
    }
}