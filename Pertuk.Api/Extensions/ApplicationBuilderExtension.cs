using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Pertuk.Business.Options;
using Pertuk.Contracts.HealthChecks;
using System.Linq;

namespace Pertuk.Api.Extensions
{
    public static class ApplicationBuilderExtension
    {
        // Swagger
        public static void ConfigureSwagger(this IApplicationBuilder app, IConfiguration configuration)
        {
            var swaggerOptions = new SwaggerOption();
            configuration.GetSection(nameof(SwaggerOption)).Bind(swaggerOptions);

            app.UseSwagger(option => option.RouteTemplate = swaggerOptions.JsonRoute);
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(swaggerOptions.UIEndpoint, swaggerOptions.Description);
            });
        }

        // HealthCheck
        public static void ConfigureHealthCheck(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/v1/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";

                    var response = new HealthCheckResponse
                    {
                        Status = report.Status.ToString(),
                        Checks = report.Entries.Select(x => new HealthCheck
                        {
                            Component = x.Key,
                            Status = x.Value.Status.ToString(),
                            Description = x.Value.Description
                        }),
                        Duration = report.TotalDuration
                    };

                    await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                }
            });
        }
    }
}