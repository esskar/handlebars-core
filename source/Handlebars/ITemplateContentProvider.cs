namespace HandlebarsDotNet
{
    public interface ITemplateContentProvider
    {
        string GetTemplateContent(string templateName, string parentTemplateName = null);
    }
}
