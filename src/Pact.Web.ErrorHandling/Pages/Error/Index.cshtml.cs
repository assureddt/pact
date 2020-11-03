using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Pact.Web.ErrorHandling.Extensions;
using Pact.Web.ErrorHandling.Interfaces;
using Pact.Web.ErrorHandling.Models;

namespace Pact.Web.ErrorHandling.Pages.Error
{
    [IgnoreAntiforgeryToken]
    [AllowAnonymous]
    public class IndexModel : PageModelBase
    {
        public IndexModel(ILogger<IndexModel> logger, IPageModelService service)
        : base(logger, service)
        {
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
    }
}
