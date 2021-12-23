using System.IO;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Pact.Core.Extensions;
using Pact.Web.Converters;
using Pact.Web.Options;
using Shouldly;
using Xunit;

namespace Pact.Web.Isolated.Tests;

public class DeserializationTests
{
    [Fact]
    public void Config_DeserializeForwardedHeadersOptions_OK()
    {
        // arrange
        var configJson = new
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
            KnownNetworks = new[] {"123.45.222.19/24"},
            KnownProxies = new[] {"48.95.73.145"}
        }.ToJson();

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(configJson));

        var config = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();

        // act
        IPAddressConverter.Register();
        IPNetworkConverter.Register();

        var opts = new ForwardedHeadersOptions();
        var ext = config.Get<ExtendedForwardedHeadersOptions>();
        ext.ApplyTo(opts);

        // assert
        var nakedOpts = new ForwardedHeadersOptions();
        opts.ShouldNotBeNull();
        opts.KnownNetworks.ShouldNotBeNull();
        opts.KnownNetworks.Count.ShouldBe(1);
        opts.KnownNetworks[0].Prefix.ToString().ShouldBe("123.45.222.0");
        opts.KnownNetworks[0].PrefixLength.ShouldBe(24);
        opts.KnownProxies.ShouldNotBeNull();
        opts.KnownProxies.Count.ShouldBe(1);
        opts.KnownProxies[0].ToString().ShouldBe("48.95.73.145");

        opts.AllowedHosts.ShouldBe(nakedOpts.AllowedHosts);
        opts.ForwardLimit.ShouldBe(nakedOpts.ForwardLimit);
        opts.ForwardedForHeaderName.ShouldBe(nakedOpts.ForwardedForHeaderName);
        opts.ForwardedHeaders.ShouldBe(ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto);
        opts.ForwardedHostHeaderName.ShouldBe(nakedOpts.ForwardedHostHeaderName);
        opts.ForwardedProtoHeaderName.ShouldBe(nakedOpts.ForwardedProtoHeaderName);
        opts.OriginalForHeaderName.ShouldBe(nakedOpts.OriginalForHeaderName);
        opts.OriginalHostHeaderName.ShouldBe(nakedOpts.OriginalHostHeaderName);
        opts.OriginalProtoHeaderName.ShouldBe(nakedOpts.OriginalProtoHeaderName);
        opts.RequireHeaderSymmetry.ShouldBe(nakedOpts.RequireHeaderSymmetry);
    }
}