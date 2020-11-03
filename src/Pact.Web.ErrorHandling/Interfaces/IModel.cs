using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

namespace Pact.Web.ErrorHandling.Interfaces
{
    public interface IModel
    {
        string Title { get; }
        IAntiforgery Xsrf { get; }
        AntiforgeryTokenSet GetAntiXsrfRequestToken(HttpContext context);
    }
}
