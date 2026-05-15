using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity.Core.Metadata.Edm;
using System.Reflection;

namespace highspeed.framework
{
    public static class DataExtension
    {
        #region DataRow Clone

        public static DataRow Clone(this DataRow dataRow)
        {
            try
            {
                DataRow row = dataRow.Table.NewRow();
                row.ItemArray = (object[])dataRow.ItemArray.Clone();
                return row;
            }
            catch { return null; }
        }

        public static DataRow Clone(this DataRow dataRow, DataRow target)
        {
            try
            {
                target.ItemArray = (object[])dataRow.ItemArray.Clone();
                return target;
            }
            catch { return null; }
        }

        public static DataRow Clone(this DataRow dataRow, IEnumerable<string> cloneColumns)
        {
            try
            {
                DataRow row = dataRow.Table.NewRow();
                for (int i = 0; i < dataRow.Table.Columns.Count; i++)
                {
                    if (cloneColumns.Contains(dataRow.Table.Columns[i].ColumnName))
                        row[i] = dataRow[i];
                }
                return row;
            }
            catch { return null; }
        }

        public static DataRow Clone(this DataRow dataRow, DataRow target, IEnumerable<string> cloneColumns)
        {
            try
            {
                foreach (DataColumn col in dataRow.Table.Columns)
                {
                    if (cloneColumns.Contains(col.ColumnName))
                        target[col.ColumnName] = dataRow[col.ColumnName];
                }
                return target;
            }
            catch { return null; }
        }

        #endregion DataRow Clone

        #region DataRow To Object

        public static T ToObject<T>(this DataRow dataRow) where T : new()
        {
            try
            {
                T item = new T();

                foreach (DataColumn column in dataRow.Table.Columns)
                {
                    PropertyInfo property = GetProperty(typeof(T), column.ColumnName, true);

                    if (property != null && dataRow[column] != DBNull.Value && dataRow[column].ToString() != "NULL" && property.PropertyType.GetType() != null)
                    { 
                        if (property.PropertyType.BaseType != null && property.PropertyType.BaseType.Name.Contains("Enum"))
                        {
                            object result; // 因为你的类型是动态的，所以先用 object 接收 
                            bool success = System.Enum.TryParse(property.PropertyType, dataRow[column]?.ToString(), true, out result);

                            if (result != null)
                            {
                                property.SetValue(item, result, null);
                            }
                            else
                            {
                                property.SetValue(item, 0, null);
                            }
                        } 
                        else
                        { 
                            string TPName = property.PropertyType.Name;

                            if(TPName.Contains("List"))
                            {
                                property.SetValue(item,null, null);
                            }
                            else
                            { 
                                property.SetValue(item, ChangeType(dataRow[column], property.PropertyType), null);
                            }
                        }
                    }
                }
                return item;
            }
            catch { throw; }
        }

        public static List<T> ToObjectList<T>(this IEnumerable<DataRow> dataRows) where T : new()
        {
            List<T> list = new List<T>();
            foreach (var row in dataRows) list.Add(row.ToObject<T>());
            return list;
        }

        private static object locker = new object();
        private static Dictionary<string, Dictionary<string, string>> propDict = new Dictionary<string, Dictionary<string, string>>();

        private static PropertyInfo GetProperty(Type type, string attributeName, bool ignoreCase = false)
        {
            PropertyInfo property = null;
            if (!ignoreCase)
                property = type.GetProperty(attributeName);
            else
            {
                if (!propDict.ContainsKey(type.FullName))
                    lock (locker)
                        if (!propDict.ContainsKey(type.FullName))
                            propDict.Add(type.FullName, new Dictionary<string, string>());

                var dict = propDict[type.FullName];
                if (dict.ContainsKey(attributeName))
                {
                    property = type.GetProperty(dict[attributeName]);
                }
                if (property == null)
                    foreach (var p in type.GetProperties())
                    {
                        if (p.Name.ToUpper().Equals(attributeName?.ToUpper()))
                        {
                            property = p;
                            if (!dict.ContainsKey(attributeName))
                                lock (locker)
                                    if (!dict.ContainsKey(attributeName))
                                        dict.Add(attributeName, p.Name);
                            break;
                        }
                    }
            }

            if (property != null)
            {
                return property;
            }

            return type.GetProperties()
                 .Where(p => p.IsDefined(typeof(DisplayAttribute), false) && p.GetCustomAttributes(typeof(DisplayAttribute), false).Cast<DisplayAttribute>().Single().Name == attributeName)
                 .FirstOrDefault();
        }

