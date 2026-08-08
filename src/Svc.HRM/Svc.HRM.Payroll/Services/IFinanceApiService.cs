using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Svc.HRM.Payroll.Services;

public class FinancePeriodInfo
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsClosed { get; set; }
}

public class FinanceAccountInfo
{
    public Guid Id { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
}

public class FinanceJournalCreateRequest
{
    public string Reference { get; set; } = default!;
    public DateTime EntryDate { get; set; }
    public string Description { get; set; } = default!;
    public string EntryType { get; set; } = "Payroll";
    public Guid? PeriodId { get; set; }
    public string? CreatedByUserName { get; set; }
    public List<FinanceJournalLineRequest> Lines { get; set; } = [];
}

public class FinanceJournalLineRequest
{
    public Guid AccountId { get; set; }
    public string Direction { get; set; } = default!; // Debit / Credit
    public decimal Amount { get; set; }
    public string Description { get; set; } = default!;
}

public class FinanceJournalCreateResult
{
    public Guid Id { get; set; }
    public string? Reference { get; set; }
    public bool IsPosted { get; set; }
}

public interface IFinanceApiService
{
    Task<FinancePeriodInfo?> GetActivePeriodAsync(DateTime? date = null, CancellationToken ct = default);
    Task<FinanceAccountInfo?> GetAccountByCodeAsync(string code, CancellationToken ct = default);
    Task<FinanceJournalCreateResult?> CreateJournalEntryAsync(FinanceJournalCreateRequest request, CancellationToken ct = default);
    Task<bool> PostJournalEntryAsync(Guid journalEntryId, CancellationToken ct = default);
    Task<FinanceJournalCreateResult?> GetJournalByReferenceAsync(string reference, CancellationToken ct = default);
}

public class FinanceApiService : IFinanceApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FinanceApiService> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public FinanceApiService(HttpClient httpClient, ILogger<FinanceApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<FinancePeriodInfo?> GetActivePeriodAsync(DateTime? date = null, CancellationToken ct = default)
    {
        var url = "/api/finance/v1/PeriodClosing/Active";
        if (date.HasValue) url += $"?date={date:yyyy-MM-dd}";

        var response = await _httpClient.GetAsync(url, ct);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Finance active period lookup failed: {Status}", response.StatusCode);
            return null;
        }

        var json = await response.Content.ReadAsStringAsync(ct);
        return ExtractData<FinancePeriodInfo>(json);
    }

    public async Task<FinanceAccountInfo?> GetAccountByCodeAsync(string code, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"/api/finance/v1/ChartOfAccounts/ByCode/{Uri.EscapeDataString(code)}", ct);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Finance COA ByCode {Code} failed: {Status}", code, response.StatusCode);
            return null;
        }

        var json = await response.Content.ReadAsStringAsync(ct);
        return ExtractData<FinanceAccountInfo>(json) ?? JsonSerializer.Deserialize<FinanceAccountInfo>(json, JsonOptions);
    }

    public async Task<FinanceJournalCreateResult?> CreateJournalEntryAsync(FinanceJournalCreateRequest request, CancellationToken ct = default)
    {
        var payload = new
        {
            request.Reference,
            request.EntryDate,
            request.Description,
            request.EntryType,
            request.PeriodId,
            CreatedByUserName = request.CreatedByUserName,
            Lines = request.Lines.Select(l => new
            {
                l.AccountId,
                l.Direction,
                l.Amount,
                l.Description
            }).ToList()
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/api/finance/v1/JournalEntry", content, ct);
        var json = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Finance journal create failed ({Status}): {Body}", response.StatusCode, json);
            throw new InvalidOperationException($"Finance journal create failed: {response.StatusCode}. {json}");
        }

        return ExtractData<FinanceJournalCreateResult>(json)
               ?? JsonSerializer.Deserialize<FinanceJournalCreateResult>(json, JsonOptions);
    }

    public async Task<bool> PostJournalEntryAsync(Guid journalEntryId, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsync($"/api/finance/v1/JournalEntry/{journalEntryId}/post", null, ct);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            _logger.LogWarning("Finance journal post failed for {Id}: {Status} {Body}", journalEntryId, response.StatusCode, body);
            return false;
        }
        return true;
    }

    public async Task<FinanceJournalCreateResult?> GetJournalByReferenceAsync(string reference, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"/api/finance/v1/JournalEntry/ByReference/{Uri.EscapeDataString(reference)}", ct);
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync(ct);
        return ExtractData<FinanceJournalCreateResult>(json)
               ?? JsonSerializer.Deserialize<FinanceJournalCreateResult>(json, JsonOptions);
    }

    private static T? ExtractData<T>(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        if (root.TryGetProperty("data", out var data) || root.TryGetProperty("Data", out data))
        {
            if (data.ValueKind == JsonValueKind.Null) return default;
            return JsonSerializer.Deserialize<T>(data.GetRawText(), JsonOptions);
        }
        return JsonSerializer.Deserialize<T>(json, JsonOptions);
    }
}
