using Contracts;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Common;

public interface IHrmProfileClient
{
    Task<HrmProResCode> GetEmpCode(string id, CancellationToken ct = default);
    Task<HrmProRes> GetEmp(string id, CancellationToken ct = default);
    Task<HrmProListRes> GetListEmp(CancellationToken ct = default);
    Task<EmpPosRes> GetPosEmp(string id, CancellationToken ct = default);
    Task<HrmProListPlcy> GetListEmpPolicy(CancellationToken ct = default);
    Task<HrmEmpPlcy> GetEmpPolicy(string id, CancellationToken ct = default);
    Task<HrmEmpId> GetEmpId(string id, CancellationToken ct = default);
    Task<HrmListEmpId> GetListEmpId(CancellationToken ct = default);
    Task<AdminEmpList> GetAdminEmpList(CancellationToken ct = default);
      Task<HrmProListRes> GetEmpNameList(CancellationToken ct = default);
      Task<HrmProResCodeList> GetEmpCodeList(CancellationToken ct = default);
       Task<EmpBasicInfoRes> GetEmpBasicInfo(string id, CancellationToken ct = default);

}

public class HrmProfileClient : IHrmProfileClient
{
    private readonly string _servUrl;
    private readonly ILogger<HrmProfileClient>? _logger;
    private readonly GrpcChannel _channel;
    private readonly IMemoryCache? _cache;

    private static readonly TimeSpan OkTtl = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan FailTtl = TimeSpan.FromSeconds(8);

   public HrmProfileClient(IConfiguration config, ILogger<HrmProfileClient>? logger = null, IMemoryCache? cache = null)
   {
       _cache = cache;
       // ✅ Try multiple keys
       _servUrl = config["ServiceUrls:HrmProfileApi"]
           ?? config["ServiceUrls:HrmProApi"]
           ?? config["HrmProUrl"]
           ?? "https://localhost:7004";
       // Inter-service gRPC is same-host: connect over loopback, not the LAN IP.
       _servUrl = GrpcTarget.Resolve(config, _servUrl);

       _logger = logger;
       _logger?.LogInformation($"🔗 Connecting to HRM Profile at: {_servUrl}");

       _channel = GrpcChannel.ForAddress(_servUrl, new GrpcChannelOptions
       {
           // Bound the connection attempt so an unreachable Profile service fails fast.
           HttpHandler = new SocketsHttpHandler
           {
               ConnectTimeout = TimeSpan.FromSeconds(1),
               SslOptions = new System.Net.Security.SslClientAuthenticationOptions
               {
                   RemoteCertificateValidationCallback = (_, _, _, _) => true
               }
           }
       });
   }
      public async Task<HrmProResCodeList> GetEmpCodeList(CancellationToken ct = default)
        {
            // Reuse the shared _channel, which is configured to accept the dev
            // certificate. Creating a new GrpcChannel.ForAddress(_servUrl) here (as
            // before) used default TLS validation and failed with
            // RemoteCertificateNameMismatch against the self-signed dev cert.
            var client = new HrmProfileService.HrmProfileServiceClient(_channel);
            var req = new HrmProListRqst();
            return await client.GetEmpCodeListAsync(req, cancellationToken: ct);
        }

