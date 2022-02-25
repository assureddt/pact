using Microsoft.AspNetCore.Builder;
using NetEscapades.AspNetCore.SecurityHeaders.Headers.ContentSecurityPolicy;
using NetEscapades.AspNetCore.SecurityHeaders.Headers.PermissionsPolicy;

namespace Pact.Web.Extensions;

public static class SecurityHeaderExtensions
{
    /// <summary>
    /// Configures a restricted permissions policy for web apps that don't need any
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static PermissionsPolicyBuilder AddDefaultPermissionsPolicy(this PermissionsPolicyBuilder builder)
    {
        builder.AddAccelerometer().None();
        builder.AddAutoplay().None();
        builder.AddCamera().None();
        builder.AddEncryptedMedia().None();
        builder.AddFullscreen().None();
        builder.AddGeolocation().None();
        builder.AddGyroscope().None();
        builder.AddMagnetometer().None();
        builder.AddMicrophone().None();
        builder.AddMidi().None();
        builder.AddPayment().None();
        builder.AddPictureInPicture().None();
        builder.AddSyncXHR().None();
        builder.AddUsb().None();
        builder.AddXR().None();

        return builder;
    }

    /// <summary>
    /// This replaces the deprecated VR Feature
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static CustomPermissionsPolicyDirectiveBuilder AddXR(this PermissionsPolicyBuilder builder) => builder.AddCustomFeature("xr-spatial-tracking");

    /// <summary>
    /// Adds default CSP From urls for Google Analytics
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static T FromGoogleAnalytics<T>(this T builder) where T : CspDirectiveBuilder
    {
        return builder.From("https://www.google-analytics.com/").From("https://www.googletagmanager.com/");
    }

    /// <summary>
    /// Adds default CSP From urls for Google Recaptcha
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static T FromGoogleRecaptcha<T>(this T builder) where T : CspDirectiveBuilder
    {
        return builder.From("https://www.google.com/").From("https://www.gstatic.com/");
    }

    /// <summary>
    /// Adds default CSP From urls for Google Fonts
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static T FromGoogleFonts<T>(this T builder) where T : CspDirectiveBuilder
    {
        return builder.From("https://fonts.gstatic.com").From("https://fonts.googleapis.com");
    }

    /// <summary>
    /// Adds just the basic Csp
    /// </summary>
    /// <param name="csp"></param>
    /// <param name="reportUri"></param>
    /// <returns></returns>
    public static CspBuilder AddDefaultCsp(this CspBuilder csp, string reportUri)
    {
        if (!string.IsNullOrWhiteSpace(reportUri))
        {
            csp.AddReportUri().To(reportUri);
        }

        csp.AddBlockAllMixedContent();
        csp.AddDefaultSrc().Self();
        csp.AddFontSrc().Self().Data();
        csp.AddStyleSrc().Self().UnsafeInline();

        return csp;
    }

    /// <summary>
    /// Adds default CSP with Restrictive Feature Policy but control over the specifics of CSP
    /// </summary>
    /// <param name="app"></param>
    /// <param name="reportOnly"></param>
    /// <param name="cspBuilder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseCspWithFeaturePolicy(this IApplicationBuilder app, bool reportOnly, Action<CspBuilder> cspBuilder)
    {
        var head = new HeaderPolicyCollection()
            .AddFrameOptionsSameOrigin()
            .AddXssProtectionBlock()
            .AddContentTypeOptionsNoSniff()
            .AddStrictTransportSecurityMaxAgeIncludeSubDomains()
            .AddReferrerPolicyStrictOriginWhenCrossOrigin()
            .AddPermissionsPolicy(fp => fp.AddDefaultPermissionsPolicy())
            .AddCustomHeader("X-Permitted-Cross-Domain-Policies", "none")
            .RemoveServerHeader()
            .AddContentSecurityPolicy(builder =>
            {
                builder.AddObjectSrc().None();
                builder.AddFormAction().Self();
                builder.AddFrameAncestors().None();
            });

        if (reportOnly)
            head.AddContentSecurityPolicyReportOnly(cspBuilder);
        else
            head.AddContentSecurityPolicy(cspBuilder);

        return app.UseSecurityHeaders(head);
    }

    /// <summary>
    /// Adds CSP with:
    /// * Restrictive Feature Policy
    /// * Allowed Google services (Analytics, Fonts, Recaptcha)
    /// * No reporting
    /// * Script Nonces
    /// * Everything else sensibly restrictive
    /// Should be good for most simple-but-secure sites
    /// </summary>
    /// <param name="app"></param>
    /// <param name="reportOnly">Indicates that CSP should only report errors, not enforce the restrictions</param>
    /// <param name="reportUri">Provide a https://report-uri.com/ URL to receive CSP reports</param>
    /// <returns></returns>
    public static IApplicationBuilder UseCspWithPactDefaults(this IApplicationBuilder app, bool reportOnly = false, string reportUri = null)
    {
        return app.UseCspWithFeaturePolicy(reportOnly, csp =>
        {
            csp.AddDefaultCsp(reportUri);
            csp.AddFrameSrc().Self().FromGoogleRecaptcha();
            csp.AddImgSrc().Self().Data().FromGoogleAnalytics();
            csp.AddConnectSrc().Self().FromGoogleAnalytics();
            csp.AddFontSrc().Self().Data().FromGoogleFonts();
            csp.AddStyleSrc().Self().FromGoogleFonts().UnsafeInline();
            csp.AddScriptSrc().Self().UnsafeEval().UnsafeInline().WithNonce().ReportSample().FromGoogleAnalytics().FromGoogleRecaptcha();
        });
    }

    /// <summary>
    /// Adds CSP with:
    /// * Restrictive Feature Policy
    /// * No google resource allowance
    /// * No reporting
    /// * Script Nonces
    /// * No Frames
    /// * Everything else sensibly restrictive
    /// Should be good for most simple-but-secure sites (that don't use google resources)
    /// </summary>
    /// <param name="app"></param>
    /// <param name="reportOnly">Indicates that CSP should only report errors, not enforce the restrictions</param>
    /// <param name="reportUri">Provide a https://report-uri.com/ URL to receive CSP reports</param>
    /// <returns></returns>
    public static IApplicationBuilder UseCspWithPactDefaultsNoGoogle(this IApplicationBuilder app, bool reportOnly = false, string reportUri = null)
    {
        return app.UseCspWithFeaturePolicy(reportOnly, csp =>
        {
            csp.AddDefaultCsp(reportUri);
            csp.AddFrameSrc().None();
            csp.AddImgSrc().Self().Data();
            csp.AddConnectSrc().Self();
            csp.AddFontSrc().Self().Data();
            csp.AddStyleSrc().Self().UnsafeInline();
            csp.AddScriptSrc().Self().UnsafeEval().UnsafeInline().WithNonce().ReportSample();
        });
    }
}