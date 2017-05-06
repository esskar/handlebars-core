using System.IO;

namespace Handlebars.Core
{
    public delegate void HandlebarsBlockHelper(TextWriter output, HandlebarsBlockHelperOptions options, dynamic context, params object[] arguments);
    public delegate void HandlebarsBlockHelperV2(HandlebarsConfiguration configuration, TextWriter output, HandlebarsBlockHelperOptions options, dynamic context, params object[] arguments);

    public interface IHandlebarsBlockHelper
    {
        string Name { get; }

        void Execute(HandlebarsConfiguration configuration, TextWriter output, HandlebarsBlockHelperOptions options, dynamic context, params object[] arguments);
    }
}
