using System;
using System.Collections.Concurrent;
using Handlebars.Core.Compiler.Resolvers;
using Handlebars.Core.Encoders;
using Handlebars.Core.Internals;
using Handlebars.Core.Logging;

namespace Handlebars.Core
{
    public class HandlebarsConfiguration
    {
        public ConcurrentDictionary<string, HandlebarsHelperV2> Helpers { get; }

        public ConcurrentDictionary<string, HandlebarsBlockHelperV2> BlockHelpers { get; }

        public IHandlebarsTemplateRegistry HandlebarsTemplateRegistry { get; }

        public IExpressionNameResolver ExpressionNameResolver { get; set; }

        public ILogger Logger { get; set; }

        public LogLevel LogLevel { get; set; } = LogLevel.Info;

        public IHandlebarsTemplateContentProvider TemplateContentProvider { get; set; }

        public ITextEncoder TextEncoder { get; set; }

	    public string UnresolvedBindingFormatter { get; set; }

	    public bool ThrowOnUnresolvedBindingExpression { get; set; }

        public HandlebarsConfiguration()
            : this(new TemplateRegistry())
        {
        }

	    public HandlebarsConfiguration(IHandlebarsTemplateRegistry templateRegistry)
        {
            Helpers = new ConcurrentDictionary<string, HandlebarsHelperV2>(StringComparer.OrdinalIgnoreCase);
            BlockHelpers = new ConcurrentDictionary<string, HandlebarsBlockHelperV2>(StringComparer.OrdinalIgnoreCase);
            HandlebarsTemplateRegistry = templateRegistry;
            TextEncoder = new HtmlEncoder();
	        ThrowOnUnresolvedBindingExpression = false;
        }
    }
}

