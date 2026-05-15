using highspeed.framework.DB.MongoDB;
using highspeed.framework.Models;

namespace highspeed.framework.Common
{
    public static class Context
    {
        public static SessionInfo? CurrentSession => GetCurrentSession();
        public static Func<SessionInfo?> GetCurrentSession { get; set; }

        public static Repository Reposhell => CurrentSession.GetReposhell();

        public static Dictionary<string, string> ConnectionStrings { get; set; } = new Dictionary<string, string>();

        public static Dictionary<string, string> BizConfigurations { get; set; } = new Dictionary<string, string>();

        public static MongoDbOperations? mongoDbOperations = null;
        public static string? MongoDBConnName = "";
        public static string? MongoDBDatabaseName = "";
    }
}