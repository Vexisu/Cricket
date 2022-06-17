﻿using System;
using System.Collections.Generic;
using Cricket.Interpreter.Error;

namespace Cricket.Interpreter.Parser.Statement.Expression;

public class CallExpression : IExpression {
    private string _calledFunction;
    private List<IExpression> _arguments;
    private List<DataType> _argumentsType;

    public CallExpression(string calledFunction, List<IExpression> arguments) {
        _calledFunction = calledFunction;
        _arguments = arguments;
        _argumentsType = new List<DataType>();
    }

    public object Interpret(Environment.Environment environment) {
        for (var i = 0; i < _arguments.Count; i++) {
            environment.PutOnStack(new ValueExpression(_arguments[i].Interpret(environment), _argumentsType[i]));
        }
        environment.CallFunction(_calledFunction, _argumentsType);
        return null;
    }

    public object Resolve(Resolver.ResolverEnvironment environment) {
        foreach (var argument in _arguments) {
            argument.Resolve(environment);
            _argumentsType.Add(argument.Returns(environment));
        }
        if (!environment.FunctionExists(_calledFunction, _argumentsType)) {
            var argumentsTypeName = new List<string>();
            _argumentsType.ForEach(type => argumentsTypeName.Add(Enum.GetName(type)));
            throw new ResolverError($"Called function {_calledFunction}({string.Join(", ", argumentsTypeName)}) does not exists.");
        }
        return null;
    }

    public DataType Returns(Resolver.ResolverEnvironment environment) {
        return environment.FunctionReturns(_calledFunction, _argumentsType);
    }
}