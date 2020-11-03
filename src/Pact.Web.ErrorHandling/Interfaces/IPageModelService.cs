using Microsoft.AspNetCore.Antiforgery;

namespace Pact.Web.ErrorHandling.Interfaces
{
    public interface IPageModelService
    {
        IAntiforgery Xsrf { get; }
    }
}
