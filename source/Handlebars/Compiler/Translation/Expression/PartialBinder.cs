using System;
using System.Linq.Expressions;
#if netstandard
using System.Reflection;
#endif

namespace HandlebarsDotNet.Compiler
{
    internal class PartialBinder : HandlebarsExpressionVisitor
    {
        public static Expression Bind(Expression expr, CompilationContext context)
        {
            return new PartialBinder(context).Visit(expr);
        }

        private PartialBinder(CompilationContext context)
            : base(context)
        {
        }

        protected override Expression VisitStatementExpression(StatementExpression sex)
        {
            if (sex.Body is PartialExpression)
            {
                return Visit(sex.Body);
            }
            return sex;
        }

        protected override Expression VisitPartialExpression(PartialExpression pex)
        {
            Expression bindingContext = CompilationContext.BindingContext;
            if (pex.Argument != null)
            {
                bindingContext = Expression.Call(
                    bindingContext,
                    typeof(BindingContext).GetMethod("CreateChildContext"),
                    pex.Argument);
            }

            var partialInvocation = Expression.Call(
#if netstandard
                new Func<string, BindingContext, HandlebarsConfiguration, bool>(InvokePartial).GetMethodInfo(),
#else
                new Func<string, BindingContext, HandlebarsConfiguration, bool>(InvokePartial).Method,
#endif
                Expression.Convert(pex.PartialName, typeof(string)),
                bindingContext,
                Expression.Constant(CompilationContext.Configuration));

            var fallback = pex.Fallback;
            if (fallback == null)
            {
                fallback = Expression.Call(
#if netstandard
                new Action<string>(HandleFailedInvocation).GetMethodInfo(),
#else
                new Action<string>(HandleFailedInvocation).Method,
#endif
                Expression.Convert(pex.PartialName, typeof(string)));
            }

            return Expression.IfThen(
                    Expression.Not(partialInvocation),
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
            if (!configuration.TemplateRegistration.TryGetTemplate(partialName, out HandlebarsTemplate template))
            {
                var partialLookupKey = string.Format("{0}+{1}", 
                    context.TemplateName ?? string.Empty, partialName);
                if (!configuration.TemplateRegistration.TryGetTemplate(partialLookupKey, out template))
                {
                    template = Handlebars.Create(configuration).CompileView(partialName, context.TemplateName, false);
                    if (template == null)
                        return false;
                    configuration.TemplateRegistration.RegisterTemplate(partialLookupKey, template);
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

