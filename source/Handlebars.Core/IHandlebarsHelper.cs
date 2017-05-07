using System.IO;

namespace Handlebars.Core
{
    public delegate void HandlebarsHelper(TextWriter output, dynamic context, params object[] arguments);
    public delegate void HandlebarsHelperV2(IHandlebarsEngine engine, TextWriter output, dynamic context, params object[] arguments);


    public interface IHandlebarsHelper
    {
        string Name { get; }

        void Execute(IHandlebarsEngine engine, TextWriter output, dynamic context, params object[] arguments);
    }
}
