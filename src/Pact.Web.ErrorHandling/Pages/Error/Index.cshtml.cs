using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pact.Core;
using Pact.Core.Extensions;
using Pact.Web.ErrorHandling.Interfaces;
using Pact.Web.ErrorHandling.Settings;
using Pact.Web.Interfaces;

namespace Pact.Web.ErrorHandling.Pages.Error
{
    [IgnoreAntiforgeryToken]
    [AllowAnonymous]
    public class IndexModel : PageModel, IModel, IErrorDetails
    {
        private readonly IOptions<ErrorHandlerSettings> _settings;

        public IndexModel(ILogger<IndexModel> logger, IAntiforgery xsrf, IOptions<ErrorHandlerSettings> settings)
        {
            _settings = settings;
            Logger = logger;
            Xsrf = xsrf;
            Title = "Error";
        }

        public int? Code { get; private set; }
        public string Details { get; private set; }

        public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            try
            {
                var isAjax = context.HttpContext.Request.IsAjaxRequest();

                Code = StatusCodes.Status500InternalServerError;
                Details = GetErrorMessage(context.HttpContext, isAjax);

                context.RouteData.Values.TryGetValue("code", out var val);
                if (int.TryParse(val?.ToString(), out var code))
                    Code = code;

                if (isAjax)
                {
                    if (_settings.Value.AjaxErrorsAsSuccess)
                        context.HttpContext.Response.StatusCode = StatusCodes.Status200OK;

                    Logger.LogWarning("Json status code response ({Code}) [{ErrorsAsSuccess}]", Code, _settings.Value.AjaxErrorsAsSuccess);

                    context.Result = new JsonResult(_settings.Value.JsonResponseFormatter(this));
                }
                else
                {
                    Logger.LogWarning("Html status code response ({Code})", Code);
                    context.Result = new PageResult();
                }

                context.HttpContext.Response.Headers["Vary"] = "X-Requested-With";
            }
            catch
            {
                // ignored
            }

            await base.OnPageHandlerExecutionAsync(context, next);
        }

        private string GetErrorMessage(HttpContext context, bool isAjax)
        {
            try
            {
                var exceptionFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var error = exceptionFeature?.Error;

                return error is FriendlyException fe
                    ? fe.Message
                    : (isAjax && _settings.Value.AjaxIncludeExceptionMessage
                        ? error?.Message
                        : null);
            }
            catch
            {
                // ignored
            }

            return null;
        }

        public ILogger Logger { get; }
        public string Title { get; }
        public IAntiforgery Xsrf { get; }
    }
}
