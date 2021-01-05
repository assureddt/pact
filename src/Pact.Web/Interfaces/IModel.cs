using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pact.Web.Interfaces
{
    public interface IModel
    {
        ILogger Logger { get; }
        string Title { get; }
        IAntiforgery Xsrf { get; }

        AntiforgeryTokenSet GetAntiXsrfRequestToken(HttpContext context)
        {
            return Xsrf.GetAndStoreTokens(context);
        }
    }
}
