using System.Collections.Generic;
using Cricket.Interpreter.Parser.Statement.Expression;

namespace Cricket.Interpreter.Environment;

public class Environment {
    private readonly Dictionary<string, VariableWrapper> _variables;
    private readonly Environment _parent;

    public Environment(Environment parent) {
        _variables = new Dictionary<string, VariableWrapper>();
        _parent = parent;
    }

    public Environment() : this(null) { }

    public bool CreateVariable(string name, DataType dataType, IExpression expression) {
        if (_variables.ContainsKey(name)) return false;
        _variables.Add(name, new VariableWrapper(expression, dataType));
        return true;
    }

    public bool UpdateVariable(string name, IExpression expression) {
        if (_variables.ContainsKey(name)) {
            _variables[name].Expression = expression;
            return true;
        }
        return _parent?.UpdateVariable(name, expression) ?? false;
    }

    public IExpression GetVariable(string name) {
        return _variables.ContainsKey(name) ? _variables[name].Expression : _parent?.GetVariable(name);
    }

    public DataType GetVariableType(string name) {
        return _variables.ContainsKey(name)
            ? _variables[name].DataType
            : _parent?.GetVariableType(name) ?? DataType.Null;
    }

    public bool VariableExists(string name) {
        return _variables.ContainsKey(name) || (_parent?.VariableExists(name) ?? false);
    }
}