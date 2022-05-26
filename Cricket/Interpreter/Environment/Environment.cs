using System.Collections.Generic;

namespace Cricket.Interpreter.Environment;

public class Environment
{
    private Dictionary<string, VariableProperties> _variables;

    public Environment()
    {
        _variables = new Dictionary<string, VariableProperties>();
    }

    public bool createVariable(string name, string type, object value)
    {
        if (_variables.ContainsKey(name))
        {
            return false;
        }

        _variables.Add(name, new VariableProperties(value, type));
        return true;
    }

    public bool updateVariable(string name, object value)
    {
        if (!_variables.ContainsKey(name))
        {
            return false;
        }

        _variables[name].Value = value;
        return true;
    }
}