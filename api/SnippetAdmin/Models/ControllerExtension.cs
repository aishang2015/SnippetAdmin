using Microsoft.AspNetCore.Mvc;

namespace SnippetAdmin.Models
{
    public static class ControllerExtension
    {
        #region 生成CommonResult

        /// <summary>
        /// 生成成功消息(不带消息内容)
        /// </summary>
        public static CommonResult SuccessCommonResult(this ControllerBase controller)
        {
            return new CommonResult
            {
                IsSuccess = true
            };
        }

        /// <summary>
        /// 生成成功消息
        /// </summary>
        public static CommonResult SuccessCommonResult(this ControllerBase controller, string code, string message)
        {
            return new CommonResult
            {
                IsSuccess = true,
                Code = code,
                Message = message
            };
        }

        /// <summary>
        /// 生成成功消息
        /// </summary>
        public static CommonResult SuccessCommonResult(this ControllerBase controller, (string, string) codeMessage)
        {
            return new CommonResult
            {
                IsSuccess = true,
                Code = codeMessage.Item1,
                Message = codeMessage.Item2
            };
        }

        /// <summary>
        /// 生成失败消息
        /// </summary>
        public static CommonResult FailCommonResult(this ControllerBase controller, string code, string message)
        {
            return new CommonResult
            {
                IsSuccess = false,
                Code = code,
                Message = message
            };
        }

        /// <summary>
        /// 生成失败消息
        /// </summary>
        public static CommonResult FailCommonResult(this ControllerBase controller, (string, string) codeMessage)
        {
            return new CommonResult
            {
                IsSuccess = false,
                Code = codeMessage.Item1,
                Message = codeMessage.Item2
            };
        }

        #endregion 生成CommonResult

        #region 生成泛型CommonResult

        /// <summary>
        /// 生成成功消息(不带消息内容)
        /// </summary>
        public static CommonResult<TData> SuccessCommonResult<TData>(this ControllerBase controller, TData data)
            where TData : class
        {
            return new CommonResult<TData>
            {
                IsSuccess = true,
                Data = data
            };
        }

        /// <summary>
        /// 生成成功消息
        /// </summary>
        public static CommonResult<TData> SuccessCommonResult<TData>(this ControllerBase controller, string code, string message, TData data)
            where TData : class
        {
            return new CommonResult<TData>
            {
                IsSuccess = true,
                Code = code,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// 生成成功消息
        /// </summary>
        public static CommonResult<TData> SuccessCommonResult<TData>(this ControllerBase controller, (string, string) codeMessage, TData data)
            where TData : class
        {
            return new CommonResult<TData>
            {
                IsSuccess = true,
                Code = codeMessage.Item1,
                Message = codeMessage.Item2,
                Data = data
            };
        }

        /// <summary>
        /// 生成失败消息
        /// </summary>
        public static CommonResult<TData> FailCommonResult<TData>(this ControllerBase controller, string code, string message, TData data)
            where TData : class
        {
            return new CommonResult<TData>
            {
                IsSuccess = false,
                Code = code,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// 生成失败消息
        /// </summary>
        public static CommonResult<TData> FailCommonResult<TData>(this ControllerBase controller, (string, string) codeMessage)
            where TData : class
        {
            return new CommonResult<TData>
            {
                IsSuccess = false,
                Code = codeMessage.Item1,
                Message = codeMessage.Item2
            };
        }

        #endregion 生成泛型CommonResult
    }
}