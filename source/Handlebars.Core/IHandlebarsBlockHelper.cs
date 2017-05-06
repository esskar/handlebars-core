using System.IO;

namespace Handlebars.Core
{
    public delegate void HandlebarsBlockHelper(TextWriter output, HelperOptions options, dynamic context, params object[] arguments);
    public delegate void HandlebarsBlockHelperV2(HandlebarsConfiguration configuration, TextWriter output, HelperOptions options, dynamic context, params object[] arguments);

    public interface IHandlebarsBlockHelper
    {
        void Execute(HandlebarsConfiguration configuration, TextWriter output, HelperOptions options, dynamic context, params object[] arguments);
    }
}
