using System.IO;

namespace Handlebars.Core
{
    public delegate void HandlebarsHelper(TextWriter output, dynamic context, params object[] arguments);
    public delegate void HandlebarsHelperV2(HandlebarsConfiguration configuration, TextWriter output, dynamic context, params object[] arguments);


    public interface IHandlebarsHelper
    {
        void Execute(HandlebarsConfiguration configuration, TextWriter output, dynamic context, params object[] arguments);
    }
}
