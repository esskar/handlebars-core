using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Handlebars.Core.Compiler.Lexer;
using Handlebars.Core.Compiler.Lexer.Tokens;

namespace Handlebars.Core.Compiler
{
    internal class HandlebarsCompiler
    {
        private readonly Tokenizer _tokenizer;
        private readonly FunctionBuilder _functionBuilder;
        private readonly TokenExpressionConverter _tokenExpressionConverter;
        private readonly IHandlebarsEngine _engine;

        public HandlebarsCompiler(IHandlebarsEngine engine)
        {
            _tokenizer = new Tokenizer();
            _tokenExpressionConverter = new TokenExpressionConverter(engine.Configuration);
            _functionBuilder = new FunctionBuilder(engine);
            _engine = engine;
        }

        public Action<TextWriter, object> Compile(TextReader source)
        {
            var tokens = _tokenizer.Tokenize(source).ToList();
            var expressions = _tokenExpressionConverter.ConvertTokensToExpressions(tokens);
            return _functionBuilder.Compile(expressions);
        }

        internal HandlebarsTemplate CompileView(string templateName, string parentTemplateName = null, bool throwOnErrors = true)
        {
            var templateLocator = _engine.Configuration.TemplateContentProvider;
            if (templateLocator == null)
            {
                if (throwOnErrors)
                    throw new InvalidOperationException("Cannot compile view when configuration.TemplateLocator is not set");
                return null;
            }
            var templateContent = templateLocator.GetTemplateContent(templateName, parentTemplateName);
            if (templateContent == null)
            {
                if (throwOnErrors)
                    throw new InvalidOperationException("Cannot find template content for templateName '" + templateName + "'");
                return null;
            }
            IEnumerable<object> tokens;
            using (var sr = new StringReader(templateContent))
            {
                tokens = _tokenizer.Tokenize(sr).ToList();
            }
            var layoutToken = tokens.OfType<LayoutToken>().SingleOrDefault();

            var expressions = _tokenExpressionConverter.ConvertTokensToExpressions(tokens);
            var compiledView = _functionBuilder.Compile(expressions, templateName);
            var compiledTemplate = new HandlebarsTemplate(compiledView, templateName);
            if (layoutToken == null)
                return compiledTemplate;

            var compiledLayout = CompileView(layoutToken.Value, templateName);
            return new HandlebarsTemplate((tw, vm) =>
            {
                var sb = new StringBuilder();
                using (var innerWriter = new StringWriter(sb))
                {
                    compiledTemplate.RenderTo(innerWriter, vm);
                }
                var inner = sb.ToString();
                compiledLayout.RenderTo(tw, new DynamicViewModel(new { body = inner }));
            });
        }

        internal class DynamicViewModel : DynamicObject
        {
            private readonly object[] _objects;
            private const BindingFlags BindingFlags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase;

            public DynamicViewModel(params object[] objects)
            {
                _objects = objects;
            }

            public override IEnumerable<string> GetDynamicMemberNames()
            {
                return _objects.Select(o => o.GetType())
                    .SelectMany(t => t.GetMembers(BindingFlags))
                    .Select(m => m.Name);
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = null;
                foreach (var target in _objects)
                {
                    var member = target.GetType().GetMember(binder.Name, BindingFlags);
                    if (member.Length > 0)
                    {
                        if (member[0] is PropertyInfo propertyInfo)
                        {
                            result = propertyInfo.GetValue(target, null);
                            return true;
                        }
                        if (member[0] is FieldInfo fieldInfo)
                        {
                            result = fieldInfo.GetValue(target);
                            return true;
                        }
                    }
                }
                return false;
            }
        }
    }
}