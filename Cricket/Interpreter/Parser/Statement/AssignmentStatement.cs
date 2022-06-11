using Cricket.Interpreter.Error;
using Cricket.Interpreter.Parser.Statement.Expression;

namespace Cricket.Interpreter.Parser.Statement; 

public class AssignmentStatement : IStatement {
    private readonly IExpression _expression;
    private readonly string _name;

    public AssignmentStatement(IExpression expression, string name) {
        _expression = expression;
        _name = name;
    }

    public object Interpret(Environment.Environment environment) {
        environment.UpdateVariable(_name, new ValueExpression(_expression.Interpret(environment), environment.GetVariableType(_name)));
        return null;
    }

    public object Resolve(Resolver.ResolverEnvironment environment) {
        if (!environment.Variables.ContainsKey(_name)) {
            throw new ResolverError($"Variable {_name} does not exists.");
        }
        var variableType = environment.Variables[_name];
        var returnedType = _expression.Returns(environment);
        if (variableType != returnedType) {
            throw new ResolverError(@$"Value assigned to variable {_name} has different type than defined. Required: {variableType}, present: {returnedType}.");
        }
        _expression.Resolve(environment);
        return null;
    }
}