using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Handlebars.Core.Compiler.Translation.Expressions;

namespace Handlebars.Core.Compiler
{
    internal class FunctionBuilder
    {
        private readonly IHandlebarsEngine _engine;

        private static readonly Expression EmptyLambda =
            Expression.Lambda<Action<TextWriter, object>>(
                Expression.Empty(),
                Expression.Parameter(typeof(TextWriter)),
                Expression.Parameter(typeof(object)));

        public FunctionBuilder(IHandlebarsEngine engine)
        {
            _engine = engine;
        }

        public Expression Compile(IEnumerable<Expression> expressions, Expression parentContext, string templateName = null)
        {
            try
            {
                var expressionsList = expressions as IList<Expression> ?? expressions.ToList();

                if (expressionsList.Count == 0)
                {
                    return EmptyLambda;
                }
                if (expressionsList.Count == 1 && expressionsList[0] is DefaultExpression)
                {
                    return EmptyLambda;
                }
                var compilationContext = new CompilationContext(_engine);
                var expression = CreateExpressionBlock(expressionsList);
                expression = CommentVisitor.Visit(expression, compilationContext);
                expression = UnencodedStatementVisitor.Visit(expression, compilationContext);
                expression = PartialBinder.Bind(expression, compilationContext);
                expression = StaticReplacer.Replace(expression, compilationContext);
                expression = IteratorBinder.Bind(expression, compilationContext);
                expression = BlockHelperFunctionBinder.Bind(expression, compilationContext);
                expression = DeferredSectionVisitor.Bind(expression, compilationContext);
                expression = HelperFunctionBinder.Bind(expression, compilationContext);
                expression = BoolishConverter.Convert(expression, compilationContext);
                expression = PathBinder.Bind(expression, compilationContext);
                expression = SubExpressionVisitor.Visit(expression, compilationContext);
                expression = ContextBinder.Bind(expression, compilationContext, parentContext, templateName);
                return expression;
            }
            catch (Exception ex)
            {
                throw new HandlebarsCompilerException("An unhandled exception occurred while trying to compile the template.", ex);
            }
        }

        public Action<TextWriter, object> Compile(IEnumerable<Expression> expressions, string templateName = null)
        {
            try
            {
                var expression = Compile(expressions, null, templateName);
                return ((Expression<Action<TextWriter, object>>)expression).Compile();
            }
            catch (Exception ex)
            {
                throw new HandlebarsCompilerException("An unhandled exception occurred while trying to compile the template.", ex);
            }
        }


        private static Expression CreateExpressionBlock(IEnumerable<Expression> expressions)
        {
            return Expression.Block(expressions);
        }
    }
}

