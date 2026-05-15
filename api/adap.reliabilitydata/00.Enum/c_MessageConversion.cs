using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BKDataAnalysis;
using System.Data;
using System.Xml;
using highspeed.business._01.Models;
using highspeed.framework;
using highspeed.framework.Common;
using adap.safetyandreliabilityapi._05.Data.Reliability_Prediction;
using Microsoft.SqlServer.Management.Smo.Broker;

namespace highspeed.business._00.Enum
{
    /// <summary>
    /// 报文处理和转换类
    /// </summary>
    public class c_MessageConversion
    {
        #region 协议常量

        /// <summary>
        /// 报文头固定值：0x7E4C
        /// </summary>
        public const ushort PROTOCOL_HEADER = 0x7E4C;

        /// <summary>
        /// 报文接收方：发送总控报文
        /// </summary>
        public const ushort MSG_DEST_MASTER = 0x0001;

        /// <summary>
        /// 方向：数据输入
        /// </summary>
        public const byte DIRECTION_INPUT = 0x01;

        /// <summary>
        /// 方向：数据输出
        /// </summary>
        public const byte DIRECTION_OUTPUT = 0x00;

        #endregion

        public string SaveType = "";
        public string SavePath1 = "";
        public string SaveDataSourceIP = "";
        public string SaveDataSourcePort = "";
        public string SaveDataSourceUser = "";
        public string SaveDataSourcePassWord = "";

        public static c_kernel32Helper kernel32Helper = new c_kernel32Helper(Environment.CurrentDirectory + "\\UserSettings\\UserSettings.ini");

        public void CheckPZ()
        {
            string IP = kernel32Helper.GetIniString("NetSettings", "IPAddress", "");
            if (IP.Split(new char[] { ';' }).Count() > 0)
            {
                string ob1 = IP.Split(new char[] { ';' })[0];

                string Local_IP = ob1.Split(new char[] { ',' })[2];//IP
                string Local_Port1 = ob1.Split(new char[] { ',' })[3];//发送端口
                string Local_Port2 = ob1.Split(new char[] { ',' })[4];//接收端口
                string Ser_IP = ob1.Split(new char[] { ',' })[5];//IP
                string Ser_Port1 = ob1.Split(new char[] { ',' })[6];//服务器发送端口
                string Ser_Port2 = ob1.Split(new char[] { ',' })[7];//服务器接收端口
            }
        }

        /// <summary>
        /// 字符串转16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        /// <summary>
        /// 通用
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string bytesToHexstr(byte[] src)
        {
            try
            {
                return string.Concat(src.Reverse().Select(b => b.ToString("X2")));
            }
            catch (Exception)
            {
                return "0000";
            }
        }

