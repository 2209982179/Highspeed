using log4net;
using log4net.Appender;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace highspeed.framework.Common
{
    /// <summary>
    /// 日志记录
    /// </summary>
    public static class Logger
    {
        private static object locker = new object();
        private static ILog log;
        static Logger()
        {
            // log4net配置文件路径
            var userLoggerConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");

            // 如果配置文件不存在，创建默认配置
            if (!File.Exists(userLoggerConfigFile))
            {
                CreateDefaultConfig(userLoggerConfigFile);
            }

            // 加载配置文件
            log4net.Config.XmlConfigurator.Configure(new Uri(userLoggerConfigFile));

            // 线程单例
            if (log == null)
                lock (locker)
                {
                    if (log == null)
                    {
                        log = LogManager.GetLogger(AppDomain.CurrentDomain.FriendlyName);

                        // 更改日志文件保存路径
                        var appenders = LogManager.GetRepository().GetAppenders();
                        foreach (var appender in appenders)
                        {
                            if (appender is RollingFileAppender)
                            {
                                var ap = appender as RollingFileAppender;
                                ap.File = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "/Logs/Log_";
                                ap.LockingModel = new FileAppender.MinimalLock();
                                ap.ActivateOptions();
                            }
                        }
                    }
                }
        }

        /// <summary>
        /// 创建默认配置
        /// </summary>
        private static void CreateDefaultConfig(string configFilePath)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "adap.framework.log4net.config";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    File.WriteAllText(configFilePath, result, Encoding.UTF8);
                }
            }
            catch { }
        }

        /// <summary>
        /// 捕获未预见的异常
        /// </summary>
        public static void HandleApplicationException()
        {
            // 应用程序未捕获异常
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            }
            catch (Exception ex)
            {
                Error("HandleApplicationException", "未能注册应用程序域未捕获异常 Handler.", ex);
            }
        }

        /// <summary>
        /// 非UI线程抛出全局异常事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var exception = e.ExceptionObject as Exception;
                if (exception != null)
                {
                    FileLog("CurrentDomainUnhandledException", exception.Message, exception);
                }
            }
            catch (Exception ex)
            {
                FileLog("CurrentDomainUnhandledException", "不可恢复的应用程序域未捕获异常: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 文件日志，用于处理崩溃异常
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void FileLog(string source, string message, Exception ex)
        {
            try
            {
                var file = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "/Logs/log_" + DateTime.Now.ToString("yyyyMMdd") + "_FileLog.txt";
                if (File.Exists(file))
                {
                    File.AppendAllText(file, DateTime.Now.ToString("yyyyMMdd HH:mm:ss,fff") + " " + LogFormat(source, message, ex));
                }
                else
                {
                    File.WriteAllText(file, DateTime.Now.ToString("yyyyMMdd HH:mm:ss,fff") + " " + LogFormat(source, message, ex));
                }
            }
            catch { }
        }

        /// <summary>
        /// DEBUG Level 指出细粒度信息事件对调试应用程序是非常有帮助的。
        /// </summary>
        /// <param name="source">日志消息来源</param>
        /// <param name="message">日志消息</param>
        public static void Debug(string source, string message)
        {
            try
            {
                log.Debug(LogFormat(source, message));
            }
            catch { }
        }

        /// <summary>
        /// DEBUG Level 指出细粒度信息事件对调试应用程序是非常有帮助的。
        /// </summary>
        /// <param name="source">日志消息来源</param>
        /// <param name="message">日志消息</param>
        /// <param name="e">Exception对象，用于记录调用堆栈</param>
        public static void Debug(string source, string message, Exception e)
        {
            try
            {
                log.Debug(LogFormat(source, message, e), e);
            }
            catch { }
        }

        /// <summary>
        /// INFO level 表明 消息在粗粒度级别上突出强调应用程序的运行过程。
        /// </summary>
        /// <param name="source">日志消息来源</param>
        /// <param name="message">日志消息</param>
        public static void Info(string source, string message)
        {
            try
            {
                log.Info(LogFormat(source, message));
            }
            catch { }
        }

        /// <summary>
        /// WARN level 表明会出现潜在错误的情形。
        /// </summary>
        /// <param name="source">日志消息来源</param>
        /// <param name="message">日志消息</param>
        public static void Warn(string source, string message)
        {
            try
            {
                log.Warn(LogFormat(source, message));
            }
            catch { }
        }

        /// <summary>
        /// ERROR level 指出虽然发生错误事件，但仍然不影响系统的继续运行。
        /// </summary>
        /// <param name="source">日志消息来源</param>
        /// <param name="message">日志消息</param>
        public static void Error(string source, string message)
        {
            try
            {
                log.Error(LogFormat(source, message));
            }
            catch { }
        }

        /// <summary>
        /// ERROR level 指出虽然发生错误事件，但仍然不影响系统的继续运行。
        /// </summary>
        /// <param name="source">日志消息来源</param>
        /// <param name="message">日志消息</param>
        /// <param name="e">Exception对象，用于记录调用堆栈</param>
        public static void Error(string source, string message, Exception e)
        {
            try
            {
                log.Error(LogFormat(source, message, e), e);
            }
            catch { }
        }

        /// <summary>
        /// ERROR level 指出虽然发生错误事件，但仍然不影响系统的继续运行。
        /// </summary>
        /// <param name="e">Exception对象，用于记录调用堆栈</param>
        public static void Error(Exception e)
        {
            try
            {
                log.Error(LogFormat(e.Source, e.Message, e), e);
            }
            catch { }
        }

        /// <summary>
        /// FATAL level 指出每个严重的错误事件将会导致应用程序的退出。
        /// </summary>
        /// <param name="source">日志消息来源</param>
        /// <param name="message">日志消息</param>
        public static void Fatal(string source, string message)
        {
            try
            {
                log.Fatal(LogFormat(source, message));
            }
            catch { }
        }

        /// <summary>
        /// FATAL level 指出每个严重的错误事件将会导致应用程序的退出。
        /// </summary>
        /// <param name="source">日志消息来源</param>
        /// <param name="message">日志消息</param>
        /// <param name="e">Exception对象，用于记录调用堆栈</param>
        public static void Fatal(string source, string message, Exception e)
        {
            try
            {
                log.Fatal(LogFormat(source, message, e), e);
            }
            catch { }
        }

        /// <summary>
        /// FATAL level 指出每个严重的错误事件将会导致应用程序的退出。
        /// </summary>
        /// <param name="e">Exception对象，用于记录调用堆栈</param>
        public static void Fatal(Exception e)
        {
            try
            {
                log.Fatal(LogFormat(e.Source, e.Message, e), e);
            }
            catch { }
        }

        private static string LogFormat(string source, string message)
        {
            return string.Format("{0} - {1}", source, message);
        }

        private static string LogFormat(string source, string message, Exception e)
        {
            if (e.StackTrace == null)
                return LogFormat(source, message);
            return string.Format("{0} - {1}\r\n{2}\r\n", source, message, e.StackTrace);
        }
    }
}
