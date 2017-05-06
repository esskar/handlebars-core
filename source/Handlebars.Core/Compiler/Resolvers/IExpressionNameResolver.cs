namespace Handlebars.Core.Compiler.Resolvers
{
    public interface IExpressionNameResolver
    {
        string ResolveExpressionName(object instance, string expressionName);
    }
}