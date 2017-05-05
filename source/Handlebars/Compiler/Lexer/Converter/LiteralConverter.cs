using System.Collections.Generic;
using HandlebarsDotNet.Compiler.Lexer;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
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

