using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Builder;

namespace Pact.Web.Options
{
    /// <summary>
    /// Extends ForwardedHeadersOptions to allow us to overwrite networks & proxies easily
    /// </summary>
    public class ExtendedForwardedHeadersOptions : ForwardedHeadersOptions
    {
        public new IList<IPAddress> KnownProxies { get; set; } = new List<IPAddress>();

        public new IList<Microsoft.AspNetCore.HttpOverrides.IPNetwork> KnownNetworks { get; set; } = new List<Microsoft.AspNetCore.HttpOverrides.IPNetwork>();

        /// <summary>
        /// Apply all values to the provided ForwardedHeadersOptions object (will replace existing KnownProxies & KnownNetworks)
        /// </summary>
        /// <param name="fho"></param>
        public void ApplyTo(ForwardedHeadersOptions fho)
        {
            fho.KnownNetworks.Clear();
            fho.KnownProxies.Clear();

            foreach (var nw in KnownNetworks)
            {
                fho.KnownNetworks.Add(nw);
            }

            foreach (var prox in KnownProxies)
            {
                fho.KnownProxies.Add(prox);
            }

            fho.AllowedHosts = AllowedHosts;
            fho.ForwardLimit = ForwardLimit;
            fho.ForwardedForHeaderName = ForwardedForHeaderName;
            fho.ForwardedHeaders = ForwardedHeaders;
            fho.ForwardedHostHeaderName = ForwardedHostHeaderName;
            fho.ForwardedProtoHeaderName = ForwardedProtoHeaderName;
            fho.OriginalForHeaderName = OriginalForHeaderName;
            fho.OriginalHostHeaderName = OriginalHostHeaderName;
            fho.OriginalProtoHeaderName = OriginalProtoHeaderName;
            fho.RequireHeaderSymmetry = RequireHeaderSymmetry;
        }
    }
}
