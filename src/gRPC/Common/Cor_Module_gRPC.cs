using Contracts;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Common;

public interface ICorModClient
{
    Task<CorModuleListResAm> GetListDept(CancellationToken ct = default);
    Task<CorModuleResAm> GetDept(string id, CancellationToken ct = default);
    Task<CorModuleListRes> GetListFiscalYear(CancellationToken ct = default);
    Task<CorModuleRes> GetFiscalYear(string id, CancellationToken ct = default);
    Task<ActFiscalYear> GetFiscYearDesc(string id, CancellationToken ct = default);
    Task<ActFiscalYear> GetActiveFiscal(CancellationToken ct = default);
    Task<HoDayListRes> GetListHoDay(CancellationToken ct = default);
    Task<HoDayRes> GetHoDay(string id, CancellationToken ct = default);
    Task<PeriodListRes> GetListPeriod(CancellationToken ct = default);
    Task<PeriodRes> GetPeriod(string id, CancellationToken ct = default);
    Task<DbcListRes> GetListDbc(CancellationToken ct = default);
    Task<DbcRes> GetDbc(string id, CancellationToken ct = default);


    Task<CorModuleResAm> GetBranch(string id, CancellationToken ct = default);
    Task<CorModuleListResAm> GetListBranch(CancellationToken ct = default);
    Task<CorModuleResAm> GetCompany(string id, CancellationToken ct = default);
    Task<CorModuleListResAm> GetListCompany(CancellationToken ct = default);

}

public class CorModClient : ICorModClient
{
    private readonly string _servUrl;
    private readonly ILogger<CorModClient>? _logger;
    private readonly GrpcChannel _channel;
    private readonly IMemoryCache? _cache;

    // Master-data lists change rarely; cache successful results for a minute and cache a
    // failure briefly so a down Core Module isn't retried on every single request.
    private static readonly TimeSpan OkTtl = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan FailTtl = TimeSpan.FromSeconds(8);

    public CorModClient(IConfiguration config, ILogger<CorModClient>? logger = null, IMemoryCache? cache = null)
    {
        _cache = cache;
        // Accept whichever key the host service configured. Different services expose the
        // Core Module address under different keys (ServiceUrls:CoreModuleApi is the modern
        // one; CorModUrl is the legacy gRPC-common one). Fall back to the standard local
        // port so a missing key degrades gracefully instead of failing every request.
        _servUrl = config["ServiceUrls:CoreModuleApi"]
            ?? config["ServiceUrls:CorModApi"]
            ?? config["CorModUrl"]
            ?? config["CorModuleUrl"]
            ?? "https://localhost:7002";
        _logger = logger;

        // ? Create a single channel that will be reused
        _channel = GrpcChannel.ForAddress(_servUrl, new GrpcChannelOptions
        {
            // Bound the connection attempt so an unreachable Core Module fails fast
            // (~5s) instead of hanging the caller until its HTTP timeout.
            HttpHandler = new SocketsHttpHandler
            {
                ConnectTimeout = TimeSpan.FromSeconds(2),
                SslOptions = new System.Net.Security.SslClientAuthenticationOptions
                {
                    RemoteCertificateValidationCallback = (_, _, _, _) => true
                }
            }
        });
    }

    public CorModClient(string url, ILogger<CorModClient>? logger = null)
    {
        _servUrl = url ?? throw new ArgumentNullException(nameof(url));
        _logger = logger;
        _logger?.LogInformation($"?? Connecting to Core Module at: {url}");

        _channel = GrpcChannel.ForAddress(_servUrl, new GrpcChannelOptions
        {
            // Bound the connection attempt so an unreachable Core Module fails fast
            // (~5s) instead of hanging the caller until its HTTP timeout.
            HttpHandler = new SocketsHttpHandler
            {
                ConnectTimeout = TimeSpan.FromSeconds(2),
                SslOptions = new System.Net.Security.SslClientAuthenticationOptions
                {
                    RemoteCertificateValidationCallback = (_, _, _, _) => true
                }
            }
        });
    }

