using System;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using Handlebars.Core.Compiler.Structure;
#if netstandard
using System.Reflection;
#endif

namespace Handlebars.Core.Compiler.Translation.Expressions
{
    internal class SubExpressionVisitor : HandlebarsExpressionVisitor
    {
        public static Expression Visit(Expression expr, CompilationContext context)
        {
            return new SubExpressionVisitor(context).Visit(expr);
        }

        private SubExpressionVisitor(CompilationContext context)
            : base(context)
        {
        }

        protected override Expression VisitSubExpression(SubExpressionExpression subex)
        {
            var helperCall = subex.Expression as MethodCallExpression;
            if (helperCall == null)
            {
                throw new HandlebarsCompilerException("Sub-expression does not contain a converted MethodCall expression");
            }
            var helper = GetHelperDelegateFromMethodCallExpression(helperCall);
            return Expression.Call(
#if netstandard
                new Func<HandlebarsHelperV2, IHandlebarsEngine, object, object[], string>(CaptureTextWriterOutputFromHelper).GetMethodInfo(),
#else
                new Func<HandlebarsHelperV2, IHandlebarsEngine, object, object[], string>(CaptureTextWriterOutputFromHelper).Method,
#endif
                Expression.Constant(helper),
                Visit(helperCall.Arguments[0]),
                Visit(helperCall.Arguments[2]),
                Visit(helperCall.Arguments[3]));
        }

        private static HandlebarsHelperV2 GetHelperDelegateFromMethodCallExpression(MethodCallExpression helperCall)
        {
            object target = helperCall.Object;
            HandlebarsHelperV2 helper;
            if (target != null)
            {
                if (target is ConstantExpression)
                {
                    target = ((ConstantExpression)target).Value;
                }
                else
                {
                    throw new NotSupportedException("Helper method instance target must be reduced to a ConstantExpression");
                }
#if netstandard
                helper = (HandlebarsHelperV2)helperCall.Method.CreateDelegate(typeof(HandlebarsHelperV2), target);
#else
                helper = (HandlebarsHelperV2)Delegate.CreateDelegate(typeof(HandlebarsHelperV2), target, helperCall.Method);
#endif
            }
            else
            {
#if netstandard
                helper = (HandlebarsHelperV2)helperCall.Method.CreateDelegate(typeof(HandlebarsHelperV2));
#else
                helper = (HandlebarsHelperV2)Delegate.CreateDelegate(typeof(HandlebarsHelperV2), helperCall.Method);
#endif
            }
            return helper;
        }

        private static string CaptureTextWriterOutputFromHelper(
            HandlebarsHelperV2 helper,
            IHandlebarsEngine engine,
            object context,
            object[] arguments)
        {
            var builder = new StringBuilder();
            using (var writer = new StringWriter(builder))
            {
                helper(engine, writer, context, arguments);
            }
            return builder.ToString();
        }
    }
}

