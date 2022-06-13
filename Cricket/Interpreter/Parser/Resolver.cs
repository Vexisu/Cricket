using System;
using System.Collections.Generic;
using Cricket.Interpreter.Error;
using Cricket.Interpreter.Parser.Statement;

namespace Cricket.Interpreter.Parser;

public class Resolver {
    private readonly List<IStatement> _statements;

    public Resolver(List<IStatement> statements) {
        _statements = statements;
    }

    public void Resolve() {
        var environment = new ResolverEnvironment();
        foreach (var statement in _statements) statement.Resolve(environment);
    }

    public static void CheckTypeIntegrity(string name, DataType present, DataType required) {
        switch (required) {
            case DataType.String:
                if (present == DataType.String) return;
                break;
            case DataType.Integer:
                if (present == DataType.Integer) return;
                break;
            case DataType.Float:
                if (present is DataType.Integer or DataType.Float) return;
                break;
            case DataType.Boolean:
                if (present == DataType.Boolean) return;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        throw new ResolverError(
            @$"Value assigned to variable {name} has different type than defined. Required: {required}, present: {present}.");
    }

    public class ResolverEnvironment {
        private readonly ResolverEnvironment _parent;
        private readonly Dictionary<string, DataType> _variables;

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
}