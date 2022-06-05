using System.Collections.Generic;
using Cricket.Interpreter.Parser.Statement;

namespace Cricket.Interpreter.Parser; 

public class Resolver {
    public class ResolverEnvironment {
        public readonly Dictionary<string, DataType> Variables;

        public ResolverEnvironment() {
            Variables = new Dictionary<string, DataType>();
        }
    }

    private readonly List<IStatement> _statements;

    public Resolver(List<IStatement> statements) {
        _statements = statements;
    }

    public void Resolve() {
        var environment = new ResolverEnvironment();
        foreach (var statement in _statements) {
            statement.Resolve(environment);
        }
    }
}