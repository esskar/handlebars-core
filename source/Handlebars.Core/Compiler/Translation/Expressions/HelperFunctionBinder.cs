using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Handlebars.Core.Compiler.Structure;

#if netstandard
using System.Reflection;
#endif

namespace Handlebars.Core.Compiler.Translation.Expressions
{
    internal class HelperFunctionBinder : HandlebarsExpressionVisitor
    {
        public static Expression Bind(Expression expr, CompilationContext context)
        {
            return new HelperFunctionBinder(context).Visit(expr);
        }

        private HelperFunctionBinder(CompilationContext context)
            : base(context)
        {
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            return Expression.Block(
                node.Variables,
                node.Expressions.Select(Visit));
        }

        protected override Expression VisitStatementExpression(StatementExpression sex)
        {
            if (sex.Body is HelperExpression)
            {
                return Visit(sex.Body);
            }
            else
            {
                return sex;
            }
        }

        protected override Expression VisitBoolishExpression(BoolishExpression bex)
        {
            return HandlebarsExpression.BoolishExpression(Visit(bex.ConditionExpression));
        }

        protected override Expression VisitBlockHelperExpression(BlockHelperExpression bhex)
        {
            return HandlebarsExpression.BlockHelperExpression(
                bhex.HelperName,
                bhex.Arguments.Select(Visit),
                Visit(bhex.Body),
                Visit(bhex.Inversion));
        }

        protected override Expression VisitSubExpression(SubExpressionExpression subex)
        {
            return HandlebarsExpression.SubExpressionExpression(
                Visit(subex.Expression));
        }

        protected override Expression VisitHelperExpression(HelperExpression hex)
        {
            var engine = CompilationContext.Engine;
            var configuration = engine.Configuration;
            if (configuration.Helpers.TryGetValue(hex.HelperName, out HandlebarsHelperV2 helper))
            {
                var arguments = new Expression[]
                {
                    Expression.Constant(engine, typeof(IHandlebarsEngine)), 
                    Expression.Property(
                        CompilationContext.BindingContext,
#if netstandard
                        typeof(BindingContext).GetRuntimeProperty("TextWriter")),
#else
                        typeof(BindingContext).GetProperty("TextWriter")),
#endif
                    Expression.Property(
                        CompilationContext.BindingContext,
#if netstandard
                        typeof(BindingContext).GetRuntimeProperty("Value")),
#else
                        typeof(BindingContext).GetProperty("Value")),
#endif
                    Expression.NewArrayInit(typeof(object), hex.Arguments.Select(Visit))
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
                else
                {
                    return Expression.Call(
#if netstandard
                        helper.GetMethodInfo(),
#else
                        helper.Method,
#endif
                        arguments);
                }
            }
            return Expression.Call(
                Expression.Constant(this),
#if netstandard
                    new Action<BindingContext, string, IEnumerable<object>>(LateBindHelperExpression).GetMethodInfo(),
#else
                new Action<BindingContext, string, IEnumerable<object>>(LateBindHelperExpression).Method,
#endif
                CompilationContext.BindingContext,
                Expression.Constant(hex.HelperName),
                Expression.NewArrayInit(typeof(object), hex.Arguments));
        }

        private void LateBindHelperExpression(
            BindingContext context,
            string helperName,
            IEnumerable<object> arguments)
        {
            var configuration = CompilationContext.Engine.Configuration;
            if (configuration.Helpers.TryGetValue(helperName, out HandlebarsHelperV2 helper))
            {
                helper(CompilationContext.Engine, context.TextWriter, context.Value, arguments.ToArray());
            }
            else
            {
                throw new HandlebarsRuntimeException(
                    $"Template references a helper that is not registered. Could not find helper '{helperName}'");
            }
        }
    }
}
