using System;
using System.Collections.Concurrent;
using HandlebarsDotNet.Compiler.Resolvers;

namespace HandlebarsDotNet
{
    public class HandlebarsConfiguration
    {
        public ConcurrentDictionary<string, HandlebarsHelper> Helpers { get; private set; }

        public ConcurrentDictionary<string, HandlebarsBlockHelper> BlockHelpers { get; private set; }

        public HandlebarsTemplateRegistration RegisteredTemplates { get; private set; }

        public IExpressionNameResolver ExpressionNameResolver { get; set; }

        public ITextEncoder TextEncoder { get; set; }

        public ViewEngineFileSystem FileSystem { get; set; }

	    public string UnresolvedBindingFormatter { get; set; }

	    public bool ThrowOnUnresolvedBindingExpression { get; set; }

	    public HandlebarsConfiguration()
        {
            Helpers = new ConcurrentDictionary<string, HandlebarsHelper>(StringComparer.OrdinalIgnoreCase);
            BlockHelpers = new ConcurrentDictionary<string, HandlebarsBlockHelper>(StringComparer.OrdinalIgnoreCase);
            RegisteredTemplates = new HandlebarsTemplateRegistration();
            TextEncoder = new HtmlEncoder();
	        ThrowOnUnresolvedBindingExpression = false;
        }
    }
}

