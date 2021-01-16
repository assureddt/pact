using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

namespace Pact.Web.Interfaces
{
    /// <summary>
    /// Provides a basis of what is commonly used in a page model
    /// This forms the expectations of the model for the built-in Pact error handling module
    /// (i.e. the resulting error page will implement this, so the layout can rely on these features being available
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// A page title for presentation
        /// </summary>
        string Title { get; }

        /// <summary>
        /// An anti-forgery token service for the layout to ensure the relevant tokens are always present
        /// </summary>
        IAntiforgery Xsrf { get; }

        /// <summary>
        /// Provides a default implementation of how the anti-forgery token service is used to retrieve the tokens
        /// Unlikely to need a custom implementation
        /// </summary>
        /// <param name="context">The Http Context of the current request</param>
        /// <returns>An Anti-forgery token set</returns>
        AntiforgeryTokenSet GetAntiXsrfRequestToken(HttpContext context)
        {
            return Xsrf.GetAndStoreTokens(context);
        }
    }
}
