namespace Handlebars.Core
{
    public interface ITemplateRegistration
    {
        void RegisterTemplate(string templateName, HandlebarsTemplate template);

        bool TryGetTemplate(string templateName, out HandlebarsTemplate template);
    }
}
