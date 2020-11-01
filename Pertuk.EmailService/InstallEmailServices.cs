using Microsoft.Extensions.DependencyInjection;
using Pertuk.EmailService.Abstract;

namespace Pertuk.EmailService
{
    public static class InstallEmailServices
    {
        public static void ConfigureEmailSender(this IServiceCollection services)
        {
            services.AddTransient<IEmailSender, IEmailSender>();
        }
    }
}