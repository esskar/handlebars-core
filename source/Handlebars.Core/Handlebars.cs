using System;
using System.IO;

namespace Handlebars.Core
{
    public delegate void HandlebarsHelper(TextWriter output, dynamic context, params object[] arguments);
    public delegate void HandlebarsBlockHelper(TextWriter output, HelperOptions options, dynamic context, params object[] arguments);

    public sealed class Handlebars
    {
        // Lazy-load Handlebars environment to ensure thread safety.  See Jon Skeet's excellent article on this for more info. http://csharpindepth.com/Articles/General/Singleton.aspx
        private static readonly Lazy<IHandlebars> Lazy = new Lazy<IHandlebars>(() => new HandlebarsEnvironment(new HandlebarsConfiguration()));

        private static IHandlebars Instance => Lazy.Value;

        public static IHandlebars Create(HandlebarsConfiguration configuration = null)
        {
            configuration = configuration ?? new HandlebarsConfiguration();
            return new HandlebarsEnvironment(configuration);
        }

        public static HandlebarsTemplate Compile(TextReader template)
        {
            return Instance.Compile(template);
        }

        public static HandlebarsTemplate Compile(string template)
        {
            return Instance.Compile(template);
        }

        public static void RegisterTemplate(string templateName, HandlebarsTemplate template)
        {
            Instance.RegisterTemplate(templateName, template);
        }

        public static void RegisterTemplate(string templateName, string template)
        {
            Instance.RegisterTemplate(templateName, template);
        }

        public static void RegisterHelper(string helperName, HandlebarsHelper helperFunction)
        {
            Instance.RegisterHelper(helperName, helperFunction);
        }

        public static void RegisterHelper(string helperName, HandlebarsBlockHelper helperFunction)
        {
            Instance.RegisterHelper(helperName, helperFunction);
        }

        /// <summary>
        /// Expose the configuration on order to have access in all Helpers and Templates.
        /// </summary>
        public static HandlebarsConfiguration Configuration => Instance.Configuration;
    }
}