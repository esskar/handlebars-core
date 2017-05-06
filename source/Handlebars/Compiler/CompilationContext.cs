using System.Linq.Expressions;
using Handlebars.Core.Compiler.Structure;

namespace Handlebars.Core.Compiler
{
    internal class CompilationContext
    {
        public CompilationContext(HandlebarsConfiguration configuration)
        {
            Configuration = configuration;
            BindingContext = Expression.Variable(typeof(BindingContext), "context");
        }

        public virtual HandlebarsConfiguration Configuration { get; }

        public virtual ParameterExpression BindingContext { get; }
    }
}
