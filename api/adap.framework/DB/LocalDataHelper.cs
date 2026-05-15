using System.Text;
using highspeed.framework.Common;

namespace highspeed.framework.Data
{
    /// <summary>
    /// 本地数据存储
    /// </summary>
    public class LocalDataHelper
    {
        #region 文件存储

        private static string _DataFileDir = Util.AppDataPath + "/LocalData/";

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="key">文件名</param>
        /// <param name="contents">内容</param>
        /// <param name="encoding">编码</param>
        /// <param name="encrypt">是否加密保存</param>
        private static void SaveFile(string key, string contents, bool encrypt, Encoding encoding)
        {
            try
            {
                if (!Directory.Exists(_DataFileDir))
                    Directory.CreateDirectory(_DataFileDir);

                if (encrypt) key = key.ToMD5();
                string file = _DataFileDir + key + ".dat";
                if (encrypt)
                {
                    var encryptKey = (key.Encrypt(key)).ToMD5();
                    var bytes = contents.Encrypt(encryptKey, encoding).Zip(encoding ?? Encoding.Default);
                    if (bytes.Length >= 2 && bytes[0] == 0x50 && bytes[1] == 0x4B)
                    {
                        bytes[0] = 0xF1;
                        bytes[1] = 0x19;
                        File.WriteAllBytes(file, bytes);
                    }
                }
                else
                    File.WriteAllText(file, contents, encoding ?? Encoding.Default);
            }
            catch (Exception ex)
            {
                Logger.Error("LocalDataHelper.SaveFile", ex.Message, ex);
            }
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="key">文件名</param>
        /// <param name="contents">内容</param>
        /// <param name="encoding">编码</param>
        /// <param name="encrypt">是否加密保存</param>
        private static void SaveFile(string key, byte[] contents, bool encrypt)
        {
            try
            {
                if (!Directory.Exists(_DataFileDir))
                    Directory.CreateDirectory(_DataFileDir);

                if (encrypt) key = key.ToMD5();
                string file = _DataFileDir + key + ".dat";
                File.WriteAllBytes(file, contents);
            }
            catch (Exception ex)
            {
                Logger.Error("LocalDataHelper.SaveFile", ex.Message, ex);
            }
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="key">文件名</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        private static byte[]? LoadFileAsBytes(string key, bool encrypt)
        {
            try
            {
                if (!Directory.Exists(_DataFileDir))
                    return null;
                if (encrypt) key = key.ToMD5();
                string file = _DataFileDir + key + ".dat";
                if (!File.Exists(file))
                    return null;

                return File.ReadAllBytes(file);
            }
            catch (Exception ex)
            {
                Logger.Error("LocalDataHelper.LoadFile", ex.Message, ex);
                return null;
            }
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="key">文件名</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        private static string LoadFile(string key, bool encrypt, Encoding encoding)
        {
            try
            {
                if (!Directory.Exists(_DataFileDir))
                    return null;
                if (encrypt) key = key.ToMD5();
                string file = _DataFileDir + key + ".dat";
                if (!File.Exists(file))
                    return null;

                string contents = null;

                if (encrypt)
                {
                    var encryptKey = (key.Encrypt(key)).ToMD5();
                    var bytes = File.ReadAllBytes(file);
                    //检查是否加密
                    if (bytes.Length >= 2 && bytes[0] == 0xF1 && bytes[1] == 0x19)
                    {
                        bytes[0] = 0x50;
                        bytes[1] = 0x4B;
                        contents = bytes.UnzipToString(encoding ?? Encoding.Default).Decrypt(encryptKey, encoding);
                    }
                }
                else
                    contents = File.ReadAllText(file, encoding ?? Encoding.Default);

                return contents;
            }
            catch (Exception ex)
            {
                Logger.Error("LocalDataHelper.LoadFile", ex.Message, ex);
                return null;
            }
        }

        /// <summary>
        /// 根据Key删除LocalData
        /// </summary>
        /// <param name="key"></param>
        /// <param name="encrypt"></param>
        public static void Delete(string key, bool encrypt = true)
        {
            try
            {
                if (!Directory.Exists(_DataFileDir)) return;
                if (encrypt) key = key.ToMD5();
                string file = _DataFileDir + key + ".dat";
                if (!File.Exists(file)) return;
                else File.Delete(file);
            }
            catch (Exception ex)
            {
                Logger.Error("LocalDataHelper.Delete", ex.Message, ex);
            }
        }

        #endregion 文件存储

        /// <summary>
        /// 检查Key对应的数据是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Exists(string key, bool encrypt = true)
        {
            if (encrypt) key = key.ToMD5();
            string file = _DataFileDir + key + ".dat";
            return File.Exists(file);
        }

        #region 保存

        /// <summary>
        /// 保存String数据
        /// </summary>
        /// <param name="key">存取数据的Key，重复时覆盖</param>
        /// <param name="stringData">数据</param>
        /// <param name="encoding">编码</param>
        public static void Save(string key, string stringData, Encoding encoding = null, bool encrypt = true)
        {
            SaveFile(key, stringData, encrypt, encoding);
        }

        /// <summary>
        /// 保存object数据
        /// </summary>
        /// <typeparam name="T">object的类型</typeparam>
        /// <param name="key">存取数据的Key，重复时覆盖</param>
        /// <param name="objectData">数据</param>
        /// <param name="encoding">编码</param>
        public static void Save<T>(string key, T objectData, Encoding encoding = null, bool encrypt = true)
        {
            string jsonString = objectData == null ? string.Empty : JsonHelper.SerializeObject(objectData);
            SaveFile(key, jsonString, encrypt, encoding);
        }

        /// <summary>
        /// 保存二进制数据
        /// </summary>
        /// <param name="key">存取数据的Key，重复时覆盖</param>
        /// <param name="contents">数据</param>
        public static void SaveBytes(string key, byte[] contents, bool encrypt = true)
        {
            SaveFile(key, contents, encrypt);
        }

        #endregion 保存

        #region 读取

        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <param name="key">存取数据的Key</param>
        /// <param name="encoding">编码</param>
        /// <returns>数据</returns>
        public static byte[]? LoadBytes(string key, bool encrypt = true)
        {
            return LoadFileAsBytes(key, encrypt);
        }

        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <param name="key">存取数据的Key</param>
        /// <param name="encoding">编码</param>
        /// <returns>数据</returns>
        public static string Load(string key, Encoding encoding = null, bool encrypt = true)
        {
            return LoadFile(key, encrypt, encoding);
        }

        /// <summary>
        /// 读取object数据
        /// </summary>
        /// <typeparam name="T">object的类型</typeparam>
        /// <param name="key">存取数据的Key，重复时覆盖</param>
        /// <param name="encoding">编码</param>
        /// <returns>object的类型实例</returns>
        public static T Load<T>(string key, Encoding encoding = null, bool encrypt = true) where T : class
        {
            try
            {
                string jsonString = LoadFile(key, encrypt, encoding);
                return string.IsNullOrEmpty(jsonString) ? null : JsonHelper.DeserializeJsonToObject<T>(jsonString);
            }
            catch (Exception ex)
            {
                Logger.Error("LocalDataHelper.Load<T>", ex.Message, ex);
                return null;
            }
        }

        /// <summary>
        /// 读取集合对象
        /// </summary>
        /// <typeparam name="T">集合中object的类型</typeparam>
        /// <param name="key">存取数据的Key，重复时覆盖</param>
        /// <param name="encoding">编码</param>
        /// <returns>object集合的实例</returns>
        public static List<T> LoadList<T>(string key, Encoding encoding = null, bool encrypt = true) where T : class
        {
            try
            {
                string jsonString = LoadFile(key, encrypt, encoding);
                return string.IsNullOrEmpty(jsonString) ? null : JsonHelper.DeserializeJsonToList<T>(jsonString);
            }
            catch (Exception ex)
            {
                Logger.Error("LocalDataHelper.LoadList<T>", ex.Message, ex);
                return null;
            }
        }

        /// <summary>
        /// 读取匿名对象
        /// </summary>
        /// <typeparam name="T">object的类型</typeparam>
        /// <param name="key">存取数据的Key，重复时覆盖</param>
        /// <param name="anonymousTypeObject">object类型的匿名实例</param>
        /// <param name="encoding">编码</param>
        /// <returns>object类型的实例</returns>
        public static T LoadAnonymousType<T>(string key, T anonymousTypeObject, Encoding encoding = null, bool encrypt = true) where T : class
        {
            try
            {
                string jsonString = LoadFile(key, encrypt, encoding);
                return string.IsNullOrEmpty(jsonString) ? null : JsonHelper.DeserializeAnonymousType(jsonString, anonymousTypeObject);
            }
            catch (Exception ex)
            {
                Logger.Error("LocalDataHelper.LoadAnonymousType<T>", ex.Message, ex);
                return null;
            }
        }

        #endregion 读取
    }
}