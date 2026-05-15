//#define PRINT_SQL
using highspeed.framework.Common;
using MySql.Data.MySqlClient;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Text;
using System.Text.RegularExpressions;

namespace highspeed.framework.Data
{
    /// <summary>
    /// DBHelper
    /// </summary>
    public class DBHelper : IDisposable
    {
        IDbHelper helper = null;
        /// <summary>
        /// 数据库类型
        /// </summary>
        public ProjectDataType DBType { get; private set; }

        private string connectionString;
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(connectionString)) connectionString = helper?.GetConnectionString();
                return connectionString;
            }
            private set
            {
                connectionString = value;
            }
        }

        public DBConnectionInfo ConnectionInfo { get; private set; }

        /// <summary>
        /// 开始批量操作数据库，在EndBatchExcute之前不会关闭数据库连接
        /// </summary>
        public void StartBatchExcute()
        {
            helper.StartBatchExcute();
        }
        /// <summary>
        /// 结束批量操作数据库，关闭数据库连接
        /// </summary>
        public void EndBatchExcute()
        {
            helper.EndBatchExcute();
        }

        private void Initialize()
        {
            switch (DBType)
            {
                case ProjectDataType.JET:
                    helper = new OleDbHelper(ConnectionString);
                    break;
                case ProjectDataType.SQLSVR:
                    helper = new MSSqlDbHelper(ConnectionString);
                    break;
                case ProjectDataType.MYSQL:
                    helper = new MySqlDbHelper(ConnectionString);
                    break;
                case ProjectDataType.SQLITE:
                    helper = new SQLiteDbHelper(ConnectionString);
                    break;
            }
            ConnectionInfo = new DBConnectionInfo(DBType, ConnectionString, this);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="connectionString">连接字符串</param>
        public DBHelper(ProjectDataType dbType, string connectionString)
        {
            this.DBType = dbType;
            this.ConnectionString = connectionString;
            Initialize();
        }

        public static DBHelper MsSqlInstance(string connectionString)
        {
            return new DBHelper(ProjectDataType.SQLSVR, connectionString);
        }
        public static DBHelper MySqlInstance(string connectionString)
        {
            return new DBHelper(ProjectDataType.MYSQL, connectionString);
        }

        public static DBHelper SQLiteInstance(string connectionString)
        {
            return new DBHelper(ProjectDataType.SQLITE, connectionString);
        }

        public static DBHelper EAPFileInstance(string file)
        {
            if (string.IsNullOrEmpty(file)) return null;
            if (!File.Exists(file)) return null;

            if (file.EndsWith(".eapx", StringComparison.OrdinalIgnoreCase))
                return new DBHelper(ProjectDataType.JET, @"Provider=Microsoft.Jet.OleDb.4.0;Persist Security Info=False;Data Source=" + file);
            else if (file.EndsWith(".eap", StringComparison.OrdinalIgnoreCase))
                return new DBHelper(ProjectDataType.JET, @"Provider=Microsoft.Jet.OleDb.3.5;Persist Security Info=False;Data Source=" + file);
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (helper != null) helper.Dispose();
        }

        /// <summary>
        /// 设置连接字符串
        /// </summary>
        /// <param name="connectionString"></param>
        public void SetConnectionString(string connectionString)
        {
            if (helper != null)
            {
                this.ConnectionString = connectionString;
                helper.SetConnectionString(connectionString);
                ConnectionInfo = new DBConnectionInfo(DBType, connectionString, this);
            }
            else throw new Exception("DBHelper is not initialized.");
        }

        /// <summary>
        /// 打开数据库连接
        /// </summary>
        public void Open()
        {
            if (helper != null) helper.Open();
            else throw new Exception("DBHelper is not initialized.");
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void Close()
        {
            if (helper != null) helper.Close();
            else throw new Exception("DBHelper is not initialized.");
        }

        #region 事务
        /// <summary>
        /// 创建新的事务
        /// </summary>
        /// <returns></returns>
        public DbTransactionOperation NewTransaction()
        {
            return new DbTransactionOperation(this, helper.CreateTransaction());
        }
        #endregion

        #region Public
        /// <summary>
        /// 执行语句，返回响应行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, Dictionary<string, object> parameters = null)
        {
            if (helper != null)
            {
                var formated = FormatSqlAndParams(sql, parameters);
#if DEBUG && PRINT_SQL
                Logger.Debug("highspeed.framework.Data.DBHelper.ExecuteNonQuery", "Formated SQL: " + formated.Key + "; Parameters: " + formated.Value.ToJson());
#endif
                return helper.ExecuteNonQuery(formated.Key, formated.Value);
            }
            throw new Exception("DBHelper is not initialized.");
        }

        /// <summary>
        /// 执行语句，返回值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, Dictionary<string, object> parameters = null)
        {
            if (helper != null)
            {
                var formated = FormatSqlAndParams(sql, parameters);
#if DEBUG && PRINT_SQL
                Logger.Debug("highspeed.framework.Data.DBHelper.ExecuteScalar", "Formated SQL: " + formated.Key + "; Parameters: " + formated.Value.ToJson());
#endif
                return helper.ExecuteScalar(formated.Key, formated.Value);
            }
            throw new Exception("DBHelper is not initialized.");
        }

        /// <summary>
        /// 执行语句，返回值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public object ExecuteScalarWithTempTable(string sql, Dictionary<string, object> parameters = null, params TempTable[] temps)
        {
            if (helper != null)
            {
                var formated = FormatSqlAndParams(sql, parameters);
#if DEBUG && PRINT_SQL
                Logger.Debug("highspeed.framework.Data.DBHelper.ExecuteScalar", "Formated SQL: " + formated.Key + "; Parameters: " + formated.Value.ToJson());
#endif
                return helper.ExecuteScalarWithTempTable(formated.Key, formated.Value, temps);
            }
            throw new Exception("DBHelper is not initialized.");
        }

        /// <summary>
        /// 执行语句，返回值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public object ExecuteScalar(Dictionary<string, Dictionary<string, object>> sqls)
        {
            if (helper != null)
            {
                Dictionary<string, List<KeyValuePair<string, object>>> new_sqls = new Dictionary<string, List<KeyValuePair<string, object>>>();
                foreach (var kv in sqls)
                {
                    var formated = FormatSqlAndParams(kv.Key, kv.Value);
                    new_sqls.Add(formated.Key, formated.Value);
                }
#if DEBUG && PRINT_SQL
                Logger.Debug("highspeed.framework.Data.DBHelper.ExecuteScalar", "Formated SQL: " + formated.Key + "; Parameters: " + formated.Value.ToJson());
#endif
                return helper.ExecuteScalar(new_sqls);
            }
            throw new Exception("DBHelper is not initialized.");
        }

        /// <summary>
        /// 执行语句，返回DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public DataSet Query(string sql, Dictionary<string, object> parameters = null)
        {
            if (helper != null)
            {
                var formated = FormatSqlAndParams(sql, parameters);
#if DEBUG && PRINT_SQL
                Logger.Debug("highspeed.framework.Data.DBHelper.Query", "Formated SQL: " + formated.Key + "; Parameters: " + formated.Value.ToJson());
#endif
                return helper.Query(formated.Key, formated.Value);
            }
            throw new Exception("DBHelper is not initialized.");
        }

        /// <summary>
        /// 使用构建临时表实现查询，返回DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters">参数</param>
        /// <param name="temps">临时表数据</param>
        /// <returns></returns>
        public DataSet QueryWithTempTable(string sql, Dictionary<string, object> parameters = null, params TempTable[] temps)
        {
            if (helper != null)
            {
                var formated = FormatSqlAndParams(sql, parameters);
                var formatedSql = formated.Key;
                #region 替换TempTable占位符
                if (temps.Length > 0)
                {
                    foreach (var temp in temps)
                        formatedSql = formatedSql.Replace($"#Temp@{temp.SqlFormatName}#", temp.TableName);
                }
                #endregion
#if DEBUG && PRINT_SQL
                Logger.Debug("highspeed.framework.Data.DBHelper.QueryDataTable", "Formated SQL: " + formated.Key + "; Parameters: " + formated.Value.ToJson());
#endif
                return helper.QueryWithTempTable(formatedSql, formated.Value, temps);
            }
            throw new Exception("DBHelper is not initialized.");
        }

        /// <summary>
        /// 执行语句，返回DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public DataTable QueryDataTable(string sql, Dictionary<string, object> parameters = null)
        {
            if (helper != null)
            {
                var formated = FormatSqlAndParams(sql, parameters);
#if DEBUG && PRINT_SQL
                Logger.Debug("highspeed.framework.Data.DBHelper.QueryDataTable", "Formated SQL: " + formated.Key + "; Parameters: " + formated.Value.ToJson());
#endif
                return helper.QueryDataTable(formated.Key, formated.Value);
            }
            throw new Exception("DBHelper is not initialized.");
        }

        /// <summary>
        /// 使用构建临时表实现查询，返回DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters">参数</param>
        /// <param name="temps">临时表数据</param>
        /// <returns></returns>
        public DataTable QueryDataTableWithTempTable(string sql, Dictionary<string, object> parameters = null, params TempTable[] temps)
        {
            if (helper != null)
            {
                var formated = FormatSqlAndParams(sql, parameters);
                var formatedSql = formated.Key;
                #region 替换TempTable占位符
                if (temps.Length > 0)
                {
                    foreach (var temp in temps)
                        formatedSql = formatedSql.Replace($"#Temp@{temp.SqlFormatName}#", temp.TableName);
                }
                #endregion
#if DEBUG && PRINT_SQL
                Logger.Debug("highspeed.framework.Data.DBHelper.QueryDataTable", "Formated SQL: " + formated.Key + "; Parameters: " + formated.Value.ToJson());
#endif
                return helper.QueryDataTableWithTempTable(formatedSql, formated.Value, temps);
            }
            throw new Exception("DBHelper is not initialized.");
        }

        /// <summary>
        /// 使用构建临时表执行sql，返回影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters">参数</param>
        /// <param name="temps">临时表数据</param>
        /// <returns></returns>
        public int ExecuteNonQueryWithTempTable(string sql, Dictionary<string, object> parameters = null, params TempTable[] temps)
        {
            if (helper != null)
            {
                var formated = FormatSqlAndParams(sql, parameters);
                var formatedSql = formated.Key;
                #region 替换TempTable占位符
                if (temps.Length > 0)
                {
                    foreach (var temp in temps)
                        formatedSql = formatedSql.Replace($"#Temp@{temp.SqlFormatName}#", temp.TableName);
                }
                #endregion
                return helper.ExecuteNonQueryWithTempTable(formatedSql, formated.Value, temps);
            }
            throw new Exception("DBHelper is not initialized.");
        }

        /// <summary>
        /// 执行存储过程，返回影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public int ExecuteProc(string sql, Dictionary<string, object> parameters = null)
        {
            if (helper != null)
            {
                var formated = FormatSqlAndParams(sql, parameters);
#if DEBUG && PRINT_SQL
                Logger.Debug("highspeed.framework.Data.DBHelper.QueryProc", "Formated SQL: " + formated.Key + "; Parameters: " + formated.Value.ToJson());
#endif
                return helper.ExecuteProc(formated.Key, formated.Value);
            }
            throw new Exception("DBHelper is not initialized.");
        }

        /// <summary>
        /// 执行存储过程，返回DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public DataSet QueryProc(string sql, Dictionary<string, object> parameters = null)
        {
            if (helper != null)
            {
                var formated = FormatSqlAndParams(sql, parameters);
#if DEBUG && PRINT_SQL
                Logger.Debug("highspeed.framework.Data.DBHelper.QueryProc", "Formated SQL: " + formated.Key + "; Parameters: " + formated.Value.ToJson());
#endif
                return helper.QueryProc(formated.Key, formated.Value);
            }
            throw new Exception("DBHelper is not initialized.");
        }

        /// <summary>
        /// 执行存储过程，返回DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public DataTable QueryDataTableProc(string sql, Dictionary<string, object> parameters = null)
        {
            if (helper != null)
            {
                var formated = FormatSqlAndParams(sql, parameters);
#if DEBUG && PRINT_SQL
                Logger.Debug("highspeed.framework.Data.DBHelper.QueryDataTableProc", "Formated SQL: " + formated.Key + "; Parameters: " + formated.Value.ToJson());
#endif
                return helper.QueryDataTableProc(formated.Key, formated.Value);
            }
            throw new Exception("DBHelper is not initialized.");
        }
        #endregion

        private const string reg_ReplaceParam = "#Replace@[^#]*#";
        private const string reg_ListParam = "#List@[^#]*#";
        private const string reg_LikeParam = "#Like@[^#]*#";
        private const string reg_LLikeParam = "#LLike@[^#]*#";
        private const string reg_RLikeParam = "#RLike@[^#]*#";
        private const string reg_ParamPrefix = "@";

        /// <summary>
        /// 用于处理 sql 中的 like 语句和 in 语句传参
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数集合</param>
        /// <returns>新的sql语句和参数集合</returns>
        private KeyValuePair<string, List<KeyValuePair<string, object>>> FormatSqlAndParams(string sql, Dictionary<string, object> parameters)
        {
            #region 处理 NotImplemented
            if ("#NotImplemented#".Equals(sql))
                throw new NotImplementedException("The sql of KEY is not implemented for current database.");
            #endregion

            #region Enum处理
            var matchs = Regex.Matches(sql, "#Enum@[^#]*#");
            #endregion

            if (parameters == null || parameters.Count == 0)
            {
                if (Regex.Match(sql, reg_ReplaceParam).Success
                    || Regex.Match(sql, reg_ListParam).Success
                    || Regex.Match(sql, reg_LikeParam).Success)
                    throw new Exception("The parameters is empty or the quantity does not match for templated sql.");
                else
                    return new KeyValuePair<string, List<KeyValuePair<string, object>>>(sql, null);
            }

            #region Replace处理
            matchs = Regex.Matches(sql, reg_ReplaceParam);
            if (matchs != null && matchs.Count > 0)
            {
                foreach (Match match in matchs)
                {
                    var paramKey = match.Value.Trim('#').Remove(0, 7);
                    if (parameters.ContainsKey(paramKey))
                    {
                        sql = sql.Replace(match.Value, parameters[paramKey].ToString());
                    }
                }
            }
            #endregion

            #region like 语句传参处理
            foreach (var reg in new string[] { reg_LikeParam, reg_LLikeParam, reg_RLikeParam })
            {
                var valueTemp = string.Empty;
                switch (reg)
                {
                    case reg_LikeParam: // Like
                        valueTemp = "%{0}%";
                        break;
                    case reg_LLikeParam: // Left Like
                        valueTemp = "{0}%";
                        break;
                    case reg_RLikeParam: // Right Like
                        valueTemp = "%{0}";
                        break;
                }
                matchs = Regex.Matches(sql, reg);
                if (matchs != null && matchs.Count > 0)
                {
                    foreach (var param in matchs.Select(m => m.Value).Distinct())
                    {
                        var paramKey = param.Trim('#').Remove(0, 4);
                        if (parameters.ContainsKey(paramKey))
                        {
                            var key = paramKey;
                            var value = string.Empty + parameters[paramKey];
                            // MS-SqlServer查询时，处理所有通配符转换为普通字符串
                            if (this.DBType == ProjectDataType.SQLSVR)
                            {
                                key += " escape '^'";
                                value = value.Replace("^", "^^")
                                             .Replace("_", "^_")
                                             .Replace("%", "^%")
                                             .Replace("!", "^!")
                                             .Replace("[", "^[")
                                             .Replace("]", "^]");
                            }
                            sql = sql.Replace(param, key);
                            parameters[paramKey] = string.Format(valueTemp, value);
                        }
                        else
                            throw new Exception("Missing 'like' parameter define. Parameter Name: " + paramKey);
                    }
                }
            }
            #endregion

            // 转换参数
            List<KeyValuePair<string, object>> parsedParams = new List<KeyValuePair<string, object>>();
            foreach (var kv in parameters)
                parsedParams.Add(new KeyValuePair<string, object>(kv.Key, kv.Value));

            #region in 语句传参处理
            matchs = Regex.Matches(sql, reg_ListParam);
            if (matchs != null && matchs.Count > 0)
            {
                foreach (Match match in matchs)
                {
                    var newParams = "";
                    var paramKey = match.Value.Trim('#').Remove(0, 4);
                    if (parsedParams.Count(kv => kv.Key == paramKey) > 0)
                    {
                        var parsedParam = parsedParams.Find(kv => kv.Key == paramKey);
                        var index = parsedParams.IndexOf(parsedParam);
                        var pValues = parsedParam.Value;
                        if (pValues != null && pValues is IEnumerable)
                        {
                            int i = 1;
                            var enumerator = (pValues as IEnumerable).GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                // 避免Key重复，在结尾加入GUID
                                var eKey = paramKey + (i++) + "_" + Guid.NewGuid().ToString("N");
                                newParams += eKey + ',';
                                parsedParams.Insert(index + 1, new KeyValuePair<string, object>(eKey, enumerator.Current));
                            }
                            newParams = newParams.TrimEnd(',');
                            // 从Key第一次出现的位置，移除原Key,加入新的Key；由于参数可能需要被多次使用，所以不能使用Replace
                            var _keyindex = sql.IndexOf(match.Value);
                            sql = sql.Remove(_keyindex, match.Value.Length).Insert(_keyindex, newParams);
                        }
                        else
                            throw new Exception("Null or Invalid value for list parameter. Parameter Name: " + paramKey);
                    }
                    else
                        throw new Exception("Missing 'list' parameter define. Parameter Name: " + paramKey);
                }
            }
            #endregion
            #region 根据sql中参数出现的顺序调整parsedParams列表顺序
            var matches = Regex.Matches(sql, @"@[a-zA-Z_0-9]+");
            if (matches.Count > 0)
            {
                List<KeyValuePair<string, object>> orderedParsedParams = new List<KeyValuePair<string, object>>();
                List<string> paramAppearSquence = new List<string>();
                foreach (Match match in matches)
                {
                    var key = match.Value;
                    if (!paramAppearSquence.Contains(key))
                    {
                        paramAppearSquence.Add(key);
                        var param = parsedParams.FirstOrDefault(p => p.Key == key);
                        if (!param.Equals(default(KeyValuePair<string, object>)))
                            orderedParsedParams.Add(param);
                    }
                }
                parsedParams = orderedParsedParams;
            }
            #endregion

            return new KeyValuePair<string, List<KeyValuePair<string, object>>>(sql, parsedParams);
        }

        /// <summary>
        /// 获取新的自增ID
        /// </summary>
        /// <param name="table">表名</param>
        /// <returns>自增ID</returns>
        public int? GetNewID(string table)
        {
            switch (DBType)
            {
                case ProjectDataType.SQLSVR:
                    return helper.GetNewID(table);
                case ProjectDataType.JET:
                default:
                    return null;
            }
        }

        #region 批量操作 

        /// <summary>
        /// 批量插入数据 
        /// </summary>
        /// <param name="data">需要插入的数据</param>
        public void BulkInsert(DataTable data)
        {
            if (helper != null)
                if (data != null)
                    helper.BulkInsert(data.TableName, data);
                else
                    throw new Exception("DBHelper is not initialized.");
        }

        /// <summary>
        /// 插入临时表数据
        /// </summary>
        /// <param name="tempTable">临时表DataTable</param>
        /// <param name="tableName">临时表名</param>
        public void BulkInsertTempTable(DataTable tempTable, string tableName)
        {
            if (helper != null && tempTable != null)
                helper.BulkInsertTempTable(tempTable, tableName);
            throw new Exception("DBHelper is not initialized.");
        }

        /// <summary>
        /// 插入临时表数据
        /// </summary>
        /// <param name="tempTables">临时表DataTable</param>
        public string[] BulkInsertTempTable(params DataTable[] tempTables)
        {
            if (helper != null && tempTables != null)
                return helper.BulkInsertTempTable(tempTables);
            throw new Exception("DBHelper is not initialized.");
        }

        /// <summary>
        /// 删除临时表数据
        /// </summary>
        /// <param name="tempTables">临时表DataTable名</param>
        public void DropTempTable(params string[] tempTables)
        {
            if (helper != null && tempTables != null)
                helper.DropTempTable(tempTables);
            throw new Exception("DBHelper is not initialized.");
        }

        /// <summary>
        /// 批量更新数据 
        /// </summary>
        /// <param name="data">需要更新的数据</param>
        public void BulkUpdate(DataTable data)
        {
            if (helper != null)
                if (data != null)
                    helper.BulkUpdate(data.TableName, data);
                else
                    throw new Exception("DBHelper is not initialized.");
        }

        #endregion

        /// <summary>
        /// 执行SQL，返回DataTable
        /// </summary>
        /// <param name="key">T297SqlKeys.KEY_xxxxxx</param>
        /// <param name="parameters">参数</param>
        /// <returns>数据集</returns>
        public DataTable ExecuteSqlQuery(string sql, Dictionary<string, object> parameters = null)
        {
            if (helper != null && !string.IsNullOrWhiteSpace(sql))
                return this.QueryDataTable(sql, parameters);
            throw new Exception("DBHelper is not initialized or undefined Sql.");
        }

        /// <summary>
        /// 执行SQL，返回DataSet
        /// </summary>
        /// <param name="key">T297SqlKeys.KEY_xxxxxx</param>
        /// <param name="parameters">参数</param>
        /// <returns>数据集</returns>
        public DataSet ExecuteSqlQuerySet(string sql, Dictionary<string, object> parameters = null)
        {
            if (helper != null && !string.IsNullOrWhiteSpace(sql))
                if (helper is MSSqlDbHelper)
                {
                    return this.Query(sql, parameters);
                }
                else
                {
                    return null;
                }
            throw new Exception("DBHelper is not initialized or undefined Sql.");
        }

    }

    #region 数据库访问对象接口定义
    /// <summary>
    /// 数据库访问对象接口定义
    /// </summary>
    internal interface IDbHelper : IDisposable
    {
        void StartBatchExcute();
        void EndBatchExcute();

        DbTransaction CreateTransaction();

        /// <summary>
        /// 设置连接字符串
        /// </summary>
        /// <param name="connectionString"></param>
        void SetConnectionString(string connectionString);

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <returns></returns>
        string GetConnectionString();

        /// <summary>
        /// 打开数据库连接
        /// </summary>
        IDbCommand Open();

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        void Close();

        /// <summary>
        /// 执行ExecuteNonQuery
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        int ExecuteNonQuery(string sql, List<KeyValuePair<string, object>> parameters = null);

        /// <summary>
        /// 使用构建临时表执行ExecuteNonQuery
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="temps"></param>
        /// <returns></returns>
        int ExecuteNonQueryWithTempTable(string sql, List<KeyValuePair<string, object>> parameters = null, params TempTable[] temps);

        /// <summary>
        /// 查询返回DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        DataSet Query(string sql, List<KeyValuePair<string, object>> parameters = null);

        /// <summary>
        /// 使用构建临时表执行Query
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters">参数</param>
        /// <param name="temps"></param>
        /// <returns></returns>
        DataSet QueryWithTempTable(string sql, List<KeyValuePair<string, object>> parameters = null, params TempTable[] temps);

        /// <summary>
        /// 查询返回DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        DataTable QueryDataTable(string sql, List<KeyValuePair<string, object>> parameters = null);

        /// <summary>
        /// 使用构建临时表实现查询，返回DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters">参数</param>
        /// <param name="temps">临时表数据</param>
        /// <returns></returns>
        DataTable QueryDataTableWithTempTable(string sql, List<KeyValuePair<string, object>> parameters = null, params TempTable[] temps);

        /// <summary>
        /// 查询返回值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        object ExecuteScalar(string sql, List<KeyValuePair<string, object>> parameters = null);

        /// <summary>
        /// 查询返回值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters">参数</param>
        /// <param name="temps">临时表</param>
        /// <returns></returns>
        object ExecuteScalarWithTempTable(string sql, List<KeyValuePair<string, object>> parameters = null, params TempTable[] temps);

        /// <summary>
        /// 查询返回值
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        object ExecuteScalar(Dictionary<string, List<KeyValuePair<string, object>>> sqls);

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        int ExecuteProc(string proc, List<KeyValuePair<string, object>> parameters = null);


        /// <summary>
        /// 查询存储过程返回DataSet
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        DataSet QueryProc(string proc, List<KeyValuePair<string, object>> parameters = null);

        /// <summary>
        /// 查询存储过程返回DataTable
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        DataTable QueryDataTableProc(string proc, List<KeyValuePair<string, object>> parameters = null);

        /// <summary>
        /// 获取新的自增ID
        /// </summary>
        /// <param name="table">表名</param>
        /// <returns></returns>
        int? GetNewID(string table);

        /// <summary>
        /// 批量插入数据 
        /// </summary>
        /// <param name="tableName">数据库表名</param>
        /// <param name="records">需要插入的数据</param>
        void BulkInsert(string tableName, DataTable records);

        /// <summary>
        /// 插入临时表数据
        /// </summary>
        /// <param name="tempTable">临时表DataTable</param>
        /// <param name="tableName">临时表名</param>
        void BulkInsertTempTable(DataTable tempTable, string tableName);

        /// <summary>
        /// 插入临时表数据
        /// </summary>
        /// <param name="tempTables">临时表DataTable</param>
        string[] BulkInsertTempTable(params DataTable[] tempTables);

        /// <summary>
        /// 删除临时表数据
        /// </summary>
        /// <param name="tableNames">临时表名</param>
        void DropTempTable(params string[] tableNames);

        /// <summary>
        /// 批量更新数据 
        /// </summary>
        /// <param name="tableName">数据库表名</param>
        /// <param name="records">需要更新的数据</param>
        void BulkUpdate(string tableName, DataTable records);
    }

    /// <summary>
    /// 项目数据保存类型
    /// </summary>
    public enum ProjectDataType
    {
        /// <summary>
        /// .EAP file, MS Access 97 to 2013 format
        /// </summary>
        [Description(".EAP file, MS Access 97 to 2013 format")]
        JET,

        /// <summary>
        /// MSSQL, Microsoft SQL Server
        /// </summary>
        [Description("MSSQL, Microsoft SQL Server")]
        SQLSVR,

        /// <summary>
        /// FIREBIRD
        /// </summary>
        [Description("FIREBIRD")]
        FIREBIRD,

        /// <summary>
        /// .accdb file, MS Access 2007+ format
        /// </summary>
        [Description(".accdb file, MS Access 2007+ format")]
        ACCESS2007,

        /// <summary>
        /// MySQL
        /// </summary>
        [Description("MySQL")]
        MYSQL,

        /// <summary>
        /// Oracle
        /// </summary>
        [Description("Oracle")]
        ORACLE,

        /// <summary>
        /// PostgreSQL
        /// </summary>
        [Description("PostgreSQL")]
        POSTGRES,

        /// <summary>
        /// 未知
        /// </summary>
        [Description("UNKNOWN")]
        UNKNOWN,

        /// <summary>
        /// SQLite
        /// </summary>
        [Description("SQLite")]
        SQLITE,
    }

    public class TempTable
    {
        private TempTable() { }

        public static TempTable New<T>(string fieldName, IEnumerable<T> values)
        {
            var tableName = "tmp_ado_" + Util.RandomString(16);
            var vals = new List<object>();
            foreach (var val in values)
                vals.Add(val);
            return new TempTable() { TableName = tableName, FieldName = fieldName, FieldType = typeof(T), Values = vals };
        }

        public string TableName { get; protected set; }

        /// <summary>
        /// 用于SqlFormat时的占位符替换
        /// </summary>
        public string SqlFormatName { get; set; }

        public string FieldName { get; protected set; }
        public Type FieldType { get; protected set; }
        public IEnumerable<object> Values { get; protected set; }

        public DataTable ConvertToDataTable()
        {
            if (string.IsNullOrWhiteSpace(FieldName) || FieldType == null) return null;

            DataTable dt = new DataTable(TableName);
            dt.Columns.Add(new DataColumn(FieldName, FieldType));
            if (Values != null)
                foreach (var val in Values)
                {
                    var row = dt.NewRow();
                    row[0] = val;
                    dt.Rows.Add(row);
                }
            return dt;
        }
    }

    public class MultiFieldTempTable
    {
        private MultiFieldTempTable() { }

        /// <summary>
        /// 创建新的多字段临时表
        /// </summary>
        /// <param name="values"></param>
        /// <param name="sqlOnMapping"></param>
        /// <returns></returns>
        public static MultiFieldTempTable New(Dictionary<string, Queue<object>> values, Dictionary<string, string> sqlOnMapping = null)
        {
            var tableName = "tmp_ado_" + Util.RandomString(16);
            return new MultiFieldTempTable()
            {
                TableName = tableName,
                Values = values,
                SqlOnString = sqlOnMapping?.Count > 0 ? "ON " + string.Join(" and ", sqlOnMapping.Select(kv => $"{kv.Key}={kv.Value}")) : null
            };
        }

        /// <summary>
        /// 创建新的多字段临时表
        /// </summary>
        /// <param name="values"></param>
        /// <param name="sqlOnMapping"></param>
        /// <returns></returns>
        public static MultiFieldTempTable New(DataTable values, Dictionary<string, string> sqlOnMapping = null)
        {
            var tableName = "tmp_ado_" + Util.RandomString(16);
            values.TableName = tableName;
            return new MultiFieldTempTable()
            {
                TableName = tableName,
                ValueTable = values,
                SqlOnString = sqlOnMapping?.Count > 0 ? "ON " + string.Join(" and ", sqlOnMapping.Select(kv => $"{kv.Key}={kv.Value}")) : null
            };
        } 

        public string TableName { get; protected set; }

        /// <summary>
        /// 用于SqlFormat时的占位符替换
        /// </summary>
        public string SqlFormatName { get; set; }

        private DataTable ValueTable { get; set; }

        private Dictionary<string, Queue<object>> Values { get; set; }

        public string SqlOnString { get; protected set; }

        public DataTable ConvertToDataTable()
        {
            if (ValueTable != null) return ValueTable;

            if (Values?.Count() > 0)
            {
                DataTable dt = new DataTable(TableName);
                foreach (var r in Values.Select(kv => new { kv.Key, Value = kv.Value.FirstOrDefault() }))
                {
                    dt.Columns.Add(new DataColumn(r.Key, r.Value.GetType()));
                }

                var count = Values.Min(r => r.Value.Count);
                for (int i = 0; i < count; i++)
                {
                    var row = dt.NewRow();
                    foreach (DataColumn col in dt.Columns)
                    {
                        row[col.ColumnName] = Values[col.ColumnName].Dequeue();
                    }
                    dt.Rows.Add(row);
                }
                return dt;
            }
            return null;
        }
    }
    #endregion

    public class DBConnectionInfo
    {
        private DBHelper _RefOwner;
        private DbConnectionStringBuilder _ConnectionBuilder;
        public DBConnectionInfo(ProjectDataType dbType, string connectionString, DBHelper refOwner = null)
        {
            _RefOwner = refOwner;
            DBType = dbType;
            Init(connectionString);
        }

        private void Init(string connectionString)
        {
            switch (DBType)
            {
                case ProjectDataType.JET:
                    var oleDbBuilder = new OleDbConnectionStringBuilder(connectionString);
                    Database = oleDbBuilder.DataSource;
                    _ConnectionBuilder = oleDbBuilder;
                    break;
                case ProjectDataType.SQLSVR:
                    var msSqlBuilder = new SqlConnectionStringBuilder(connectionString);
                    Server = msSqlBuilder.DataSource;
                    Database = msSqlBuilder.InitialCatalog;
                    User = msSqlBuilder.UserID;
                    Password = msSqlBuilder.Password;
                    _ConnectionBuilder = msSqlBuilder;
                    break;
                case ProjectDataType.MYSQL:
                    var mySqlBuilder = new MySqlConnectionStringBuilder(connectionString);
                    Server = mySqlBuilder.Server;
                    Database = mySqlBuilder.Database;
                    User = mySqlBuilder.UserID;
                    Password = mySqlBuilder.Password;
                    _ConnectionBuilder = mySqlBuilder;
                    break;
                case ProjectDataType.SQLITE:
                    var liteSqlBuilder = new SQLiteConnectionStringBuilder(connectionString);
                    Database = liteSqlBuilder.DataSource;
                    Password = liteSqlBuilder.Password;
                    _ConnectionBuilder = liteSqlBuilder;
                    break;
                default:
                    _ConnectionBuilder = null;
                    break;
            }
        }

        public ProjectDataType DBType { get; private set; }
        /// <summary>
        /// 数据库服务器地址
        /// </summary>
        public string Server { get; private set; }
        /// <summary>
        /// 数据库名
        /// </summary>
        public string Database { get; private set; }
        /// <summary>
        /// 连接用户ID
        /// </summary>
        public string User { get; private set; }
        /// <summary>
        /// 连接用户密码
        /// </summary>
        public string Password { get; private set; }
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString => ToString();

        public bool ChangeDatabase(string database)
        {
            try
            {
                if (_ConnectionBuilder == null) return false;
                switch (DBType)
                {
                    case ProjectDataType.JET:
                        (_ConnectionBuilder as OleDbConnectionStringBuilder).DataSource = database;
                        break;
                    case ProjectDataType.SQLSVR:
                        (_ConnectionBuilder as SqlConnectionStringBuilder).InitialCatalog = database;
                        break;
                    case ProjectDataType.MYSQL:
                        (_ConnectionBuilder as MySqlConnectionStringBuilder).Database = database;
                        break;
                    case ProjectDataType.SQLITE:
                        (_ConnectionBuilder as SQLiteConnectionStringBuilder).DataSource = database;
                        break;
                    default:
                        return false;
                }
                Database = database;
                var newConnStr = _ConnectionBuilder.ConnectionString;
                _RefOwner.SetConnectionString(newConnStr);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CheckConnection()
        {
            if (_ConnectionBuilder == null) return false;
            try
            {
                IDbConnection connection = null;
                switch (DBType)
                {
                    case ProjectDataType.JET:
                        connection = new OleDbConnection(ConnectionString);
                        break;
                    case ProjectDataType.SQLSVR:
                        connection = new SqlConnection(ConnectionString);
                        break;
                    case ProjectDataType.MYSQL:
                        connection = new MySqlConnection(ConnectionString);
                        break;
                    case ProjectDataType.SQLITE:
                        connection = new SQLiteConnection(ConnectionString);
                        break;
                    default:
                        connection = null;
                        break;
                }
                connection.Open();
                connection.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override string ToString()
        {
            return _ConnectionBuilder?.ConnectionString;
        }
    }

    public class SqlTransactionBuilder
    {
        private StringBuilder builder = new StringBuilder();

        public SqlTransactionBuilder()
        {
            InitBegin();
        }

        private void InitBegin()
        {
            builder.Clear();
            builder.AppendLine("BEGIN TRY");
            builder.AppendLine("BEGIN TRANSACTION;");
        }

        private bool endInited = false;
        private void InitEnd()
        {
            if (endInited) return;
            builder.AppendLine("COMMIT TRANSACTION;");
            builder.AppendLine("END TRY");
            builder.AppendLine("BEGIN CATCH");
            builder.AppendLine("SELECT ERROR_NUMBER() AS ERRORNUMBER;");
            builder.AppendLine("ROLLBACK TRANSACTION;");
            builder.AppendLine("END CATCH");
            endInited = true;
        }

        public void Reset() => InitBegin();

        public void AppendSql(string sql) => builder.AppendLine(sql);

        public string GeSqlString()
        {
            InitEnd();
            return builder.ToString();
        }

        public override string ToString() => GeSqlString();
    }
}
