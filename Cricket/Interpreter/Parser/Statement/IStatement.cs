namespace Cricket.Interpreter.Parser.Statement;

/**
 * Interfejs deklaracji.
 */
public interface IStatement {
    public object Interpret(Environment.Environment environment);
    public object Resolve(Resolver.ResolverEnvironment environment);
}