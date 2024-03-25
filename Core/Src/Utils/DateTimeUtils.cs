using System.Globalization;

namespace Core.Utils;

static public class DateTimeUtils {
    static public DateTime GetDefaultDateTime() {
        return new DateTime(1970, 1, 1, 0, 0, 0, 0);
    }
    /// <summary>
    /// * 获取当前的时间戳（秒）- UTC 标准时
    /// </summary>
    /// <returns>时间戳字符串</returns>
    static public string GetCurrentTimestampSecond() {
        return Convert.ToString(
            Convert.ToInt64(
                (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds
            )
        );
    }
    static public string GetCurrentTimestampMilliSecond() {
        return Convert.ToString(
            Convert.ToInt64(
                (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds
            )
        );
    }
    /// <summary>
    /// * 将 DateTime结构体转换为时间戳（秒）- UTC标准时
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns>时间戳字符串</returns>
    static public string GetTimestampSecond(DateTime dateTime) {
        return Convert.ToString(
            Convert.ToInt64(
                (DateTime.UtcNow - dateTime).TotalSeconds
            )
        );
    }
    /// <summary>
    /// * 将时间戳转换为 DateTime 结构体(入口)
    /// </summary>
    /// <param name="timestampStr"></param>
    /// <returns>时间戳对应的DateTime结构</returns>
    public static DateTime TimestampToDateTime(string timestampStr) {
        var timestamp = long.Parse(timestampStr);
        string currentTimestamp = GetCurrentTimestampSecond();
        if (timestampStr.Length > currentTimestamp.Length) {
            return TimestampToDateTime(timestamp, true);
        } else {
            return TimestampToDateTime(timestamp);
        }
    }
    /// <summary>
    /// * 将时间戳转换为 DateTime 结构体(主处理函数)
    /// </summary>
    /// <param name="timestampVal"></param>
    /// <param name="isMilliseconds">是否为毫秒位</param>
    /// <returns></returns>
    static DateTime TimestampToDateTime(long timestampVal, bool isMilliseconds = false) {
        return 
            isMilliseconds ? 
            DateTimeOffset.FromUnixTimeMilliseconds(timestampVal).DateTime.ToLocalTime() :
            DateTimeOffset.FromUnixTimeSeconds(timestampVal).DateTime.ToLocalTime();
    }
    /// <summary>
    /// * 将时间格式文字转换为DateTime结构体
    /// </summary>
    /// <param name="timeString"></param>
    /// <param name="format">字符串的format</param>
    /// <returns>时间文字对应的DateTime结构体</returns>
    public static DateTime TimeStringToDateTime(string timeString, string format) {
        // * 默认用的 米国 日期文字信息
        return DateTime.ParseExact(
            timeString,
            format,
            CultureInfo.GetCultureInfo("en-us"));
    }
}