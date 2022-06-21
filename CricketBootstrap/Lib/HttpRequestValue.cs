using Cricket.Interpreter;
using Cricket.Interpreter.Parser;
using Cricket.Interpreter.Parser.Statement;

namespace CricketBootstrap.Lib;

public static class HttpRequestValue {
    public static void Link(Interpreter interpreter) {
        var statement = new FunctionStatement("HttpRequestValue",
            new List<FunctionStatement.FunctionArgument>()
                {new("arg", DataType.String)},
            new List<IStatement>()
                {new HttpRequestValueStatement()},
            DataType.String);
        interpreter.AddExternalFunction(statement);
    }

    private class HttpRequestValueStatement : IStatement {
        public object Interpret(Cricket.Interpreter.Environment.Environment environment) {
            var valueName = (string) environment.GetVariable("arg").Interpret(environment);
            if (environment.VariableExists(valueName)) {
                throw new ReturnStatement.HackyReturnException(environment.GetVariable(valueName).Interpret(environment),
                    DataType.String);
            }
            throw new ReturnStatement.HackyReturnException(string.Empty,
                DataType.String);
        }

        public object Resolve(Resolver.ResolverEnvironment environment) {
            return null;
        }
    }
}