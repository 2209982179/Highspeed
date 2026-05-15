using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace highspeed.framework
{
    public static class CommonExtensions
    {
        public static T DeepClone<T>(this T t) where T : class
        {
            T model = System.Activator.CreateInstance<T>();                     //实例化一个T类型对象
            PropertyInfo[] propertyInfos = model.GetType().GetProperties();     //获取T对象的所有公共属性
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                //判断值是否为空，如果空赋值为null见else
                if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                {
                    //如果convertsionType为nullable类，声明一个NullableConverter类，该类提供从Nullable类到基础基元类型的转换
                    NullableConverter nullableConverter = new NullableConverter(propertyInfo.PropertyType);
                    //将convertsionType转换为nullable对的基础基元类型
                    propertyInfo.SetValue(model, Convert.ChangeType(propertyInfo.GetValue(t), nullableConverter.UnderlyingType), null);
                }
                else
                {
                    propertyInfo.SetValue(model, Convert.ChangeType(propertyInfo.GetValue(t), propertyInfo.PropertyType), null);
                }
            }
            return model;
        }

        public static List<T> DeepCloneList<T>(this List<T> tList) where T : class
        {
            List<T> listNew = new List<T>();
            foreach (var item in tList)
            {
                T model = System.Activator.CreateInstance<T>();                     //实例化一个T类型对象
                PropertyInfo[] propertyInfos = model.GetType().GetProperties();     //获取T对象的所有公共属性
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    //判断值是否为空，如果空赋值为null见else
                    if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {
                        //如果convertsionType为nullable类，声明一个NullableConverter类，该类提供从Nullable类到基础基元类型的转换
                        NullableConverter nullableConverter = new NullableConverter(propertyInfo.PropertyType);
                        //将convertsionType转换为nullable对的基础基元类型
                        propertyInfo.SetValue(model, Convert.ChangeType(propertyInfo.GetValue(item), nullableConverter.UnderlyingType), null);
                    }
                    else
                    {
                        propertyInfo.SetValue(model, Convert.ChangeType(propertyInfo.GetValue(item), propertyInfo.PropertyType), null);
                    }
                }
                listNew.Add(model);
            }
            return listNew;
        }

        /// <summary>
        /// 复制序列中的数据
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="iEnumberable">原数据</param>
        /// <param name="startIndex">原数据开始复制的起始位置</param>
        /// <param name="length">需要复制的数据长度</param>
        /// <returns></returns>
        public static IEnumerable<T> Copy<T>(this IEnumerable<T> iEnumberable, int startIndex, int length)
        {
            var sourceArray = iEnumberable.ToArray();
            T[] newArray = new T[length];
            Array.Copy(sourceArray, startIndex, newArray, 0, length);

            return newArray;
        }

        /// <summary>
        /// 给IEnumerable拓展ForEach方法
        /// </summary>
        /// <typeparam name="T">模型类</typeparam>
        /// <param name="iEnumberable">数据源</param>
        /// <param name="func">方法</param>
        public static void ForEach<T>(this IEnumerable<T> iEnumberable, Action<T> func)
        {
            foreach (var item in iEnumberable)
            {
                func(item);
            }
        }

        /// <summary>
        /// 给IEnumerable拓展ForEach方法
        /// </summary>
        /// <typeparam name="T">模型类</typeparam>
        /// <param name="iEnumberable">数据源</param>
        /// <param name="func">方法</param>
        public static void ForEach<T>(this IEnumerable<T> iEnumberable, Action<T, int> func)
        {
            var array = iEnumberable.ToArray();
            for (int i = 0; i < array.Count(); i++)
            {
                func(array[i], i);
            }
        }

        /// <summary>
        /// IEnumerable转换为List'T'
        /// </summary>
        /// <typeparam name="T">参数</typeparam>
        /// <param name="source">数据源</param>
        /// <returns></returns>
        public static List<T> CastToList<T>(this IEnumerable source)
        {
            return new List<T>(source.Cast<T>());
        }
             
        /// <summary>
        /// byte[] CheckSum
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte CheckSum(this byte[]? bytes)
        {
            if (bytes == null) return 0x00;
            byte sum = 0x00;
            foreach (byte b in bytes)
                sum ^= b;
            return sum;
        }

        /// <summary>
        /// string CheckHashValue
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string CheckHashValue(this string content)
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                return CheckHashValue(stream);
            }
        }

        /// <summary>
        /// stream CheckHashValue
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string CheckHashValue(this Stream stream)
        {
            using (var hashAlgorithm = SHA256.Create())
            {
                var hash = hashAlgorithm.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        /// <summary>
        /// 格式化字符串，并在参数与占位符数量不匹配时自动匹配
        /// （注：参数中null值会被排除）
        /// </summary>
        /// <param name="str"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string TryFormat(this string str, params object[] values)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;
            var count = Regex.Matches(str, "{?{\\d[^}]*}}?").Where(m => !(m.Value.StartsWith("{{") && m.Value.StartsWith("}}"))).Count();
            // 去除null
            var vals = values.Where(v => v != null).ToArray();
            if (count > vals.Length)
            {
                List<object> paramArr = Enumerable.Range(1, count - vals.Length)
                                              .Select(i => string.Empty)
                                              .ToList<object>();
                paramArr = vals.Concat(paramArr).ToList();
                return string.Format(str, paramArr);
            }
            else if (count < vals.Length)
            {
                List<object> paramArr = vals.ToList().GetRange(0, count - 1);
                return string.Format(str, vals);
            }
            else return string.Format(str, vals);
        }
        
        /// <summary>
        /// int值转为16进制字符串，例：0x0A，0x03E1
        /// </summary>
        /// <param name="number">int值</param>
        /// <param name="length">16进制字符串长度，不指定时自动计算长度</param>
        /// <param name="withPrefix">是否带0x前缀，默认为true</param>
        /// <returns></returns>
        public static string ToHexString(this int number, int? length = null, bool withPrefix = true)
        {
            var hexStr = number.ToString("X");
            int count = hexStr.Length;
            count = count / 2 * 2 + (count % 2) * 2;
            if (count > 4) count = count / 4 * 4 + ((count % 4) / 2) * 4;
            if (length > count) count = length.Value;
            hexStr = hexStr.PadLeft(count, '0');
            return withPrefix ? $"0x{hexStr}" : hexStr;
        }

        /// <summary>
        /// 16进制字符串转为int值
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static int? HexStringToInt(this string hex)
        {
            if (string.IsNullOrEmpty(hex)) return null;
            var _hex = hex.ToLower().Replace("0x", "");
            if (string.IsNullOrEmpty(_hex)) return null;
            return Convert.ToInt32(_hex, 16);
        }

        /// <summary>
        /// Int转中文表达（最大为“亿”级）
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string ToChinese(this int num)
        {
            string[] nums = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
            string[] units = { "", "十", "百", "千", "万", "十", "百", "千", "亿", "十", "百", "千" };

            if (num == 0)
            {
                return nums[0];
            }
            string sign = "";
            if (num < 0)
            {
                sign = "负";
                num = -num;
            }
            string strNum = num.ToString();
            char[] chars = strNum.ToCharArray();
            Array.Reverse(chars);
            strNum = new string(chars);
            string result = "";
            for (int i = 0; i < strNum.Length; i++)
            {
                int digit = int.Parse(strNum[i].ToString());
                result = nums[digit] + units[i] + result;
            }
            result = result.Replace("零十", "零").Replace("零百", "零").Replace("零千", "零").Replace("亿万", "亿").Replace("一十", "十");
            while (result.Contains("零零"))
            {
                result = result.Replace("零零", "零");
            }
            if (result.EndsWith("零"))
            {
                result = result.Remove(result.Length - 1);
            }
            result = sign + result;
            return result;
        }
    }
}
