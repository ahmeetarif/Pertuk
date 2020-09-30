using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Pertuk.Common.Exceptions;
using System.Net;

namespace Pertuk.Business.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var errorResponse = new PertukExceptionServiceErrorResponse();

            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            if (context.Exception is PertukApiException pertukApiException)
            {
                errorResponse.Message = pertukApiException.Message;
            }
            else
            {
                errorResponse.Message = context.Exception.Message;
            }

            context.HttpContext.Items.Add("exception", context.Exception);
            context.HttpContext.Items.Add("exceptionMessage", errorResponse.Message.ToString());
            context.Result = new JsonResult(errorResponse);
        }
    }
}
