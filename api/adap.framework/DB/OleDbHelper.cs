using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;

namespace highspeed.framework.Data
{
    /// <summary>
    /// OleDb数据库访问
    /// </summary>
    internal class OleDbHelper : IDbHelper
    {
        public OleDbHelper(string connectionString)
        {
            this.connectionString = connectionString;
            Conn = new OleDbConnection(connectionString);
        }

        private string connectionString;
        private OleDbConnection Conn;
        private OleDbTransaction Transaction;

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
            if (Conn == null) Conn = new OleDbConnection(this.connectionString);
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
            var Cmd = new OleDbCommand();
            if (!_BatchExcuteFlag) Conn = new OleDbConnection(this.connectionString);
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
                            Cmd.Parameters.Add(new OleDbParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
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
        /// DataSet类
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public DataSet Query(string sql, List<KeyValuePair<string, object>> parameters = null)
        {
            try
            {
                using (var Cmd = Open())
                using (var Da = new OleDbDataAdapter())
                {
                    Cmd.CommandType = CommandType.Text;
                    Cmd.CommandText = sql;
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new OleDbParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                        }
                    }
                    Da.SelectCommand = Cmd as OleDbCommand;
                    var Ds = new DataSet();
                    Da.Fill(Ds);
                    return Ds;
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
        /// DataTable 类
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public DataTable QueryDataTable(string sql, List<KeyValuePair<string, object>> parameters = null)
        {
            try
            {
                using (var Cmd = Open())
                using (var Da = new OleDbDataAdapter())
                {
                    Cmd.CommandType = CommandType.Text;
                    Cmd.CommandText = sql;
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new OleDbParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
                        }
                    }
                    Da.SelectCommand = Cmd as OleDbCommand;
                    var Dt = new DataTable();
                    Da.Fill(Dt);
                    return Dt;
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
        /// <returns></returns>
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
                            Cmd.Parameters.Add(new OleDbParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
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
                    Cmd.CommandText = sql;
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new OleDbParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
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
                    Cmd.CommandText = sql;
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            Cmd.Parameters.Add(new OleDbParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
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
        /// <param name="sqls">SQL语句</param>
        /// <returns></returns>
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
                                Cmd.Parameters.Add(new OleDbParameter(p.Key, p.Value == null ? DBNull.Value : p.Value));
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
                using (var Cmd = Open())
                using (var Da = new OleDbDataAdapter())
                {

                    Cmd.CommandType = CommandType.Text;
                    Cmd.CommandText = "select * from " + tableName;
                    Da.SelectCommand = Cmd as OleDbCommand;
                    OleDbCommandBuilder cmdBuilder = new OleDbCommandBuilder(Da);
                    cmdBuilder.QuotePrefix = "[";
                    cmdBuilder.QuoteSuffix = "]";
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
                        //if (!pks.Contains(col.ColumnName))
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
            try
            {
                using (var Cmd = Open())
                using (var Da = new OleDbDataAdapter())
                {

                    Cmd.CommandType = CommandType.Text;
                    Cmd.CommandText = "select * from " + tableName;
                    Da.SelectCommand = Cmd as OleDbCommand;
                    OleDbCommandBuilder cmdBuilder = new OleDbCommandBuilder(Da);
                    cmdBuilder.QuotePrefix = "[";
                    cmdBuilder.QuoteSuffix = "]";
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
                        //if (!pks.Contains(col.ColumnName))
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
        /// 
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
        /// <returns></returns>
        public int? GetNewID(string table)
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

        public DataSet QueryWithTempTable(string sql, List<KeyValuePair<string, object>> parameters = null, params MultiFieldTempTable[] temps)
        {
            throw new NotImplementedException("Not Supported.");
        }

        public DataTable QueryDataTableWithTempTable(string sql, List<KeyValuePair<string, object>> parameters = null, params TempTable[] temps)
        {
            throw new NotImplementedException("Not Supported.");
        }

        public DataTable QueryDataTableWithTempTable(string sql, List<KeyValuePair<string, object>> parameters = null, params MultiFieldTempTable[] temps)
        {
            throw new NotImplementedException("Not Supported.");
        }

        public int ExecuteNonQueryWithTempTable(string sql, List<KeyValuePair<string, object>> parameters = null, params TempTable[] temps)
        {
            throw new NotImplementedException();
        }

        public int ExecuteNonQueryWithTempTable(string sql, List<KeyValuePair<string, object>> parameters = null, params MultiFieldTempTable[] temps)
        {
            throw new NotImplementedException();
        }
    }
}
