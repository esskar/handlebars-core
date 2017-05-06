using System.Linq.Expressions;
using System.Reflection;
using Handlebars.Core.Compiler.Structure;

namespace Handlebars.Core.Compiler.Translation.Expressions
{
    internal class BlockHelperFunctionBinder : HandlebarsExpressionVisitor
    {
        public static Expression Bind(Expression expr, CompilationContext context)
        {
            return new BlockHelperFunctionBinder(context).Visit(expr);
        }

        private BlockHelperFunctionBinder(CompilationContext context)
            : base(context)
        {
        }

        protected override Expression VisitStatementExpression(StatementExpression sex)
        {
            if (sex.Body is BlockHelperExpression)
            {
                return Visit(sex.Body);
            }
            return sex;
        }

        protected override Expression VisitBlockHelperExpression(BlockHelperExpression bhex)
        {
            var configuration = CompilationContext.Configuration;
            var fb = new FunctionBuilder(configuration);
            var body = fb.Compile(((BlockExpression)bhex.Body).Expressions, CompilationContext.BindingContext);
            var inversion = fb.Compile(((BlockExpression)bhex.Inversion).Expressions, CompilationContext.BindingContext);
            var helper = configuration.BlockHelpers[bhex.HelperName.Replace("#", "")];
            var arguments = new Expression[]
            {
                Expression.Constant(configuration, typeof(HandlebarsConfiguration)), 
                Expression.Property(
                    CompilationContext.BindingContext,
                    typeof(BindingContext).GetProperty("TextWriter")),
                Expression.New(
                        typeof(HelperOptions).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0],
                        body,
                        inversion),
                Expression.Property(
                    CompilationContext.BindingContext,
                    typeof(BindingContext).GetProperty("Value")),
                Expression.NewArrayInit(typeof(object), bhex.Arguments)
            };
            if (helper.Target != null)
            {
                return Expression.Call(
                    Expression.Constant(helper.Target),
#if netstandard
                    helper.GetMethodInfo(),
#else
                    helper.Method,
#endif
                    arguments);
            }
            return Expression.Call(
#if netstandard
                    helper.GetMethodInfo(),
#else
                helper.Method,
#endif
                arguments);
        }
    }
}

