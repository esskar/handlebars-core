using System;
using System.Collections.Concurrent;

namespace HandlebarsDotNet
{
    public class TemplateRegistration : ITemplateRegistration
    {
        private readonly ConcurrentDictionary<string, HandlebarsTemplate> _registeredTemplates;

        public TemplateRegistration()
        {
            _registeredTemplates = new ConcurrentDictionary<string, HandlebarsTemplate>(StringComparer.OrdinalIgnoreCase);
        }

        public void RegisterTemplate(string templateName, HandlebarsTemplate template)
        {
            _registeredTemplates.AddOrUpdate(templateName, n => template, (n, t) => template);
        }

        public bool TryGetTemplate(string templateName, out HandlebarsTemplate template)
        {
            return _registeredTemplates.TryGetValue(templateName, out template);
        }
    }
}
