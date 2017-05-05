using System.Collections.Generic;
using HandlebarsDotNet.Compiler.Lexer;

namespace HandlebarsDotNet.Compiler
{
    internal class StaticConverter : ITokenConverter
    {
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

