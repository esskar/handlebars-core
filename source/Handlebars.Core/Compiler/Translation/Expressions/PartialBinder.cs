using System;
using Handlebars.Core.Compiler.Structure;
#if netstandard
using System.Reflection;
#endif

namespace Handlebars.Core.Compiler.Translation.Expressions
{
    internal class PartialBinder : HandlebarsExpressionVisitor
    {
        public static System.Linq.Expressions.Expression Bind(System.Linq.Expressions.Expression expr, CompilationContext context)
        {
            return new PartialBinder(context).Visit(expr);
        }

        private PartialBinder(CompilationContext context)
            : base(context)
        {
        }

        protected override System.Linq.Expressions.Expression VisitStatementExpression(StatementExpression sex)
        {
            if (sex.Body is PartialExpression)
            {
                return Visit(sex.Body);
            }
            return sex;
        }

        protected override System.Linq.Expressions.Expression VisitPartialExpression(PartialExpression pex)
        {
            System.Linq.Expressions.Expression bindingContext = CompilationContext.BindingContext;
            if (pex.Argument != null)
            {
                bindingContext = System.Linq.Expressions.Expression.Call(
                    bindingContext,
                    typeof(BindingContext).GetMethod("CreateChildContext"),
                    pex.Argument);
            }

            var partialInvocation = System.Linq.Expressions.Expression.Call(
#if netstandard
                new Func<string, BindingContext, HandlebarsConfiguration, bool>(InvokePartial).GetMethodInfo(),
#else
                new Func<string, BindingContext, HandlebarsConfiguration, bool>(InvokePartial).Method,
#endif
                System.Linq.Expressions.Expression.Convert(pex.PartialName, typeof(string)),
                bindingContext,
                System.Linq.Expressions.Expression.Constant(CompilationContext.Configuration));

            var fallback = pex.Fallback;
            if (fallback == null)
            {
                fallback = System.Linq.Expressions.Expression.Call(
#if netstandard
                new Action<string>(HandleFailedInvocation).GetMethodInfo(),
#else
                new Action<string>(HandleFailedInvocation).Method,
#endif
                System.Linq.Expressions.Expression.Convert(pex.PartialName, typeof(string)));
            }

            return System.Linq.Expressions.Expression.IfThen(
                    System.Linq.Expressions.Expression.Not(partialInvocation),
                    fallback);
        }

        private static void HandleFailedInvocation(
            string partialName)
        {
            throw new HandlebarsRuntimeException(
                $"Referenced partial name {partialName} could not be resolved");
        }

        private static bool InvokePartial(
            string partialName,
            BindingContext context,
            HandlebarsConfiguration configuration)
        {
            if (!configuration.HandlebarsTemplateRegistry.TryGetTemplate(partialName, out HandlebarsTemplate template))
            {
                var partialLookupKey = $"{context.TemplateName ?? string.Empty}+{partialName}";
                if (!configuration.HandlebarsTemplateRegistry.TryGetTemplate(partialLookupKey, out template))
                {
                    var engine = new HandlebarsEngine(configuration);
                    template = engine.CompileView(partialName, context.TemplateName, false);
                    if (template == null)
                        return false;
                    configuration.HandlebarsTemplateRegistry.RegisterTemplate(partialLookupKey, template);
                }
                else
                {
                    return false;
                }
            }

            try
            {
                template.RenderTo(context.TextWriter, context);
                return true;
            }
            catch (Exception exception)
            {
                throw new HandlebarsRuntimeException(
                    $"Runtime error while rendering partial '{partialName}', see inner exception for more information",
                    exception);
            }

        }
    }
}

