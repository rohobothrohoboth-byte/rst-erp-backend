using Microsoft.Extensions.Configuration;

namespace Common;

internal static class GrpcTarget
{
    // Service-to-service gRPC is co-located (all services run on the same host in this
    // deployment). The configured ServiceUrls:* often use the machine's LAN IP or a
    // {ServiceHost} placeholder meant for the *frontend* to reach the gateway — dialing that
    // LAN IP for inter-service gRPC hits a binding/cert that isn't listening there and the TLS
    // connect times out (making recruitment slow / names blank).
    //
    // So connect over the loopback host (or an explicit ServiceUrls:GrpcHost override), keeping
    // the configured scheme + port. This matches the local Kestrel binding and the localhost dev
    // certificate, so the handshake succeeds immediately.
    public static string Resolve(IConfiguration config, string configuredUrl)
    {
        try
        {
            var uri = new Uri(configuredUrl);
            var host = config["ServiceUrls:GrpcHost"];
            if (string.IsNullOrWhiteSpace(host)) { host = "localhost"; }
            return $"{uri.Scheme}://{host}:{uri.Port}";
        }
        catch
        {
            return configuredUrl;
        }
    }
}
