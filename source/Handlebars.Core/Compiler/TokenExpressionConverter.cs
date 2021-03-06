﻿using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Handlebars.Core.Compiler.Lexer.Converter;

namespace Handlebars.Core.Compiler
{
    internal class TokenExpressionConverter
    {
        private readonly List<ITokenConverter> _tokenConverters;

        public TokenExpressionConverter(HandlebarsConfiguration configuration)
        {
            _tokenConverters = new List<ITokenConverter>
            {
                new CommentAndLayoutConverter(),
                new LiteralConverter(),
                new HelperConverter(configuration),
                new HashParametersConverter(),
                new PathConverter(),
                new SubExpressionConverter(),
                new PartialConverter(),
                new HelperArgumentAccumulator(),
                new ExpressionScopeConverter(),
                new WhitespaceRemover(),
                new StaticConverter(),
                new BlockAccumulator(configuration)
            };
        }

        public IEnumerable<Expression> ConvertTokensToExpressions(IEnumerable<object> tokens)
        {
            foreach (var tokenConverter in _tokenConverters)
            {
                tokens = tokenConverter.ConvertTokens(tokens);
                tokens = tokens as IList<object> ?? tokens.ToList();
            }
            return tokens.Cast<Expression>();
        }
    }
}
