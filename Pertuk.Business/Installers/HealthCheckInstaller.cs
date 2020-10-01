using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pertuk.Business.HealthChecks;
using Pertuk.DataAccess;

namespace Pertuk.Business.Installers
{
    public class HealthCheckInstaller : IBaseInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddDbContextCheck<PertukDbContext>()
                .AddCheck<RedisHealthCheck>("Redis");
        }
    }
}
