using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Pact.Core;
using Pact.Core.Extensions;

namespace Pact.Web.ErrorHandling.Extensions
{   
    public static class ErrorHandlingExtensions
    {
        public static IApplicationBuilder UseTitanErrorHandling(this IApplicationBuilder builder)
        {
            builder.UseStatusCodePagesWithReExecute("/Error/{0}");
            builder.UseExceptionHandler("/Error");

            return builder;
        }

        public static int ManageError(this PageHandlerExecutingContext context, ILogger logger, out string details)
        {
            var returnCode = StatusCodes.Status500InternalServerError;
            details = GetErrorMessage(context.HttpContext);

            context.RouteData.Values.TryGetValue("code", out var val);
            if (int.TryParse(val?.ToString(), out var code))
                returnCode = code;

            if (context.HttpContext.Request.IsAjaxRequest())
            {
                logger.LogWarning("Json status code response ({Code})", returnCode);
                context.Result = new JsonResult(new {message = details});
            }
            else
            {
                logger.LogWarning("Html status code response ({Code})", returnCode);
                context.Result = new PageResult();
            }

            context.HttpContext.Response.Headers["Vary"] = "X-Requested-With";

            return returnCode;
        }

        private static string GetErrorMessage(HttpContext context)
        {
            try
            {
                var exceptionFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var error = exceptionFeature?.Error;

                if (error == null) return null;

                return error is FriendlyException fe ? fe.Message : null;
            }
            catch
            {
                // ignored
            }

            return null;
        }
    }
}