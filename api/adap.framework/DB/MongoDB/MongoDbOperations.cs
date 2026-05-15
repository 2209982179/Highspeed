using global::MongoDB.Bson;
using global::MongoDB.Driver;
using Microsoft.SqlServer.Management.HadrData;
using MongoDB.Driver.GridFS;
using System.IO;


namespace highspeed.framework.DB.MongoDB
{
    public class MongoDbOperations
    {
        public readonly IMongoDatabase _database = null;
        public readonly GridFSBucket _bucket = null;

        public MongoDbOperations(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            if (client != null)
            {
                _database = client.GetDatabase(databaseName);
                _bucket = new GridFSBucket(_database); //这个是初始化gridFs存储的
            }
        }

        public async Task InsertDataAsync(string fileName, byte[] data)
        {
            var id = await _bucket.UploadFromBytesAsync(fileName, data);
        }

        public List<ObjectId> GetUploadFileId(string fileName)
        {
            var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.Filename, fileName);
            //eq方法，就是等于，还有其他的方法，具体看Mongo的api文档
            var sort = Builders<GridFSFileInfo>.Sort.Descending(x => x.UploadDateTime);
            //按上传时间来倒叙一下
            var options = new GridFSFindOptions
            {
                Limit = 1,
                Sort = sort
            };

            List<ObjectId> result = new List<ObjectId>();
            using (var cursor = _bucket.Find(filter, options))
            {
                foreach (var fileInfo in cursor.ToList())
                {
                    if (fileInfo != null && fileInfo.Length > 0)
                    {
                        result.Add(fileInfo.Id);
                    }
                }
                return result;
            }
        }

        public Task<byte[]> DownLoadFileFromGirdFs(ObjectId id)
        {
            return _bucket.DownloadAsBytesAsync(id);
        }
    }
}
