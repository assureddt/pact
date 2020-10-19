using Microsoft.AspNetCore.Http;

namespace Pact.Core.Extensions
{
    public static class ConnectionInfoExtensions
    {
        public static bool IsLocal(this ConnectionInfo conn)
        {
            if (!conn.RemoteIpAddress.IsSet())
                return true;

            // we have a remote address set up
            // is local is same as remote, then we are local
            // else we are remote if the remote IP address is not a loopback address
            return conn.LocalIpAddress.IsSet() ? conn.RemoteIpAddress.Equals(conn.LocalIpAddress) : conn.RemoteIpAddress.IsLoopback();
        }
    }
}