        /// <summary>
        /// 字节数组转16进制字符串(带协议的)
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string bytesToHex(byte[] src)
        {
            try
            {
                if (src == null || src.Length <= 0)
                {
                    return "";
                }

                int CLen = BitConverter.ToInt32(src, 30);//报文长度（新协议偏移30：2报文头+4发送方IP+2发送方端口+4接收方IP+2接收方端口+6备用序号+2报文接收方+2报文类型+4时间+2毫秒=30）
                ushort CType = BitConverter.ToUInt16(src, 22);//报文类型（新协议偏移22：2报文头+4发送方IP+2发送方端口+4接收方IP+2接收方端口+6备用序号+2报文接收方=22）

                int v1 = CType & 0xff;
                string hv1 = Convert.ToString(v1, 16);
                if (hv1.Length < 2)
                {
                    hv1 = "0" + hv1;
                }
                hv1 = "0x" + hv1;

                List<byte> newsrc = new List<byte>();
                for (int m = 0; m <= 34 + CLen - 1; m++)  // 新协议报文头34字节（原32字节+2字节报文接收方）
                {
                    newsrc.Add(src[m]);
                }

                src = newsrc.ToArray();

                StringBuilder ss = new StringBuilder();

                ss.Append("(" + hv1 + "):");

                for (int i = 0; i < src.Length; i++)
                {
                    int v = src[i] & 0xff;
                    string hv = Convert.ToString(v, 16);
                    if (hv.Length < 2)
                    {
                        ss.Append(0);
                    }
                    ss.Append(hv + " ");
                }

                return ss.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 16进制字符串转二进制数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string BinaryToString(string str)
        {
            System.Text.RegularExpressions.CaptureCollection cs = System.Text.RegularExpressions.Regex.Match(str, @"([01]{8})+").Groups[1].Captures;
            string[] data = new string[cs.Count];
            for (int i = 0; i < cs.Count; i++)
            {
                data[i] = Convert.ToByte(cs[i].Value, 2).ToString("x2"); ;
            }
            return string.Join("", data).ToUpper();
        }

        /// <summary>
        /// 二进制数组转字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string StringToBinary(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 8);
            foreach (byte item in data)
            {
                sb.Append(Convert.ToString(item, 2).PadLeft(8, '0'));
            }
            return sb.ToString();
        }

        #region 组合报文方法
        /// <summary>
        /// 根据测试用例和协议数据库比对，生成配置报文内容列表（使用协议数据库DataTable精确匹配）
        /// 与BuildConfigContentList的区别：此方法返回byte[]数组列表，可直接用于报文组装
        /// </summary>
        /// <param name="testCaseDt">测试用例DataTable，需包含列：Category(类别名)、Description(配置项描述)、Content(配置内容)</param>
        /// <param name="protocolDt">协议数据库DataTable，需包含列：CategoryID、Category、Description、OptionID、Content</param>
        /// <param name="direction">方向：0x01数据输入，0x00数据输出，默认0x01</param>
        /// <returns>匹配成功的指令字节数组列表（每条指令格式：方向1字节 + 配置类别1字节 + 配置项1字节 + 配置内容8字节 = 11字节）</returns>
        public static List<byte[]> BuildConfigMessageByteList(int TestCaseID, byte direction = DIRECTION_INPUT)
        {
            try
            {
                List<byte[]> ConfigMessageByteList = new List<byte[]>();
                ProtocolConfig protocol = new ProtocolConfig();
                List<ConfigureMessageContent> AddContentList = new List<ConfigureMessageContent>();
                List<ConfigureMessageContent> AddExecuteList = new List<ConfigureMessageContent>();

                //协议信号
                List<ConfigureMessageContent> ContentList = new List<ConfigureMessageContent>();
                ContentList = TestCase_Handles.GetProtocolConfig();

                //测试用例数据
                TestCaseModel testCaseModel= TestCase_Handles.GetOneTestCaseDetail(TestCaseID);

                List<byte[]> ConfigList = new List<byte[]>();
                if (testCaseModel == null)
                {
                    return ConfigList;
                }

                if (ContentList == null || ContentList.Count == 0)
                {
                    return ConfigList;
                }

                string IP = kernel32Helper.GetIniString("NetSettings", "IPAddress", "");
                string ob1 = "";
                string Local_IP = "";
                string Local_Port1 = "";
                string Local_Port2 = "";
                string Ser_IP = "";
                string Ser_Port1 = "";
                string Ser_Port2 = "";
                if (IP.Split(new char[] { ';' }).Count() > 0)
                {
                     Local_IP = IP.Split(new char[] { ',' })[0];//IP
                     Local_Port1 = IP.Split(new char[] { ',' })[1];//发送端口
                     Local_Port2 = IP.Split(new char[] { ',' })[2];//接收端口
                     Ser_IP = IP.Split(new char[] { ',' })[3];//IP
                     Ser_Port1 = IP.Split(new char[] { ',' })[4];//服务器发送端口
                     Ser_Port2 = IP.Split(new char[] { ',' })[5];//服务器接收端口
                }

                protocol.SenderPortID = new char[] { Convert.ToChar(Convert.ToInt32(Local_IP.Split(".".ToArray()[0])[3])), Convert.ToChar(Convert.ToInt32(Local_IP.Split(".".ToArray()[0])[2])), Convert.ToChar(Convert.ToInt32(Local_IP.Split(".".ToArray()[0])[1])), Convert.ToChar(Convert.ToInt32(Local_IP.Split(".".ToArray()[0])[0])) };
                protocol.SenderPortNumber = ushort.Parse(Local_Port1);
                protocol.ReceiverPortID = new char[] { Convert.ToChar(Convert.ToInt32(Ser_IP.Split(".".ToArray()[0])[3])), Convert.ToChar(Convert.ToInt32(Ser_IP.Split(".".ToArray()[0])[2])), Convert.ToChar(Convert.ToInt32(Ser_IP.Split(".".ToArray()[0])[1])), Convert.ToChar(Convert.ToInt32(Ser_IP.Split(".".ToArray()[0])[0])) };
                protocol.ReceiverPortNumber = ushort.Parse(Ser_Port2);
                protocol.MessageSequenceNumber = 0x00001;
                protocol.MessageRecipient = 0x00001;

                TimeSpan toNow = DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                protocol.Time = Convert.ToInt32(toNow.TotalSeconds);
                protocol.Milliseconds = Convert.ToUInt16(toNow.Milliseconds);

                #region 列车信息报文
                //添加报文内容
                ConfigureMessageContent configure = new ConfigureMessageContent();
                configure.Category = 0x01;
                configure.CategoryRemark = "列车信息";
                configure.Description = 0x02;
                configure.DescriptionRemark = "试验名称";
                configure.Content = ContentList.FindAll(x => x.Description == 0x02).ToList().FindAll(y => y.ContentRemark.Contains(testCaseModel.ExtendedField1.Substring(0,4))).FirstOrDefault().Content;
                configure.ContentRemark = ContentList.FindAll(x => x.Description == 0x02).ToList().FindAll(y => y.ContentRemark.Contains(testCaseModel.ExtendedField1.Substring(0, 4))).FirstOrDefault().ContentRemark;
                AddContentList.Add(configure);

                ConfigureMessageContent configure1 = new ConfigureMessageContent();
                configure1.Category = 0x01;
                configure1.CategoryRemark = "列车信息";
                configure1.Description = 0x03;
                configure1.DescriptionRemark = "车型";
                configure1.Content = 0x01;
                configure1.ContentRemark = testCaseModel.Model.ToString();
                AddContentList.Add(configure1);

                ConfigureMessageContent configure2 = new ConfigureMessageContent();
                configure2.Category = 0x01;
                configure2.CategoryRemark = "列车信息";
                configure2.Description = 0x04;
                configure2.DescriptionRemark = "试验对象";
                configure2.Content = 0x01;
                configure2.ContentRemark = testCaseModel.TrainObject.ToString();
                AddContentList.Add(configure2);

                ConfigureMessageContent configure3 = new ConfigureMessageContent();
                configure3.Category = 0x01;
                configure3.CategoryRemark = "列车信息";
                configure3.Description = 0x05;
                configure3.DescriptionRemark = "试验速度";
                configure3.Content = 0x01;
                configure3.ContentRemark = testCaseModel.TrainSpeed.ToString();
                AddContentList.Add(configure3);

                ConfigureMessageContent configure4 = new ConfigureMessageContent();
                configure4.Category = 0x01;
                configure4.CategoryRemark = "列车信息";
                configure4.Description = 0x06;
                configure4.DescriptionRemark = "载荷状态";
                configure4.Content = ContentList.FindAll(x => x.Description == 0x02).ToList().FindAll(y => y.ContentRemark.Contains(testCaseModel.TrainType.ToString())).FirstOrDefault().Content;
                configure4.ContentRemark = testCaseModel.TrainType.ToString();
                AddContentList.Add(configure4);

                ConfigureMessageContent configure5 = new ConfigureMessageContent();
                configure5.Category = 0x01;
                configure5.CategoryRemark = "列车信息";
                configure5.Description = 0x07;
                configure5.DescriptionRemark = "专业";
                configure5.Content = ContentList.FindAll(x => x.Description == 0x07).ToList().FindAll(y => y.ContentRemark.Contains(testCaseModel.ExtendedField1.Substring(0, 4))).FirstOrDefault().Content;
                configure5.ContentRemark = ContentList.FindAll(x => x.Description == 0x02).ToList().FindAll(y => y.ContentRemark.Contains(testCaseModel.ExtendedField1.Substring(0, 4))).FirstOrDefault().ContentRemark;
                AddContentList.Add(configure5);

                ConfigureMessageContent configure6 = new ConfigureMessageContent();
                configure6.Category = 0x01;
                configure6.CategoryRemark = "列车信息";
                configure6.Description = 0x08;
                configure6.DescriptionRemark = "项点";
                configure6.Content = ContentList.FindAll(x => x.Description == 0x08).ToList().FindAll(y => y.ContentRemark.Contains(testCaseModel.ExtendedField1)).FirstOrDefault().Content;
                configure6.ContentRemark = ContentList.FindAll(x => x.Description == 0x02).ToList().FindAll(y => y.ContentRemark.Contains(testCaseModel.ExtendedField1)).FirstOrDefault().ContentRemark;
                AddContentList.Add(configure6);

                ConfigureMessageContent configure7 = new ConfigureMessageContent();
                configure7.Category = 0x02;
                configure7.CategoryRemark = "前置环境";
                configure7.Description = 0x09;
                configure7.DescriptionRemark = "供电";
                configure7.Content = 0x01;
                configure7.ContentRemark = "网压AC 25 kV～29 kV";
                AddContentList.Add(configure7);

                ConfigureMessageContent configure8 = new ConfigureMessageContent();
                configure8.Category = 0x02;
                configure8.CategoryRemark = "前置环境";
                configure8.Description = 0x0A;
                configure8.DescriptionRemark = "工务";
                configure8.Content = 0x01;
                configure8.ContentRemark = "平直道、坡道，线路黏着条件良好";
                AddContentList.Add(configure8);

                ConfigureMessageContent configure9 = new ConfigureMessageContent();
                configure9.Category = 0x02;
                configure9.CategoryRemark = "前置环境";
                configure9.Description = 0x0B;
                configure9.DescriptionRemark = "环境";
                configure9.Content = 0x01;
                configure9.ContentRemark = "风速不大于3.3 m/s";
                AddContentList.Add(configure9);
                #endregion
                

                foreach (TestRecodeModel recode in testCaseModel.TestRecodes)
                {
                    ConfigureMessageContent configurerecode = new ConfigureMessageContent();
                    if (recode.RecodeName == "列车速度")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x0C;
                        configurerecode.DescriptionRemark = "列车速度";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "网压")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x0D;
                        configurerecode.DescriptionRemark = "网压";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "网流")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x0E;
                        configurerecode.DescriptionRemark = "网流";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "牵引制动级位")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x0F;
                        configurerecode.DescriptionRemark = "牵引制动级位";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "牵引力需求值")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x10;
                        configurerecode.DescriptionRemark = "牵引力需求值";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "牵引力反馈值")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x11;
                        configurerecode.DescriptionRemark = "牵引力反馈值";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "逆变器输出电压")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x12;
                        configurerecode.DescriptionRemark = "逆变器输出电压";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "逆变器输出电流")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x13;
                        configurerecode.DescriptionRemark = "逆变器输出电流";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "电机电流")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x14;
                        configurerecode.DescriptionRemark = "电机电流";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "制动初速度")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x15;
                        configurerecode.DescriptionRemark = "制动初速度";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "实时加速度")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x16;
                        configurerecode.DescriptionRemark = "实时加速度";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "电制动力需求值")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x17;
                        configurerecode.DescriptionRemark = "电制动力需求值";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "电制动力反馈值")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x18;
                        configurerecode.DescriptionRemark = "电制动力反馈值";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "制动距离")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x19;
                        configurerecode.DescriptionRemark = "制动距离";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "制动级位")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x1A;
                        configurerecode.DescriptionRemark = "制动级位";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "制动指令")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x1B;
                        configurerecode.DescriptionRemark = "制动指令";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "电制动切除")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x1C;
                        configurerecode.DescriptionRemark = "电制动切除";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "主断闭合断开")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x1D;
                        configurerecode.DescriptionRemark = "主断闭合断开";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "逆变器输入电流")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x1E;
                        configurerecode.DescriptionRemark = "逆变器输入电流";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "逆变器温升")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x1F;
                        configurerecode.DescriptionRemark = "逆变器温升";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "电机温升")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x20;
                        configurerecode.DescriptionRemark = "电机温升";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "站停时间")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x21;
                        configurerecode.DescriptionRemark = "站停时间";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "列车总能耗")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x22;
                        configurerecode.DescriptionRemark = "列车总能耗";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "牵引能耗")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x23;
                        configurerecode.DescriptionRemark = "牵引能耗";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "再生能耗")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x24;
                        configurerecode.DescriptionRemark = "再生能耗";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "牵引级位")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x25;
                        configurerecode.DescriptionRemark = "牵引级位";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "加速度")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x26;
                        configurerecode.DescriptionRemark = "加速度";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "压力")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x27;
                        configurerecode.DescriptionRemark = "压力";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "燃弧")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x28;
                        configurerecode.DescriptionRemark = "燃弧";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "激光系统传感器")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x29;
                        configurerecode.DescriptionRemark = "激光系统传感器";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "测速雷达")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x2A;
                        configurerecode.DescriptionRemark = "测速雷达";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "视频监控")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x2B;
                        configurerecode.DescriptionRemark = "视频监控";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "拖车制动缸压力")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x2C;
                        configurerecode.DescriptionRemark = "拖车制动缸压力";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "动车制动缸压力")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x2D;
                        configurerecode.DescriptionRemark = "动车制动缸压力";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "紧急制动指令")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x2E;
                        configurerecode.DescriptionRemark = "紧急制动指令";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "车辆紧急制动时的速度")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x2F;
                        configurerecode.DescriptionRemark = "车辆紧急制动时的速度";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "制动盘温度")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x30;
                        configurerecode.DescriptionRemark = "制动盘温度";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "闸片温度")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x31;
                        configurerecode.DescriptionRemark = "闸片温度";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "拖车压力")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x32;
                        configurerecode.DescriptionRemark = "拖车压力";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "每站停站时间")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x33;
                        configurerecode.DescriptionRemark = "每站停站时间";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                    else if (recode.RecodeName == "对应车轮温度")
                    {
                        configurerecode.Category = 0x03;
                        configurerecode.CategoryRemark = "总控记录数据指令";
                        configurerecode.Description = 0x34;
                        configurerecode.DescriptionRemark = "对应车轮温度";
                        configurerecode.Content = 0x01;
                        configurerecode.ContentRemark = "";
                        AddContentList.Add(configurerecode);
                    }
                }

                
                foreach (TestStepModel testStep in testCaseModel.TestCommands)
                {
                    ConfigureMessageContent configureStep = new ConfigureMessageContent();
                    if (testStep.TestStepName == "制动缓解")
                    {
                        configureStep.Category = 0x04;
                        configureStep.CategoryRemark = "执行指令";
                        configureStep.Description = 0x35;
                        configureStep.DescriptionRemark = "制动缓解";
                        configureStep.Content = 0x01;
                        configureStep.ContentRemark = "制动缓解";
                        AddExecuteList.Add(configureStep);
                    }
                    else if (testStep.TestStepName == "牵引级位")
                    {
                        configureStep.Category = 0x04;
                        configureStep.CategoryRemark = "执行指令";
                        configureStep.Description = 0x36;
                        configureStep.DescriptionRemark = "牵引级位";
                        configureStep.Content = ContentList.FindAll(x => x.Description == 0x36).ToList().FindAll(y => y.ContentRemark.Contains(testStep.StepValue.ToString())).FirstOrDefault().Content;
                        configureStep.ContentRemark = testStep.StepValue.ToString();
                        AddExecuteList.Add(configureStep);
                    }
                    else if (testStep.TestStepName == "速度")
                    {
                        configureStep.Category = 0x04;
                        configureStep.CategoryRemark = "执行指令";
                        configureStep.Description = 0x37;
                        configureStep.DescriptionRemark = "速度";
                        configureStep.Content = Convert.ToByte(testStep.StepValue);
                        configureStep.ContentRemark = "用户自定义";
                        AddExecuteList.Add(configureStep);
                    }
                    else if (testStep.TestStepName == "惰行")
                    {
                        configureStep.Category = 0x04;
                        configureStep.CategoryRemark = "执行指令";
                        configureStep.Description = 0x38;
                        configureStep.DescriptionRemark = "惰行";
                        configureStep.Content = Convert.ToByte(testStep.StepValue);
                        configureStep.ContentRemark = "惰行";
                        AddExecuteList.Add(configureStep);
                    }
                    else if (testStep.TestStepName == "制动")
                    {
                        configureStep.Category = 0x04;
                        configureStep.CategoryRemark = "执行指令";
                        configureStep.Description = 0x39;
                        configureStep.DescriptionRemark = "制动";
                        configureStep.Content = ContentList.FindAll(x => x.Description == 0x39).ToList().FindAll(y => y.ContentRemark.Contains(testStep.StepValue.ToString())).FirstOrDefault().Content;
                        configureStep.ContentRemark = testStep.StepValue.ToString();
                        AddExecuteList.Add(configureStep);
                    }
                    else if (testStep.TestStepName == "延时等待")
                    {
                        configureStep.Category = 0x04;
                        configureStep.CategoryRemark = "执行指令";
                        configureStep.Description = 0x3A;
                        configureStep.DescriptionRemark = "延时等待";
                        configureStep.Content = Convert.ToByte(testStep.StepValue);
                        configureStep.ContentRemark = "用户自定义";
                        AddExecuteList.Add(configureStep);
                    }
                    else if (testStep.TestStepName == "网压中断")
                    {
                        configureStep.Category = 0x04;
                        configureStep.CategoryRemark = "执行指令";
                        configureStep.Description = 0x3B;
                        configureStep.DescriptionRemark = "网压中断";
                        configureStep.Content = 0x01;
                        configureStep.ContentRemark = "网压中断";
                        AddExecuteList.Add(configureStep);
                    }
                    else if (testStep.TestStepName == "网压恢复")
                    {
                        configureStep.Category = 0x04;
                        configureStep.CategoryRemark = "执行指令";
                        configureStep.Description = 0x3C;
                        configureStep.DescriptionRemark = "网压恢复";
                        configureStep.Content = 0x01;
                        configureStep.ContentRemark = "网压恢复";
                        AddExecuteList.Add(configureStep);
                    }
                    else if (testStep.TestStepName == "DCU")
                    {
                        configureStep.Category = 0x04;
                        configureStep.CategoryRemark = "执行指令";
                        configureStep.Description = 0x3D;
                        configureStep.DescriptionRemark = "DCU";
                        configureStep.Content = 0x00;
                        configureStep.ContentRemark = "";
                        AddExecuteList.Add(configureStep);
                    }
                    else if (testStep.TestStepName == "连挂车辆载荷")
                    {
                        configureStep.Category = 0x04;
                        configureStep.CategoryRemark = "执行指令";
                        configureStep.Description = 0x3E;
                        configureStep.DescriptionRemark = "连挂车辆载荷";
                        configureStep.Content = 0x01;
                        configureStep.ContentRemark = "连挂车辆载荷";
                        AddExecuteList.Add(configureStep);
                    }
                    else if (testStep.TestStepName == "电制动")
                    {
                        configureStep.Category = 0x04;
                        configureStep.CategoryRemark = "执行指令";
                        configureStep.Description = 0x3F;
                        configureStep.DescriptionRemark = "电制动";
                        configureStep.Content = 0x01;
                        configureStep.ContentRemark = "电制动";
                        AddExecuteList.Add(configureStep);
                    }
                    else if (testStep.TestStepName == "紧急制动")
                    {
                        configureStep.Category = 0x04;
                        configureStep.CategoryRemark = "执行指令";
                        configureStep.Description = 0x41;
                        configureStep.DescriptionRemark = "紧急制动";
                        configureStep.Content = 0x01;
                        configureStep.ContentRemark = "紧急制动";
                        AddExecuteList.Add(configureStep);
                    }
                    else if (testStep.TestStepName == "保持制动")
                    {
                        configureStep.Category = 0x04;
                        configureStep.CategoryRemark = "执行指令";
                        configureStep.Description = 0x42;
                        configureStep.DescriptionRemark = "保持制动";
                        configureStep.Content = 0x01;
                        configureStep.ContentRemark = "保持制动";
                        AddExecuteList.Add(configureStep);
                    }
                    else if (testStep.TestStepName == "For循环")
                    {
                        configureStep.Category = 0x04;
                        configureStep.CategoryRemark = "执行指令";
                        configureStep.Description = 0x43;
                        configureStep.DescriptionRemark = "For 循环";
                        configureStep.Content = 0x00;
                        configureStep.ContentRemark = "";
                        AddExecuteList.Add(configureStep);
                    }
                    else if (testStep.TestStepName == "If判断")
                    {
                        configureStep.Category = 0x04;
                        configureStep.CategoryRemark = "执行指令";
                        configureStep.Description = 0x44;
                        configureStep.DescriptionRemark = "If 判断";
                        configureStep.Content = 0x00;
                        configureStep.ContentRemark = "";
                        AddExecuteList.Add(configureStep);
                    }
                    else if (testStep.TestStepName == "M2主控")
                    {
                        configureStep.Category = 0x04;
                        configureStep.CategoryRemark = "执行指令";
                        configureStep.Description = 0x45;
                        configureStep.DescriptionRemark = "M2主控";
                        configureStep.Content = 0x01;
                        configureStep.ContentRemark = "M2主控";
                        AddExecuteList.Add(configureStep);
                    }
                    else if (testStep.TestStepName == "M1主控")
                    {
                        configureStep.Category = 0x04;
                        configureStep.CategoryRemark = "执行指令";
                        configureStep.Description = 0x46;
                        configureStep.DescriptionRemark = "M1主控";
                        configureStep.Content = 0x01;
                        configureStep.ContentRemark = "M1主控";
                        AddExecuteList.Add(configureStep);
                    }

                }

                foreach (var item in AddContentList)
                {
                    byte[] bytes = new byte[]
                    {
                        item.Category,
                        item.Description,
                        item.Content
                    };
                    ConfigMessageByteList.Add(bytes);
                }

                return ConfigMessageByteList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("BuildConfigMessageByteList错误: " + ex.Message);
                return new List<byte[]>();
            }
        }

        /// <summary>
        /// 将报文与报文头进行组合 生成报文List<byte[]>
        /// </summary>
        /// <param name="SynchronousHead"></param>
        /// <param name="protocol"></param>
        /// <param name="MessageType"></param>
        /// <param name="messageByte"></param>
        /// <returns></returns>
        public static List<byte[]> CreateHeadMessage(ProtocolConfig protocol, List<byte[]> messageByte)
        {
            try
            {
                ushort rp1 = PROTOCOL_HEADER;//同步头
                /*char[] rp2 = new char[] { Convert.ToChar(Convert.ToInt32(protocol.SenderPortID.Split(".".ToArray()[0])[3])), Convert.ToChar(Convert.ToInt32(LocalIP.Split(".".ToArray()[0])[2])), Convert.ToChar(Convert.ToInt32(LocalIP.Split(".".ToArray()[0])[1])), Convert.ToChar(Convert.ToInt32(LocalIP.Split(".".ToArray()[0])[0])) };//发送方终端号IP
                ushort rp3 = ushort.Parse(LocalPort);//发送方端口号
                char[] rp4 = new char[] { Convert.ToChar(Convert.ToInt32(ServerIP.Split(".".ToArray()[0])[3])), Convert.ToChar(Convert.ToInt32(ServerIP.Split(".".ToArray()[0])[2])), Convert.ToChar(Convert.ToInt32(ServerIP.Split(".".ToArray()[0])[1])), Convert.ToChar(Convert.ToInt32(ServerIP.Split(".".ToArray()[0])[0])) };//接收方终端号IP
                ushort rp5 = ushort.Parse(ServerPort);//接收方端口号*/
                ushort rp6 = 0x00;//备用（默认后两位为序号）

                //时间
                TimeSpan toNow = DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                int rp8 = Convert.ToInt32(toNow.TotalSeconds);
                ushort rp8_ms = Convert.ToUInt16(toNow.Milliseconds);

                byte[] MessageContent = new byte[] { };//报文内容
                int rp9 = 0x00000000; //报文长度

                ushort i = 0;
                List<byte> resArr_Message = new List<byte>();
                List<string> CW = new List<string>();

                byte[] p1 = BitConverter.GetBytes(rp1);
                byte[] p2 = new byte[] { BitConverter.GetBytes(protocol.SenderPortID[0])[0], BitConverter.GetBytes(protocol.SenderPortID[1])[0], BitConverter.GetBytes(protocol.SenderPortID[2])[0], BitConverter.GetBytes(protocol.SenderPortID[3])[0] };
                byte[] p3 = BitConverter.GetBytes(protocol.SenderPortNumber);
                byte[] p4 = new byte[] { BitConverter.GetBytes(protocol.ReceiverPortID[0])[0], BitConverter.GetBytes(protocol.ReceiverPortID[1])[0], BitConverter.GetBytes(protocol.ReceiverPortID[2])[0], BitConverter.GetBytes(protocol.ReceiverPortID[3])[0] };
                byte[] p5 = BitConverter.GetBytes(protocol.ReceiverPortNumber);
                // 新协议：备用4字节(固定0x00) + 报文序号2字节，共6字节（总字节数与旧协议相同）
                byte[] p6 = new byte[] { 0, 0, 0, 0, BitConverter.GetBytes(rp6)[0], BitConverter.GetBytes(rp6)[1] };
                // 新增：报文接收方2字节（默认0x0001代表发送总控报文）
                byte[] p6_dest = BitConverter.GetBytes(0x0001);
                byte[] p8 = BitConverter.GetBytes(rp8);
                byte[] p8_ms = BitConverter.GetBytes(rp8_ms);
                byte[] p9 = BitConverter.GetBytes(rp9);
                byte[] p10 = MessageContent;

                List<byte> res = new List<byte>();
                List<byte[]> resArr = new List<byte[]>();
                foreach (var item in messageByte)
                {
                    res.AddRange(p1);
                    res.AddRange(p2);
                    res.AddRange(p3);
                    res.AddRange(p4);
                    res.AddRange(p5);
                    res.AddRange(p6);
                    res.AddRange(p6_dest); // 新协议新增：报文接收方
                    res.AddRange(p8);
                    res.AddRange(p8_ms);
                    res.AddRange(p9);
                    res.AddRange(p10);

                    byte[] Send = res.ToArray();
                    resArr.Add(Send);
                }

                return resArr;
            }
            catch (Exception ex)
            {
                throw;
                return new List<byte[]>();
            }
        }

        /// <summary>
        /// 将指令列表组装为完整的配置报文字节数组
        /// </summary>
        /// <param name="instructionList">由BuildConfigMessageByteList返回的指令列表</param>
        /// <returns>完整的报文内容byte[]（含2字节指令条数 + 所有指令数据）</returns>
        public static byte[] PackConfigMessage(List<byte[]> instructionList)
        {
            if (instructionList == null || instructionList.Count == 0)
            {
                return new byte[0];
            }

            try
            {
                List<byte> fullContent = new List<byte>();

                // 指令条数：2字节ushort（小端序）
                ushort count = (ushort)instructionList.Count;
                byte[] countBytes = BitConverter.GetBytes(count);
                fullContent.Add(countBytes[0]); // 低字节
                fullContent.Add(countBytes[1]); // 高字节

                // 追加所有指令
                foreach (byte[] instr in instructionList)
                {
                    fullContent.AddRange(instr);
                }

                return fullContent.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine("PackConfigMessage错误: " + ex.Message);
                return new byte[0];
            }
        }

        /// <summary>
        /// 解析配置报文字节数组为ConfigureMessageContent列表
        /// </summary>
        /// <param name="messageBytes">报文内容字节数组</param>
        /// <param name="direction">报文方向</param>
        /// <returns>配置内容列表</returns>
        public static List<ConfigureMessageContent> ParseConfigMessage(byte[] messageBytes, byte direction = DIRECTION_OUTPUT)
        {
            List<ConfigureMessageContent> ContentList = new List<ConfigureMessageContent>();

            if (messageBytes == null || messageBytes.Length < 2)
            {
                return ContentList;
            }

            try
            {
                int offset = 0;

                // 解析指令条数（2字节ushort）
                ushort commandCount = BitConverter.ToUInt16(messageBytes, offset);
                offset += 2;

                // 逐条解析指令
                // 格式：方向(1) + 配置类别(1) + 配置项(1) + 备用(1) + 配置内容(8) = 12字节
                int instructionLength = 12;

                for (int i = 0; i < commandCount && offset + instructionLength <= messageBytes.Length; i++)
                {
                    /*ConfigureMessageContent configContent = new ConfigureMessageContent
                    {
                        Direction = messageBytes[offset],
                        Category = messageBytes[offset + 1],
                        Description = messageBytes[offset + 2],
                        Content = messageBytes[offset + 4]
                    };

                    // 解析Double类型配置内容（字节5-12）
                    double contentValue = BitConverter.ToDouble(messageBytes, offset + 5);

                    // 填充描述信息（根据ID查表）
                    if (CategoryIDToName.ContainsKey(configContent.Category))
                    {
                        configContent.CategoryRemark = CategoryIDToName[configContent.Category];
                    }

                    // 填充内容备注
                    configContent.ContentRemark = contentValue.ToString();

                    ContentList.Add(configContent);
                    offset += instructionLength;*/
                }

                return ContentList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ParseConfigMessage错误: " + ex.Message);
                return ContentList;
            }
        }

        

