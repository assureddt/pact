using System;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace Pact.Core.Extensions
{
    public static class HttpContextExtensions
    {
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

        private const string NullIPv6 = "::1";

        public static bool IsLocal(this ConnectionInfo conn)
        {
            if (!conn.RemoteIpAddress.IsSet())
                return true;

            // we have a remote address set up
            // is local is same as remote, then we are local
            // else we are remote if the remote IP address is not a loopback address
            return conn.LocalIpAddress.IsSet() ? conn.RemoteIpAddress.Equals(conn.LocalIpAddress) : conn.RemoteIpAddress.IsLoopback();
        }

        public static bool IsLocal(this HttpContext ctx)
        {
            return ctx.Connection.IsLocal();
        }

        public static bool IsLocal(this HttpRequest req)
        {
            return req.HttpContext.IsLocal();
        }

        public static bool IsSet(this IPAddress address)
        {
            return address != null && address.ToString() != NullIPv6;
        }

        public static bool IsLoopback(this IPAddress address)
        {
            return IPAddress.IsLoopback(address);
        }

        /// <summary>
        /// Determines whether the specified HTTP request is an AJAX request.
        /// </summary>
        /// 
        /// <returns>
        /// true if the specified HTTP request is an AJAX request; otherwise, false.
        /// </returns>
        /// <param name="request">The HTTP request.</param><exception cref="T:System.ArgumentNullException">The <paramref name="request"/> parameter is null (Nothing in Visual Basic).</exception>
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Headers != null)
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";

            return false;
        }
    }
}
