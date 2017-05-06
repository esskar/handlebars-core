namespace Handlebars.Core
{
    public interface IHandlebarsTemplateRegistry
    {
        void RegisterTemplate(string templateName, HandlebarsTemplate template);

        bool TryGetTemplate(string templateName, out HandlebarsTemplate template);
    }
}
