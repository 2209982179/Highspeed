using highspeed.framework.Common;
using Newtonsoft.Json;

namespace highspeed.framework
{
    /// <summary>
    /// Json 转换扩展方法
    /// </summary>
    public static class JsonHelperExtension
    {
        public static string StructToJson<T>(this T t) where T : struct
        {
            return JsonHelper.SerializeObject(t);
        }

        public static string ToJson<T>(this T t) where T : class
        {
            return JsonHelper.SerializeObject(t);
        }

        public static string ToJson<T>(this T t, JsonSerializerSettings settings) where T : class
        {
            return JsonHelper.SerializeObject(t, settings);
        }

        public static string ToFormattedJson<T>(this T t) where T : class
        {
            return JsonHelper.SerializeObjectFormatted(t);
        }

        public static string ToFormattedJson<T>(this T t, JsonSerializerSettings settings) where T : class
        {
            return JsonHelper.SerializeObjectFormatted(t, settings);
        }

        public static T JsonToObject<T>(this string s) where T : class
        {
            return JsonHelper.DeserializeJsonToObject<T>(s);
        }

        public static object JsonToObject(this string s, System.Type type)
        {
            return JsonHelper.DeserializeJsonToObject(s, type);
        }

        public static List<T> JsonToList<T>(this string s) where T : class
        {
            return JsonHelper.DeserializeJsonToList<T>(s);
        }

        public static dynamic JsonToDynamic(this string s)
        {
            return JsonHelper.DeserializeDynamic(s);
        }
    }
}
