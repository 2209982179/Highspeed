using highspeed.framework.Common;
using highspeed.framework.Data;
using highspeed.framework.Enum;
using Newtonsoft.Json;

namespace highspeed.framework.Models
{
    public abstract class SessionInfo
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool IsOnline { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public ProjectDataType ProjectDataType { get; set; } = ProjectDataType.SQLSVR;
        public string ConnectionString { get; set; }
        public Repository GetReposhell() => Repository.Instance(this);
        public T GetReposhell<T>() where T : Repository => Repository.Instance<T>(this);
        /// <summary>
        /// 基于项目连接的SessionKey
        /// </summary>
        public int ProjectSessionKey => (ConnectionString ?? string.Empty).GetHashCode();
        /// <summary>
        /// 基于用户的SessionKey。当Session包含项目连接时，SessionKey按项目连接+用户ID计算
        /// </summary>
        public int UserSessionKey => $"{ConnectionString}@{UserId}".GetHashCode();
    }
}
