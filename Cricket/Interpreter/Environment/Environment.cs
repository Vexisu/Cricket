using System;
using System.Collections.Generic;
using Cricket.Interpreter.Parser.Statement;
using Cricket.Interpreter.Parser.Statement.Expression;

namespace Cricket.Interpreter.Environment;

public class Environment {
    private readonly Environment _parent;
    private readonly Dictionary<string, VariableWrapper> _variables;
    private readonly List<FunctionStatement> _functions;
    private readonly Stack<IExpression> _stack;

    public Environment(Environment parent) {
        _variables = new Dictionary<string, VariableWrapper>();
        _parent = parent;
        _stack = parent == null ? new Stack<IExpression>() : parent._stack;
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

    //TODO: Implement function creation.
    public void CreateFunction(string name) {
        
    }

    public void CallFunction(string name, List<DataType> argumentsType) {
        foreach (var function in _functions) {
            if (function.Name == name && CompareFunctionVariables(function, argumentsType)) {
                function.Call(this);
                return;
            }
        }
    }

    private bool CompareFunctionVariables(FunctionStatement function, List<DataType> argumentsType) {
        if (argumentsType.Count != function.Arguments.Count) {
            return false;
        }
        for (var i = 0; i < argumentsType.Count; i++) {
            if (argumentsType[i] != function.Arguments[i].Type) {
                return false;
            }
        }
        return true;
    }

    public void PutOnStack(IExpression expression) {
        _stack.Push(expression);
    }

    public IExpression PopFromStack() {
        return _stack.Pop();
    }

    public Environment GetGlobal() {
        return _parent == null ? this : _parent.GetGlobal();
    }
}