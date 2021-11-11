using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace SnippetAdmin.Core.Method
{
    public static class DistributeCacheExtension
    {
        #region 异步方法

        #region int

        /// <summary>
        /// 设置数值
        /// </summary>
        public static async Task SetIntAsync(this IDistributedCache cache, string key, int t,
            DistributedCacheEntryOptions options = null)
        {
            await cache.SetStringAsync(key, t.ToString(), options);
        }

        /// <summary>
        /// 取得数值
        /// </summary>
        public static async Task<int?> GetIntAsync(this IDistributedCache cache, string key)
        {
            var data = await cache.GetStringAsync(key);
            return data is not null && int.TryParse(data, out var result) ?
                null : int.Parse(data);
        }

        #endregion int

        #region double

        /// <summary>
        /// 设置数值
        /// </summary>
        public static async Task SetDoubleAsync(this IDistributedCache cache, string key, double t,
            DistributedCacheEntryOptions options = null)
        {
            await cache.SetStringAsync(key, t.ToString(), options);
        }

        /// <summary>
        /// 取得数值
        /// </summary>
        public static async Task<double?> GetDoubleAsync(this IDistributedCache cache, string key)
        {
            var data = await cache.GetStringAsync(key);
            return data is not null && double.TryParse(data, out var result) ?
                null : double.Parse(data);
        }

        #endregion double

        #region class

        /// <summary>
        /// 设置对象
        /// </summary>
        public static async Task SetObjectAsync<T>(this IDistributedCache cache, string key, T t,
            DistributedCacheEntryOptions options = null) where T : class
        {
            var json = JsonSerializer.Serialize(t);
            await cache.SetStringAsync(key, json, options);
        }

        /// <summary>
        /// 取得对象
        /// </summary>
        public static async Task<T> GetObjectAsync<T>(this IDistributedCache cache, string key)
             where T : class
        {
            var json = await cache.GetStringAsync(key);
            return json is null ? null : JsonSerializer.Deserialize<T>(json);
        }

        #endregion class

        #endregion 异步方法

        #region 同步方法

        #region int

        /// <summary>
        /// 设置数值
        /// </summary>
        public static void SetInt(this IDistributedCache cache, string key, int t,
            DistributedCacheEntryOptions options = null)
        {
            cache.SetString(key, t.ToString(), options);
        }

        /// <summary>
        /// 取得数值
        /// </summary>
        public static int? GetInt(this IDistributedCache cache, string key)
        {
            var data = cache.GetString(key);
            return data is not null && int.TryParse(data, out var result) ?
                null : int.Parse(data);
        }

        #endregion int

        #region double

        /// <summary>
        /// 设置数值
        /// </summary>
        public static void SetDouble(this IDistributedCache cache, string key, double t,
            DistributedCacheEntryOptions options = null)
        {
            cache.SetString(key, t.ToString(), options);
        }

        /// <summary>
        /// 取得数值
        /// </summary>
        public static double? GetDouble(this IDistributedCache cache, string key)
        {
            var data = cache.GetString(key);
            return data is not null && double.TryParse(data, out var result) ?
                null : double.Parse(data);
        }

        #endregion double

        #region class

        /// <summary>
        /// 设置对象
        /// </summary>
        public static void SetObject<T>(this IDistributedCache cache, string key, T t,
            DistributedCacheEntryOptions options = null) where T : class
        {
            var json = JsonSerializer.Serialize(t);
            cache.SetString(key, json, options);
        }

        /// <summary>
        /// 取得对象
        /// </summary>
        public static T GetObject<T>(this IDistributedCache cache, string key) where T : class
        {
            var json = cache.GetString(key);
            return json is null ? null : JsonSerializer.Deserialize<T>(json);
        }

        #endregion class

        #endregion 同步方法
    }
}