using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;

namespace highspeed.framework.Data
{
    /// <summary>
    /// MySql数据库访问
    /// </summary>
    internal class MySqlDbHelper : IDbHelper
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        public MySqlDbHelper(string connectionString)
        {
            this.connectionString = connectionString;
            Conn = new MySqlConnection(connectionString);
        }

        private string connectionString;
        private MySqlConnection Conn;
        private MySqlTransaction Transaction;

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
            if (Conn == null) Conn = new MySqlConnection(this.connectionString);
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
            var Cmd = new MySqlCommand();
            if (!_BatchExcuteFlag) Conn = new MySqlConnection(this.connectionString);
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
        /// 运行sql脚本文件
        /// </summary>
        /// <param name="script">sql脚本文件</param>
        public static void ExecutScript(DBConnectionInfo connectionInfo, string script)
        {
            if (string.IsNullOrWhiteSpace(script)) return;

            using (MySqlConnection connection = new MySqlConnection(connectionInfo.ConnectionString))
            {
                connection.Open();
                MySqlTransaction transaction = null;

                try
                {
                    transaction = connection.BeginTransaction();
                    using (MySqlCommand command = new MySqlCommand(script, connection, transaction))
                    {
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction?.Rollback();
                    throw;
                }
                finally
                {
                    connection.Close();
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
                    Cmd.CommandText = sql.Replace("[", "").Replace("]", "");
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new MySqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
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
                // 创建临时表数据
                foreach (var temp in temps)
                {
                    DataTable dt = temp.ConvertToDataTable();
                    ExecuteNonQuery(dt.GetCreateSQL(temp.TableName));
                    BulkInsert(temp.TableName, dt);
                }

                using (var Cmd = Open())
                {
                    Cmd.CommandType = CommandType.Text;
                    Cmd.CommandText = sql.Replace("[", "").Replace("]", "");
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new MySqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
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
                // 删除临时表
                try
                {
                    var sql_drop = string.Empty;
                    foreach (var temp in temps)
                    {
                        sql_drop += "DROP table " + temp.TableName + "; ";
                    }
                    ExecuteNonQuery(sql_drop);
                }
                catch { }
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
        public int ExecuteNonQueryWithTempTable(string sql, List<KeyValuePair<string, object>> parameters = null, params MultiFieldTempTable[] temps)
        {
            // 没有临时表数据时直接执行存储过程
            if (temps == null || temps.Length == 0) return ExecuteNonQuery(sql, parameters);

            try
            {
                // 创建临时表数据
                foreach (var temp in temps)
                {
                    DataTable dt = temp.ConvertToDataTable();
                    ExecuteNonQuery(dt.GetCreateSQL(temp.TableName));
                    BulkInsert(temp.TableName, dt);
                }

                using (var Cmd = Open())
                {
                    Cmd.CommandType = CommandType.Text;
                    Cmd.CommandText = sql.Replace("[", "").Replace("]", "");
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new MySqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
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
                // 删除临时表
                try
                {
                    var sql_drop = string.Empty;
                    foreach (var temp in temps)
                    {
                        sql_drop += "DROP table " + temp.TableName + "; ";
                    }
                    ExecuteNonQuery(sql_drop);
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
                using (var Da = new MySqlDataAdapter())
                {
                    Cmd.CommandType = CommandType.Text;
                    Cmd.CommandText = sql.Replace("[", "").Replace("]", "");
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new MySqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                        }
                    }
                    Da.SelectCommand = Cmd as MySqlCommand;
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
                foreach (var temp in temps)
                {
                    DataTable dt = temp.ConvertToDataTable();
                    ExecuteNonQuery(dt.GetCreateSQL(temp.TableName));
                    BulkInsert(temp.TableName, dt);
                }

                using (var Cmd = Open())
                using (var Da = new MySqlDataAdapter())
                {
                    Cmd.CommandType = CommandType.Text;
                    Cmd.CommandText = sql.Replace("[", "").Replace("]", "");
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new MySqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                        }
                    }
                    Da.SelectCommand = Cmd as MySqlCommand;
                    var Ds = new DataSet();
                    Da.Fill(Ds);
                    return Ds;
                }
            }
            catch (Exception e)
            {
                //Logger.Error("MySqlDbHelper.QueryWithTempTable", "Encounter error when executing sql.\r\nSQL: " + sql + "\r\nparameters: " + parameters?.ToJson(), e);
                throw;
            }
            finally
            {
                try
                {
                    var sql_drop = string.Empty;
                    foreach (var temp in temps)
                    {
                        sql_drop += "DROP table " + temp.TableName + "; ";
                    }
                    ExecuteNonQuery(sql_drop);
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
                foreach (var temp in temps)
                {
                    DataTable dt = temp.ConvertToDataTable();
                    ExecuteNonQuery(dt.GetCreateSQL(temp.TableName));
                    BulkInsert(temp.TableName, dt);
                }

                using (var Cmd = Open())
                using (var Da = new MySqlDataAdapter())
                {
                    Cmd.CommandType = CommandType.Text;
                    Cmd.CommandText = sql.Replace("[", "").Replace("]", "");
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new MySqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                        }
                    }
                    Da.SelectCommand = Cmd as MySqlCommand;
                    var Ds = new DataSet();
                    Da.Fill(Ds);
                    return Ds;
                }
            }
            catch (Exception e)
            {
                //Logger.Error("MySqlDbHelper.QueryWithTempTable", "Encounter error when executing sql.\r\nSQL: " + sql + "\r\nparameters: " + parameters?.ToJson(), e);
                throw;
            }
            finally
            {
                try
                {
                    var sql_drop = string.Empty;
                    foreach (var temp in temps)
                    {
                        sql_drop += "DROP table " + temp.TableName + "; ";
                    }
                    ExecuteNonQuery(sql_drop);
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
                using (var Da = new MySqlDataAdapter())
                {
                    Cmd.CommandType = CommandType.Text;
                    Cmd.CommandText = sql.Replace("[", "").Replace("]", "");
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new MySqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                        }
                    }
                    Da.SelectCommand = Cmd as MySqlCommand;
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
                foreach (var temp in temps)
                {
                    DataTable dt = temp.ConvertToDataTable();
                    ExecuteNonQuery(dt.GetCreateSQL(temp.TableName));
                    BulkInsert(temp.TableName, dt);
                }

                using (var Cmd = Open())
                using (var Da = new MySqlDataAdapter())
                {
                    Cmd.CommandType = CommandType.Text;
                    Cmd.CommandText = sql.Replace("[", "").Replace("]", "");
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new MySqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                        }
                    }
                    Da.SelectCommand = Cmd as MySqlCommand;
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
                        sql_drop += "DROP table " + temp.TableName + "; ";
                    }
                    ExecuteNonQuery(sql_drop);
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
                foreach (var temp in temps)
                {
                    DataTable dt = temp.ConvertToDataTable();
                    ExecuteNonQuery(dt.GetCreateSQL(temp.TableName));
                    BulkInsert(temp.TableName, dt);
                }

                using (var Cmd = Open())
                using (var Da = new MySqlDataAdapter())
                {
                    Cmd.CommandType = CommandType.Text;
                    Cmd.CommandText = sql.Replace("[", "").Replace("]", "");
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new MySqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                        }
                    }
                    Da.SelectCommand = Cmd as MySqlCommand;
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
                        sql_drop += "DROP table " + temp.TableName + "; ";
                    }
                    ExecuteNonQuery(sql_drop);
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
                using (var Da = new MySqlDataAdapter())
                {
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.CommandText = sql.Replace("[", "").Replace("]", "");
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new MySqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                        }
                    }
                    Da.SelectCommand = Cmd as MySqlCommand;
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
                            Cmd.Parameters.Add(new MySqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
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
                using (var Da = new MySqlDataAdapter())
                {
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.CommandText = proc;
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new MySqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                        }
                    }
                    Da.SelectCommand = Cmd as MySqlCommand;
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
                    Cmd.CommandText = sql.Replace("[", "").Replace("]", "");
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new MySqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
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
                foreach (var temp in temps)
                {
                    DataTable dt = temp.ConvertToDataTable();
                    ExecuteNonQuery(dt.GetCreateSQL(temp.TableName));
                    BulkInsert(temp.TableName, dt);
                }

