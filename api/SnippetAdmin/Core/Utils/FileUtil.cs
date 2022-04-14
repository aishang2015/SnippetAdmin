namespace SnippetAdmin.Core.Utils
{
    public static class FileUtil
    {
        public static void WriteToFile(string directory, string fileName, string fileContent, string filePath = "")
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = AppContext.BaseDirectory;
            }
            filePath = Path.Combine(filePath, directory);

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            var fullpath = Path.Combine(filePath, fileName);

            using (var file = new StreamWriter(File.Create(fullpath)))
            {
                file.Write(fileContent);
            }
        }
    }
}
