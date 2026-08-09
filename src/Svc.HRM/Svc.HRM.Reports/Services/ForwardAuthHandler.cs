namespace Svc.HRM.Reports.Services;

/// <summary>
/// Forwards the inbound Authorization header to upstream HR APIs.
/// </summary>
public sealed class ForwardAuthHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var auth = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrWhiteSpace(auth) && !request.Headers.Contains("Authorization"))
            request.Headers.TryAddWithoutValidation("Authorization", auth);

        return base.SendAsync(request, cancellationToken);
    }
}
