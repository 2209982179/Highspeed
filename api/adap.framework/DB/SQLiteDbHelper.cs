using highspeed.framework.Common;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace highspeed.framework.Data
{
    /// <summary>
    /// SQlite数据库访问
    /// </summary>
    public class SQLiteDbHelper : IDbHelper
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString"></param>
        public SQLiteDbHelper(string connectionString)
        {
            this.connectionString = connectionString;
            Conn = new SQLiteConnection(connectionString);
        }

        private string connectionString;
        private SQLiteConnection Conn;
        private SQLiteTransaction Transaction;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public void SetConnectionString(string connectionString)
        {
            this.connectionString = connectionString;
            Conn.ConnectionString = connectionString;
        }

        public string GetConnectionString()
        {
            return this.connectionString;
        }

        private bool _BatchExcuteFlag = false;
        public void StartBatchExcute()
        {
            _BatchExcuteFlag = true;
        }
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
            if (Conn == null) Conn = new SQLiteConnection(this.connectionString);
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
            var Cmd = new SQLiteCommand();
            if (!_BatchExcuteFlag) Conn = new SQLiteConnection(this.connectionString);
            Cmd.Connection = Conn;
            if (Transaction != null && Transaction.Connection?.State == ConnectionState.Open) Cmd.Transaction = Transaction;
            Cmd.CommandTimeout = 60 * 1000;
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
        /// 执行ExecuteNonQuery()
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, List<KeyValuePair<string, object>> parameters = null)
        {
            try
            {
                sql = FormatSql(sql);
                var Cmd = Open();
                Cmd.CommandType = CommandType.Text;
                Cmd.CommandText = sql;
                if (parameters != null)
                {
                    foreach (var p in parameters)
                    {
                        Cmd.Parameters.Add(new SQLiteParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                    }
                }
                return Cmd.ExecuteNonQuery();
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
        /// DataSet类
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public DataSet Query(string sql, List<KeyValuePair<string, object>> parameters = null)
        {
            try
            {
                sql = FormatSql(sql);
                var Cmd = Open();
                Cmd.CommandType = CommandType.Text;
                Cmd.CommandText = sql;
                if (parameters != null)
                {
                    foreach (var p in parameters)
                    {
                        Cmd.Parameters.Add(new SQLiteParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                    }
                }
                var Da = new SQLiteDataAdapter();
                Da.SelectCommand = Cmd as SQLiteCommand;
                var Ds = new DataSet();
                Da.Fill(Ds);
                return Ds;
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
        /// DataTable 类
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public DataTable QueryDataTable(string sql, List<KeyValuePair<string, object>> parameters = null)
        {
            try
            {
                sql = FormatSql(sql);
                var Cmd = Open();
                Cmd.CommandType = CommandType.Text;
                Cmd.CommandText = sql;
                if (parameters != null)
                {
                    foreach (var p in parameters)
                    {
                        Cmd.Parameters.Add(new SQLiteParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                    }
                }
                var Da = new SQLiteDataAdapter();
                Da.SelectCommand = Cmd as SQLiteCommand;
                var Dt = new DataTable();
                Da.Fill(Dt);
                return Dt;
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
        /// <returns></returns>
        public object ExecuteScalar(string sql, List<KeyValuePair<string, object>> parameters = null)
        {
            try
            {
                sql = FormatSql(sql);
                var Cmd = Open();
                Cmd.CommandType = CommandType.Text;
                Cmd.CommandText = sql;
                if (parameters != null)
                {
                    foreach (var p in parameters)
                    {
                        Cmd.Parameters.Add(new SQLiteParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                    }
                }
                return Cmd.ExecuteScalar();
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

                sql = FormatSql(sql);
                var Cmd = Open();
                Cmd.CommandType = CommandType.Text;
                Cmd.CommandText = sql;
                if (parameters != null)
                {
                    foreach (var p in parameters)
                    {
                        Cmd.Parameters.Add(new SQLiteParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                    }
                }
                return Cmd.ExecuteScalar();
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
        /// <param name="sqls">SQL语句</param>
        /// <returns></returns>
        public object ExecuteScalar(Dictionary<string, List<KeyValuePair<string, object>>> sqls)
        {
            try
            {
                object result = null;
                var Cmd = Open();
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
                            Cmd.Parameters.Add(new SQLiteParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                        }
                    }
                    if (i < sqls.Count) Cmd.ExecuteNonQuery();
                    else result = Cmd.ExecuteScalar();
                }
                return result;
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
        /// 执行事务
        /// </summary>
        /// <param name="sqls"></param>
        public void DbTransaction(string[] sqls)
        {
            string curr_sql = null;
            try
            {
                if (!Conn.State.Equals(ConnectionState.Open) && !Conn.State.Equals(ConnectionState.Connecting))
                {
                    Conn.Open();
                }
                using (SQLiteTransaction trans = Conn.BeginTransaction())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(Conn))
                    {
                        cmd.Transaction = trans;
                        try
                        {
                            foreach (var sql in sqls)
                            {
                                cmd.CommandText = FormatSql(sql);
                                curr_sql = sql;
                                cmd.ExecuteNonQuery();
                            }
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("adap.framework.Data.SQLiteDbHelper.DbTransaction", "Encounter error when execute sql:" + curr_sql, e);
                throw;
            }
            finally
            {
                if (!_BatchExcuteFlag) Close();
            }
        }

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        public void DbTransaction(string sql, IEnumerable<Dictionary<string, string>> parameters)
        {
            string curr_sql = null;
            try
            {
                if (!Conn.State.Equals(ConnectionState.Open) && !Conn.State.Equals(ConnectionState.Connecting))
                {
                    Conn.Open();
                }
                using (SQLiteTransaction trans = Conn.BeginTransaction())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(Conn))
                    {
                        cmd.Transaction = trans;
                        try
                        {
                            cmd.CommandText = FormatSql(sql);
                            foreach (var param in parameters)
                            {
                                cmd.Parameters.Clear();
                                foreach (var kv in param)
                                    cmd.Parameters.AddWithValue(kv.Key, kv.Value);
                                cmd.ExecuteNonQuery();
                            }
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("adap.framework.Data.SQLiteDbHelper.DbTransaction", "Encounter error when execute sql:" + curr_sql, e);
                throw;
            }
            finally
            {
                if (!_BatchExcuteFlag) Close();
            }
        }

        private string FormatSql(string sql)
        {
            sql = Regex.Replace(sql, "isnull", "ifnull", RegexOptions.IgnoreCase);
            return sql;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Close();
            Conn.Dispose();
        }
        #region 批量操作
        
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
     
        public int? GetNewID(string table)
        {
            throw new NotImplementedException("Not Supported.");
        }

        public void BulkInsert(string tableName, DataTable records)
        {
            throw new NotImplementedException("Not Supported.");
        }

        public void BulkUpdate(string tableName, DataTable records)
        {
            throw new NotImplementedException("Not Supported.");
        }

        public DataSet QueryProc(string proc, List<KeyValuePair<string, object>> parameters = null)
        {
            throw new NotImplementedException("Not Supported.");
        }

        public DataTable QueryDataTableProc(string proc, List<KeyValuePair<string, object>> parameters = null)
        {
            throw new NotImplementedException("Not Supported.");
        }

        public int ExecuteProc(string proc, List<KeyValuePair<string, object>> parameters = null)
        {
            throw new NotImplementedException("Not Supported.");
        }

        public DataSet QueryWithTempTable(string sql, List<KeyValuePair<string, object>> parameters = null, params TempTable[] temps)
        {
            throw new NotImplementedException("Not Supported.");
        }
         
        public DataTable QueryDataTableWithTempTable(string sql, List<KeyValuePair<string, object>> parameters = null, params TempTable[] temps)
        {
            throw new NotImplementedException("Not Supported.");
        }
         
        public int ExecuteNonQueryWithTempTable(string sql, List<KeyValuePair<string, object>> parameters = null, params TempTable[] temps)
        {
            throw new NotImplementedException();
        } 
        #endregion

    }
}
