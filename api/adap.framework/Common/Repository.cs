using highspeed.framework.Data;
using highspeed.framework.Models;
using System.Data;

namespace highspeed.framework.Common
{
    public class Repository
    {
        private static object locker = new object();

        public Guid Guid { get; private set; }
        public SessionInfo Session { get; private set; }

        private string Key { get; set; }
        private DateTime CreationTime { get; set; }
        private DateTime LastRequestTime { get; set; }
        private static List<object> reposhells = new List<object>();

        public static Repository Instance(SessionInfo session)
        {
            if (session?.ProjectDataType == null || string.IsNullOrWhiteSpace(session?.ConnectionString))
                return null;
            return Instance<Repository>(new DBHelper(session.ProjectDataType, session.ConnectionString), session);
        }

        public static T Instance<T>(SessionInfo session) where T : Repository
        {
            if (session?.ProjectDataType == null || string.IsNullOrWhiteSpace(session?.ConnectionString))
                return null;
            return Instance<T>(new DBHelper(session.ProjectDataType, session.ConnectionString), session);
        }

        /// <summary>
        /// 获取线程单例
        /// </summary>
        /// <param name="dbHelper">项目数据库访问类</param>
        /// <returns></returns>
        private static T Instance<T>(DBHelper dbHelper, SessionInfo? session = null) where T : Repository
        {
            var type = typeof(T);
            var fixedKey = (dbHelper.DBType + dbHelper.ConnectionString).ToMD5();
            var instance = reposhells.FirstOrDefault(b => (b as Repository).Key == fixedKey && b.GetType().Equals(type)) as T;
            if (instance == null)
                lock (locker)
                {
                    instance = reposhells.FirstOrDefault(b => (b as Repository).Key == fixedKey) as T;
                    if (instance == null)
                    {
                        instance = (T)type.Assembly.CreateInstance(type.FullName);
                        instance.Key = fixedKey;
                        instance.Guid = Guid.NewGuid();
                        instance.DBHelper = dbHelper;
                        instance.CreationTime = DateTime.Now;
                        instance.Session = session ?? GlobalContext.CurrentSession;
                        instance.Init(instance.Session);
                        reposhells.Add(instance);
                    }
                }
            instance.LastRequestTime = DateTime.Now;
            //清理长期不使用的实例(10Min)
            lock (locker) reposhells = reposhells.Except(reposhells.Where(o => DateTime.Now.Subtract((o as Repository).LastRequestTime).TotalMinutes >= 10)).ToList();
            return instance;
        }

        /// <summary>
        /// 初始化Reposhell
        /// </summary>
        protected virtual void Init(SessionInfo session)
        {
        }
        private DBConnectionInfo connectionInfo;

        public DBHelper DBHelper
        {
            get
            {
                // 每次请求产生一个新的数据库操作对象
                return new DBHelper(connectionInfo.DBType, connectionInfo.ConnectionString);
            }
            private set
            {
                connectionInfo = value.ConnectionInfo;
            }
        }
    }
}