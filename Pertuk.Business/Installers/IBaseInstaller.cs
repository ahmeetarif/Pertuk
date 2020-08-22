using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pertuk.Business.Installers
{
    public interface IBaseInstaller
    {
        void InstallServices(IServiceCollection services, IConfiguration configuration);
    }
}