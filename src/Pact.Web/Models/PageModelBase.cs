using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pact.Web.Interfaces;

namespace Pact.Web.Models
{
    /// <summary>
    /// Provides a few standard things
    /// </summary>
    public abstract class PageModelBase : PageModel, ITitleModel, IAlertsModel
    {
        ///<inheritdoc/>
        public virtual string Title { get; protected set; }

        /// <summary>
        /// An error message, held in TempData
        /// </summary>
        [TempData]
        public string Error { get; set; }

        /// <summary>
        /// An informative message, held in TempData
        /// </summary>
        [TempData]
        public string Info { get; set; }

        /// <summary>
        /// A success message, held in TempData
        /// </summary>
        [TempData]
        public string Success { get; set; }

        /// <summary>
        /// An warning message, held in TempData
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
