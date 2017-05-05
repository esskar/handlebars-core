namespace Handlebars.Compiler.Resolvers
{
    public interface IExpressionNameResolver
    {
        string ResolveExpressionName(object instance, string expressionName);
    }
}