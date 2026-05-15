using System.Data;
using System.Data.Common;
using System.Linq;

namespace highspeed.framework.Data
{
    /// <summary>
    /// 数据库事务操作
    /// </summary>
    public class DbTransactionOperation
    {
        DBHelper dbHelper;
        DbTransaction transaction;
        List<Action<DBHelper>> Actions = new();
        List<TempTable> TempTables = new(); 
        internal DbTransactionOperation(DBHelper dbHelper, DbTransaction transaction)
        {
            this.dbHelper = dbHelper;
            this.transaction = transaction;
        }

        /// <summary>
        /// 加入事务过程中依赖的的TempTable
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public DbTransactionOperation Append(params TempTable[] temps)
        {
            TempTables = TempTables.Concat(temps).Where(t => t != null).ToList();
            return this;
        }
         
        /// <summary>
        /// 加入事务过程中执行的Action
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public DbTransactionOperation Append(Action<DBHelper> action)
        {
            Actions.Add(action);
            return this;
        }

        /// <summary>
        /// 开始执行事务
        /// </summary>
        public void Run()
        {
            if (Actions.Count == 0) return;
            var dbhTemp = new DBHelper(dbHelper.DBType, dbHelper.ConnectionString);
            List<string> tempTables = new List<string>();
            try
            {
                // 创建临时表数据
                if (TempTables.Count > 0)
                {
                    var tables = TempTables.Select(t => t.ConvertToDataTable());
                    tempTables.AddRange(dbhTemp.BulkInsertTempTable(tables.ToArray()));
                } 

                var conn = transaction.Connection;
                if (!conn.State.Equals(ConnectionState.Open) && !conn.State.Equals(ConnectionState.Connecting))
                {
                    conn.Open();
                }

                Actions.ForEach(act => act.Invoke(dbHelper));
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                transaction.Dispose();
                dbHelper.EndBatchExcute();

                if (tempTables.Count > 0)
                    try { dbhTemp.DropTempTable(tempTables.ToArray()); }
                    catch { }
            }
        }
    }
}
