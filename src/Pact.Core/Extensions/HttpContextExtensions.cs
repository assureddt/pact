using System.Net;
using Microsoft.AspNetCore.Http;

namespace Pact.Core.Extensions;

public static class HttpContextExtensions
{
    /// <summary>
    /// Compares the remote IP of the context to a provided set of "Safe" IPs
    /// </summary>
    /// <param name="context"></param>
    /// <param name="safeIps"></param>
    /// <returns></returns>
    public static bool IsSafe(this HttpContext context, params string[] safeIps)
    {
        if (safeIps == null)
            return true;

        var remoteIp = context.Connection.RemoteIpAddress;

        var badIp = true;
        foreach (var address in safeIps)
        {
            if (remoteIp.IsIPv4MappedToIPv6)
            {
                remoteIp = remoteIp.MapToIPv4();
            }

            var testIp = IPAddress.Parse(address);
            if (!testIp.Equals(remoteIp)) continue;

            badIp = false;
            break;
        }

        return !badIp;
    }

    /// <summary>
    /// Compares remote and local endpoints of the connection to determine locality
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    public static bool IsLocal(this HttpContext ctx)
    {
        return ctx.Connection.IsLocal();
    }
}