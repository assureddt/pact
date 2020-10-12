using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Pact.Cache
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDistributedCache(this IServiceCollection services,
            CacheSettings settings,
            IDataProtectionBuilder dataProtectionBuilder = null)
        {
            switch (settings?.Provider)
            {
                case CacheProvider.Redis:
                    var redis = ConnectionMultiplexer.Connect(settings.ConnectionString);
                    services.AddStackExchangeRedisCache(opts =>
                    {
                        opts.Configuration = settings.ConnectionString;
                        opts.InstanceName = settings.StorageName;
                    });
                    if (dataProtectionBuilder == null) break;

                    if (settings.StorageKey != null)
                    {
                        dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, settings.StorageKey);
                    }
                    else
                    {
                        dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis);
                    }
                    break;
                case CacheProvider.SqlServer:
                    services.AddDistributedSqlServerCache(settings.SqlServerOptions);
                    break;
                default:
                    services.AddDistributedMemoryCache();
                    break;
            }
            services.AddSingleton<IDistributedCacheService, DistributedCacheService>();

            return services;
        }
    }
}
