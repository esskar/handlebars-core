using System.Linq.Expressions;
using Handlebars.Core.Compiler.Structure;

namespace Handlebars.Core.Compiler
{
    internal class CompilationContext
    {
        public CompilationContext(IHandlebarsEngine engine)
        {
            Engine = engine;
            BindingContext = Expression.Variable(typeof(BindingContext), "context");
        }

        public IHandlebarsEngine Engine { get; }

        public ParameterExpression BindingContext { get; }
    }
}
