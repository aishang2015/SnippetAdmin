namespace SnippetAdmin.Core.FileStore
{
    public class FileStoreOption
    {
        /// <summary>
        /// 是否是绝对路径
        /// </summary>
        public bool IsAbsolute { get; set; }

        /// <summary>
        /// 基本路径
        /// </summary>
        public string BasePath { get; set; }

        /// <summary>
        /// 限制上传文件大小（MB）
        /// </summary>
        public int MaxSize { get; set; }
    }
}
