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
        /// <summary>
        /// A logger instance
        /// </summary>
        protected ILogger Logger { get; }

        ///<inheritdoc/>
        public IAntiforgery Xsrf { get; }

        ///<inheritdoc/>
        public string Title { get; protected set; }

        protected PageModelBase(ILogger logger, IAntiforgery xsrf)
        {
            Logger = logger;
            Xsrf = xsrf;
        }

        ///<inheritdoc/>
        public AntiforgeryTokenSet GetAntiXsrfRequestToken(HttpContext context) => ((IModel)this).GetAntiXsrfRequestToken(context);

        /// <summary>
        /// An error message temp-data container for ease of display in the layout following a redirect
        /// </summary>
        [TempData]
        public string Error { get; set; }

        /// <summary>
        /// An informative message temp-data container for ease of display in the layout following a redirect
        /// </summary>
        [TempData]
        public string Info { get; set; }

        /// <summary>
        /// A success message temp-data container for ease of display in the layout following a redirect
        /// </summary>
        [TempData]
        public string Success { get; set; }

        /// <summary>
        /// A warning message temp-data container for ease of display in the layout following a redirect
        /// </summary>
        [TempData]
        public string Warning { get; set; }

        /// <summary>
        /// Adds a simple Forbid response option to the default Razor Pages model
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected ForbidResult Forbid(string message)
        {
            Error = message;

            return Forbid();
        }
    }
}
