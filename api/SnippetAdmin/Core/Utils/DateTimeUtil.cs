namespace SnippetAdmin.Core.Utils
{
    public static class DateTimeUtil
    {
        /// <summary>
        /// 日期转时间戳
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <param name="milliseconds">true：13位，false：10位</param>
        /// <returns>时间戳</returns>
        public static string DateTime2TimeStamp(bool milliseconds = false)
            => milliseconds ?
            DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString() :
            DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        /// <summary>
        /// 时间戳转日期
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <param name="milliseconds">true：13位，false：10位</param>
        /// <returns>日期</returns>
        public static DateTime TimeStamp2DateTime(long timestamp, bool milliseconds = false)
            => milliseconds ?
            DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime :
            DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
    }
}