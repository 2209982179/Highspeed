using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace highspeed.framework.Common
{
    /// <summary>
    /// Xml序列化与反序列化
    /// </summary>
    public class XmlHelper
    {
        #region 反序列化
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="xml">XML字符串</param>
        /// <returns>对象</returns>
        public static object Deserialize(Type type, string xml)
        {
            try
            {
                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xmldes = new XmlSerializer(type);
                    return xmldes.Deserialize(sr);
                }
            }
            catch (Exception ex) { return null; }
        }


        /// <summary>
        /// XML反序列化
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Deserialize(string filePath, Type type)
        {
            FileStream fs = File.Open(filePath, FileMode.Open);
            using StreamReader sr = new(fs);
            XmlSerializer xz = new(type);
            return xz.Deserialize(sr);
        }

        #endregion

        #region 序列化
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="obj">对象</param>
        /// <returns>XML字符串</returns>
        public static string Serializer(Type type, object obj)
        {
            MemoryStream Stream = new MemoryStream();
            XmlSerializer xml = new XmlSerializer(type);
            try
            {
                //序列化对象
                xml.Serialize(Stream, obj);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            Stream.Position = 0;
            StreamReader sr = new StreamReader(Stream);
            string str = sr.ReadToEnd();

            sr.Dispose();
            Stream.Dispose();

            str = Regex.Replace(str, @"\s+xmlns:xsi=""[^""]*""\s+xmlns:xsd=""[^""]*""", "");
            return str;
        }
        #endregion

        /// <summary>
        /// 创建XML文件，若文件已存在则覆盖该文件
        /// </summary>
        /// <param name="xmlPath">xml路径</param>
        /// <param name="xmlData">xml内容</param>
        public static void CreateXML(string xmlPath, string xmlData)
        {
            StreamWriter writer;
            FileInfo xml = new FileInfo(xmlPath);
            if (!xml.Exists)
            {
                writer = xml.CreateText();
            }
            else
            {
                xml.Delete();
                writer = xml.CreateText();
            }
            writer.Write(xmlData);
            writer.Close();
        }
        /// <summary>
        /// 读取XML文件
        /// </summary>
        /// <param name="xmlPath">xml路径</param>
        /// <returns>xml内容</returns>
        public static string LoadXML(string xmlPath)
        {
            StreamReader r = File.OpenText(xmlPath);
            string info = r.ReadToEnd();
            r.Close();
            return info;
        }


        /// <summary>
        /// 读取Xml字符串，转为XMLDocument对象
        /// </summary>
        /// <param name="xmlString">Xml字符串</param>
        /// <returns>XMLDocument。转换失败时返回null。</returns>
        public static XmlDocument? TryGetXMLDocument(string xmlString)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xmlString);
                return xmlDocument;
            }
            catch { return null; }
        }

        /// <summary>
        /// 读取Html字符串，转为HtmlDocument对象
        /// </summary>
        /// <param name="htmlString">Html字符串</param>
        /// <returns>HtmlDocument。转换失败时返回null。</returns>
        public static HtmlDocument? TryGetHtmlDocument(string htmlString)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlString);
                return doc;
            }
            catch { return null; }
        }
    }
}
