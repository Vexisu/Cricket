using System;
using Cricket.Interpreter.Parser.Statement.Expression;

namespace Cricket.Interpreter.Parser.Statement;

public class PrintStatement : IStatement {
    private readonly IExpression _expression;

    public PrintStatement(IExpression expression) {
        _expression = expression;
    }

    public object Interpreter(Environment.Environment environment) {
        Console.Out.WriteLine(_expression.Interpreter(environment));
        return null;
    }
}