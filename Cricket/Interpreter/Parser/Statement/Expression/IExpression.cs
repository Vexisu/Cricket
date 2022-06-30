namespace Cricket.Interpreter.Parser.Statement.Expression;

/**
 * Interfejs wyrażenia.
 */
public interface IExpression : IStatement {
    public DataType Returns(Resolver.ResolverEnvironment environment);
}