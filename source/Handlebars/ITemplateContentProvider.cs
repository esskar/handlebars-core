namespace Handlebars.Core
{
    public interface ITemplateContentProvider
    {
        string GetTemplateContent(string templateName, string parentTemplateName = null);
    }
}
