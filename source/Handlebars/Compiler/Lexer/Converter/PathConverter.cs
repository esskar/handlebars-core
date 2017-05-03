using System.Collections.Generic;
using HandlebarsDotNet.Compiler.Lexer;
using System.Linq;

namespace HandlebarsDotNet.Compiler
{
    internal class PathConverter : ITokenConverter
    {
        public static IEnumerable<object> Convert(IEnumerable<object> sequence)
        {
            return new PathConverter().ConvertTokens(sequence).ToList();
        }

        private PathConverter()
        {
        }

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

