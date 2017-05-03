﻿using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet.Compiler.Lexer;

namespace HandlebarsDotNet.Compiler
{
    internal class CommentAndLayoutConverter : ITokenConverter
    {
        public static IEnumerable<object> Convert(IEnumerable<object> sequence)
        {
            return new CommentAndLayoutConverter().ConvertTokens(sequence).ToList();
        }

        private CommentAndLayoutConverter()
        {
        }

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

