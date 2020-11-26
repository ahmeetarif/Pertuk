using Microsoft.AspNetCore.Builder;
using Pertuk.Common.MiddleWare;

namespace Pertuk.Business.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static void ConfigureTokenExpiredResponse(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExpiredTokenMiddleware>();
        }
    }
}