    // Avoid a pointless (and potentially slow) round-trip when the id is empty/all-zeros.
    private static bool IsEmptyId(string? id) =>
        string.IsNullOrEmpty(id) || id == "00000000-0000-0000-0000-000000000000" || id == Guid.Empty.ToString();

    public async Task<CorModuleListResAm> GetListDept(CancellationToken ct = default)
    {
        const string key = "cormod:list:dept";
        if (_cache != null && _cache.TryGetValue(key, out CorModuleListResAm? c) && c != null) return c;
        try
        {
            var client = new CorModuleService.CorModuleServiceClient(_channel);
            var req = new CorModuleListRqst();
            var res = await client.GetListDeptAsync(req, deadline: DateTime.UtcNow.AddSeconds(6), cancellationToken: ct);
            _cache?.Set(key, res, OkTtl);
            return res;
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetListDept: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            var empty = new CorModuleListResAm();
            _cache?.Set(key, empty, FailTtl);
            return empty;
        }
    }

    public async Task<CorModuleResAm> GetDept(string id, CancellationToken ct = default)
    {
        if (IsEmptyId(id)) { return new CorModuleResAm(); }
        try
        {
            var client = new CorModuleService.CorModuleServiceClient(_channel);
            var req = new CorModuleRqst { Id = id };
            return await client.GetDeptAsync(req, deadline: DateTime.UtcNow.AddSeconds(6), cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetDept for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            return new CorModuleResAm();
        }
    }

    public async Task<CorModuleListRes> GetListFiscalYear(CancellationToken ct = default)
    {
        try
        {
            var client = new CorModuleService.CorModuleServiceClient(_channel);
            var req = new CorModuleListRqst();
            return await client.GetListFiscalYearAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetListFiscalYear: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<CorModuleRes> GetFiscalYear(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new CorModuleService.CorModuleServiceClient(_channel);
            var req = new CorModuleRqst { Id = id };
            return await client.GetFiscalYearAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetFiscalYear for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<ActFiscalYear> GetFiscYearDesc(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new CorModuleService.CorModuleServiceClient(_channel);
            var req = new CorModuleRqst { Id = id };
            return await client.GetFiscYearDescAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetFiscYearDesc for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<ActFiscalYear> GetActiveFiscal(CancellationToken ct = default)
    {
        try
        {
            var client = new CorModuleService.CorModuleServiceClient(_channel);
            var req = new CorModuleListRqst();
            return await client.GetActiveFiscalAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetActiveFiscal: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<HoDayListRes> GetListHoDay(CancellationToken ct = default)
    {
        try
        {
            var client = new CorModuleService.CorModuleServiceClient(_channel);
            var req = new CorModuleListRqst();
            return await client.GetListHoDayAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetListHoDay: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<HoDayRes> GetHoDay(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new CorModuleService.CorModuleServiceClient(_channel);
            var req = new CorModuleRqst { Id = id };
            return await client.GetHoDayAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetHoDay for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<PeriodListRes> GetListPeriod(CancellationToken ct = default)
    {
        try
        {
            const string key = "cormod:list:period";
            if (_cache != null && _cache.TryGetValue(key, out PeriodListRes? c) && c != null) return c;
            var client = new CorModuleService.CorModuleServiceClient(_channel);
            var req = new CorModuleListRqst();
            var res = await client.GetListPeriodAsync(req, deadline: DateTime.UtcNow.AddSeconds(6), cancellationToken: ct);
            _cache?.Set(key, res, OkTtl);
            return res;
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetListPeriod: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            var empty = new PeriodListRes();
            _cache?.Set("cormod:list:period", empty, FailTtl);
            return empty;
        }
    }

  // Common/Cor_Module_gRPC.cs

  public async Task<PeriodRes> GetPeriod(string id, CancellationToken ct = default)
  {
      // ? Check if ID is null or empty
      if (string.IsNullOrEmpty(id) || id == "00000000-0000-0000-0000-000000000000")
      {
          _logger?.LogWarning("GetPeriod called with empty or invalid ID");
          return new PeriodRes { Name = "N/A" };
      }

      try
      {
          var client = new CorModuleService.CorModuleServiceClient(_channel);
          var req = new CorModuleRqst { Id = id };
          return await client.GetPeriodAsync(req, deadline: DateTime.UtcNow.AddSeconds(6), cancellationToken: ct);
      }
      catch (RpcException ex)
      {
          _logger?.LogError(ex, "gRPC error in GetPeriod for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
          return new PeriodRes { Name = "N/A" };
      }
  }

    public async Task<DbcListRes> GetListDbc(CancellationToken ct = default)
    {
        try
        {
            var client = new CorModuleService.CorModuleServiceClient(_channel);
            var req = new CorModuleListRqst();
            return await client.GetListDbcAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetListDbc: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }


public async Task<CorModuleResAm> GetBranch(string id, CancellationToken ct = default)
    {
        if (IsEmptyId(id)) { return new CorModuleResAm(); }
        try
        {
            var client = new CorModuleService.CorModuleServiceClient(_channel);
            var req = new CorModuleRqst { Id = id };
            return await client.GetBranchAsync(req, deadline: DateTime.UtcNow.AddSeconds(6), cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetBranch for ID: {Id}, Status: {Status}, Detail: {Detail}",
                id, ex.StatusCode, ex.Status.Detail);
            return new CorModuleResAm();
        }
    }

    public async Task<CorModuleListResAm> GetListBranch(CancellationToken ct = default)
    {
        try
        {
            const string key = "cormod:list:branch";
            if (_cache != null && _cache.TryGetValue(key, out CorModuleListResAm? c) && c != null) return c;
            var client = new CorModuleService.CorModuleServiceClient(_channel);
            var req = new CorModuleListRqst();
            var res = await client.GetListBranchAsync(req, deadline: DateTime.UtcNow.AddSeconds(6), cancellationToken: ct);
            _cache?.Set(key, res, OkTtl);
            return res;
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetListBranch: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            var empty = new CorModuleListResAm();
            _cache?.Set("cormod:list:branch", empty, FailTtl);
            return empty;
        }
    }

    // ==================== NEW COMPANY METHODS ====================

    public async Task<CorModuleResAm> GetCompany(string id, CancellationToken ct = default)
    {
        if (IsEmptyId(id)) { return new CorModuleResAm(); }
        try
        {
            var client = new CorModuleService.CorModuleServiceClient(_channel);
            var req = new CorModuleRqst { Id = id };
            return await client.GetCompanyAsync(req, deadline: DateTime.UtcNow.AddSeconds(6), cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetCompany for ID: {Id}, Status: {Status}, Detail: {Detail}",
                id, ex.StatusCode, ex.Status.Detail);
            return new CorModuleResAm();
        }
    }

    public async Task<CorModuleListResAm> GetListCompany(CancellationToken ct = default)
    {
        try
        {
            const string key = "cormod:list:company";
            if (_cache != null && _cache.TryGetValue(key, out CorModuleListResAm? c) && c != null) return c;
            var client = new CorModuleService.CorModuleServiceClient(_channel);
            var req = new CorModuleListRqst();
            var res = await client.GetListCompanyAsync(req, deadline: DateTime.UtcNow.AddSeconds(6), cancellationToken: ct);
            _cache?.Set(key, res, OkTtl);
            return res;
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetListCompany: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            var empty = new CorModuleListResAm();
            _cache?.Set("cormod:list:company", empty, FailTtl);
            return empty;
        }
    }


// Same for GetCompany and GetListCompany
    public async Task<DbcRes> GetDbc(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new CorModuleService.CorModuleServiceClient(_channel);
            var req = new CorModuleRqst { Id = id };
            return await client.GetDbcAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetDbc for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }
}