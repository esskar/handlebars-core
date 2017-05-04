using System;
using System.Collections.Concurrent;
using HandlebarsDotNet.Compiler.Resolvers;

namespace HandlebarsDotNet
{
    public class HandlebarsConfiguration
    {
        public ConcurrentDictionary<string, HandlebarsHelper> Helpers { get; }

        public ConcurrentDictionary<string, HandlebarsBlockHelper> BlockHelpers { get; }

        public ITemplateRegistration TemplateRegistration { get; }

        public IExpressionNameResolver ExpressionNameResolver { get; set; }

        public ITemplateContentProvider TemplateContentProvider { get; set; }

        public ITextEncoder TextEncoder { get; set; }

	    public string UnresolvedBindingFormatter { get; set; }

	    public bool ThrowOnUnresolvedBindingExpression { get; set; }

        public HandlebarsConfiguration()
            : this(new TemplateRegistration())
        {
        }

	    public HandlebarsConfiguration(ITemplateRegistration templateRegistration)
        {
            Helpers = new ConcurrentDictionary<string, HandlebarsHelper>(StringComparer.OrdinalIgnoreCase);
            BlockHelpers = new ConcurrentDictionary<string, HandlebarsBlockHelper>(StringComparer.OrdinalIgnoreCase);
            TemplateRegistration = templateRegistration;
            TextEncoder = new HtmlEncoder();
	        ThrowOnUnresolvedBindingExpression = false;
        }
    }
}

