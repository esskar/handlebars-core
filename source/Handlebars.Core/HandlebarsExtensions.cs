using System.IO;
using Handlebars.Core.Internals;

namespace Handlebars.Core.Internals
{
}

namespace Handlebars.Core
{
    public static class HandlebarsExtensions
    {
        public static void WriteSafeString(this TextWriter writer, string value)
        {
            writer.Write(new SafeString(value));
        }

        public static void WriteSafeString(this TextWriter writer, object value)
        {
            writer.WriteSafeString(value.ToString());
        }
    }
}