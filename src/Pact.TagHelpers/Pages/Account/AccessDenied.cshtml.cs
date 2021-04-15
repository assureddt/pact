using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pact.Web.Models;

namespace Pact.TagHelpers.Pages.Account
{
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public class AccessDeniedModel : PageModelBase
    {
        public override string Title => "Access Denied";
    }
}
