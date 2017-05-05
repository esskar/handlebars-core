using System.Collections.Generic;
using Handlebars.Compiler.Lexer.Tokens;
using Handlebars.Compiler.Structure;

namespace Handlebars.Compiler.Lexer.Converter
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

