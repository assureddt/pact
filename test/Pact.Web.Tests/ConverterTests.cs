using System;
using System.ComponentModel;
using System.Net;
using Pact.Web.Converters;
using Shouldly;
using Xunit;

namespace Pact.Web.Tests;

public class ConverterTests
{
    [Fact]
    public void IPAddress_NoReg_Null()
    {
        // arrange
        const string ip = "127.0.0.1";

        // act & assert
        Assert.Throws<NotSupportedException>(() => (IPAddress)TypeDescriptor.GetConverter(typeof(IPAddress)).ConvertFrom(ip));
    }

    [Fact]
    public void IPAddress_Registered_AsExpected()
    {
        // arrange
        const string ip = "127.0.0.1";

        // act
        // NOTE: can't use register methods here as it's app-lifetime-scoped & can't be removed
        var ipAddress = (IPAddress)new IPAddressConverter().ConvertFrom(ip);

        // assert
        ipAddress.ShouldNotBeNull();
        ipAddress.ToString().ShouldBe(ip);
    }

    [Fact]
    public void IPNetwork_NoReg_Null()
    {
        // arrange
        const string ipn = "123.45.222.19/24";

        // act & assert
        Assert.Throws<NotSupportedException>(() =>
            (Microsoft.AspNetCore.HttpOverrides.IPNetwork) TypeDescriptor
                .GetConverter(typeof(Microsoft.AspNetCore.HttpOverrides.IPNetwork)).ConvertFrom(ipn));
    }

    [Fact]
    public void IPNetwork_Registered_AsExpected()
    {
        // arrange
        const string ipn = "123.45.222.19/24";

        // act
        // NOTE: can't use register methods here as it's app-lifetime-scoped & can't be removed
        var ipNetwork = (Microsoft.AspNetCore.HttpOverrides.IPNetwork)new IPNetworkConverter().ConvertFrom(ipn);

        // assert
        ipNetwork.ShouldNotBeNull();
        ipNetwork.Prefix.ToString().ShouldBe("123.45.222.0");
        ipNetwork.PrefixLength.ShouldBe(24);
    }
}