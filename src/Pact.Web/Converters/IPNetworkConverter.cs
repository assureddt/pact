using System.ComponentModel;

namespace Pact.Web.Converters;

/// <summary>
/// Provides a direct means of converting a string to an IPNetwork
/// </summary>
/// <remarks>
/// Primary intention here is to enable us to hold IPNetwork in appsettings.json and have them deserialized without effort
/// </remarks>
public class IPNetworkConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    {
        if (value is string s)
        {
            // NOTE: dependency on IPNetwork2 here is due to lack of support for parsing CIDR notation in-framework
            // see: https://github.com/dotnet/aspnetcore/issues/8606
            // the parsing is quite involved and the library is small and exclusively focused on this purpose
            var ipn = System.Net.IPNetwork.Parse(s);
            return new Microsoft.AspNetCore.HttpOverrides.IPNetwork(ipn.Network, ipn.Cidr);
        }
        return base.ConvertFrom(context, culture, value);
    }

    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType != typeof(string)) return base.ConvertTo(context, culture, value, destinationType);

        var val = (Microsoft.AspNetCore.HttpOverrides.IPNetwork)value;
        return $"{val.Prefix}/{val.PrefixLength}";
    }

    /// <summary>
    /// Register the IPNetworkConverter to be used for future conversions
    /// </summary>
    public static void Register()
    {
        TypeDescriptor.AddAttributes(typeof(Microsoft.AspNetCore.HttpOverrides.IPNetwork), new TypeConverterAttribute(typeof(IPNetworkConverter)));
    }
}