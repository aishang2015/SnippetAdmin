namespace SnippetAdmin.Endpoint.Models
{
    public partial record CommonResult
    {
        public static CommonResult Success() => new CommonResult
        {
            IsSuccess = true
        };

        public static CommonResult Success(string code, string message) => new CommonResult
        {
            IsSuccess = true,
            Code = code,
            Message = message
        };

        public static CommonResult Success((string, string) codeMessage) => new CommonResult
        {
            IsSuccess = true,
            Code = codeMessage.Item1,
            Message = codeMessage.Item2
        };

        public static CommonResult Fail(string code, string message) => new CommonResult
        {
            IsSuccess = false,
            Code = code,
            Message = message
        };

        public static CommonResult Fail((string, string) codeMessage) => new CommonResult
        {
            IsSuccess = false,
            Code = codeMessage.Item1,
            Message = codeMessage.Item2
        };
        //}

        //public partial record CommonResult<T> : CommonResult where T : class
        //{
        /// <summary>
        /// 生成成功消息(不带消息内容)
        /// </summary>
        public static CommonResult<TData> Success<TData>(TData data)
            where TData : class => new CommonResult<TData>
            {
                IsSuccess = true,
                Data = data
            };


        /// <summary>
        /// 生成成功消息
        /// </summary>
        public static CommonResult<TData> Success<TData>(string code, string message, TData data)
            where TData : class => new CommonResult<TData>
            {
                IsSuccess = true,
                Code = code,
                Message = message,
                Data = data
            };


        /// <summary>
        /// 生成成功消息
        /// </summary>
        public static CommonResult<TData> Success<TData>((string, string) codeMessage, TData data)
            where TData : class => new CommonResult<TData>
            {
                IsSuccess = true,
                Code = codeMessage.Item1,
                Message = codeMessage.Item2,
                Data = data
            };


        /// <summary>
        /// 生成失败消息
        /// </summary>
        public static CommonResult<TData> Fail<TData>(string code, string message, TData data)
            where TData : class => new CommonResult<TData>
            {
                IsSuccess = false,
                Code = code,
                Message = message,
                Data = data
            };

    }
}