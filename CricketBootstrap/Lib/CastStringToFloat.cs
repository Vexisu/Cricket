using Cricket.Interpreter;
using Cricket.Interpreter.Parser;
using Cricket.Interpreter.Parser.Statement;

namespace CricketBootstrap.Lib; 

public static class CastStringToFloat {
    public static void Link(Interpreter interpreter) {
        interpreter.AddExternalFunction(new FunctionStatement("CastStringToFloat",
            new List<FunctionStatement.FunctionArgument>()
                {new("arg", DataType.String)},
            new List<IStatement>() {new CastStringToFloatStatement()}, DataType.Float));
    }
    
    private class CastStringToFloatStatement : IStatement{
        public object Interpret(Cricket.Interpreter.Environment.Environment environment) {
            var value = environment.GetVariable("arg").Interpret(environment);
            if (float.TryParse((string) value, out var casted)) {
                throw new ReturnStatement.HackyReturnException(casted, DataType.Float);
            }
            throw new ReturnStatement.HackyReturnException(0.0, DataType.Float);
        }

        public object Resolve(Resolver.ResolverEnvironment environment) {
            return null;
        }
    }
}