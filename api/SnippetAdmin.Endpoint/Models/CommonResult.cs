namespace SnippetAdmin.Endpoint.Models
{
    /// <summary>
    /// 通用action返回模型
    /// </summary>
    public partial record CommonResult
    {
        /// <summary>
        /// 业务处理结果
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 业务处理编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 处理结果消息
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// 通用action返回模型泛型
    /// </summary>
    public partial record CommonResult<T> : CommonResult where T : class
    {
        /// <summary>
        /// 处理返回数据
        /// </summary>
        public T Data { get; set; }
    }
}