using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Pact.Web.Interfaces;

namespace Pact.Web.Models
{
    /// <summary>
    /// Intended to form the base page model if you're opting into the error handling, but you can just implement IModel instead
    /// Adds some other useful standard functionality to the page model (AntiForgery, Page title, TempData-based alerts and a Forbid response)
    /// </summary>
    public abstract class PageModelBase : PageModel, IModel
    {
        public ILogger Logger { get; }
        public IAntiforgery Xsrf { get; }

        public string Title { get; protected set; }

        protected PageModelBase(ILogger logger, IAntiforgery xsrf)
        {
            Logger = logger;
            Xsrf = xsrf;
        }

        public AntiforgeryTokenSet GetAntiXsrfRequestToken(HttpContext context) => ((IModel)this).GetAntiXsrfRequestToken(context);

        [TempData]
        public string Error { get; set; }

        [TempData]
        public string Info { get; set; }

        [TempData]
        public string Success { get; set; }

        [TempData]
        public string Warning { get; set; }

        protected ForbidResult Forbid(string message)
        {
            Error = message;

            return Forbid();
        }
    }
}
