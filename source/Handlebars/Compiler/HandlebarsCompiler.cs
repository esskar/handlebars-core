using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using HandlebarsDotNet.Compiler.Lexer;

namespace HandlebarsDotNet.Compiler
{
    internal class HandlebarsCompiler
    {
        private readonly Tokenizer _tokenizer;
        private readonly FunctionBuilder _functionBuilder;
        private readonly ExpressionBuilder _expressionBuilder;
        private readonly HandlebarsConfiguration _configuration;

        public HandlebarsCompiler(HandlebarsConfiguration configuration)
        {
            _configuration = configuration;
            _tokenizer = new Tokenizer();
            _expressionBuilder = new ExpressionBuilder(configuration);
            _functionBuilder = new FunctionBuilder(configuration);
        }

        public Action<TextWriter, object> Compile(TextReader source)
        {
            var tokens = _tokenizer.Tokenize(source).ToList();
            var expressions = _expressionBuilder.ConvertTokensToExpressions(tokens);
            return _functionBuilder.Compile(expressions);
        }

        internal Action<TextWriter, object> CompileView(string templatePath)
        {
            var fs = _configuration.FileSystem;
            if (fs == null)
                throw new InvalidOperationException("Cannot compile view when configuration.FileSystem is not set");

            var template = fs.GetFileContent(templatePath);
            if (template == null)
                throw new InvalidOperationException("Cannot find template at '" + templatePath + "'");

            IEnumerable<object> tokens;
            using (var sr = new StringReader(template))
            {
                tokens = _tokenizer.Tokenize(sr).ToList();
            }
            var layoutToken = tokens.OfType<LayoutToken>().SingleOrDefault();

            var expressions = _expressionBuilder.ConvertTokensToExpressions(tokens);
            var compiledView = _functionBuilder.Compile(expressions, templatePath);
            if (layoutToken == null)
                return compiledView;

            var layoutPath = fs.Closest(templatePath, layoutToken.Value + ".hbs");
            if (layoutPath == null)
                throw new InvalidOperationException("Cannot find layout path for template '" + templatePath + "'");

            var compiledLayout = CompileView(layoutPath);
            return (tw, vm) =>
            {
                var sb = new StringBuilder();
                using (var innerWriter = new StringWriter(sb))
                {
                    compiledView(innerWriter, vm);
                }
                var inner = sb.ToString();
                compiledLayout(tw, new DynamicViewModel(new { body = inner }));
            };
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

