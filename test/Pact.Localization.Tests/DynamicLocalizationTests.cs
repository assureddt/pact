using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using Xunit;

namespace Pact.Localization.Tests
{
    public class DynamicLocalizationTests
    {
        [SkippableFact]
        public async Task Minimal_Defaults_enGB()
        {
            Skip.If(RuntimeInformation.IsOSPlatform(OSPlatform.Linux));

            // arrange
            var resolver = new Mock<ISupportedCulturesResolver>();
            var options = new OptionsWrapper<RequestLocalizationOptions>(new RequestLocalizationOptions());
            var mw = new DynamicLocalizationMiddleware(innerHttpContext =>
            {
                innerHttpContext.Response.WriteAsync(CultureInfo.CurrentCulture.ToString());
                return Task.CompletedTask;
            }, options);

            var services = new ServiceCollection();
            services.AddSingleton(resolver.Object);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.RequestServices = services.BuildServiceProvider();

            // act
            await mw.InvokeAsync(context);

            // assert
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
            body.ShouldBe("en-GB");
        }

        [SkippableFact]
        public async Task Extended_Match_frFR()
        {
            Skip.If(RuntimeInformation.IsOSPlatform(OSPlatform.Linux));

            // arrange
            var resolver = new Mock<ISupportedCulturesResolver>();
            resolver.Setup(m => m.GetSupportedCulturesAsync(It.IsAny<HttpContext>()))
                .ReturnsAsync(new List<CultureInfo>
                {
                    new CultureInfo("en-GB"),
                    new CultureInfo("fr-FR"),
                    new CultureInfo("de-DE")
                });

            var options = new OptionsWrapper<RequestLocalizationOptions>(new RequestLocalizationOptions());
            var mw = new DynamicLocalizationMiddleware(innerHttpContext =>
            {
                innerHttpContext.Response.WriteAsync(CultureInfo.CurrentCulture.ToString());
                return Task.CompletedTask;
            }, options);

            var services = new ServiceCollection();
            services.AddSingleton(resolver.Object);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.RequestServices = services.BuildServiceProvider();
            context.Request.Headers["Accept-Language"] = "fr-FR";

            // act
            await mw.InvokeAsync(context);

            // assert
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
            body.ShouldBe("fr-FR");
        }

        [SkippableFact]
        public async Task Extended_NoMatch_enGB()
        {
            Skip.If(RuntimeInformation.IsOSPlatform(OSPlatform.Linux));

            // arrange
            var resolver = new Mock<ISupportedCulturesResolver>();
            resolver.Setup(m => m.GetSupportedCulturesAsync(It.IsAny<HttpContext>()))
                .ReturnsAsync(new List<CultureInfo>
                {
                    new CultureInfo("en-GB"),
                    new CultureInfo("fr-FR"),
                    new CultureInfo("de-DE")
                });

            var options = new OptionsWrapper<RequestLocalizationOptions>(new RequestLocalizationOptions());
            var mw = new DynamicLocalizationMiddleware(innerHttpContext =>
            {
                innerHttpContext.Response.WriteAsync(CultureInfo.CurrentCulture.ToString());
                return Task.CompletedTask;
            }, options);

            var services = new ServiceCollection();
            services.AddSingleton(resolver.Object);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.RequestServices = services.BuildServiceProvider();
            context.Request.Headers["Accept-Language"] = "es-ES";

            // act
            await mw.InvokeAsync(context);

            // assert
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
            body.ShouldBe("en-GB");
        }
        
        [SkippableFact]
        public async Task Extended_Partial_frCA_NoFallback_enGB()
        {
            Skip.If(RuntimeInformation.IsOSPlatform(OSPlatform.Linux));

            // arrange
            var resolver = new Mock<ISupportedCulturesResolver>();
            resolver.Setup(m => m.GetSupportedCulturesAsync(It.IsAny<HttpContext>()))
                .ReturnsAsync(new List<CultureInfo>
                {
                    new CultureInfo("en-GB"),
                    new CultureInfo("fr"),
                    new CultureInfo("fr-FR"),
                    new CultureInfo("de-DE")
                });

            var options = new OptionsWrapper<RequestLocalizationOptions>(new RequestLocalizationOptions
            {
                FallBackToParentCultures = false,
                FallBackToParentUICultures = false
            });
            var mw = new DynamicLocalizationMiddleware(innerHttpContext =>
            {
                innerHttpContext.Response.WriteAsync(CultureInfo.CurrentCulture.ToString());
                return Task.CompletedTask;
            }, options);

            var services = new ServiceCollection();
            services.AddSingleton(resolver.Object);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.RequestServices = services.BuildServiceProvider();
            context.Request.Headers["Accept-Language"] = "fr-CA";

            // act
            await mw.InvokeAsync(context);

            // assert
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
            body.ShouldBe("en-GB");
        }

        [SkippableFact]
        public async Task Extended_Partial_frCA_WithFallback_frFR()
        {
            Skip.If(RuntimeInformation.IsOSPlatform(OSPlatform.Linux));

            // arrange
            var resolver = new Mock<ISupportedCulturesResolver>();
            resolver.Setup(m => m.GetSupportedCulturesAsync(It.IsAny<HttpContext>()))
                .ReturnsAsync(new List<CultureInfo>
                {
                    new CultureInfo("en-GB"),
                    new CultureInfo("fr"),
                    new CultureInfo("fr-FR"),
                    new CultureInfo("de-DE")
                });

            var options = new OptionsWrapper<RequestLocalizationOptions>(new RequestLocalizationOptions());
            var mw = new DynamicLocalizationMiddleware(innerHttpContext =>
            {
                innerHttpContext.Response.WriteAsync(CultureInfo.CurrentCulture.ToString());
                return Task.CompletedTask;
            }, options);

            var services = new ServiceCollection();
            services.AddSingleton(resolver.Object);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.RequestServices = services.BuildServiceProvider();
            context.Request.Headers["Accept-Language"] = "fr-CA";

            // act
            await mw.InvokeAsync(context);

            // assert
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
            body.ShouldBe("fr");
        }
    }
}