 public async Task<EmpBasicInfoRes> GetEmpBasicInfo(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new HrmProfileService.HrmProfileServiceClient(_channel);
            var req = new HrmProRqst { Id = id };
            return await client.GetEmpBasicInfoAsync(req, deadline: DateTime.UtcNow.AddSeconds(6), cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetEmpBasicInfo for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            return new EmpBasicInfoRes();
        }
    }
    public async Task<HrmProResCode> GetEmpCode(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new HrmProfileService.HrmProfileServiceClient(_channel);
            var req = new HrmProRqst { Id = id };
            return await client.GetEmpCodeAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetEmpCode for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            return new HrmProResCode();
        }
    }
 public async Task<HrmProListRes> GetEmpNameList(CancellationToken ct = default)
 {
     try
     {
         // Use the existing GetListEmp method
         return await GetListEmp(ct);
     }
     catch (RpcException ex)
     {
         _logger?.LogError(ex, "gRPC error in GetEmpNameList: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
         return new HrmProListRes();
     }
 }
    private static bool IsEmptyId(string? id) =>
        string.IsNullOrEmpty(id) || id == "00000000-0000-0000-0000-000000000000" || id == Guid.Empty.ToString();

    public async Task<HrmProRes> GetEmp(string id, CancellationToken ct = default)
    {
        if (IsEmptyId(id)) { return new HrmProRes(); }
        try
        {
            var client = new HrmProfileService.HrmProfileServiceClient(_channel);
            var req = new HrmProRqst { Id = id };
            // Pass CancellationToken.None + a hard deadline so a slow call is bounded here and a
            // client-cancellation can't trigger a retry/backoff storm from an outer policy.
            return await client.GetEmpAsync(req, deadline: DateTime.UtcNow.AddSeconds(2), cancellationToken: CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "gRPC error in GetEmp for ID: {Id}", id);
            return new HrmProRes();
        }
    }

    public async Task<HrmProListRes> GetListEmp(CancellationToken ct = default)
    {
        try
        {
            const string key = "hrmpro:list:emp";
            if (_cache != null && _cache.TryGetValue(key, out HrmProListRes? c) && c != null) return c;
            var client = new HrmProfileService.HrmProfileServiceClient(_channel);
            var req = new HrmProListRqst();
            var res = await client.GetListEmpAsync(req, deadline: DateTime.UtcNow.AddSeconds(6), cancellationToken: ct);
            _cache?.Set(key, res, OkTtl);
            return res;
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetListEmp: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            var empty = new HrmProListRes();
            _cache?.Set("hrmpro:list:emp", empty, FailTtl);
            return empty;
        }
    }

    public async Task<EmpPosRes> GetPosEmp(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new HrmProfileService.HrmProfileServiceClient(_channel);
            var req = new HrmProRqst { Id = id };
            return await client.GetPosEmpAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetPosEmp for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            return new EmpPosRes();
        }
    }

    public async Task<HrmProListPlcy> GetListEmpPolicy(CancellationToken ct = default)
    {
        try
        {
            var client = new HrmProfileService.HrmProfileServiceClient(_channel);
            var req = new HrmProListRqst();
            return await client.GetListEmpPolicyAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetListEmpPolicy: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            return new HrmProListPlcy();
        }
    }

    public async Task<HrmEmpPlcy> GetEmpPolicy(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new HrmProfileService.HrmProfileServiceClient(_channel);
            var req = new HrmProRqst { Id = id };
            return await client.GetEmpPolicyAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetEmpPolicy for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            return new HrmEmpPlcy();
        }
    }

    public async Task<HrmEmpId> GetEmpId(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new HrmProfileService.HrmProfileServiceClient(_channel);
            var req = new HrmProRqst { Id = id };
            return await client.GetEmpIdAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetEmpId for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            return new HrmEmpId();
        }
    }

    public async Task<HrmListEmpId> GetListEmpId(CancellationToken ct = default)
    {
        try
        {
            var client = new HrmProfileService.HrmProfileServiceClient(_channel);
            var req = new HrmProListRqst();
            return await client.GetListEmpIdAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetListEmpId: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            return new HrmListEmpId();
        }
    }

    public async Task<AdminEmpList> GetAdminEmpList(CancellationToken ct = default)
    {
        try
        {
            var client = new HrmProfileService.HrmProfileServiceClient(_channel);
            var req = new HrmProListRqst();
            return await client.GetAdminEmpListAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetAdminEmpList: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            return new AdminEmpList();
        }
    }

    public HrmProfileClient(string url, ILogger<HrmProfileClient>? logger = null)
    {
        _servUrl = url ?? throw new ArgumentNullException(nameof(url));
        _logger = logger;
        _logger?.LogInformation($"?? Connecting to HRM Profile at: {url}");

        _channel = GrpcChannel.ForAddress(_servUrl, new GrpcChannelOptions
        {
            HttpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            }
        });
    }

}