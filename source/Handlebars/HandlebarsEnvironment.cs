using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet.Compiler;

namespace HandlebarsDotNet
{
    internal class HandlebarsEnvironment : IHandlebars
    {
        private readonly HandlebarsCompiler _compiler;

        public HandlebarsEnvironment(HandlebarsConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            
            Configuration = configuration;
            _compiler = new HandlebarsCompiler(Configuration);
            RegisterBuiltinHelpers();
        }

        public HandlebarsConfiguration Configuration { get; }

        public Func<object, string> CompileView(string templatePath)
        {
            var compiledView = _compiler.CompileView(templatePath);
            return vm =>
            {
                var sb = new StringBuilder();
                using (var tw = new StringWriter(sb))
                {
                    compiledView(tw, vm);
                }
                return sb.ToString();
            };
        }

        public Action<TextWriter, object> Compile(TextReader template)
        {
            return _compiler.Compile(template);
        }

        public Func<object, string> Compile(string template)
        {
            using (var reader = new StringReader(template))
            {
                var compiledTemplate = Compile(reader);
                return context =>
                {
                    var builder = new StringBuilder();
                    using (var writer = new StringWriter(builder))
                    {
                        compiledTemplate(writer, context);
                    }
                    return builder.ToString();
                };
            }
        }

        public void RegisterTemplate(string templateName, Action<TextWriter, object> template)
        {
            Configuration.RegisteredTemplates.AddOrUpdate(templateName, template);
        }

        public void RegisterTemplate(string templateName, string template)
        {
            using (var reader = new StringReader(template))
            {
                RegisterTemplate(templateName, Compile(reader));
            }
        }

        public void RegisterHelper(string helperName, HandlebarsHelper helperFunction)
        {
            Configuration.Helpers.AddOrUpdate(helperName, n => helperFunction, (n, h) => helperFunction);
        }

        public void RegisterHelper(string helperName, HandlebarsBlockHelper helperFunction)
        {
            Configuration.BlockHelpers.AddOrUpdate(helperName, n => helperFunction, (n, h) => helperFunction);
        }

        private void RegisterBuiltinHelpers()
        {
            foreach (var helperDefinition in BuiltinHelpers.Helpers)
            {
                RegisterHelper(helperDefinition.Key, helperDefinition.Value);
            }
            foreach (var helperDefinition in BuiltinHelpers.BlockHelpers)
            {
                RegisterHelper(helperDefinition.Key, helperDefinition.Value);
            }
        }
    }
}
