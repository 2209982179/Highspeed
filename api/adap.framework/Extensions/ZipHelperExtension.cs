using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using highspeed.framework.Common;

namespace highspeed.framework
{
    /// <summary>
    /// ZipHelper扩展方法类
    /// </summary>
    public static class ZipHelperExtension
    {
        /// <summary>
        /// 压缩字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] Zip(this string str, Encoding encoding)
        {
            return ZipHelper.ZipString(str, encoding);
        }

        /// <summary>
        /// 解压缩字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string UnzipToString(this byte[] bytes, Encoding encoding)
        {
            return ZipHelper.UnzipString(bytes, encoding);
        }

        /// <summary>
        /// Hex String To Bytes
        /// </summary>
        /// <param name="hexStr"></param>
        /// <returns></returns>
        public static byte[] HexStringToBytes(this string hexStr)
        {
            return ZipHelper.ToBytes(hexStr);
        }

        /// <summary>
        /// Bytes To Hex String
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string BytesToHexString(this byte[] bytes)
        {
            return ZipHelper.ToHexString(bytes);
        }

        /// <summary>
        /// BinContent To MDG
        /// </summary>
        /// <param name="hexStr"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string BinContentToMDG(this string hexStr, Encoding encoding)
        {
            return ZipHelper.BinContentToMDG(hexStr, encoding);
        }

        /// <summary>
        /// BinContent To MDG
        /// </summary>
        /// <param name="binBytes"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string BinContentToMDG(this byte[] binBytes, Encoding encoding)
        {
            return ZipHelper.BinContentToMDG(binBytes, encoding);
        }

        public static byte[] ZipByte(this byte[] data)
        {
            return ZipHelper.ZipByte(data);
        }

        public static byte[] UnzipByte(this byte[] zipped)
        {
            return ZipHelper.UnzipByte(zipped);
        }

        #region Object压缩
        public static byte[] ZipObject(this object obj)
        {
            return ZipHelper.ZipByte(Serialize(obj));
        }

        public static T UnzipObject<T>(this byte[] obj)
        {
            return (T)Deserialize(ZipHelper.UnzipByte(obj));
        }

        private static byte[] Serialize(object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            ms.Position = 0;
            return ms.ToArray();
        }

        private static object Deserialize(byte[] bytes)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(bytes);
            ms.Position = 0;
            return bf.Deserialize(ms);
        }
        #endregion
    }
}
