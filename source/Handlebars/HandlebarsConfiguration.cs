using System;
using System.Collections.Generic;
using System.IO;
using HandlebarsDotNet.Compiler.Resolvers;

namespace HandlebarsDotNet
{
    public class HandlebarsConfiguration
    {
        public IDictionary<string, HandlebarsHelper> Helpers { get; private set; }

        public IDictionary<string, HandlebarsBlockHelper> BlockHelpers { get; private set; }

        public IDictionary<string, Action<TextWriter, object>> RegisteredTemplates { get; private set; }

        public IExpressionNameResolver ExpressionNameResolver { get; set; }

        public ITextEncoder TextEncoder { get; set; }

        public ViewEngineFileSystem FileSystem { get; set; }

	    public string UnresolvedBindingFormatter { get; set; }

	    public bool ThrowOnUnresolvedBindingExpression { get; set; }

	    public HandlebarsConfiguration()
        {
            Helpers = new Dictionary<string, HandlebarsHelper>(StringComparer.OrdinalIgnoreCase);
            BlockHelpers = new Dictionary<string, HandlebarsBlockHelper>(StringComparer.OrdinalIgnoreCase);
            RegisteredTemplates = new Dictionary<string, Action<TextWriter, object>>(StringComparer.OrdinalIgnoreCase);
            TextEncoder = new HtmlEncoder();
	        ThrowOnUnresolvedBindingExpression = false;
        }
    }
}

