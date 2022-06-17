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
        private readonly List<ResolverFunction> _functions;
        
        public ResolverEnvironment(ResolverEnvironment parent) {
            _variables = new Dictionary<string, DataType>();
            _functions = parent == null ? new List<ResolverFunction>() : parent._functions;
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

        public void AddFunction(string name, List<DataType> arguments, DataType returns) {
            var function = new ResolverFunction(name, arguments, returns);
            _functions.Add(function);
        }

        public bool FunctionExists(string name, List<DataType> arguments) {
            foreach (var function in _functions) {
                if (function.Name != name ||  function.Arguments.Count != arguments.Count) {
                    continue;
                }
                for (var i = 0; i < arguments.Count; i++) {
                    if (function.Arguments[i] != arguments[i]) {
                        break;
                    }
                }
                return true;
            }
            return false;
        }

        public DataType FunctionReturns(string name, List<DataType> arguments) {
            foreach (var function in _functions) {
                if (function.Name != name ||  function.Arguments.Count != arguments.Count) {
                    continue;
                }
                for (var i = 0; i < arguments.Count; i++) {
                    if (function.Arguments[i] != arguments[i]) {
                        break;
                    }
                }
                return function.Returns;
            }
            return DataType.Null;
        }
    }

    private class ResolverFunction {
        public string Name { get; }
        public List<DataType> Arguments { get; }
        public DataType Returns { get; }

        public ResolverFunction(string name, List<DataType> arguments, DataType returns) {
            Name = name;
            Arguments = arguments;
            Returns = returns;
        }
    }
}