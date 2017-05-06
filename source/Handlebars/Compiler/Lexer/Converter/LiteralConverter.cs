using System.Collections.Generic;
using System.Linq.Expressions;
using Handlebars.Core.Compiler.Lexer.Tokens;

namespace Handlebars.Core.Compiler.Lexer.Converter
{
    internal class LiteralConverter : ITokenConverter
    {
        public IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            foreach (var item in sequence)
            {
                var literalExpressionToken = item as LiteralExpressionToken;
                if (literalExpressionToken != null)
                {
                    yield return Expression.Constant(literalExpressionToken.Value);
                }
                else
                {
                    yield return item;
                }
            }
        }
    }
}

