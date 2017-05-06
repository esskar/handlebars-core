using System.IO;

namespace Handlebars.Core
{
    public interface IHandlebarsEngine
    {
        HandlebarsConfiguration Configuration { get; }

        HandlebarsTemplate Compile(TextReader template);

        HandlebarsTemplate Compile(string template);

        HandlebarsTemplate CompileView(string templateName, string parentTemplateName = null, bool throwOnErrors = true);

        void RegisterTemplate(string templateName, HandlebarsTemplate template);

        void RegisterTemplate(string templateName, string template);

        void RegisterHelper(string helperName, HandlebarsHelper helperFunction);

        void RegisterHelper(string helperName, HandlebarsBlockHelper helperFunction);

        void RegisterHelper(string helperName, HandlebarsHelperV2 helperFunction);

        void RegisterHelper(string helperName, HandlebarsBlockHelperV2 helperFunction);

        void RegisterHelper(string helperName, IHandlebarsHelper helper);

        void RegisterHelper(string helperName, IHandlebarsBlockHelper helper);
    }
}

