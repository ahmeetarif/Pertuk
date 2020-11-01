using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pertuk.Business.Externals.Managers.Abstract;
using Pertuk.Business.Externals.Managers.Concrete;
using Pertuk.Business.Options;

namespace Pertuk.Business.Installers
{
    public class FacebookAuthInstaller : IBaseInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var facebookAuthOptions = new FacebookAuthOptions();
            configuration.GetSection(nameof(FacebookAuthOptions)).Bind(facebookAuthOptions);

            services.AddSingleton(facebookAuthOptions);

            services.AddHttpClient();
            services.AddSingleton<IFacebookAuthenticationManager, FacebookAuthenticationManager>();
        }
    }
}