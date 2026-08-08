// Shared/Helpers/Extensions/HttpClientExtensions.cs

using System.Net.Http;
using System.Net.Http.Headers;

namespace Shared.Helpers.Extensions;

public static class HttpClientExtensions
{
    /// <summary>
    /// Adds API key authentication headers to the HttpClient
    /// </summary>
    public static void AddApiKeyAuthentication(this HttpClient client, string apiKey, string serviceName = "FinanceService")
    {
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("X-API-Key", apiKey);
        client.DefaultRequestHeaders.Add("X-Service-Name", serviceName);
    }
}