#endregion

        #region 辅助方法

        /// <summary>
        /// 获取DataTable中匹配的列名
        /// </summary>
        private static string GetColumnName(DataTable dt, params string[] possibleNames)
        {
            foreach (string name in possibleNames)
            {
                if (dt.Columns.Contains(name))
                {
                    return name;
                }
            }
            return possibleNames[0];
        }

        /// <summary>
        /// 获取DataRow中指定字段的值
        /// </summary>
        private static string GetFieldValue(DataRow row, string fieldName)
        {
            // 尝试多个可能的列名
            string[] possibleNames = { fieldName };

            foreach (string name in possibleNames)
            {
                if (row.Table.Columns.Contains(name))
                {
                    return row[name]?.ToString()?.Trim() ?? "";
                }
            }
            return "";
        }

        /// <summary>
        /// 解析类别ID
        /// </summary>
        private static byte ParseCategoryID(DataRow row, string categoryCol, string categoryIdCol, string testCategory)
        {
            // 首先尝试从CategoryID列获取
            if (protocolDt != null && protocolDt.Columns.Contains("CategoryID"))
            {
                var categoryIdValue = row["CategoryID"];
                if (categoryIdValue != null && categoryIdValue != DBNull.Value)
                {
                    return ParseHexOrDec(categoryIdValue.ToString());
                }
            }

            /*// 从类别名称映射获取
            if (CategoryNameToID.ContainsKey(testCategory))
            {
                return CategoryNameToID[testCategory];
            }*/

            return 0x00;
        }

        // 临时变量用于辅助方法
        private static DataTable protocolDt;

        /// <summary>
        /// 解析配置项ID
        /// </summary>
        private static byte ParseDescriptionID(DataRow row, string descriptionCol)
        {
            if (row[descriptionCol] != null && row[descriptionCol] != DBNull.Value)
            {
                return ParseHexOrDec(row[descriptionCol].ToString());
            }
            return 0x00;
        }

        /// <summary>
        /// 解析选项ID
        /// </summary>
        private static byte ParseContentID(DataRow row, string contentCol, string testContent)
        {
            // 尝试从OptionID列获取
            if (row.Table.Columns.Contains("OptionID"))
            {
                var optionIdValue = row["OptionID"];
                if (optionIdValue != null && optionIdValue != DBNull.Value)
                {
                    return ParseHexOrDec(optionIdValue.ToString());
                }
            }

            // 尝试从Content列获取
            if (row.Table.Columns.Contains(contentCol))
            {
                var contentValue = row[contentCol];
                if (contentValue != null && contentValue != DBNull.Value)
                {
                    return ParseHexOrDec(contentValue.ToString());
                }
            }

            return 0x00;
        }

        /// <summary>
        /// 解析十六进制或十进制字符串为byte
        /// </summary>
        private static byte ParseHexOrDec(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return 0x00;
            }

            value = value.Trim();
            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                return Convert.ToByte(value.Substring(2), 16);
            }
            else
            {
                return Convert.ToByte(value);
            }
        }

        /// <summary>
        /// SQL转义
        /// </summary>
        private static string EscapeSql(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            return value.Replace("'", "''");
        }

        /// <summary>
        /// IP地址转字节数组
        /// </summary>
        private static byte[] IPToBytes(string ip)
        {
            byte[] bytes = new byte[4];
            try
            {
                string[] parts = ip.Split('.');
                if (parts.Length == 4)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        bytes[i] = Convert.ToByte(parts[i]);
                    }
                }
            }
            catch
            {
                // 解析失败返回全零
            }
            return bytes;
        }

        #endregion
    } 
}