        public static object ChangeType(object value, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }
                return Convert.ChangeType(value, Nullable.GetUnderlyingType(type));
            }
            return Convert.ChangeType(value, type);
        }

        #endregion DataRow To Object

        #region DataTable

        public static string GetCreateSQL(this DataTable table, string tableName)
        {
            return GetCreateFromDataTableSQL(tableName, table);
        }

        public static string GetCreateTableTypeSQL(this DataTable table, string tableName)
        {
            var sql = GetCreateFromDataTableSQL(tableName, table);
            sql = sql.Replace($"CREATE TABLE [{tableName}]", $"CREATE TYPE [{tableName}] AS TABLE");
            return sql;
        }

        #region private

        private static string GetCreateSQL(string tableName, DataTable schema, int[] primaryKeys)
        {
            string sql = "CREATE TABLE [" + tableName + "] (\n";

            // columns
            foreach (DataRow column in schema.Rows)
            {
                if (!(schema.Columns.Contains("IsHidden") && (bool)column["IsHidden"]))
                {
                    sql += "\t[" + column["ColumnName"].ToString() + "] " + SQLGetType(column);

                    if (schema.Columns.Contains("AllowDBNull") && (bool)column["AllowDBNull"] == false)
                        sql += " NOT NULL";

                    sql += ",\n";
                }
            }
            sql = sql.TrimEnd(new char[] { ',', '\n' }) + "\n";

            // primary keys
            string pk = ", CONSTRAINT PK_" + tableName + " PRIMARY KEY CLUSTERED (";
            bool hasKeys = (primaryKeys != null && primaryKeys.Length > 0);
            if (hasKeys)
            {
                // user defined keys
                foreach (int key in primaryKeys)
                {
                    pk += schema.Rows[key]["ColumnName"].ToString() + ", ";
                }
            }
            else
            {
                // check schema for keys
                string keys = string.Join(", ", GetPrimaryKeys(schema));
                pk += keys;
                hasKeys = keys.Length > 0;
            }
            pk = pk.TrimEnd(new char[] { ',', ' ', '\n' }) + ")\n";
            if (hasKeys) sql += pk;

            sql += ");";

            return sql;
        }

        private static string GetCreateFromDataTableSQL(string tableName, DataTable table)
        {
            string sql = "CREATE TABLE [" + tableName + "] (\n";
            // columns
            foreach (DataColumn column in table.Columns)
            {
                sql += "[" + column.ColumnName + "] " + SQLGetType(column) + ",\n";
            }
            sql = sql.TrimEnd(new char[] { ',', '\n' }) + "\n";
            // primary keys
            if (table.PrimaryKey.Length > 0)
            {
                sql += "CONSTRAINT [PK_" + tableName + "] PRIMARY KEY CLUSTERED (";
                foreach (DataColumn column in table.PrimaryKey)
                {
                    sql += "[" + column.ColumnName + "],";
                }
                sql = sql.TrimEnd(new char[] { ',' }) + "))\n";
            }

            //if not ends with ")"
            if ((table.PrimaryKey.Length == 0) && (!sql.EndsWith(")")))
            {
                sql += ")";
            }

            return sql;
        }

        private static string[] GetPrimaryKeys(DataTable schema)
        {
            List<string> keys = new List<string>();

            foreach (DataRow column in schema.Rows)
            {
                if (schema.Columns.Contains("IsKey") && (bool)column["IsKey"])
                    keys.Add(column["ColumnName"].ToString());
            }

            return keys.ToArray();
        }

        // Return T-SQL data type definition, based on schema definition for a column
        private static string SQLGetType(object type, int columnSize, int numericPrecision, int numericScale)
        {
            switch (type.ToString())
            {
                //使用NVARCHAR兼容特殊符合
                case "System.String":
                    return "NVARCHAR(" + ((columnSize == -1) ? "510" : (columnSize > 16000) ? "MAX" : columnSize.ToString()) + ")";

                case "System.Decimal":
                    if (numericScale > 0)
                        return "REAL";
                    else if (numericPrecision > 10)
                        return "BIGINT";
                    else
                        return "INT";

                case "System.Double":
                case "System.Single":
                    return "REAL";

                case "System.Int64":
                    return "BIGINT";

                case "System.Int16":
                case "System.Int32":
                    return "INT";

                case "System.DateTime":
                    return "DATETIME";

                case "System.Boolean":
                    return "BIT";

                case "System.Byte":
                    return "TINYINT";

                case "System.Guid":
                    return "UNIQUEIDENTIFIER";

                default:
                    throw new Exception(type.ToString() + " not implemented.");
            }
        }

        // Overload based on row from schema table
        private static string SQLGetType(DataRow schemaRow)
        {
            return SQLGetType(schemaRow["DataType"],
                                int.Parse(schemaRow["ColumnSize"].ToString()),
                                int.Parse(schemaRow["NumericPrecision"].ToString()),
                                int.Parse(schemaRow["NumericScale"].ToString()));
        }

        // Overload based on DataColumn from DataTable type
        private static string SQLGetType(DataColumn column)
        {
            return SQLGetType(column.DataType, column.MaxLength, 10, 2);
        }

        #endregion private

        #endregion DataTable
    }
}
