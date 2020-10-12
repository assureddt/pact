using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Pact.Localization
{
    public interface ISupportedCulturesResolver
    {
        Task<IList<CultureInfo>> GetSupportedCulturesAsync(HttpContext context);
    }
}
