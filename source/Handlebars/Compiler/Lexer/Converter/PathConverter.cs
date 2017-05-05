using System.Collections.Generic;
using HandlebarsDotNet.Compiler.Lexer;

namespace HandlebarsDotNet.Compiler
{
    internal class PathConverter : ITokenConverter
    {
        public IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            foreach (var item in sequence)
            {
                if (item is WordExpressionToken wordExpressionToken)
                {
                    yield return HandlebarsExpression.PathExpression(wordExpressionToken.Value);
                }
                else
                {
                    yield return item;
                }
            }
        }
    }
}

