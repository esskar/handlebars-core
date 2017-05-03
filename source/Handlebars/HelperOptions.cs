using System;
using System.IO;

namespace HandlebarsDotNet
{
    public sealed class HelperOptions
    {
        internal HelperOptions(
            Action<TextWriter, object> template,
            Action<TextWriter, object> inverse)
        {
            Template = template;
            Inverse = inverse;
        }

        public Action<TextWriter, object> Template { get; }

        public Action<TextWriter, object> Inverse { get; }
    }
}

