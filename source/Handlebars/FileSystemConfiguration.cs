namespace HandlebarsDotNet
{
    public class FileSystemConfiguration
    {
        public FileSystemConfiguration(string targetPath = null)
        {
            TargetPath = targetPath;
            FileNameExtension = ".hbs";
        }

        public string TargetPath { get; }

        public string FileNameExtension { get; set; }
    }
}
