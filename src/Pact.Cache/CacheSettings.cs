using Microsoft.Extensions.Caching.SqlServer;

namespace Pact.Cache
{
    public class CacheSettings
    {
        public CacheProvider Provider { get; set; } = CacheProvider.Memory;
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConnectionString { get; set; }
        public string Url { get; set; }
        public string StorageName { get; set; }
        public string StorageKey { get; set; }
        public int DefaultMemoryExpirySeconds { get; set; } = 60;

        public void SqlServerOptions(SqlServerCacheOptions obj)
        {
            obj.ConnectionString = ConnectionString;
            obj.SchemaName = "dbo";
            obj.TableName = StorageName;
        }
    }

    public enum CacheProvider
    {
        Memory,
        Redis,
        SqlServer,
        MemAndRedis,
        MemAndSql
    }
}
