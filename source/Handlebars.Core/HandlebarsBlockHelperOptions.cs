using System;
using System.IO;

namespace Handlebars.Core
{
    public sealed class HandlebarsBlockHelperOptions
    {
        internal HandlebarsBlockHelperOptions(Action<TextWriter, object> template, Action<TextWriter, object> inverse)
        {
            Template = template;
            Inverse = inverse;
        }

        public Action<TextWriter, object> Template { get; }

        public Action<TextWriter, object> Inverse { get; }
    }
}

