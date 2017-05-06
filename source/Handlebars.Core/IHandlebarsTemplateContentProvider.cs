namespace Handlebars.Core
{
    public interface IHandlebarsTemplateContentProvider
    {
        string GetTemplateContent(string templateName, string parentTemplateName = null);
    }
}
