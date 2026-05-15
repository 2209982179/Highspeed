using ICSharpCode.SharpZipLib.Zip;
using System.IO.Compression;
using System.Text;

namespace highspeed.framework.Common
{
    /// <summary>
    /// Zip压缩与解压缩
    /// </summary>
    public class ZipHelper
    {
        #region 压缩字符串
        #region Zip
        /// <summary>
        /// 压缩 String
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>压缩字节流</returns>
        public static byte[] ZipString(string str, Encoding encoding)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true, encoding))
                {
                    var demoFile = zipArchive.CreateEntry(Guid.NewGuid().ToString());
                    using (var entryStream = demoFile.Open())
                    {
                        using (var streamWriter = new StreamWriter(entryStream))
                        {
                            streamWriter.Write(str);
                        }
                    }
                }

                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// 解压缩 byte[] 为 String
        /// </summary>
        /// <param name="zipped">压缩字节流</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>字符串</returns>
        public static string UnzipString(byte[] zipped, Encoding encoding)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            if (zipped.Length < 2 || zipped[0] != 0x50 || zipped[1] != 0x4B) return null;

            var zippedStream = new MemoryStream(zipped);
            using (ZipInputStream s = new ZipInputStream(zippedStream))
            {
                ZipEntry theEntry;
                byte[] result = null;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    using (MemoryStream streamWriter = new MemoryStream())
                    {
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                        result = streamWriter.ToArray();
                    }
                }

                if (result != null)
                {
                    return encoding.GetString(result);
                }
                else
                    return null;
            }
        }
        #endregion

        #region GZip
        /// <summary>
        /// 压缩 String
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>压缩字节流</returns>
        public static byte[] GZipString(string str, Encoding encoding)
        {
            var bytes = (encoding ?? Encoding.UTF8).GetBytes(str);
            using (var memoryStream = new MemoryStream())
            {
                using (var stream = new GZipStream(memoryStream, CompressionMode.Compress))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush();
                    stream.Dispose();
                    return memoryStream.ToArray();
                }
            }
        }

        /// <summary>
        /// 解压缩 byte[] 为 String
        /// </summary>
        /// <param name="zipped">压缩字节流</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>字符串</returns>
        public static string UnGZipString(byte[] zipped, Encoding encoding)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            using (var zippedStream = new MemoryStream(zipped))
            using (var stream = new GZipStream(zippedStream, CompressionMode.Decompress))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
        #endregion

        #region 压缩Stream
        /// <summary>
        /// 压缩 Stream
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns>压缩字节流</returns>
        public static byte[] ZipByte(byte[] bytes)
        {
            using (MemoryStream compressStream = new MemoryStream())
            {
                using (var zipStream = new GZipStream(compressStream, CompressionMode.Compress))
                    zipStream.Write(bytes, 0, bytes.Length);
                return compressStream.ToArray();
            }
        }

        /// <summary>
        /// 解压缩为 byte[]
        /// </summary>
        /// <param name="zipped"></param>
        /// <returns>字符串</returns>
        public static byte[] UnzipByte(byte[] zipped)
        {
            using (var compressStream = new MemoryStream(zipped))
            using (var zipStream = new GZipStream(compressStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }
        #endregion

        #region MDG 压缩与解压

        /// <summary>
        /// 将 MDG BinContent 转为 MDG XML
        /// </summary>
        /// <param name="hexString">16进制字符串</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>XML字符串</returns>
        public static string BinContentToMDG(string hexString, Encoding encoding)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            if (hexString.StartsWith("0x"))
                hexString = hexString.Substring(2);

            if (!hexString.StartsWith("504B", StringComparison.InvariantCultureIgnoreCase)) return null;

            var zipped = ToBytes(hexString);
            if (zipped == null) return null;

            var mdg = UnzipString(zipped, encoding);
            if (mdg != null)
                return mdg.Replace("\0", string.Empty);
            return null;
        }

        /// <summary>
        /// 将 MDG BinContent 转为 MDG XML
        /// </summary>
        /// <param name="binBytes">字节流</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>XML字符串</returns>
        public static string BinContentToMDG(byte[] binBytes, Encoding encoding)
        {
            var mdg = UnzipString(binBytes, encoding);
            if (mdg != null)
                return mdg.Replace("\0", string.Empty);
            return null;
        }
        #endregion

        /// <summary>
        /// byte[] 转为 16进制字符串
        /// </summary>
        /// <param name="bytes">字节流</param>
        /// <returns>16进制字符串</returns>
        public static string ToHexString(byte[] bytes)
        {
            StringBuilder result = new StringBuilder();
            bytes.ToList().ForEach((b) =>
            {
                var str = "00" + Convert.ToString(b, 16);
                result.Append(str.Substring(str.Length - 2, 2));
            });
            return result.ToString();
        }

        /// <summary>
        /// 16进制字符串 转为 byte[]
        /// </summary>
        /// <param name="hexStr">16进制字符串</param>
        /// <returns>字节流</returns>
        public static byte[] ToBytes(string hexStr)
        {
            if (hexStr.Length % 2 == 1)
            {
                Logger.Error("adap.framework.Common.ZipHelper.ToBytes", "Hex string length is odd number.");
                return null;
            }

            hexStr = hexStr.Replace(" ", "");
            List<string> chars = new List<string>();
            for (int i = 0; i < hexStr.Length;)
            {
                chars.Add(hexStr.Substring(i, 2));
                i += 2;
            }
            byte[] b = new byte[chars.Count];
            //逐个字符变为16进制字节数据
            for (int i = 0; i < chars.Count; i++)
            {
                b[i] = Convert.ToByte(chars[i], 16);
            }
            return b;
        }
        #endregion

        #region 压缩文件
        /// <summary>
        /// 压缩单个文件
        /// </summary>
        /// <param name="fileToZip">要压缩的文件</param>
        /// <param name="zipedFile">压缩后的文件</param>
        /// <param name="compressionLevel">压缩等级</param>
        /// <param name="blockSize">每次写入大小</param>
        public static void ZipFile(string fileToZip, string zipedFile, int compressionLevel, int blockSize)
        {
            //如果文件没有找到，则报错
            if (!System.IO.File.Exists(fileToZip))
            {
                throw new System.IO.FileNotFoundException("指定要压缩的文件: " + fileToZip + " 不存在!");
            }

            using (System.IO.FileStream ZipFile = System.IO.File.Create(zipedFile))
            {
                using (ZipOutputStream ZipStream = new ZipOutputStream(ZipFile))
                {
                    using (System.IO.FileStream StreamToZip = new System.IO.FileStream(fileToZip, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        string fileName = fileToZip.Substring(fileToZip.LastIndexOf("\\") + 1);

                        ZipEntry ZipEntry = new ZipEntry(fileName);

                        ZipStream.PutNextEntry(ZipEntry);

                        ZipStream.SetLevel(compressionLevel);

                        byte[] buffer = new byte[blockSize];

                        int sizeRead = 0;

                        try
                        {
                            do
                            {
                                sizeRead = StreamToZip.Read(buffer, 0, buffer.Length);
                                ZipStream.Write(buffer, 0, sizeRead);
                            }
                            while (sizeRead > 0);
                        }
                        catch (System.Exception ex)
                        {
                            throw ex;
                        }

                        StreamToZip.Close();
                    }

                    ZipStream.Finish();
                    ZipStream.Close();
                }

                ZipFile.Close();
            }
        }

        /// <summary>
        /// 压缩单个文件
        /// </summary>
        /// <param name="fileToZip">要进行压缩的文件名</param>
        /// <param name="zipedFile">压缩后生成的压缩文件名</param>
        public static void ZipFile(string fileToZip, string zipedFile)
        {
            //如果文件没有找到，则报错
            if (!File.Exists(fileToZip))
            {
                throw new System.IO.FileNotFoundException("指定要压缩的文件: " + fileToZip + " 不存在!");
            }

            using (FileStream fs = File.OpenRead(fileToZip))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();

                using (FileStream ZipFile = File.Create(zipedFile))
                {
                    using (ZipOutputStream ZipStream = new ZipOutputStream(ZipFile))
                    {
                        string fileName = fileToZip.Substring(fileToZip.LastIndexOf("\\") + 1);
                        ZipEntry ZipEntry = new ZipEntry(fileName);
                        ZipStream.PutNextEntry(ZipEntry);
                        ZipStream.SetLevel(5);

                        ZipStream.Write(buffer, 0, buffer.Length);
                        ZipStream.Finish();
                        ZipStream.Close();
                    }
                }
            }
        }


        /// <summary>
        /// 解压缩一个 zip 文件。
        /// </summary>
        /// <param name="zipedFile">压缩文件</param>
        /// <param name="strDirectory">解压缩的目录</param>
        /// <param name="password">zip 文件的密码。</param>
        /// <param name="overWrite">是否覆盖已存在的文件。</param>
        public static void UnZip(string zipedFile, string strDirectory, string password, bool overWrite)
        {
            if (strDirectory == "")
                strDirectory = Directory.GetCurrentDirectory();
            if (!strDirectory.EndsWith("\\"))
                strDirectory = strDirectory + "\\";

            UnZip(File.OpenRead(zipedFile), strDirectory, password, overWrite);
        }

        /// <summary>
        /// 解压缩一个 zip 文件的 Stream。
        /// </summary>
        /// <param name="zipedStream">压缩文件的Stream</param>
        /// <param name="strDirectory">解压缩的目录</param>
        /// <param name="password">zip 文件的密码。</param>
        /// <param name="overWrite">是否覆盖已存在的文件。</param>
        public static void UnZip(Stream zipedStream, string strDirectory, string password, bool overWrite)
        {
            using (ZipInputStream s = new ZipInputStream(zipedStream))
            {
                s.Password = password;
                ZipEntry theEntry;

                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directoryName = "";
                    string pathToZip = "";
                    pathToZip = theEntry.Name;

                    if (pathToZip != "")
                        directoryName = Path.GetDirectoryName(pathToZip) + "\\";

                    string fileName = Path.GetFileName(pathToZip);

                    Directory.CreateDirectory(strDirectory + directoryName);

                    if (fileName != "")
                    {
                        if ((File.Exists(strDirectory + directoryName + fileName) && overWrite) || (!File.Exists(strDirectory + directoryName + fileName)))
                        {
                            using (FileStream streamWriter = File.Create(strDirectory + directoryName + fileName))
                            {
                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);

                                    if (size > 0)
                                        streamWriter.Write(data, 0, size);
                                    else
                                        break;
                                }
                                streamWriter.Close();
                            }
                        }
                    }
                }

                s.Close();
            }
        }

        #endregion

        #region 压缩文件夹/解压缩文件夹

        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="sourceFolder">源文件夹路径</param>
        /// <param name="zipFilePath">生成的zip文件路径</param>
        /// <param name="compressionLevel">压缩级别(0-9)</param>
        public static void ZipDirectory(string sourceFolder, string zipFilePath, int compressionLevel = 6)
        {
            if (!Directory.Exists(sourceFolder))
                throw new DirectoryNotFoundException($"源文件夹不存在: {sourceFolder}");

            using var zipStream = new ZipOutputStream(File.Create(zipFilePath));
            zipStream.SetLevel(compressionLevel); // 设置压缩级别
            ZipFolder(sourceFolder, sourceFolder, zipStream);
            zipStream.Finish();
        }

        private static void ZipFolder(string rootFolder, string currentFolder, ZipOutputStream zipStream)
        {
            var files = Directory.GetFiles(currentFolder);
            foreach (var file in files)
            {
                var relativePath = file[rootFolder.Length..].TrimStart('\\');
                var entry = new ZipEntry(relativePath) { DateTime = DateTime.Now };

                using var fs = File.OpenRead(file);
                entry.Size = fs.Length;
                zipStream.PutNextEntry(entry);
                fs.CopyTo(zipStream);
                zipStream.CloseEntry();
            }

            var folders = Directory.GetDirectories(currentFolder);
            foreach (var folder in folders)
            {
                ZipFolder(rootFolder, folder, zipStream);
            }
        }

        // <summary>
        /// 解压ZIP文件到指定目录
        /// </summary>
        /// <param name="zipFilePath">ZIP文件路径</param>
        /// <param name="outputDirectory">输出目录</param>
        /// <param name="password">可选密码</param>
        public static bool ExtractZip(byte[] zipContent, string outputDirectory, string password = null)
        {
            using MemoryStream stream = new(zipContent);

            stream.Position = 0;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, (int)stream.Length);
            return ExtractZip(stream, outputDirectory, password);
        }

        /// <summary>
        /// 解压ZIP文件到指定目录
        /// </summary>
        /// <param name="zipFilePath">ZIP文件路径</param>
        /// <param name="outputDirectory">输出目录</param>
        /// <param name="password">可选密码</param>
        public static bool ExtractZip(Stream stream, string outputDirectory, string password = null)
        {
            try
            {
                Directory.CreateDirectory(outputDirectory);

                using var zipFile = new ICSharpCode.SharpZipLib.Zip.ZipFile(stream);
                if (!string.IsNullOrEmpty(password))
                    zipFile.Password = password;

                foreach (ZipEntry entry in zipFile)
                {
                    if (!entry.IsFile) continue;

                    string entryPath = Path.Combine(outputDirectory, entry.Name);
                    string directoryPath = Path.GetDirectoryName(entryPath);

                    if (!Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    using (var zipStream = zipFile.GetInputStream(entry))
                    using (var fileStream = System.IO.File.Create(entryPath))
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        while ((bytesRead = zipStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fileStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        } 
        #endregion
    }
}
