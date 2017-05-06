using System;
using System.Linq;

namespace Handlebars.Core
{
    public abstract class FileSystemTemplateContentProvider : ITemplateContentProvider
    {
        protected FileSystemTemplateContentProvider(string targetPath = null)
            : this(new FileSystemConfiguration(targetPath)) { }

        protected FileSystemTemplateContentProvider(FileSystemConfiguration fileSystemConfiguration)
        {
            if (fileSystemConfiguration == null)
                throw new ArgumentNullException(nameof(fileSystemConfiguration));
            FileSystemConfiguration = fileSystemConfiguration;
        }

        public FileSystemConfiguration FileSystemConfiguration { get; }

        public string GetTemplateContent(string templateName, string parentTemplateName)
        {
            var fileName = templateName;
            if (!fileName.EndsWith(FileSystemConfiguration.FileNameExtension, StringComparison.OrdinalIgnoreCase))
                fileName += FileSystemConfiguration.FileNameExtension;

            string fullFileName;
            if (!string.IsNullOrWhiteSpace(parentTemplateName))
            {
                fullFileName = Closest(parentTemplateName, fileName) ??
                               Closest(parentTemplateName, "partials/" + fileName);
            }
            else if (!string.IsNullOrWhiteSpace(FileSystemConfiguration.TargetPath))
            {
                fullFileName = CombinePath(FileSystemConfiguration.TargetPath, fileName);
            }
            else
            {
                fullFileName = fileName;
            }

            return string.IsNullOrWhiteSpace(fullFileName) ? null : GetFileContent(fullFileName);
        }

        protected abstract string CombinePath(string dir, string otherFileName);

        protected abstract bool FileExists(string filePath);

        protected abstract string GetFileContent(string fileName);

        private string Closest(string fileName, string otherFileName)
        {
            var dir = GetDir(fileName);
            while (dir != null)
            {
                var fullFileName = CombinePath(dir, otherFileName);
                if (FileExists(fullFileName))
                    return fullFileName;
                dir = GetDir(dir);
            }
            return null;
        }

        private static string GetDir(string currentFilePath)
        {
            if (string.IsNullOrWhiteSpace(currentFilePath))
                return null;
            var parts = currentFilePath.Split('\\', '/');
            return parts.Length == 1
                ? ""
                : string.Join("/", parts.Take(parts.Length - 1));
        }
    }
}
