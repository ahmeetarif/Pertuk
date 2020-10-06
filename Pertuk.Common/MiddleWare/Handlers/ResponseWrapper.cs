using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pertuk.Dto.Responses;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Pertuk.Common.MiddleWare.Handlers
{
    public class ResponseWrapper
    {
        private readonly RequestDelegate _next;
        public ResponseWrapper(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
            }
            var currentBody = context.Response.Body;

            using (var memoryStream = new MemoryStream())
            {
                //set the current response to the memorystream.
                context.Response.Body = memoryStream;

                await _next(context);

                context.Response.Body = currentBody;
                memoryStream.Seek(0, SeekOrigin.Begin);

                var readToEnd = new StreamReader(memoryStream).ReadToEnd();

                object objResult = null;
                var result = new PertukResponse((HttpStatusCode)context.Response.StatusCode);

                if (readToEnd.ValidateJSON())
                {
                    objResult = JsonConvert.DeserializeObject(readToEnd);
                    result = PertukResponse.Create((HttpStatusCode)context.Response.StatusCode, objResult, null);
                }
                else
                {
                    objResult = readToEnd;
                    result = PertukResponse.Create((HttpStatusCode)context.Response.StatusCode, objResult, null);
                }

                string errorMessage = string.Empty;

                if (context.Items["exception"] != null)
                {
                    errorMessage = context.Items["exceptionMessage"].ToString();
                    result = PertukResponse.Create((HttpStatusCode)context.Response.StatusCode, null, errorMessage);
                }

                await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
            }

        }

    }
    public static class ResponseWrapperExtensions
    {
        public static IApplicationBuilder UseResponseWrapper(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ResponseWrapper>();
        }
    }

    public static class JsonHelper
    {
        public static bool ValidateJSON(this string s)
        {
            var response = false;
            try
            {
                JToken.Parse(s);
                response = true;
            }
            catch (JsonReaderException)
            {
                response = false;
            }
            return response;
        }
    }
}