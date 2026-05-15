using Newtonsoft.Json;
using System.Text;

namespace highspeed.framework.Common
{
    /// <summary>
    /// Json帮助类
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// 将对象序列化为JSON格式
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="settings">序列化设置</param>
        /// <returns>json字符串</returns>
        public static string SerializeObject(object o, JsonSerializerSettings settings = null)
        {
            string json = null;
            if (settings == null)
                json = JsonConvert.SerializeObject(o);
            else
                json = JsonConvert.SerializeObject(o, settings);
            return json;
        }

        /// <summary>
        /// 将对象序列化为JSON格式(格式化的)
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="settings">序列化设置</param>
        /// <returns>json字符串</returns>
        public static string SerializeObjectFormatted(object o, JsonSerializerSettings settings = null)
        {
            JsonSerializer serializer = settings == null
                                        ? JsonSerializer.Create()
                                        : JsonSerializer.Create(settings);
            if (o != null)
            {
                StringWriter sw = new StringWriter();
                JsonTextWriter jsonTextWriter = new JsonTextWriter(sw)
                {
                    Formatting = Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonTextWriter, o);
                return sw.ToString();
            }
            else return string.Empty;
        }

        /// <summary>
        /// 格式化Json字符串
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string FormatJsonString(string json)
        {
            JsonSerializer serializer = new JsonSerializer();
            TextReader reader = new StringReader(json);
            JsonTextReader jsonReader = new JsonTextReader(reader);
            object obj = serializer.Deserialize(jsonReader);
            if (obj != null)
            {
                StringWriter sw = new StringWriter();
                JsonTextWriter jsonTextWriter = new JsonTextWriter(sw)
                {
                    Formatting = Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonTextWriter, obj);
                return sw.ToString();
            }
            else return json;
        }

        /// <summary>
        /// 解析JSON字符串生成对象实体
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json字符串(eg.{"ID":"112","Name":"石子儿"})</param>
        /// <returns>对象实体</returns>
        public static T DeserializeJsonToObject<T>(string json) where T : class
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(T));
            T t = o as T;
            return t;
        }

        /// <summary>
        /// 解析JSON字符串生成对象实体
        /// </summary>
        /// <param name="json">json字符串(eg.{"ID":"112","Name":"石子儿"})</param>
        /// <param name="type">对象类型</param>
        /// <returns>对象实体</returns>
        public static object DeserializeJsonToObject(string json, Type type)
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            return serializer.Deserialize(new JsonTextReader(sr), type);
        }

        /// <summary>
        /// 解析JSON数组生成对象实体集合
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json数组字符串(eg.[{"ID":"112","Name":"石子儿"}])</param>
        /// <returns>对象实体集合</returns>
        public static List<T> DeserializeJsonToList<T>(string json) where T : class
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(List<T>));
            List<T> list = o as List<T>;
            return list;
        }

        /// <summary>
        /// 反序列化JSON到给定的匿名对象.
        /// </summary>
        /// <typeparam name="T">匿名对象类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <param name="anonymousTypeObject">匿名对象</param>
        /// <returns>匿名对象</returns>
        public static T DeserializeAnonymousType<T>(string json, T anonymousTypeObject)
        {
            T t = JsonConvert.DeserializeAnonymousType(json, anonymousTypeObject);
            return t;
        }

        /// <summary>
        /// 反序列化为动态对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static dynamic DeserializeDynamic(string json)
        {
            return JsonConvert.DeserializeObject(json);
        }
    }    
}