using System;
using System.Collections.Concurrent;
using System.IO;

namespace HandlebarsDotNet
{
    public class HandlebarsTemplateRegistration
    {
        private readonly ConcurrentDictionary<string, Action<TextWriter, object>> _registeredTemplates;

        public HandlebarsTemplateRegistration()
        {
            _registeredTemplates = new ConcurrentDictionary<string, Action<TextWriter, object>>(StringComparer.OrdinalIgnoreCase);
        }

        public void AddOrUpdate(string templateName, Action<TextWriter, object> template)
        {
            _registeredTemplates.AddOrUpdate(templateName, n => template, (n, t) => template);
        }

        public bool TryGetTemplate(string templateName, out Action<TextWriter, object> template)
        {
            return _registeredTemplates.TryGetValue(templateName, out template);
        }
    }
}
