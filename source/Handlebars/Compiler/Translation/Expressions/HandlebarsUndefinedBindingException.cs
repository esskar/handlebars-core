using System;

namespace Handlebars.Compiler.Translation.Expressions
{
    public class HandlebarsUndefinedBindingException : Exception
    {
        public HandlebarsUndefinedBindingException(string path, string missingKey) : base(missingKey + " is undefined")
        {
            Path = path;
            MissingKey = missingKey;
        }

        public string Path { get; set; }

        public string MissingKey { get; set; }
    }
}
