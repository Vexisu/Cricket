namespace Cricket.Interpreter.Parser.Statement.Expression;

public interface IExpression : IStatement {
    public DataType Returns(Resolver.ResolverEnvironment environment);
}