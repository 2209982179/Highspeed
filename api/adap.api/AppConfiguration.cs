namespace highspeed.api
{
    public class AppConfiguration
    {
        public static string FaultTreeBaseUrl { get; set; }

        /// <summary>
        /// 终端配置，可以为数值或字符串
        /// 数值表达总连接数
        /// 字符串表达允许连接的终端，以逗号分隔
        /// </summary>
        public static string EndPoints { get; set; }

        /// <summary>
        /// 是否允许异地登录
        /// </summary>
        public static bool AuthMultiEndPoint { get; set; }

        /// <summary>
        /// 是否允许单个终端多个用户登录
        /// </summary>
        public static bool AuthMultiUser { get; set; }
    }
}
