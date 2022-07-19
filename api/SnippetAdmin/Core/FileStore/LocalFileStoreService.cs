using Microsoft.Extensions.Options;

namespace SnippetAdmin.Core.FileStore
{
    public class LocalFileStoreService : IFileStoreService
    {
        private readonly FileStoreOption option;

        private readonly string _basePath;

        public LocalFileStoreService(IOptions<FileStoreOption> options)
        {
            option = options.Value;
            _basePath = option.IsAbsolute ? option.BasePath :
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, option.BasePath);
        }

        public Task CopyFileAsync(string sourcePath, string targetPath)
        {
            var sourceFullPath = GetFullPath(sourcePath);
            if (!File.Exists(sourceFullPath))
            {
                throw new FileStoreException("文件不存在！");
            }
            var targetFullPath = GetFullPath(targetPath);
            CheckDirectory(targetFullPath);
            File.Copy(sourceFullPath, targetFullPath);
            return Task.CompletedTask;
        }

        public Task DeleteFileAsync(string path = null)
        {
            var fullPath = GetFullPath(path);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            return Task.CompletedTask;
        }

        public async Task<byte[]> GetFileContentsAsync(string path = null)
        {
            var fullPath = GetFullPath(path);
            if (!File.Exists(fullPath))
            {
                throw new FileStoreException("文件不存在！");
            }
            return await File.ReadAllBytesAsync(fullPath);
        }

        public Task<Stream> GetFileStreamAsync(string path = null)
        {
            var fullPath = GetFullPath(path);
            if (!File.Exists(fullPath))
            {
                throw new FileStoreException("文件不存在！");
            }
            var stream = File.OpenRead(fullPath);
            return Task.FromResult<Stream>(stream);
        }

        public async Task SaveFromContentsAsync(byte[] contents, string path = null)
        {
            var fullPath = GetFullPath(path);
            if (File.Exists(fullPath))
            {
                throw new FileStoreException("文件已存在！");
            }
            CheckDirectory(fullPath);
            var fileInfo = new FileInfo(fullPath);
            using var outputStream = fileInfo.Create();
            await outputStream.WriteAsync(contents);
        }

        public async Task SaveFromStreamAsync(Stream stream, string path = null)
        {
            var fullPath = GetFullPath(path);
            if (File.Exists(fullPath))
            {
                throw new FileStoreException("文件已存在！");
            }
            CheckDirectory(fullPath);
            var fileInfo = new FileInfo(fullPath);
            using var outputStream = fileInfo.Create();
            await stream.CopyToAsync(outputStream);
        }

        private string GetFullPath(string path) => Path.Join(_basePath, path);

        private void CheckDirectory(string fullPath)
        {
            var directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}
