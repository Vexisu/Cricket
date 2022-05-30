using System.Collections.Generic;
using Cricket.Interpreter.Parser.Statement.Expression;

namespace Cricket.Interpreter.Environment;

public class Environment {
    private Dictionary<string, VariableWrapper> _variables;

    public Environment() {
        _variables = new Dictionary<string, VariableWrapper>();
    }

    public bool CreateVariable(string name, DataType dataType, IExpression expression) {
        if (_variables.ContainsKey(name)) return false;
        _variables.Add(name, new VariableWrapper(expression, dataType));
        return true;
    }

    public bool UpdateVariable(string name, IExpression expression) {
        if (!_variables.ContainsKey(name)) return false;
        _variables[name].Expression = expression;
        return true;
    }

    public IExpression GetVariable(string name) {
        return _variables[name].Expression;
    }
}