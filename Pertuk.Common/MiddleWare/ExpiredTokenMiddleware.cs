using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Pertuk.Common.MiddleWare
{
    public class ExpiredTokenMiddleware
    {
        private readonly RequestDelegate _next;
        public ExpiredTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (string.IsNullOrEmpty(context.Response.Headers["www-authenticate"]))
            {
                context.Response.StatusCode = 403;

                var jsonString = "{\"message\":this token has been expired,\"isExpired\":true}";
                byte[] data = Encoding.UTF8.GetBytes(jsonString);

                await context.Response.Body.WriteAsync(data, 0, data.Length);
                // DO NOT CALL NEXT. THIS SHORTCIRCUITS THE PIPELINE
            }
            else
            {
                await _next(context);
            }
        }

    }
}