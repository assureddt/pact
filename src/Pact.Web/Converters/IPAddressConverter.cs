using System.ComponentModel;
using System.Net;

namespace Pact.Web.Converters;

/// <summary>
/// Provides a direct means of converting a string to an IPAddress
/// </summary>
/// <remarks>
/// Primary intention here is to enable us to hold IPAddresses in appsettings.json and have them deserialized without effort
/// </remarks>
public class IPAddressConverter : TypeConverter
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
            return IPAddress.Parse(s);
        }
        return base.ConvertFrom(context, culture, value);
    }

    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    {
        return destinationType == typeof(string) ? ((IPAddress)value).ToString() : base.ConvertTo(context, culture, value, destinationType);
    }

    /// <summary>
    /// Register the IPAddressConverter to be used for future conversions
    /// </summary>
    public static void Register()
    {
        TypeDescriptor.AddAttributes(typeof(IPAddress), new TypeConverterAttribute(typeof(IPAddressConverter)));
    }
}