using System.Collections.Generic;
using HandlebarsDotNet.Compiler.Lexer;
using System.Linq;

namespace HandlebarsDotNet.Compiler
{
    internal class StaticConverter : ITokenConverter
    {
        public static IEnumerable<object> Convert(IEnumerable<object> sequence)
        {
            return new StaticConverter().ConvertTokens(sequence).ToList();
        }

        private StaticConverter()
        {
        }

        public IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            foreach (var item in sequence)
            {
                if (item is StaticToken staticToken)
                {
                    if (staticToken.Value != string.Empty)
                    {
                        yield return HandlebarsExpression.StaticExpression(((StaticToken)item).Value);
                    }
                }
                else
                {
                    yield return item;
                }
            }
        }
    }
}

