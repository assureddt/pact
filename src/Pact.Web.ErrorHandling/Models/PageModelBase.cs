using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Pact.Web.ErrorHandling.Interfaces;

namespace Pact.Web.ErrorHandling.Models
{
    /// <summary>
    /// Intended to form the base page model if you're opting into the error handling
    /// Adds some other useful standard functionality to the page model (AntiForgery, Page title, TempData-based alerts and a Forbid response)
    /// </summary>
    public abstract class PageModelBase : PageModel, IModel
    {
        protected ILogger Logger { get; }
        protected IPageModelService Service { get; }

        public IAntiforgery Xsrf => Service.Xsrf;
        public string Title { get; protected set; }

        protected PageModelBase(ILogger logger, IPageModelService service)
        {
            Logger = logger;
            Service = service;
        }

        public AntiforgeryTokenSet GetAntiXsrfRequestToken(HttpContext context)
        {
            return Xsrf.GetAndStoreTokens(context);
        }

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
