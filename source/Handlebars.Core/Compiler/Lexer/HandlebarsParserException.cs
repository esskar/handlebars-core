using System;

namespace Handlebars.Core.Compiler.Lexer
{
    public class HandlebarsParserException : HandlebarsException
    {
        public HandlebarsParserException(string message)
            : base(message)
        {
        }

        public HandlebarsParserException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

