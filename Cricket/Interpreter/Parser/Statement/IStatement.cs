namespace Cricket.Interpreter.Parser.Statement;

public interface IStatement {
    public object Interpret(Environment.Environment environment);
    public object Resolve(Resolver.ResolverEnvironment environment);
}