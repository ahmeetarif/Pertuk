using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pertuk.Business.Options;
using Pertuk.Business.Services.Abstract;
using Pertuk.Business.Services.Concrete;

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
            services.AddSingleton<IFacebookAuthService, FacebookAuthService>();
        }
    }
}
