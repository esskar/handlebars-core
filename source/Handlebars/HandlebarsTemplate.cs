using System;
using System.IO;
using System.Text;

namespace HandlebarsDotNet
{
    public class HandlebarsTemplate
    {
        private readonly Action<TextWriter, object> _template;
        private Func<object, string> _renderer;

        public HandlebarsTemplate(Action<TextWriter, object> template, string templateName = null)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));
            _template = template;

            Name = templateName;
        }

        public string Name { get; }

        public Func<object, string> Renderer
        {
            get
            {
                return _renderer ?? (_renderer = data =>
                {
                    var sb = new StringBuilder();
                    using (var writer = new StringWriter(sb))
                    {
                        RenderTo(writer, data);
                    }
                    return sb.ToString();
                });
            }
        }

        public string Render(object data)
        {
            return Renderer(data);
        }

        public void RenderTo(TextWriter writer, object data)
        {
            _template(writer, data);
        }

        public static implicit operator HandlebarsTemplate(Action<TextWriter, object> template)
        {
            return new HandlebarsTemplate(template);
        }
    }
}