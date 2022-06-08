using System;
using Cricket.Interpreter.Error;
using Cricket.Interpreter.Parser.Statement.Expression;

namespace Cricket.Interpreter.Parser.Statement;

public class VariableStatement : IStatement {
    private readonly IExpression _expression;
    private readonly string _name;
    private readonly DataType _type;

    public VariableStatement(string name, DataType type, IExpression expression) {
        _name = name;
        _type = type;
        _expression = expression;
    }

    public object Interpreter(Environment.Environment environment) {
        environment.CreateVariable(_name, _type, _expression);
        return null;
    }

    public object Resolve(Resolver.ResolverEnvironment environment) {
        if (Program.Debug) Console.Out.WriteLine($"Resolver: Defining {_name} ({_type}).");
        _expression.Resolve(environment);
        CheckTypeIntegrity(_expression.Returns(environment));
        environment.AddVariable(_name, _type);
        return null;
    }

    private void CheckTypeIntegrity(DataType returnedType) {
        switch (_type) {
            case DataType.String:
                if (returnedType == DataType.String) return;
                break;
            case DataType.Integer:
                if (returnedType == DataType.Integer) return;
                break;
            case DataType.Float:
                if (returnedType is DataType.Integer or DataType.Float) return;
                break;
            case DataType.Boolean:
                if (returnedType == DataType.Boolean) return;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        throw new ResolverError(
            @$"Value assigned to variable {_name} has different type than defined. Required: {_type}, present: {returnedType}.");
    }
}