using Cricket.Interpreter;
using Cricket.Interpreter.Parser;
using Cricket.Interpreter.Parser.Statement;

namespace CricketBootstrap.Lib;

public class CastStringToInteger {
    public static void Link(Interpreter interpreter) {
        interpreter.AddExternalFunction(new FunctionStatement("CastStringToInteger",
            new List<FunctionStatement.FunctionArgument>()
                {new("arg", DataType.String)},
            new List<IStatement>() {new CastStringToIntegerStatement()}, DataType.Integer));
    }

    private class CastStringToIntegerStatement : IStatement {
        public object Interpret(Cricket.Interpreter.Environment.Environment environment) {
            var value = environment.GetVariable("arg").Interpret(environment);
            if (int.TryParse((string) value, out var casted)) {
                throw new ReturnStatement.HackyReturnException(casted, DataType.Integer);
            }
            throw new ReturnStatement.HackyReturnException(0, DataType.Integer);
        }

        public object Resolve(Resolver.ResolverEnvironment environment) {
            return null;
        }
    }
}