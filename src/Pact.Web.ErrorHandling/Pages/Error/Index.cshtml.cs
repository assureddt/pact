using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Pact.Web.ErrorHandling.Extensions;
using Pact.Web.Interfaces;

namespace Pact.Web.ErrorHandling.Pages.Error
{
    [IgnoreAntiforgeryToken]
    [AllowAnonymous]
    public class IndexModel : PageModel, IModel
    {
        public IndexModel(ILogger<IndexModel> logger, IAntiforgery xsrf)
        {
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
                Code = context.ManageError(Logger, out var details);
                Details = details;
            }
            catch
            {
                // ignored
            }

            await base.OnPageHandlerExecutionAsync(context, next);
        }

        public ILogger Logger { get; }
        public string Title { get; }
        public IAntiforgery Xsrf { get; }
    }
}
