namespace SnippetAdmin.Core.FileStore
{
	public interface IFileStoreService
	{
		public Task SaveFromContentsAsync(byte[] contents, string path = null);

		public Task SaveFromStreamAsync(Stream stream, string path = null);

		public Task<byte[]> GetFileContentsAsync(string path = null);

		public Task<Stream> GetFileStreamAsync(string path = null);

		public Task DeleteFileAsync(string path = null);

		public Task CopyFileAsync(string sourcePath, string targetPath);
	}
}
