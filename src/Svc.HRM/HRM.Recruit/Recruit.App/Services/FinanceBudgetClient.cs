using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Recruit.App.Services;

public class BudgetAvailability
{
    public Guid BudgetId { get; set; }
    public string BudgetName { get; set; } = "";
    public decimal Allocated { get; set; }
    public decimal Spent { get; set; }
    public decimal Committed { get; set; }
    public decimal Available { get; set; }
    public decimal Requested { get; set; }
    public bool Ok { get; set; }
    public string Message { get; set; } = "";
}

public interface IFinanceBudgetClient
{
    Task<BudgetAvailability> CheckAsync(Guid budgetId, decimal amount, CancellationToken ct = default);
    Task<BudgetAvailability> ReserveAsync(Guid budgetId, Guid referenceId, string referenceType, decimal amount, string? note, CancellationToken ct = default);
    Task ReleaseAsync(Guid referenceId, string referenceType, CancellationToken ct = default);
    Task ConsumeAsync(Guid referenceId, string referenceType, decimal? amount, CancellationToken ct = default);
}

// Talks to Finance's internal BudgetCheck endpoints over a bounded, cert-tolerant HttpClient
// that does NOT go through the resilience handler (so a slow/unreachable Finance fails fast
// instead of retry-storming). Uses the loopback host since services are co-located.
public class FinanceBudgetClient : IFinanceBudgetClient
{
    private readonly HttpClient _http;
    private readonly ILogger<FinanceBudgetClient>? _logger;

    public FinanceBudgetClient(IConfiguration config, ILogger<FinanceBudgetClient>? logger = null)
    {
        _logger = logger;
        var configured = config["ServiceUrls:FinanceApi"] ?? config["FinanceUrl"] ?? "https://localhost:7008";
        var baseUrl = ToLoopback(config, configured);

        var handler = new SocketsHttpHandler
        {
            ConnectTimeout = TimeSpan.FromSeconds(2),
            SslOptions = new System.Net.Security.SslClientAuthenticationOptions
            {
                RemoteCertificateValidationCallback = (_, _, _, _) => true
            }
        };
        _http = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/"),
            Timeout = TimeSpan.FromSeconds(3),
        };
    }

    private static string ToLoopback(IConfiguration config, string url)
    {
        try
        {
            var u = new Uri(url);
            var host = config["ServiceUrls:GrpcHost"];
            if (string.IsNullOrWhiteSpace(host)) { host = "localhost"; }
            return $"{u.Scheme}://{host}:{u.Port}";
        }
        catch { return url; }
    }

    private sealed class Wrapper { public BudgetAvailability? data { get; set; } }

    public async Task<BudgetAvailability> CheckAsync(Guid budgetId, decimal amount, CancellationToken ct = default)
    {
        var res = await _http.GetFromJsonAsync<Wrapper>($"api/finance/v1.0/BudgetCheck/Availability/{budgetId}?amount={amount}", ct);
        return res?.data ?? throw new InvalidOperationException("Empty budget response.");
    }

    public async Task<BudgetAvailability> ReserveAsync(Guid budgetId, Guid referenceId, string referenceType, decimal amount, string? note, CancellationToken ct = default)
    {
        var body = new { budgetId, referenceId, referenceType, amount, note };
        var resp = await _http.PostAsJsonAsync("api/finance/v1.0/BudgetCheck/Reserve", body, ct);
        resp.EnsureSuccessStatusCode();
        var res = await resp.Content.ReadFromJsonAsync<Wrapper>(cancellationToken: ct);
        return res?.data ?? throw new InvalidOperationException("Empty budget response.");
    }

    public async Task ReleaseAsync(Guid referenceId, string referenceType, CancellationToken ct = default)
    {
        var body = new { referenceId, referenceType };
        var resp = await _http.PostAsJsonAsync("api/finance/v1.0/BudgetCheck/Release", body, ct);
        resp.EnsureSuccessStatusCode();
    }

    public async Task ConsumeAsync(Guid referenceId, string referenceType, decimal? amount, CancellationToken ct = default)
    {
        var body = new { referenceId, referenceType, amount };
        var resp = await _http.PostAsJsonAsync("api/finance/v1.0/BudgetCheck/Consume", body, ct);
        resp.EnsureSuccessStatusCode();
    }
}
