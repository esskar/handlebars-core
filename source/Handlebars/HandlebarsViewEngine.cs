using System.Linq;

namespace HandlebarsDotNet
{

    public abstract class ViewEngineFileSystem
    {
        public abstract string GetFileContent(string filename);

        private static string GetDir(string currentFilePath)
        {
            if (string.IsNullOrWhiteSpace(currentFilePath))
                return null;
            var parts = currentFilePath.Split('\\', '/');
            return parts.Length == 1 
                ? "" 
                : string.Join("/", parts.Take(parts.Length - 1));
        }

        public string Closest(string filename, string otherFileName)
        {
            var dir = GetDir(filename);
            while (true)
            {
                if (dir == null)
                    break;
                var fullFileName = CombinePath(dir, otherFileName);
                if (FileExists(fullFileName))
                    return fullFileName;
                dir = GetDir(dir);
            }
            return null;
        }

        protected abstract string CombinePath(string dir, string otherFileName);

        public abstract bool FileExists(string filePath);
    }
}
