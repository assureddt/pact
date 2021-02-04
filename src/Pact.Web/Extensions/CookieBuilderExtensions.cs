using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Pact.Web.Extensions
{
    public static class CookieBuilderExtensions
    {
        public static string ApplicationName { get; set; } = string.Empty;
        public static string Path { get; set; }

        public static string GetCookieName(CookieType type) => $"{ApplicationName}.{type}";

        private static SameSiteMode GetSameSiteMode(CookieType type) =>
            type switch
            {
                CookieType.Application => SameSiteMode.Lax,
                CookieType.OpenId => SameSiteMode.None,
                CookieType.External => SameSiteMode.None,
                CookieType.CookieConsent => SameSiteMode.None,
                CookieType.Nonce => SameSiteMode.None,
                CookieType.Correlation => SameSiteMode.None,
                _ => SameSiteMode.Strict
            };

        public static CookieBuilder SetDefaults(this CookieBuilder options, CookieType type)
        {
            options.Name = GetCookieName(type);
            options.Path = Path;

            options.IsEssential = true;
            options.SecurePolicy = CookieSecurePolicy.Always;
            options.HttpOnly = !NonHttpOnly.Contains(type);
            options.SameSite = GetSameSiteMode(type);

            return options;
        }

        public static CookieBuilder SetDefaults(this CookieBuilder options)
        {
            options.Path = Path;

            options.IsEssential = true;
            options.SecurePolicy = CookieSecurePolicy.Always;
            options.HttpOnly = true;
            options.SameSite = SameSiteMode.None;

            return options;
        }

        private static readonly CookieType [] NonHttpOnly = { CookieType.CookieConsent };
    }

    public enum CookieType
    {
        AntiForgery,
        Application,
        CookieConsent,
        External,
        Session,
        TwoFactorRememberMe,
        TwoFactorId,
        OpenId,
        Correlation,
        Nonce
    }
}
