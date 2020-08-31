using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pertuk.Business.CustomIdentity;
using Pertuk.Business.CustomIdentity.Providers;
using Pertuk.DataAccess;
using Pertuk.Entities.Models;

namespace Pertuk.Business.Installers
{
    public class DbInstaller : IBaseInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PertukDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("MainConnectionString")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // TODO : Simplify
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredUniqueChars = 0;
                options.User.RequireUniqueEmail = true;
            })
                .AddDefaultTokenProviders()
                .AddUserManager<PertukUserManager>()
                .AddTokenProvider<DigitTokenProvider>(DigitTokenProvider.EmailDigit)
                .AddTokenProvider<DigitTokenProvider>(DigitTokenProvider.PhoneDigit)
                .AddEntityFrameworkStores<PertukDbContext>();
        }
    }
}
