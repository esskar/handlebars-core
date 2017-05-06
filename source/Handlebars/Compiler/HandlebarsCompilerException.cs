using System;

namespace Handlebars.Core.Compiler
{
    public class HandlebarsCompilerException : HandlebarsException
    {
        public HandlebarsCompilerException(string message)
            : base(message)
        {
        }

        public HandlebarsCompilerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