                using (var Cmd = Open())
                {
                    Cmd.CommandType = CommandType.Text;
                    Cmd.CommandText = sql.Replace("[", "").Replace("]", "");
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new MySqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
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
                try
                {
                    var sql_drop = string.Empty;
                    foreach (var temp in temps)
                    {
                        sql_drop += "DROP table " + temp.TableName + "; ";
                    }
                    ExecuteNonQuery(sql_drop);
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
                foreach (var temp in temps)
                {
                    DataTable dt = temp.ConvertToDataTable();
                    ExecuteNonQuery(dt.GetCreateSQL(temp.TableName));
                    BulkInsert(temp.TableName, dt);
                }

                using (var Cmd = Open())
                {
                    Cmd.CommandType = CommandType.Text;
                    Cmd.CommandText = sql.Replace("[", "").Replace("]", "");
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new MySqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
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
                try
                {
                    var sql_drop = string.Empty;
                    foreach (var temp in temps)
                    {
                        sql_drop += "DROP table " + temp.TableName + "; ";
                    }
                    ExecuteNonQuery(sql_drop);
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
                        Cmd.CommandText = sql.Replace("[", "").Replace("]", "");
                        if (parameters != null)
                        {
                            foreach (var p in parameters)
                            {
                                Cmd.Parameters.Add(new MySqlParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
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
                //using (var cmd = Open())
                //{
                //    var bulkCopy = new MySqlBulkCopy(Conn);
                //    bulkCopy.BulkCopyTimeout = 3600;
                //    bulkCopy.DestinationTableName = tableName;
                //    bulkCopy.WriteToServer(records);
                //}

                using (var Cmd = Open())
                using (var Da = new MySqlDataAdapter())
                {

                    Cmd.CommandType = CommandType.Text;
                    Cmd.CommandText = "select * from " + tableName;
                    Da.SelectCommand = Cmd as MySqlCommand;
                    MySqlCommandBuilder cmdBuilder = new MySqlCommandBuilder(Da);
                    Da.MissingSchemaAction = MissingSchemaAction.Add;
                    DataSet ds = new DataSet();

                    Da.Fill(ds, tableName);

                    var dbTable = ds.Tables[tableName];
                    var pks = dbTable.PrimaryKey.Select(c => c.ColumnName).ToList();
                    List<string> cols = new List<string>();
                    var colEnumerator = records.Columns.GetEnumerator();
                    while (colEnumerator.MoveNext())
                    {
                        var col = colEnumerator.Current as DataColumn;
                        if (!pks.Contains(col.ColumnName))
                            cols.Add(col.ColumnName);
                    }

                    foreach (DataRow row in records.Rows)
                    {
                        DataRow newRow = dbTable.NewRow();
                        cols.ForEach(ck => newRow[ck] = row[ck]);
                        dbTable.Rows.Add(newRow);
                    }
                    Da.Update(ds, tableName);
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
        public void DropTempTable(params string[] tableNames) {
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
                //using (var Cmd = Open())
                //{
                //    var pkcs = records.PrimaryKey;
                //    var pks = pkcs.Select(c => c.ColumnName).ToList();
                //    var cols = records.Columns;

                //    foreach (DataColumn col in cols)
                //    {
                //        #region createTemp
                //        string dbType = null;
                //        if (col.DataType.FullName == typeof(int).FullName)
                //            dbType = "int";
                //        else if (col.DataType.FullName == typeof(DateTime).FullName)
                //            dbType = "datetime";
                //        else
                //            dbType = "text";

                //        tempCols += col.ColumnName + " " + dbType + " , ";
                //        #endregion

                //        #region updateSql
                //        if (pks.Contains(col.ColumnName))
                //        {
                //            onSql += "T." + col.ColumnName + "=Temp." + col.ColumnName + " and ";
                //        }
                //        else
                //        {
                //            setSql += "T." + col.ColumnName + "=Temp." + col.ColumnName + ", ";
                //        }
                //        #endregion
                //    }
                //    createTemp = string.Format("CREATE TEMPORARY TABLE TmpTable_" + tableName + "( {0} )", tempCols.Trim().TrimEnd(','));
                //    updateSql = string.Format("UPDATE " + tableName + " T INNER JOIN TmpTable_" + tableName + " Temp ON {1} SET {0}; DROP TABLE TmpTable_" + tableName + ";", setSql.Trim().TrimEnd(','), onSql.Substring(0, onSql.LastIndexOf("and ")));

                //    //Creating temp table on database
                //    Cmd.CommandText = createTemp;
                //    Cmd.ExecuteNonQuery();

                //    //Bulk insert into temp table
                //    var bulkCopy = new MySqlBulkCopy(Conn);
                //    bulkCopy.BulkCopyTimeout = 3600;
                //    bulkCopy.DestinationTableName = "TmpTable_" + tableName;
                //    bulkCopy.WriteToServer(records);

                //    // Updating destination table, and dropping temp table
                //    Cmd.CommandTimeout = 3600;
                //    Cmd.CommandText = updateSql;
                //    Cmd.ExecuteNonQuery();
                //}

                using (var Cmd = Open())
                using (var Da = new MySqlDataAdapter())
                {

                    Cmd.CommandType = CommandType.Text;
                    Cmd.CommandText = "select * from " + tableName;
                    Da.SelectCommand = Cmd as MySqlCommand;
                    MySqlCommandBuilder cmdBuilder = new MySqlCommandBuilder(Da);
                    Da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                    DataSet ds = new DataSet();

                    Da.Fill(ds, tableName);

                    var dbTable = ds.Tables[tableName];
                    var pks = dbTable.PrimaryKey.Select(c => c.ColumnName).ToList();
                    List<string> cols = new List<string>();
                    var colEnumerator = records.Columns.GetEnumerator();
                    while (colEnumerator.MoveNext())
                    {
                        var col = colEnumerator.Current as DataColumn;
                        if (!pks.Contains(col.ColumnName))
                            cols.Add(col.ColumnName);
                    }

                    foreach (DataRow row in records.Rows)
                    {
                        List<object> keys = new List<object>(pks.Count);
                        pks.ForEach(pk => keys.Add(row[pk]));
                        var modiRow = dbTable.Rows.Find(keys.ToArray());
                        cols.ForEach(ck => modiRow[ck] = row[ck]);
                    }
                    Da.Update(ds, tableName);
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
                    Conn.InfoMessage += new MySqlInfoMessageEventHandler((sender, args) =>
                    {
                        var result = (args as MySqlInfoMessageEventArgs).errors?.First()?.Message;
                        if (!string.IsNullOrEmpty(result))
                        {
                            var match = System.Text.RegularExpressions.Regex.Match(result, "'[\\d]+'");
                            if (match.Success) currId = match.Value;
                        }
                    });
                    Cmd.CommandText = "DBCC CHECKIDENT(@table, NORESEED)";
                    Cmd.Parameters.Add(new MySqlParameter("@table", table));
                    Cmd.ExecuteNonQuery();

                    if (string.IsNullOrEmpty(currId))
                        Cmd.CommandText = "select IDENT_CURRENT(@table)";
                    else
                        Cmd.CommandText = "select IDENT_CURRENT(@table) + IDENT_INCR(@table)";
                    return int.Parse(Cmd.ExecuteScalar().ToString());
                }
            }
            catch (MySqlException se)
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
