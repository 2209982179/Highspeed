using highspeed.framework.Common;
using System.Text;

namespace highspeed.framework.Data
{
    public class ProjectDataHelper
    {
        #region 项目存储

        private Repository Repo;
        private static object _lock = new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        public ProjectDataHelper(Repository repo)
        { Repo = repo; }

        /// <summary>
        /// 保存项
        /// </summary>
        /// <param name="key">项名称</param>
        /// <param name="contents">内容</param>
        private void SaveData(string key, string contents, bool encrypt = true)
        {
            object data = contents;
            if (encrypt)
            {
                key = key.ToMD5();
                data = contents.Zip(Encoding.UTF8);
            }
            try
            {
                if (!Exists(key, false))
                {
                    Repo.DBHelper.ExecuteNonQuery(@"INSERT INTO t_adap_ProjectData
([UID] ,[Key] ,[Value])
VALUES
(@uid,@key,@value)"
                        , new Dictionary<string, object> { { "@uid", Guid.NewGuid().ToString() }, { "@key", key }, { "@value", data } });
                }
                else
                {
                    Repo.DBHelper.ExecuteNonQuery(@"UPDATE t_adap_ProjectData
SET [Value] = @value
WHERE [Key] = @key"
                        , new Dictionary<string, object> { { "@key", key }, { "@value", data } });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("ProjectDataHelper.SaveData", ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// 读取项
        /// </summary>
        /// <param name="model">Model</param>
        /// <param name="key">项名称</param>
        /// <returns></returns>
        private string LoadData(string key, bool encrypt = true)
        {
            if (encrypt) key = key.ToMD5();
            try
            {
                var val = Repo.DBHelper.ExecuteScalar("SELECT [Value] FROM t_adap_ProjectData WHERE [Key] = @key"
                              , new Dictionary<string, object> { { "@key", key } });
                if (val != null && val != DBNull.Value) return encrypt ? ((byte[])val).UnzipToString(Encoding.UTF8) : val.ToString();
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error("ProjectDataHelper.LoadData", ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// 删除项
        /// </summary>
        /// <param name="model">Model</param>
        /// <param name="key">项名称</param>
        public void Delete(string key, bool encrypt = true)
        {
            if (encrypt) key = key.ToMD5();
            Repo.DBHelper.ExecuteNonQuery("DELETE t_adap_ProjectData WHERE [Key] = @key"
                , new Dictionary<string, object> { { "@key", key } });
        }

        #endregion 项目存储

        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <param name="key">存取数据的Key</param>
        /// <returns>数据</returns>
        public string Load(string key, bool encrypt = true)
        {
            return LoadData(key, encrypt);
        }

        /// <summary>
        /// 保存string数据
        /// </summary>
        /// <param name="key">存取数据的Key，重复时覆盖</param>
        /// <param name="content">数据</param>
        public void Save(string key, string content, bool encrypt = true)
        {
            try
            {
                SaveData(key, content, encrypt);
            }
            catch (Exception ex)
            {
                Logger.Error("ProjectDataHelper.Save<T>", ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// 保存object数据
        /// </summary>
        /// <typeparam name="T">object的类型</typeparam>
        /// <param name="key">存取数据的Key，重复时覆盖</param>
        /// <param name="objectData">数据</param>
        public void SaveObject<T>(string key, T objectData, bool encrypt = true)
        {
            try
            {
                string jsonString = objectData == null ? string.Empty : JsonHelper.SerializeObject(objectData);
                SaveData(key, jsonString, encrypt);
            }
            catch (Exception ex)
            {
                Logger.Error("ProjectDataHelper.Save<T>", ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// 读取object数据
        /// </summary>
        /// <typeparam name="T">object的类型</typeparam>
        /// <param name="key">存取数据的Key，重复时覆盖</param>
        /// <returns>object的类型实例</returns>
        public T Load<T>(string key, bool encrypt = true) where T : class
        {
            try
            {
                string jsonString = LoadData(key, encrypt);
                return string.IsNullOrEmpty(jsonString) ? null : JsonHelper.DeserializeJsonToObject<T>(jsonString);
            }
            catch (Exception ex)
            {
                Logger.Error("ProjectDataHelper.Load<T>", ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// 读取集合对象
        /// </summary>
        /// <typeparam name="T">集合中object的类型</typeparam>
        /// <param name="key">存取数据的Key，重复时覆盖</param>
        /// <returns>object集合的实例</returns>
        public List<T> LoadList<T>(string key, bool encrypt = true) where T : class
        {
            try
            {
                string jsonString = LoadData(key, encrypt);
                return string.IsNullOrEmpty(jsonString) ? null : JsonHelper.DeserializeJsonToList<T>(jsonString);
            }
            catch (Exception ex)
            {
                Logger.Error("ProjectDataHelper.LoadList<T>", ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// 读取匿名对象
        /// </summary>
        /// <typeparam name="T">object的类型</typeparam>
        /// <param name="key">存取数据的Key，重复时覆盖</param>
        /// <param name="anonymousTypeObject">object类型的匿名实例</param>
        /// <returns>object类型的实例</returns>
        public T LoadAnonymousType<T>(string key, T anonymousTypeObject, bool encrypt = true) where T : class
        {
            try
            {
                string jsonString = LoadData(key, encrypt);
                return string.IsNullOrEmpty(jsonString) ? null : JsonHelper.DeserializeAnonymousType(jsonString, anonymousTypeObject);
            }
            catch (Exception ex)
            {
                Logger.Error("ProjectDataHelper.LoadAnonymousType<T>", ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// 保存二进制数据
        /// </summary>
        /// <param name="key">存取数据的Key，重复时覆盖</param>
        /// <param name="contents">数据</param>
        public void SaveBytes(string key, byte[] contents, bool encrypt = true)
        {
            if (encrypt)
            {
                key = key.ToMD5();
                contents = contents.ZipByte();
            }
            try
            {
                if (!Exists(key, false))
                {
                    Repo.DBHelper.ExecuteNonQuery(@"INSERT INTO t_adap_ProjectData
([UID] ,[Key] ,[Value])
VALUES
(@uid,@key,@value)"
                        , new Dictionary<string, object> { { "@uid", Guid.NewGuid().ToString() }, { "@key", key }, { "@value", contents } });
                }
                else
                {
                    Repo.DBHelper.ExecuteNonQuery(@"UPDATE t_adap_ProjectData
SET [Value] = @value
WHERE [Key] = @key"
                        , new Dictionary<string, object> { { "@key", key }, { "@value", contents } });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("ProjectDataHelper.SaveBytes", ex.Message, ex);
                throw;
            }
        }

		/// <summary>
		/// 保存二进制数据(重载)
		/// </summary>
		/// <param name="key">存取数据的Key</param>
		/// <param name="evdata"></param>
		/// <param name="contents">数据</param>
		/// <param name="encrypt"></param>
		public void SaveBytes(string key,string evdata, byte[] contents, bool encrypt = true)
		{
			if (encrypt)
			{
				key = key.ToMD5();
				contents = contents.ZipByte();
			}
			try
			{
				if (!Exists(key, false))
				{
					Repo.DBHelper.ExecuteNonQuery(@"INSERT INTO t_adap_ProjectData
([UID] ,[Key] ,[Value],[exDATA1])
VALUES
(@uid,@key,@value,@exDATA1)"
						, new Dictionary<string, object> { { "@uid", Guid.NewGuid().ToString() }, { "@key", key }, { "@value", contents }, { "@exDATA1", evdata } });
				}
				else
				{
					Repo.DBHelper.ExecuteNonQuery(@"UPDATE t_adap_ProjectData
SET [Value] = @value,[exDATA1] = @exDATA
WHERE [Key] = @key"
						, new Dictionary<string, object> { { "@key", key  }, { "@value", contents },{ "@exDATA", evdata } });
				}
			}
			catch (Exception ex)
			{
				Logger.Error("ProjectDataHelper.SaveBytes", ex.Message, ex);
				throw;
			}
		}

		/// <summary>
		/// 读取二进制数据
		/// </summary>
		/// <param name="key">存取数据的Key</param>
		/// <returns>数据</returns>
		public byte[]? LoadBytes(string key, bool encrypt = true)
        {
            if (encrypt) key = key.ToMD5();
            try
            {
                var val = Repo.DBHelper.ExecuteScalar("SELECT [Value] FROM t_adap_ProjectData WHERE [Key] = @key"
                              , new Dictionary<string, object> { { "@key", key } });
                if (val != null && val != DBNull.Value) return encrypt ? ((byte[])val).UnzipByte() : (byte[])val;
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error("ProjectDataHelper.LoadBytes", ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// 检查Key对应的数据是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exists(string key, bool encrypt = true)
        {
            if (encrypt) key = key.ToMD5();
            try
            {
                var val = Repo.DBHelper.ExecuteScalar("SELECT COUNT(0) FROM t_adap_ProjectData WHERE [Key] = @key"
                              , new Dictionary<string, object> { { "@key", key } });
                return val != null && val != DBNull.Value && (int)val > 0;
            }
            catch (Exception ex)
            {
                Logger.Error("ProjectDataHelper.Exists", ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// 检查Key对应的数据是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool DocExists(string key)
        {
            try
            {
                var val = Repo.DBHelper.ExecuteScalar("SELECT COUNT(0) FROM t_adap_ProjectData WHERE [exDATA1] = @key"
                              , new Dictionary<string, object> { { "@key", key } });
                return val != null && val != DBNull.Value && (int)val > 0;
            }
            catch (Exception ex)
            {
                Logger.Error("ProjectDataHelper.Exists", ex.Message, ex);
                throw;
            }
        }

        /// <summary>
		/// 读取二进制数据
		/// </summary>
		/// <param name="key">存取数据的Key</param>
		/// <returns>数据</returns>
		public byte[]? DocLoadBytes(string key)
        {
            try
            {
                var val = Repo.DBHelper.ExecuteScalar("SELECT [Value] FROM t_adap_ProjectData WHERE [exDATA1] = @key"
                              , new Dictionary<string, object> { { "@key", key } });
                if (val != null && val != DBNull.Value) return ((byte[])val).UnzipByte();
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error("ProjectDataHelper.LoadBytes", ex.Message, ex);
                throw;
            }
        }
    }
}