using System.IO;

namespace Handlebars.Core.Internals
{
    internal class WithHelper : IHandlebarsBlockHelper
    {
        public string Name => "with";

        public void Execute(HandlebarsConfiguration configuration, TextWriter output, HandlebarsBlockHelperOptions options,
            dynamic context, params object[] arguments)
        {
            if (arguments.Length != 1)
            {
                throw new HandlebarsException("{{with}} helper must have exactly one argument");
            }

            if (HandlebarsUtils.IsTruthyOrNonEmpty(arguments[0]))
            {
                options.Template(output, arguments[0]);
            }
            else
            {
                options.Inverse(output, context);
            }
        }
    }
}
