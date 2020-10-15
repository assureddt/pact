using System;
using Microsoft.AspNetCore.Builder;
using NetEscapades.AspNetCore.SecurityHeaders.Headers.ContentSecurityPolicy;

namespace Pact.Web.Extensions
{
    public static class SecurityHeaderExtensions
    {
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

        public static T FromAnalytics<T>(this T builder) where T : CspDirectiveBuilder
        {
            return builder.From("https://www.google-analytics.com/").From("https://www.googletagmanager.com/");
        }

        public static T FromRecaptcha<T>(this T builder) where T : CspDirectiveBuilder
        {
            return builder.From("https://www.google.com/").From("https://www.gstatic.com/");
        }

        public static CspBuilder AddDefaultCsp(this CspBuilder csp, string reportUri)
        {
            if (!string.IsNullOrWhiteSpace(reportUri))
            {
                csp.AddReportUri().To(reportUri);
            }

            csp.AddBlockAllMixedContent();
            csp.AddDefaultSrc().Self();
            csp.AddFontSrc().Self().Data().From("https://fonts.gstatic.com");
            csp.AddStyleSrc().Self().From("https://fonts.googleapis.com").UnsafeInline();

            return csp;
        }

        public static IApplicationBuilder Use360CspWithFeaturePolicy(this IApplicationBuilder app, bool reportOnly, Action<CspBuilder> cspBuilder)
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
    }
}
