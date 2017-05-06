using System;
using System.Collections.Concurrent;
using Handlebars.Core.Compiler.Resolvers;

namespace Handlebars.Core
{
    public class HandlebarsConfiguration
    {
        public ConcurrentDictionary<string, HandlebarsHelperV2> Helpers { get; }

        public ConcurrentDictionary<string, HandlebarsBlockHelperV2> BlockHelpers { get; }

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
            Helpers = new ConcurrentDictionary<string, HandlebarsHelperV2>(StringComparer.OrdinalIgnoreCase);
            BlockHelpers = new ConcurrentDictionary<string, HandlebarsBlockHelperV2>(StringComparer.OrdinalIgnoreCase);
            TemplateRegistration = templateRegistration;
            TextEncoder = new HtmlEncoder();
	        ThrowOnUnresolvedBindingExpression = false;
        }
    }
}

