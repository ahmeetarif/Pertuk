using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pertuk.Business.Cache;
using Pertuk.Business.Services.Abstract;
using Pertuk.Business.Services.Concrete;
using StackExchange.Redis;

namespace Pertuk.Business.Installers
{
    public class CacheInstaller : IBaseInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var redisCacheSettings = new RedisCacheSettings();
            configuration.GetSection(nameof(RedisCacheSettings)).Bind(redisCacheSettings);
            services.AddSingleton(redisCacheSettings);

            if (!redisCacheSettings.Enabled)
            {
                return;
            }

            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisCacheSettings.ConnectionString));
            services.AddStackExchangeRedisCache(options => options.Configuration = redisCacheSettings.ConnectionString);
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();
        }
    }
}
