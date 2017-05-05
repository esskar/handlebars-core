using System.Collections.Generic;
using System.Linq;
using Handlebars.Compiler.Lexer.Tokens;
using Handlebars.Compiler.Structure;

namespace Handlebars.Compiler.Lexer.Converter
{
    internal class CommentAndLayoutConverter : ITokenConverter
    {
        public IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            return sequence.Select(Convert);
        }

        private static object Convert(object item)
        {
            if (item is CommentToken commentToken)
            {
                return HandlebarsExpression.CommentExpression(commentToken.Value);
            }
            if (item is LayoutToken)
            {
                return HandlebarsExpression.CommentExpression(null);
            }
            return item;
        }
    }
}

