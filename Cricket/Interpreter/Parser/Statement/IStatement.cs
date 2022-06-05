namespace Cricket.Interpreter.Parser.Statement;

public interface IStatement {
    public object Interpreter(Environment.Environment environment);
    public object Resolve(Resolver.ResolverEnvironment environment);
}