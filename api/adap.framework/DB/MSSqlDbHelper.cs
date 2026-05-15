using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace highspeed.framework.Data
{
    /// <summary>
    /// Sql数据库访问
    /// </summary>
    public class MSSqlDbHelper : IDbHelper
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        public MSSqlDbHelper(string connectionString)
        {
            this.connectionString = connectionString;
            Conn = new SqlConnection(connectionString);
        }

        /// <summary>
        /// 运行sql脚本文件
        /// </summary>
        /// <param name="script">sql脚本文件</param>
        public static void ExecutScript(DBConnectionInfo connectionInfo, string script)
        {
            if (string.IsNullOrWhiteSpace(script)) return;
            using (SqlConnection connection = new SqlConnection(connectionInfo.ConnectionString))
            {
                Server server = new Server(new ServerConnection(connection));
                try
                {
                    server.ConnectionContext.BeginTransaction();
                    server.ConnectionContext.ExecuteNonQuery(script);
                    server.ConnectionContext.CommitTransaction();
                }
                catch (Exception)
                {
                    server.ConnectionContext.RollBackTransaction();
                    throw;
                }
                finally
                {
                    server.ConnectionContext.Disconnect();
                }
            }
        }

        /// <summary>
        /// 在目标数据库中，创建源数据库的连接对象
        /// </summary>
        /// <param name="linkedDb">源数据库</param>
        /// <param name="ownDb">目标数据库</param>
        /// <returns>连接对象表访问前缀，例：[server].[database].[dbo].</returns>
        public static string CreateLinkedServer(DBHelper linkedDb, DBHelper ownDb)
        {
            #region 创建LinkedServer

            var linkedDbServer = linkedDb.ConnectionInfo.Server;
            if (linkedDbServer.Trim() == "." || linkedDbServer.Trim() == "127.0.0.1") linkedDbServer = "localhost";
            var ownDbServer = ownDb.ConnectionInfo.Server;
            if (ownDbServer.Trim() == "." || ownDbServer.Trim() == "127.0.0.1") ownDbServer = "localhost";

            // LinkedServer名字
            var dbLink = Regex.Replace(linkedDbServer, "[^a-zA-Z0-9]", "_").Trim(new char[] { ' ', '_' });
            // 检查是否存在现有的LinkedServer
            var dbLinks = ownDb.QueryDataTable($"SELECT * FROM sys.servers where [name] = '{dbLink}'");
            var exists = dbLinks?.Rows.Count > 0;
            if (!exists && linkedDbServer != "localhost" && linkedDbServer != ownDbServer)
            {
                // 创建LinkedServer
                ownDb.ExecuteProc("sp_addlinkedserver", new Dictionary<string, object> {
                    {"@server",dbLink },
                    {"@srvproduct","" },
                    {"@provider","SQLNCLI" },
                    {"@datasrc",linkedDb.ConnectionInfo.Server },
                });
                // 配置登录信息
                ownDb.ExecuteProc("sp_addlinkedsrvlogin", new Dictionary<string, object> {
                    {"@rmtsrvname",dbLink },
                    {"@useself","FALSE" },
                    {"@locallogin",null },
                    {"@rmtuser",linkedDb.ConnectionInfo.User },
                    {"@rmtpassword",linkedDb.ConnectionInfo.Password },
                });
                // 转换为查询用的字符串
                dbLink = $"[{dbLink}].[{linkedDb.ConnectionInfo.Database}].[dbo].";
            }
            else dbLink = exists ? $"[{dbLink}].[{linkedDb.ConnectionInfo.Database}].[dbo]."
                                 : $"[{linkedDb.ConnectionInfo.Database}].[dbo].";

            #endregion 创建LinkedServer

            return dbLink;
        }

        private string connectionString;
        private SqlConnection Conn;
        private SqlTransaction Transaction;

        /// <summary>
        /// 设置连接字符串
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        public void SetConnectionString(string connectionString)
        {
            this.connectionString = connectionString;
            Conn.ConnectionString = connectionString;
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <returns>连接字符串</returns>
        public string GetConnectionString()
        {
            return this.connectionString;
        }

        private bool _BatchExcuteFlag = false;

        /// <summary>
        /// 开始批量执行
        /// </summary>
        public void StartBatchExcute()
        {
            _BatchExcuteFlag = true;
        }

        /// <summary>
        /// 接受批量执行
        /// </summary>
        public void EndBatchExcute()
        {
            _BatchExcuteFlag = false;
            Close();
        }

        /// <summary>
        /// 创建事务
        /// </summary>
        /// <returns></returns>
        public DbTransaction CreateTransaction()
        {
            _BatchExcuteFlag = true;
            if (Conn == null) Conn = new SqlConnection(this.connectionString);
            if (!Conn.State.Equals(ConnectionState.Open) && !Conn.State.Equals(ConnectionState.Connecting))
            {
                Conn.Open();
            }
            return Transaction = Conn.BeginTransaction();
        }

        /// <summary>
        /// 打开连接
        /// </summary>
        public IDbCommand Open()
        {
            var Cmd = new SqlCommand();
            if (!_BatchExcuteFlag) Conn = new SqlConnection(this.connectionString);
            Cmd.Connection = Conn;
            if (Transaction != null && Transaction.Connection?.State == ConnectionState.Open) Cmd.Transaction = Transaction;
            Cmd.CommandTimeout = 10 * 60 * 1000;
            if (!Conn.State.Equals(ConnectionState.Open) && !Conn.State.Equals(ConnectionState.Connecting))
            {
                Conn.Open();
            }
            return Cmd;
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            //if (!Conn.State.Equals(ConnectionState.Closed))
            //{
            //    Conn.Close();
            //}
            int tryCount = 40;
            while (!Conn.State.Equals(ConnectionState.Closed) && tryCount > 0)
            {
                try
                {
                    Thread.Sleep(50);
                    Conn.Close();
                }
                catch { }
                finally
                {
                    tryCount--;
                }
            }
        }

        /// <summary>
        /// 执行ExecuteNonQuery()
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, List<KeyValuePair<string, object>> parameters = null)
        {
            try
            {
                using (var Cmd = Open())
                {
                    Cmd.CommandType = CommandType.Text;
                    Cmd.CommandText = sql;
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new SqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                        }
                    }
                    return Cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (!_BatchExcuteFlag) Close();
            }
        }

        /// <summary>
        /// 使用构建临时表执行ExecuteNonQuery
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <param name="temps">临时表数据</param>
        /// <returns>影响的行数</returns>
        public int ExecuteNonQueryWithTempTable(string sql, List<KeyValuePair<string, object>> parameters = null, params TempTable[] temps)
        {
            // 没有临时表数据时直接执行存储过程
            if (temps == null || temps.Length == 0) return ExecuteNonQuery(sql, parameters);

            try
            {
                using (var Cmd = Open())
                {
                    StartBatchExcute();
                    Cmd.CommandType = CommandType.Text;
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new SqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                        }
                    }
                    if (temps != null)
                    {
                        var typeSql = "";
                        foreach (var temp in temps)
                        {
                            sql = sql.Replace(temp.TableName, $"@{temp.TableName}");
                            DataTable table = temp.ConvertToDataTable();
                            typeSql += table.GetCreateTableTypeSQL(temp.TableName) + "; ";
                            SqlParameter parameter = new SqlParameter();
                            parameter.ParameterName = $"@{temp.TableName}";
                            parameter.SqlDbType = SqlDbType.Structured;
                            parameter.TypeName = temp.TableName;
                            parameter.Value = table;
                            Cmd.Parameters.Add(parameter);
                        }
                        ExecuteNonQuery(typeSql);
                    }
                    Cmd.CommandText = sql;
                    return Cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                //Logger.Error("MSSqlDbHelper.QueryDataTableProc", "Encounter error when executing sql.\r\nSQL: " + sql + "\r\nparameters: " + parameters?.ToJson(), e);
                throw;
            }
            finally
            {
                // 删除临时表
                try
                {
                    var sql_drop = string.Empty;
                    foreach (var temp in temps)
                    {
                        sql_drop += "DROP TYPE IF EXISTS " + temp.TableName + "; ";
                    }
                    ExecuteNonQuery(sql_drop);
                    EndBatchExcute();
                }
                catch { }
                if (!_BatchExcuteFlag) Close();
            }
        }
         
        /// <summary>
        /// 查询数据库
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>数据集DataSet</returns>
        public DataSet Query(string sql, List<KeyValuePair<string, object>> parameters = null)
        {
            try
            {
                using (var Cmd = Open())
                using (var Da = new SqlDataAdapter())
                {
                    Cmd.CommandType = CommandType.Text;
                    Cmd.CommandText = sql;
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new SqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                        }
                    }
                    Da.SelectCommand = Cmd as SqlCommand;
                    var Ds = new DataSet();
                    Da.Fill(Ds);
                    return Ds;
                }
            }
            catch (Exception e)
            {
                //Logger.Error("MSSqlDbHelper.Query", "Encounter error when executing sql.\r\nSQL: " + sql + "\r\nparameters: " + parameters?.ToJson(), e);
                throw;
            }
            finally
            {
                if (!_BatchExcuteFlag) Close();
            }
        }

        /// <summary>
        /// 使用构建临时表执行Query
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters">参数</param>
        /// <param name="temps"></param>
        /// <returns></returns>
        public DataSet QueryWithTempTable(string sql, List<KeyValuePair<string, object>> parameters = null, params TempTable[] temps)
        {
            if (temps == null || temps.Length == 0) return Query(sql, parameters);

            try
            {
                using (var Cmd = Open())
                using (var Da = new SqlDataAdapter())
                {
                    StartBatchExcute();
                    Cmd.CommandType = CommandType.Text;
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new SqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                        }
                    }
                    if (temps != null)
                    {
                        var typeSql = "";
                        foreach (var temp in temps)
                        {
                            sql = sql.Replace(temp.TableName, $"@{temp.TableName}");
                            DataTable table = temp.ConvertToDataTable();
                            typeSql += table.GetCreateTableTypeSQL(temp.TableName) + "; ";
                            SqlParameter parameter = new SqlParameter();
                            parameter.ParameterName = $"@{temp.TableName}";
                            parameter.SqlDbType = SqlDbType.Structured;
                            parameter.TypeName = temp.TableName;
                            parameter.Value = table;
                            Cmd.Parameters.Add(parameter);
                        }
                        ExecuteNonQuery(typeSql);
                    }
                    Cmd.CommandText = sql;
                    Da.SelectCommand = Cmd as SqlCommand;
                    var Ds = new DataSet();
                    Da.Fill(Ds);
                    return Ds;
                }
            }
            catch (Exception e)
            {
                //Logger.Error("MSSqlDbHelper.QueryWithTempTable", "Encounter error when executing sql.\r\nSQL: " + sql + "\r\nparameters: " + parameters?.ToJson(), e);
                throw;
            }
            finally
            {
                try
                {
                    var sql_drop = string.Empty;
                    foreach (var temp in temps)
                    {
                        sql_drop += "DROP TYPE IF EXISTS " + temp.TableName + "; ";
                    }
                    ExecuteNonQuery(sql_drop);
                    EndBatchExcute();
                }
                catch { }

                if (!_BatchExcuteFlag) Close();
            }
        }

        /// <summary>
        /// 使用构建临时表执行Query
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters">参数</param>
        /// <param name="temps"></param>
        /// <returns></returns>
        public DataSet QueryWithTempTable(string sql, List<KeyValuePair<string, object>> parameters = null, params MultiFieldTempTable[] temps)
        {
            if (temps == null || temps.Length == 0) return Query(sql, parameters);

            try
            {
                using (var Cmd = Open())
                using (var Da = new SqlDataAdapter())
                {
                    StartBatchExcute();
                    Cmd.CommandType = CommandType.Text;
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new SqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                        }
                    }
                    if (temps != null)
                    {
                        var typeSql = "";
                        foreach (var temp in temps)
                        {
                            sql = sql.Replace(temp.TableName, $"@{temp.TableName}");
                            DataTable table = temp.ConvertToDataTable();
                            typeSql += table.GetCreateTableTypeSQL(temp.TableName) + "; ";
                            SqlParameter parameter = new SqlParameter();
                            parameter.ParameterName = $"@{temp.TableName}";
                            parameter.SqlDbType = SqlDbType.Structured;
                            parameter.TypeName = temp.TableName;
                            parameter.Value = table;
                            Cmd.Parameters.Add(parameter);
                        }
                        ExecuteNonQuery(typeSql);
                    }
                    Cmd.CommandText = sql;
                    Da.SelectCommand = Cmd as SqlCommand;
                    var Ds = new DataSet();
                    Da.Fill(Ds);
                    return Ds;
                }
            }
            catch (Exception e)
            {
                //Logger.Error("MSSqlDbHelper.QueryWithTempTable", "Encounter error when executing sql.\r\nSQL: " + sql + "\r\nparameters: " + parameters?.ToJson(), e);
                throw;
            }
            finally
            {
                try
                {
                    var sql_drop = string.Empty;
                    foreach (var temp in temps)
                    {
                        sql_drop += "DROP TYPE IF EXISTS " + temp.TableName + "; ";
                    }
                    ExecuteNonQuery(sql_drop);
                    EndBatchExcute();
                }
                catch { }

                if (!_BatchExcuteFlag) Close();
            }
        }

        /// <summary>
        /// 查询数据库
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>数据集DataTable</returns>
        public DataTable QueryDataTable(string sql, List<KeyValuePair<string, object>> parameters = null)
        {
            try
            {
                using (var Cmd = Open())
                using (var Da = new SqlDataAdapter())
                {
                    Cmd.CommandType = CommandType.Text;
                    Cmd.CommandText = sql;
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new SqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                        }
                    }
                    Da.SelectCommand = Cmd as SqlCommand;
                    var Dt = new DataTable();
                    Da.Fill(Dt);
                    return Dt;
                }
            }
            catch (Exception e)
            {
                //Logger.Error("MSSqlDbHelper.QueryDataTable", "Encounter error when executing sql.\r\nSQL: " + sql + "\r\nparameters: " + parameters?.ToJson(), e);
                throw;
            }
            finally
            {
                if (!_BatchExcuteFlag) Close();
            }
        }

        /// <summary>
        /// 使用临时表查询数据库
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="temps">临时表</param>
        /// <returns>数据集DataTable</returns>
        public DataTable QueryDataTableWithTempTable(string sql, List<KeyValuePair<string, object>> parameters = null, params TempTable[] temps)
        {
            if (temps == null || temps.Length == 0) return QueryDataTable(sql, parameters);

            try
            {
                using (var Cmd = Open())
                using (var Da = new SqlDataAdapter())
                {
                    StartBatchExcute();
                    Cmd.CommandType = CommandType.Text;
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new SqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                        }
                    }
                    if (temps != null)
                    {
                        var typeSql = "";
                        foreach (var temp in temps)
                        {
                            sql = sql.Replace(temp.TableName, $"@{temp.TableName}");
                            DataTable table = temp.ConvertToDataTable();
                            typeSql += table.GetCreateTableTypeSQL(temp.TableName) + "; ";
                            SqlParameter parameter = new SqlParameter();
                            parameter.ParameterName = $"@{temp.TableName}";
                            parameter.SqlDbType = SqlDbType.Structured;
                            parameter.TypeName = temp.TableName;
                            parameter.Value = table;
                            Cmd.Parameters.Add(parameter);
                        }
                        ExecuteNonQuery(typeSql);
                    }
                    Cmd.CommandText = sql;
                    Da.SelectCommand = Cmd as SqlCommand;
                    var Dt = new DataTable();
                    Da.Fill(Dt);
                    return Dt;
                }
            }
            catch (Exception e)
            {
                //Logger.Error("MSSqlDbHelper.QueryDataTable", "Encounter error when executing sql.\r\nSQL: " + sql + "\r\nparameters: " + parameters?.ToJson(), e);
                throw;
            }
            finally
            {
                try
                {
                    var sql_drop = string.Empty;
                    foreach (var temp in temps)
                    {
                        sql_drop += "DROP TYPE IF EXISTS " + temp.TableName + "; ";
                    }
                    ExecuteNonQuery(sql_drop);
                    EndBatchExcute();
                }
                catch { }

                if (!_BatchExcuteFlag) Close();
            }
        }

        /// <summary>
        /// 使用临时表查询数据库
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="temps">临时表</param>
        /// <returns>数据集DataTable</returns>
        public DataTable QueryDataTableWithTempTable(string sql, List<KeyValuePair<string, object>> parameters = null, params MultiFieldTempTable[] temps)
        {
            if (temps == null || temps.Length == 0) return QueryDataTable(sql, parameters);

            try
            {
                using (var Cmd = Open())
                using (var Da = new SqlDataAdapter())
                {
                    StartBatchExcute();
                    Cmd.CommandType = CommandType.Text;
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new SqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                        }
                    }
                    if (temps != null)
                    {
                        var typeSql = "";
                        foreach (var temp in temps)
                        {
                            sql = sql.Replace(temp.TableName, $"@{temp.TableName}");
                            DataTable table = temp.ConvertToDataTable();
                            typeSql += table.GetCreateTableTypeSQL(temp.TableName) + "; ";
                            SqlParameter parameter = new SqlParameter();
                            parameter.ParameterName = $"@{temp.TableName}";
                            parameter.SqlDbType = SqlDbType.Structured;
                            parameter.TypeName = temp.TableName;
                            parameter.Value = table;
                            Cmd.Parameters.Add(parameter);
                        }
                        ExecuteNonQuery(typeSql);
                    }
                    Cmd.CommandText = sql;
                    Da.SelectCommand = Cmd as SqlCommand;
                    var Dt = new DataTable();
                    Da.Fill(Dt);
                    return Dt;
                }
            }
            catch (Exception e)
            {
                //Logger.Error("MSSqlDbHelper.QueryDataTable", "Encounter error when executing sql.\r\nSQL: " + sql + "\r\nparameters: " + parameters?.ToJson(), e);
                throw;
            }
            finally
            {
                try
                {
                    var sql_drop = string.Empty;
                    foreach (var temp in temps)
                    {
                        sql_drop += "DROP TYPE IF EXISTS " + temp.TableName + "; ";
                    }
                    ExecuteNonQuery(sql_drop);
                    EndBatchExcute();
                }
                catch { }

                if (!_BatchExcuteFlag) Close();
            }
        }

        /// <summary>
        /// 查询存储过程返回DataSet
        /// </summary>
        /// <param name="sql">存储过程</param>
        /// <param name="parameters">参数</param>
        /// <returns>数据集DataSet</returns>
        public DataSet QueryProc(string sql, List<KeyValuePair<string, object>> parameters = null)
        {
            try
            {
                using (var Cmd = Open())
                using (var Da = new SqlDataAdapter())
                {
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.CommandText = sql;
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new SqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                        }
                    }
                    Da.SelectCommand = Cmd as SqlCommand;
                    var Ds = new DataSet();
                    Da.Fill(Ds);
                    return Ds;
                }
            }
            catch (Exception e)
            {
                //Logger.Error("MSSqlDbHelper.QueryProc", "Encounter error when executing sql.\r\nSQL: " + sql + "\r\nparameters: " + parameters?.ToJson(), e);
                throw;
            }
            finally
            {
                if (!_BatchExcuteFlag) Close();
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="proc">存储过程</param>
        /// <param name="parameters">参数</param>
        /// <returns>影响的行数</returns>
        public int ExecuteProc(string proc, List<KeyValuePair<string, object>> parameters = null)
        {
            try
            {
                using (var Cmd = Open())
                {
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.CommandText = proc;
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new SqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                        }
                    }
                    return Cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                //Logger.Error("MSSqlDbHelper.QueryDataTableProc", "Encounter error when executing sql.\r\nSQL: " + sql + "\r\nparameters: " + parameters?.ToJson(), e);
                throw;
            }
            finally
            {
                if (!_BatchExcuteFlag) Close();
            }
        }

        /// <summary>
        /// 查询存储过程返回DataTable
        /// </summary>
        /// <param name="proc">存储过程</param>
        /// <param name="parameters">参数</param>
        /// <returns>数据集DataTable</returns>
        public DataTable QueryDataTableProc(string proc, List<KeyValuePair<string, object>> parameters = null)
        {
            try
            {
                using (var Cmd = Open())
                using (var Da = new SqlDataAdapter())
                {
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.CommandText = proc;
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new SqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                        }
                    }
                    Da.SelectCommand = Cmd as SqlCommand;
                    var Dt = new DataTable();
                    Da.Fill(Dt);
                    return Dt;
                }
            }
            catch (Exception e)
            {
                //Logger.Error("MSSqlDbHelper.QueryDataTableProc", "Encounter error when executing sql.\r\nSQL: " + sql + "\r\nparameters: " + parameters?.ToJson(), e);
                throw;
            }
            finally
            {
                if (!_BatchExcuteFlag) Close();
            }
        }

        /// <summary>
        /// 执行 ExecuteScalar
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>首行首列值</returns>
        public object ExecuteScalar(string sql, List<KeyValuePair<string, object>> parameters = null)
        {
            try
            {
                using (var Cmd = Open())
                {
                    Cmd.CommandType = CommandType.Text;
                    Cmd.CommandText = sql;
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new SqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                        }
                    }
                    return Cmd.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (!_BatchExcuteFlag) Close();
            }
        }

        /// <summary>
        /// 执行 ExecuteScalar
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="temps">临时表</param>
        /// <returns>首行首列值</returns>
        public object ExecuteScalarWithTempTable(string sql, List<KeyValuePair<string, object>> parameters = null, params TempTable[] temps)
        {
            if (temps == null || temps.Length == 0) return ExecuteScalar(sql, parameters);

            try
            {
                using (var Cmd = Open())
                {
                    StartBatchExcute();
                    Cmd.CommandType = CommandType.Text;
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new SqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                        }
                    }
                    if (temps != null)
                    {
                        var typeSql = "";
                        foreach (var temp in temps)
                        {
                            sql = sql.Replace(temp.TableName, $"@{temp.TableName}");
                            DataTable table = temp.ConvertToDataTable();
                            typeSql += table.GetCreateTableTypeSQL(temp.TableName) + "; ";
                            SqlParameter parameter = new SqlParameter();
                            parameter.ParameterName = $"@{temp.TableName}";
                            parameter.SqlDbType = SqlDbType.Structured;
                            parameter.TypeName = temp.TableName;
                            parameter.Value = table;
                            Cmd.Parameters.Add(parameter);
                        }
                        ExecuteNonQuery(typeSql);
                    }
                    Cmd.CommandText = sql;
                    return Cmd.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                try
                {
                    var sql_drop = string.Empty;
                    foreach (var temp in temps)
                    {
                        sql_drop += "DROP TYPE IF EXISTS " + temp.TableName + "; ";
                    }
                    ExecuteNonQuery(sql_drop);
                    EndBatchExcute();
                }
                catch { }

                if (!_BatchExcuteFlag) Close();
            }
        }

        /// <summary>
        /// 执行 ExecuteScalar
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="temps">临时表</param>
        /// <returns>首行首列值</returns>
        public object ExecuteScalarWithTempTable(string sql, List<KeyValuePair<string, object>> parameters = null, params MultiFieldTempTable[] temps)
        {
            if (temps == null || temps.Length == 0) return ExecuteScalar(sql, parameters);

            try
            {
                using (var Cmd = Open())
                {
                    StartBatchExcute();
                    Cmd.CommandType = CommandType.Text;
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new SqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                        }
                    }
                    if (temps != null)
                    {
                        var typeSql = "";
                        foreach (var temp in temps)
                        {
                            sql = sql.Replace(temp.TableName, $"@{temp.TableName}");
                            DataTable table = temp.ConvertToDataTable();
                            typeSql += table.GetCreateTableTypeSQL(temp.TableName) + "; ";
                            SqlParameter parameter = new SqlParameter();
                            parameter.ParameterName = $"@{temp.TableName}";
                            parameter.SqlDbType = SqlDbType.Structured;
                            parameter.TypeName = temp.TableName;
                            parameter.Value = table;
                            Cmd.Parameters.Add(parameter);
                        }
                        ExecuteNonQuery(typeSql);
                    }
                    Cmd.CommandText = sql;
                    return Cmd.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                try
                {
                    var sql_drop = string.Empty;
                    foreach (var temp in temps)
                    {
                        sql_drop += "DROP TYPE IF EXISTS " + temp.TableName + "; ";
                    }
                    ExecuteNonQuery(sql_drop);
                    EndBatchExcute();
                }
                catch { }

                if (!_BatchExcuteFlag) Close();
            }
        }

        /// <summary>
        /// 执行 ExecuteScalar
        /// </summary>
        /// <param name="sqls">SQL语句集合</param>
        /// <returns>首行首列值</returns>
        public object ExecuteScalar(Dictionary<string, List<KeyValuePair<string, object>>> sqls)
        {
            try
            {
                object result = null;
                using (var Cmd = Open())
                {
                    Cmd.CommandType = CommandType.Text;
                    int i = 0;
                    foreach (var kv in sqls)
                    {
                        i++;
                        var sql = kv.Key;
                        var parameters = kv.Value;
                        Cmd.Parameters.Clear();
                        Cmd.CommandText = sql;
                        if (parameters != null)
                        {
                            foreach (var p in parameters)
                            {
                                Cmd.Parameters.Add(new SqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                            }
                        }
                        if (i < sqls.Count) Cmd.ExecuteNonQuery();
                        else result = Cmd.ExecuteScalar();
                    }
                    return result;
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (!_BatchExcuteFlag) Close();
            }
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="tableName">数据库表名</param>
        /// <param name="records">需要插入的数据</param>
        public void BulkInsert(string tableName, DataTable records)
        {
            try
            {
                Open();
                using (SqlTransaction transaction = Conn.BeginTransaction())
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(Conn, SqlBulkCopyOptions.KeepNulls, transaction))
                {
                    bulkCopy.BulkCopyTimeout = 1000;
                    bulkCopy.DestinationTableName = "dbo." + tableName;
                    foreach (DataColumn col in records.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }
                    bulkCopy.WriteToServer(records);
                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (!_BatchExcuteFlag) Close();
            }
        }

        /// <summary>
        /// 插入临时表数据
        /// </summary>
        /// <param name="tempTable">临时表DataTable</param>
        /// <param name="tableName">临时表名</param>
        public void BulkInsertTempTable(DataTable tempTable, string tableName)
        {
            var name = string.IsNullOrWhiteSpace(tableName) ? tempTable.TableName : tableName;
            if (string.IsNullOrWhiteSpace(name)) throw new Exception("The name of the temp table is empty.");
            ExecuteNonQuery(tempTable.GetCreateSQL(name));
            BulkInsert(name, tempTable);
        }

        /// <summary>
        /// 插入临时表数据
        /// </summary>
        /// <param name="tempTables">临时表DataTable</param>
        public string[] BulkInsertTempTable(params DataTable[] tempTables)
        {
            foreach (var temp in tempTables)
            {
                if (string.IsNullOrWhiteSpace(temp.TableName))
                    temp.TableName = $"tmp-{Guid.NewGuid().ToString()}";
                ExecuteNonQuery(temp.GetCreateSQL(temp.TableName));
                BulkInsert(temp.TableName, temp);
            }
            return tempTables.Select(t => t.TableName).ToArray();
        }

        /// <summary>
        /// 删除临时表数据
        /// </summary>
        /// <param name="tableNames">临时表名</param>
        public void DropTempTable(params string[] tableNames)
        {
            try
            {
                var sql_drop = string.Empty;
                foreach (var temp in tableNames)
                {
                    sql_drop += "DROP table " + temp + "; ";
                }
                ExecuteNonQuery(sql_drop);
            }
            catch { }
        }

        /// <summary>
        /// 批量更新数据
        /// </summary>
        /// <param name="tableName">数据库表名</param>
        /// <param name="records">需要更新的数据</param>
        public void BulkUpdate(string tableName, DataTable records)
        {
            string tempCols = "";
            string setSql = "";
            string onSql = "";
            string createTemp = null;
            string updateSql = null;

            try
            {
                using (var Cmd = Open())
                {
                    var pkcs = records.PrimaryKey;
                    var pks = pkcs.Select(c => c.ColumnName).ToList();
                    var cols = records.Columns;

                    foreach (DataColumn col in cols)
                    {
                        #region createTemp

                        string dbType = null;
                        if (col.DataType.FullName == typeof(int).FullName)
                            dbType = "int";
                        else if (col.DataType.FullName == typeof(DateTime).FullName)
                            dbType = "datetime";
                        else
                            dbType = "nvarchar(max)";

                        tempCols += col.ColumnName + " " + dbType + " , ";

                        #endregion createTemp

                        #region updateSql

                        if (pks.Contains(col.ColumnName))
                        {
                            onSql += "T." + col.ColumnName + "=Temp." + col.ColumnName + " and ";
                        }
                        else
                        {
                            setSql += "T." + col.ColumnName + "=Temp." + col.ColumnName + ", ";
                        }

                        #endregion updateSql
                    }
                    createTemp = string.Format("CREATE TABLE #TmpTable_" + tableName + "( {0} )", tempCols.Trim().TrimEnd(','));
                    updateSql = string.Format("UPDATE T SET {0} FROM dbo." + tableName + " T INNER JOIN #TmpTable_" + tableName + " Temp ON {1}; DROP TABLE #TmpTable_" + tableName + ";", setSql.Trim().TrimEnd(','), onSql.Substring(0, onSql.LastIndexOf("and ")));

                    //Creating temp table on database
                    Cmd.CommandText = createTemp;
                    Cmd.ExecuteNonQuery();

                    //Bulk insert into temp table
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(Conn))
                    {
                        bulkCopy.BulkCopyTimeout = 1000;
                        bulkCopy.DestinationTableName = "#TmpTable_" + tableName;
                        foreach (DataColumn col in cols)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }
                        bulkCopy.WriteToServer(records);
                        bulkCopy.Close();
                    }

                    // Updating destination table, and dropping temp table
                    Cmd.CommandTimeout = 300;
                    Cmd.CommandText = updateSql;
                    Cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (!_BatchExcuteFlag) Close();
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Close();
            Conn.Dispose();
        }

        /// <summary>
        /// 获取新的自增ID
        /// </summary>
        /// <param name="table">表名</param>
        /// <returns>自增ID</returns>
        public int? GetNewID(string table)
        {
            try
            {
                string currId = null;
                using (var Cmd = Open())
                {
                    Conn.InfoMessage += new SqlInfoMessageEventHandler((sender, args) =>
                        {
                            var result = (args as SqlInfoMessageEventArgs).Message;
                            if (!string.IsNullOrEmpty(result))
                            {
                                var match = System.Text.RegularExpressions.Regex.Match(result, "'[\\d]+'");
                                if (match.Success) currId = match.Value;
                            }
                        });
                    Cmd.CommandText = "DBCC CHECKIDENT(@table, NORESEED)";
                    Cmd.Parameters.Add(new SqlParameter("@table", table));
                    Cmd.ExecuteNonQuery();

                    if (string.IsNullOrEmpty(currId))
                        Cmd.CommandText = "select IDENT_CURRENT(@table)";
                    else
                        Cmd.CommandText = "select IDENT_CURRENT(@table) + IDENT_INCR(@table)";
                    return int.Parse(Cmd.ExecuteScalar().ToString());
                }
            }
            catch (SqlException se)
            {
                if (se.Number == 7997) // 不支持自增列
                    return null;
                throw se;
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (!_BatchExcuteFlag) Close();
            }
        }
    }
}