using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace highspeed.framework.Common
{
    public static class FileHelper
    {
        /// <summary>
        /// 检查文件是否存在，如果不存在则创建
        /// </summary>
        /// <param name="file"></param>
        /// <param name="create"></param>
        /// <returns>是否已存在该文件</returns>
        public static bool CheckOrCreateFile(string file, bool create = true)
        {
            if (File.Exists(file)) return true;
            else
            {
                var dir = file.Substring(0, file.LastIndexOf("\\"));
                CheckOrCreateDirctory(dir);
                if (create) File.Create(file).Close();
                return false;
            }
        }

        /// <summary>
        /// 检查文件夹是否存在，如果不存在则创建
        /// </summary>
        /// <param name="dir"></param>
        public static void CheckOrCreateDirctory(string dir)
        {
            if (Directory.Exists(dir)) return;
            else
            {
                var pDir = dir.Substring(0, dir.LastIndexOf('\\'));
                CheckOrCreateDirctory(pDir);
                Directory.CreateDirectory(dir);
            }
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="content"></param>
        /// <param name="append">是否追加到文件。true：追加 false：覆盖</param>
        /// <param name="encoding"></param>
        public static void Write(string file, string content, bool append = false, Encoding encoding = null)
        {
            if (encoding == null) encoding = Encoding.Default;
            if (CheckOrCreateFile(file))
            {
                using (StreamWriter sw = new StreamWriter(file, append, encoding, 1024))
                {
                    sw.Write(content);
                    sw.Flush();
                }
            }
        }

        /// <summary>
        /// 重新创建文件，防止文件占用；
        /// 如果文件已存在，先删除原来的文件再创建
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>true操作成功；false操作失败；</returns>
        public static bool ReCreateFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.Create(filePath)?.Close();
            }
            catch (Exception ex)
            {
                Logger.Error("FileHelper", "File.Create", ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 重新创建文件，防止文件占用
        /// 如果文件已存在，采用新的名称创建文件；
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>true操作成功；false操作失败；</returns>
        public static bool ReCreateFile(ref string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    filePath = $"{Path.GetDirectoryName(filePath)}\\{Path.GetFileNameWithoutExtension(filePath)}_{DateTime.Now.ToString("yyyyMMddHHmmss")}{Path.GetExtension(filePath)}";
                }
                File.Create(filePath)?.Close();
            }
            catch (Exception ex)
            {
                Logger.Error("FileHelper", "File.Create", ex);
                return false;
            }
            return true;
        }
    }
}
