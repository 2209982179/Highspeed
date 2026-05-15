using highspeed.framework.Data;
using highspeed.framework.DB.MongoDB;
using highspeed.framework.Models;
using System.Reflection;

namespace highspeed.framework.Common
{
    public class GlobalContext
    {
        public static void Setup()
        {
            AsposeHelper.SetupLicense();
            InitialHighSpeedTestDB(ProjectDataType.MYSQL);
            MainDBMigration(ProjectDataType.MYSQL);
        }

        /// <summary>
        /// 初始化测试用例数据库
        /// </summary>
        /// <returns></returns>
        private static bool InitialHighSpeedTestDB(ProjectDataType DBType)
        {
            try
            {
                if (DBType == ProjectDataType.MYSQL)
                {
                    var dbh = DBHelper.MySqlInstance(GlobalContext.ConnectionStrings["AdminDB_MySQL"]);
                    string dbName = $"HighSpeedTest";
                    var sql = $"CREATE DATABASE IF NOT EXISTS `{dbName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";
                    dbh.ExecuteNonQuery(sql);
                }
                else
                {
                    var dbh = DBHelper.MsSqlInstance(GlobalContext.ConnectionStrings["AdminDB"]);
                    string dbName = $"HighSpeedTest";
                    var sql = $" if NOT EXISTS(SELECT * FROM sysdatabases WHERE NAME='{dbName}')" +
                        $"  CREATE DATABASE {dbName}";
                    dbh.ExecuteNonQuery(sql);
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("GlobalContext.InitialHighSpeedTestDB", ex.Message, ex);
                return false;
            }
        }

        /// <summary>
        /// 初始化测试用例数据库 - 表结构
        /// </summary>
        /// <returns></returns>
        private static bool MainDBMigration(ProjectDataType DBType)
        {
            try
            {
                if (DBType == ProjectDataType.MYSQL)
                {
                    //初始化CurrentSession
                    GlobalContext.GetCurrentSession = () =>
                    {
                        return new ApiSessionInfo
                        {
                            UserId = "Admin",
                            UserName = "123456",
                            ProjectId = "001",
                            ProjectName = "Test",
                            ProjectDataType = ProjectDataType.MYSQL,
                            ConnectionString = GlobalContext.ConnectionStrings["MainDB_MySQL"],
                            IsOnline = true,
                        };
                    };
                    var dbh = DBHelper.MySqlInstance(GlobalContext.ConnectionStrings["MainDB_MySQL"]);
                    var assembly = Assembly.GetExecutingAssembly();
                    var resSchema = "highspeed.framework.MainDB_MySql.sql";

                    using (Stream streamSchema = assembly.GetManifestResourceStream(resSchema))
                    using (StreamReader readerSchema = new StreamReader(streamSchema))
                    {
                        string script = readerSchema.ReadToEnd();
                        MySqlDbHelper.ExecutScript(dbh.ConnectionInfo, script);
                    }
                }
                else
                {
                    //初始化CurrentSession
                    GlobalContext.GetCurrentSession = () =>
                    {
                        return new ApiSessionInfo
                        {
                            UserId = "Admin",
                            UserName = "123456",
                            ProjectId = "001",
                            ProjectName = "Test",
                            ProjectDataType = ProjectDataType.SQLSVR,
                            ConnectionString = GlobalContext.ConnectionStrings["MainDB"],
                            IsOnline = true,
                        };
                    };
                    var dbh = DBHelper.MsSqlInstance(GlobalContext.ConnectionStrings["MainDB"]);
                    var assembly = Assembly.GetExecutingAssembly();
                    var resSchema = "highspeed.framework.MainDB.sql";

                    using (Stream streamSchema = assembly.GetManifestResourceStream(resSchema))
                    using (StreamReader readerSchema = new StreamReader(streamSchema))
                    {
                        string script = readerSchema.ReadToEnd();
                        MSSqlDbHelper.ExecutScript(dbh.ConnectionInfo, script);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("GlobalContext.MainDBMigration", ex.Message, ex);
                return false;
            }
        }

        public static SessionInfo? CurrentSession => GetCurrentSession();
        public static Func<SessionInfo?> GetCurrentSession { get => Context.GetCurrentSession; set => Context.GetCurrentSession = value; }
        public static Reposhell Reposhell => CurrentSession.GetReposhell<Reposhell>();

        public static Dictionary<string, string> ConnectionStrings => Context.ConnectionStrings;

        public static Dictionary<string, string> BizConfigurations => Context.BizConfigurations;

        public static MongoDbOperations? mongoDbOperations = Context.mongoDbOperations;
        public static string? MongoDBConnName = Context.MongoDBConnName;
        public static string? MongoDBDatabaseName = Context.MongoDBDatabaseName;
        /// <summary>
        /// 初始化MongoDB
        /// </summary>
        /// <returns></returns>
        public static MongoDbOperations? InitialMongoDB()
        {
            try
            {
                MongoDBConnName = GlobalContext.BizConfigurations["MongoDBConnName"];
                MongoDBDatabaseName = GlobalContext.BizConfigurations["MongoDBDatabaseName"];
                mongoDbOperations = new MongoDbOperations(MongoDBConnName, MongoDBDatabaseName);
                return mongoDbOperations;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }
    }
}