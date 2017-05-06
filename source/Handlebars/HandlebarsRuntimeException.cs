using System;

namespace Handlebars.Core
{
    public class HandlebarsRuntimeException : HandlebarsException
    {
        public HandlebarsRuntimeException(string message)
            : base(message)
        {
        }

        public HandlebarsRuntimeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
