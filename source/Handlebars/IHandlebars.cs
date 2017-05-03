using System;
using System.IO;

namespace HandlebarsDotNet
{
    public interface IHandlebars
    {
        HandlebarsConfiguration Configuration { get; }

        Action<TextWriter, object> Compile(TextReader template);

        Func<object, string> Compile(string template);

        Func<object, string> CompileView(string templatePath);

        void RegisterTemplate(string templateName, Action<TextWriter, object> template);

        void RegisterTemplate(string templateName, string template);

        void RegisterHelper(string helperName, HandlebarsHelper helperFunction);

        void RegisterHelper(string helperName, HandlebarsBlockHelper helperFunction);
    }
}

