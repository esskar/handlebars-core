using System.Collections.Generic;
using System.Linq.Expressions;
using Handlebars.Core.Compiler.Lexer.Converter.BlockAccumulators;
using Handlebars.Core.Compiler.Structure;

namespace Handlebars.Core.Compiler.Lexer.Converter
{
    internal class BlockAccumulator : ITokenConverter
    {
        private readonly HandlebarsConfiguration _configuration;

        public BlockAccumulator(HandlebarsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            var enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = (Expression)enumerator.Current;
                var context = BlockAccumulatorContext.Create(item, _configuration);
                if (context != null)
                {
                    // item = UnwrapStatement(item);
                    yield return AccumulateBlock(enumerator, context);
                }
                else
                {
                    yield return item;
                }
            }
        }

        private Expression AccumulateBlock(
            IEnumerator<object> enumerator, 
            BlockAccumulatorContext context)
        {
            while (enumerator.MoveNext())
            {
                var item = (Expression)enumerator.Current;
                var innerContext = BlockAccumulatorContext.Create(item, _configuration);
                if (innerContext != null)
                {
                    context.HandleElement(AccumulateBlock(enumerator, innerContext));
                }
                else if (context.IsClosingElement(item))
                {
                    return context.GetAccumulatedBlock();
                }
                else
                {
                    context.HandleElement(item);
                }
            }
            throw new HandlebarsCompilerException(
                $"Reached end of template before block expression '{context.Name}' was closed");
        }

        private static Expression UnwrapStatement(Expression item)
        {
            var statementExpression = item as StatementExpression;
            return statementExpression != null ? statementExpression.Body : item;
        }
    }
}

