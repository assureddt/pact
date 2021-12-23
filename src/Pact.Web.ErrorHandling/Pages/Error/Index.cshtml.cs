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
using Pact.Web.Models;

namespace Pact.Web.ErrorHandling.Pages.Error;

[IgnoreAntiforgeryToken]
[AllowAnonymous]
public class IndexModel : PageModelBase, IErrorDetails
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IOptions<ErrorHandlerSettings> _settings;

    public IndexModel(ILogger<IndexModel> logger, IOptions<ErrorHandlerSettings> settings)
    {
        _logger = logger;
        _settings = settings;
    }

    public int? Code { get; private set; }
    public string Details { get; private set; }

    public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        try
        {
            Code = StatusCodes.Status500InternalServerError;
            Details = GetErrorMessage(context.HttpContext);

            context.RouteData.Values.TryGetValue("code", out var val);
            if (int.TryParse(val?.ToString(), out var code))
                Code = code;

            var asJson = !context.HttpContext.Request.AcceptsHtml();

            if (asJson)
            {
                if (_settings.Value.JsonErrorsAsSuccess)
                    context.HttpContext.Response.StatusCode = StatusCodes.Status200OK;

                _logger.LogWarning("Json status code response ({Code}) [{ErrorsAsSuccess}]", Code, _settings.Value.JsonErrorsAsSuccess);

                context.Result = new JsonResult(_settings.Value.JsonResponseFormatter(this));
            }
            else
            {
                _logger.LogWarning("Html status code response ({Code})", Code);
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

    private static string GetErrorMessage(HttpContext context)
    {
        try
        {
            var exceptionFeature = context.Features.Get<IExceptionHandlerPathFeature>();
            var error = exceptionFeature?.Error;

            return error is FriendlyException fe ? fe.Message : null;
        }
        catch
        {
            // ignored
        }

        return null;
    }

    public override string Title => "Error";
}