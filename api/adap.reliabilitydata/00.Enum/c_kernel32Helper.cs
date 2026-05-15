using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Security.Permissions;

namespace BKDataAnalysis
{
    /// <summary>
    /// 系统API操作—kernel32
    /// </summary>
    public class c_kernel32Helper
    {
        string strIniFilePath;  // ini配置文件路径

        /// <summary>
        /// 有参构造函数
        /// </summary>
        /// <param name="strIniFilePath">ini配置文件路径</param>
        /// <returns></returns>
        public c_kernel32Helper(string strIniFilePath)
        {
            if (strIniFilePath != null && strIniFilePath != "")
            {
                this.strIniFilePath = strIniFilePath;
            }
            else
            {
                //初始化ini文件路径
                this.strIniFilePath = Environment.CurrentDirectory + "\\UserSettings\\UserSettings.ini";
            }
        }

        #region 读写Ini文件
        // 返回0表示失败，非0为成功
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        // 返回取得字符串缓冲区的长度
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern long GetPrivateProfileString(string section, string key, string strDefault, StringBuilder retVal, int size, string filePath);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetPrivateProfileInt(string section, string key, int nDefault, string filePath);

        /// <summary>
        /// 获取ini配置文件中的字符串
        /// </summary>
        /// <param name="section">节点名</param>
        /// <param name="key">键名</param>
        /// <param name="strDefault">默认值</param>
        /// <returns>成功true,失败false</returns>
        public string GetIniString(string section, string key, string strDefault)
        {
            StringBuilder sb = new StringBuilder(1024);
            long liRet = GetPrivateProfileString(section, key, strDefault, sb, sb.Capacity, strIniFilePath);
        
            if (sb != null)
            {
                return sb.ToString();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 获取ini配置文件中的整型值
        /// </summary>
        /// <param name="section">节点名</param>
        /// <param name="key">键名</param>
        /// <param name="nDefault">默认值</param>
        /// <returns></returns>
        public int GetIniInt(string section, string key, int nDefault)
        {
            return GetPrivateProfileInt(section, key, nDefault, strIniFilePath);
        }

        /// <summary>
        /// 往ini配置文件写入字符串
        /// </summary>
        /// <param name="section">节点名</param>
        /// <param name="key">键名</param>
        /// <param name="val">要写入的字符串</param>
        /// <returns>成功true,失败false</returns>
        public bool WriteIniString(string section, string key, string val)
        {
            long liRet = WritePrivateProfileString(section, key, val, strIniFilePath);
            return (liRet != 0);
        }

        /// <summary>
        /// 往ini配置文件写入整型数据
        /// </summary>
        /// <param name="section">节点名</param>
        /// <param name="key">键名</param>
        /// <param name="val">要写入的数据</param>
        /// <returns>成功true,失败false</returns>
        public bool WriteIniInt(string section, string key, int val)
        {
            return WriteIniString(section, key, val.ToString());
        }

        #endregion

        #region 精准延迟时间设置
        [DllImport("kernel32.dll")]
        extern static short QueryPerformanceCounter(ref long x);
        [DllImport("kernel32.dll")]
        extern static short QueryPerformanceFrequency(ref long x);

        /// <summary>
        /// 延迟时间
        /// </summary>
        /// <param name="delay_Time">单位微秒</param>
        public void SuperSleep(long delay_Time)
        {
            long stop_Value = 0;
            long start_Value = 0;
            long freq = 0;
            long n = 0;

            QueryPerformanceFrequency(ref freq);//获取CPU频率

            //这里计算CPU进行多少个时钟信号震荡周期，delay_Time写成1000000就是1微秒，写成1000就是1毫秒
            long count = delay_Time * freq / 1000000;

            QueryPerformanceCounter(ref start_Value); //获取初始前值
            while (n < count) //不能精确判定
            {
                QueryPerformanceCounter(ref stop_Value);//获取终止变量值
                n = stop_Value - start_Value;
            }
        }
        #endregion 
    }
}
