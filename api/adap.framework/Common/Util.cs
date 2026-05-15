namespace highspeed.framework.Common
{
    public static class Util
    {
        /// <summary>
        /// 分隔符
        /// </summary>
        public const char SplitterChar = '\uf0ff';

        #region 生成随机字符串
        private static Random random = new Random();
        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <param name="length"></param>
        /// <param name="chars">随机字符范围</param>
        /// <returns></returns>
        public static string RandomString(int length, string chars = null)
        {
            if (string.IsNullOrWhiteSpace(chars)) chars = "0123456789ABCDEF";
            return new string(Enumerable.Repeat(chars, length)
                              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        #endregion

        /// <summary>
        /// AppData路径
        /// </summary>
        public static string AppDataPath
        {
            get
            {
                var appDataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "/highspeed/";
                if (!Directory.Exists(appDataPath)) Directory.CreateDirectory(appDataPath);
                return appDataPath;
            }
        }
    }

    public static class Extension
    {
        public static int ToInt(this long value)
        {
            return Convert.ToInt32(value);
        }
    }
}