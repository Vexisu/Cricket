using System.Collections.Generic;
using Cricket.Interpreter.Parser.Statement;

namespace Cricket.Interpreter.Parser;

public class Resolver {
    public class ResolverEnvironment {
        private readonly Dictionary<string, DataType> _variables;
        private readonly ResolverEnvironment _parent;

        public ResolverEnvironment(ResolverEnvironment parent) {
            _variables = new Dictionary<string, DataType>();
            _parent = parent;
        }

        public ResolverEnvironment() : this(null) { }

        public bool VariableExists(string name) {
            if (_variables.ContainsKey(name)) {
                return true;
            }
            return _parent?.VariableExists(name) ?? false;
        }

        public DataType VariableReturns(string name) {
            return _variables.ContainsKey(name) ? _variables[name] : _parent.VariableReturns(name);
        }

        public void AddVariable(string name, DataType type) {
            _variables[name] = type;
        }
    }

    private readonly List<IStatement> _statements;

    public Resolver(List<IStatement> statements) {
        _statements = statements;
    }

    public void Resolve() {
        var environment = new ResolverEnvironment();
        foreach (var statement in _statements) statement.Resolve(environment);
    }
}