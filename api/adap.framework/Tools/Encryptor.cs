using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace highspeed.framework.Common
{
    /// <summary>
    /// 加密解密类
    /// </summary>
    public class Encryptor
    {
        #region AES
        /// <summary>
        ///  AES 加密
        /// </summary>
        /// <param name="str">明文（待加密）</param>
        /// <param name="key">The secret key to be used for the symmetric algorithm. The key size must be 128, 192, or 256 bits.</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string AesEncrypt(string str, string key, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(str)) return null;
            if (string.IsNullOrEmpty(key)) return null;
            encoding = encoding ?? Encoding.Default;
            Byte[] toEncryptArray = encoding.GetBytes(str);

            RijndaelManaged rm = new RijndaelManaged
            {
                Key = encoding.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rm.CreateEncryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray);
        }

        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="str">明文（待解密）</param>
        /// <param name="key">The secret key to be used for the symmetric algorithm. The key size must be 128, 192, or 256 bits.</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string AesDecrypt(string str, string key, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(str)) return null;
            if (string.IsNullOrEmpty(key)) return null;
            Byte[] toEncryptArray = Convert.FromBase64String(str);

            encoding = encoding ?? Encoding.Default;
            RijndaelManaged rm = new RijndaelManaged
            {
                Key = encoding.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rm.CreateDecryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return encoding.GetString(resultArray);
        }

        /// <summary>
        ///  AES 加密【结果与 adap.framework.Standard.Common.Encryptor.AesEncrypt 一致】
        /// </summary>
        /// <param name="str">明文（待加密）</param>
        /// <param name="key">The secret key to be used for the symmetric algorithm. The key size must be 128, 192, or 256 bits.</param>
        /// <param name="IV">(MD5 code)The IV to be used for the symmetric algorithm.</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string AesEncrypt(string str, string key, string IV, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(str)) return null;
            if (string.IsNullOrEmpty(key)) return null;
            if (string.IsNullOrEmpty(IV)) return null;
            encoding = encoding ?? Encoding.UTF8;
            Byte[] toEncryptArray = encoding.GetBytes(str);

            var rgbKey = encoding.GetBytes(key);
            var rgbIV = encoding.GetBytes(IV).Take(16).ToArray();
            RijndaelManaged rm = new RijndaelManaged
            {
                Key = encoding.GetBytes(key),
                IV = encoding.GetBytes(IV).Take(16).ToArray()
            };

            ICryptoTransform cTransform = rm.CreateEncryptor(rgbKey, rgbIV);
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray);
        }

        /// <summary>
        ///  AES 解密【结果与 adap.framework.Standard.Common.Encryptor.AesDecrypt 一致】
        /// </summary>
        /// <param name="str">明文（待解密）</param>
        /// <param name="key">The secret key to be used for the symmetric algorithm. The key size must be 128, 192, or 256 bits.</param>
        /// <param name="IV">(MD5 code)The IV to be used for the symmetric algorithm.</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string AesDecrypt(string str, string key, string IV, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(str)) return null;
            if (string.IsNullOrEmpty(key)) return null;
            if (string.IsNullOrEmpty(IV)) return null;
            Byte[] toEncryptArray = Convert.FromBase64String(str);

            encoding = encoding ?? Encoding.UTF8;
            var rgbKey = encoding.GetBytes(key);
            var rgbIV = encoding.GetBytes(IV).Take(16).ToArray();
            RijndaelManaged rm = new RijndaelManaged
            {
                Key = rgbKey,
                IV = rgbIV
            };

            ICryptoTransform cTransform = rm.CreateDecryptor(rgbKey, rgbIV);
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return encoding.GetString(resultArray);
        }
        #endregion

        #region DES
        //密钥
        private static byte[] _DES_KEY = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
        //向量
        private static byte[] _DES_IV = new byte[] { 0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01 };

        /// <summary>
        /// DES加密操作
        /// </summary>
        /// <param name="normalTxt"></param>
        /// <returns></returns>
        private static string DesEncrypt(string normalTxt)
        {
            //byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(_KEY);
            //byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(_IV);

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            int i = cryptoProvider.KeySize;
            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(_DES_KEY, _DES_IV), CryptoStreamMode.Write);

            StreamWriter sw = new StreamWriter(cst);
            sw.Write(normalTxt);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();

            string strRet = Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
            return strRet;
        }

        /// <summary>
        /// DES解密操作
        /// </summary>
        /// <param name="securityTxt">加密字符串</param>
        /// <returns></returns>
        private static string DesDecrypt(string securityTxt)//解密  
        {
            //byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(_KEY);
            //byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(_IV);
            byte[] byEnc;
            try
            {
                securityTxt.Replace("_%_", "/");
                securityTxt.Replace("-%-", "#");
                byEnc = Convert.FromBase64String(securityTxt);
            }
            catch
            {
                return null;
            }
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream(byEnc);
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(_DES_KEY, _DES_IV), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cst);
            return sr.ReadToEnd();
        }
        #endregion

        #region RSA
        private static readonly int RsaKeySize = 2048;
        private static readonly string publicRsaKeyFileName = "RSA.Pub";
        private static readonly string privateRsaKeyFileName = "RSA.Private";

        /// <summary>
        ///在给定路径中生成XML格式的私钥和公钥。
        /// </summary>
        private static void RsaGenerateKeys(string path)
        {
            using (var rsa = new RSACryptoServiceProvider(RsaKeySize))
            {
                try
                {
                    // 获取私钥和公钥。
                    var publicKey = rsa.ToXmlString(false);
                    var privateKey = rsa.ToXmlString(true);

                    // 保存到磁盘
                    File.WriteAllText(Path.Combine(path, publicRsaKeyFileName), publicKey);
                    File.WriteAllText(Path.Combine(path, privateRsaKeyFileName), privateKey);

                    //Console.WriteLine(string.Format("生成的RSA密钥的路径: {0}\\ [{1}, {2}]", path, publicKeyFileName, privateKeyFileName));
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        /// <summary>
        /// 用给定路径的RSA公钥文件加密纯文本。
        /// </summary>
        /// <param name="plainText">要加密的文本</param>
        /// <param name="pathToPublicKey">用于加密的公钥路径.</param>
        /// <returns>表示加密数据的64位编码字符串.</returns>
        private static string RsaEncrypt(string plainText, string pathToPublicKey)
        {
            using (var rsa = new RSACryptoServiceProvider(RsaKeySize))
            {
                try
                {
                    //加载公钥
                    var publicXmlKey = File.ReadAllText(pathToPublicKey);
                    rsa.FromXmlString(publicXmlKey);

                    var bytesToEncrypt = System.Text.Encoding.Unicode.GetBytes(plainText);

                    var bytesEncrypted = rsa.Encrypt(bytesToEncrypt, false);

                    return Convert.ToBase64String(bytesEncrypted);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        /// <summary>
        /// Decrypts encrypted text given a RSA private static key file path.给定路径的RSA私钥文件解密 加密文本
        /// </summary>
        /// <param name="encryptedText">加密的密文</param>
        /// <param name="pathToPrivateKey">用于加密的私钥路径.</param>
        /// <returns>未加密数据的字符串</returns>
        private static string RsaDecrypt(string encryptedText, string pathToPrivateKey)
        {
            using (var rsa = new RSACryptoServiceProvider(RsaKeySize))
            {
                try
                {
                    var privateXmlKey = File.ReadAllText(pathToPrivateKey);
                    rsa.FromXmlString(privateXmlKey);

                    var bytesEncrypted = Convert.FromBase64String(encryptedText);

                    var bytesPlainText = rsa.Decrypt(bytesEncrypted, false);

                    return System.Text.Encoding.Unicode.GetString(bytesPlainText);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }
        #endregion

        #region SHA(单向，不可解密)
        public static string SHAmd5Encrypt(string normalTxt)
        {
            var bytes = Encoding.Default.GetBytes(normalTxt);//求Byte[]数组  
            var Md5 = new MD5CryptoServiceProvider();
            var encryptbytes = Md5.ComputeHash(bytes);//求哈希值  
            return Base64To16(encryptbytes);//将Byte[]数组转为净荷明文(其实就是字符串)  
        }

        private static string SHA1Encrypt(string normalTxt)
        {
            var bytes = Encoding.Default.GetBytes(normalTxt);
            var SHA = new SHA1CryptoServiceProvider();
            var encryptbytes = SHA.ComputeHash(bytes);
            return Base64To16(encryptbytes);
        }
        private static string SHA256Encrypt(string normalTxt)
        {
            var bytes = Encoding.Default.GetBytes(normalTxt);
            var SHA256 = new SHA256CryptoServiceProvider();
            var encryptbytes = SHA256.ComputeHash(bytes);
            return Base64To16(encryptbytes);
        }
        private static string SHA384Encrypt(string normalTxt)
        {
            var bytes = Encoding.Default.GetBytes(normalTxt);
            var SHA384 = new SHA384CryptoServiceProvider();
            var encryptbytes = SHA384.ComputeHash(bytes);
            return Base64To16(encryptbytes);
        }
        private static string SHA512Encrypt(string normalTxt)
        {
            var bytes = Encoding.Default.GetBytes(normalTxt);
            var SHA512 = new SHA512CryptoServiceProvider();
            var encryptbytes = SHA512.ComputeHash(bytes);
            return Base64To16(encryptbytes);
        }

        private static string Base64To16(byte[] buffer)
        {
            string md_str = string.Empty;
            for (int i = 0; i < buffer.Length; i++)
            {
                md_str += buffer[i].ToString("x2");
            }
            return md_str;
        }
        #endregion
    }

    /// <summary>
    /// Encryptor扩展方法
    /// </summary>
    public static class EncryptorExtension
    {
        /// <summary>
        /// String转MD5
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToMD5(this string str)
        {
            return Encryptor.SHAmd5Encrypt(str);
        }

        /// <summary>
        /// String加密
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key">加密的Key</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string Encrypt(this string str, string key, Encoding encoding = null)
        {
            return Encryptor.AesEncrypt(str, key);
        }

        /// <summary>
        /// String解密
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key">加密时的Key</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string Decrypt(this string str, string key, Encoding encoding = null)
        {
            return Encryptor.AesDecrypt(str, key);
        }
    }
}
