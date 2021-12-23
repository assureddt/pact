using System.Net;

namespace Pact.Core.Extensions;

public static class IpAddressExtensions
{
    private const string NullIPv6 = "::1";

    public static bool IsSet(this IPAddress address)
    {
        return address != null && address.ToString() != NullIPv6;
    }

    public static bool IsLoopback(this IPAddress address)
    {
        return IPAddress.IsLoopback(address);
    }
}