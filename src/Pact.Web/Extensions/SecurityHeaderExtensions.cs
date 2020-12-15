using System;
using Microsoft.AspNetCore.Builder;
using NetEscapades.AspNetCore.SecurityHeaders.Headers.ContentSecurityPolicy;

namespace Pact.Web.Extensions
{
    public static class SecurityHeaderExtensions
    {
        /// <summary>
        /// Configures a restricted feature policy for web apps that don't need any
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static FeaturePolicyBuilder AddDefaultFeaturePolicy(this FeaturePolicyBuilder builder)
        {
            builder.AddAccelerometer().None();
            builder.AddAmbientLightSensor().None();
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
            builder.AddSpeaker().None();
            builder.AddSyncXHR() .None();
            builder.AddUsb().None();
            builder.AddVR().None();

            return builder;
        }

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
                .AddFeaturePolicy(fp => fp.AddDefaultFeaturePolicy())
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

            app.UseSecurityHeaders(head);

            return app;
        }

        /// <summary>
        /// Adds CSP with:
        /// * Restrictive Feature Policy
        /// * Allowed Google services (Analytics, Fonts, Recaptcha)
        /// * No reporting
        /// * Everything else sensibly restrictive
        /// Should be good for most simple-but-secure sites
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCspWithPactDefaults(this IApplicationBuilder app)
        {
            app.UseCspWithFeaturePolicy(false, csp =>
            {
                csp.AddDefaultCsp(null);
                csp.AddFrameSource().Self().FromGoogleRecaptcha();
                csp.AddImgSrc().Self().Data().FromGoogleAnalytics();
                csp.AddConnectSrc().Self().FromGoogleAnalytics();
                csp.AddFontSrc().Self().Data().FromGoogleFonts();
                csp.AddStyleSrc().Self().FromGoogleFonts().UnsafeInline();
                csp.AddScriptSrc().Self().UnsafeEval().UnsafeInline().ReportSample().FromGoogleAnalytics().FromGoogleRecaptcha();
            });

            return app;
        }
    }
}
