using Contracts;
using Grpc.Core;
using Grpc.Net.Client;
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

   public HrmProfileClient(IConfiguration config, ILogger<HrmProfileClient>? logger = null)
   {
       // ✅ Try multiple keys
       _servUrl = config["ServiceUrls:HrmProfileApi"]
           ?? config["ServiceUrls:HrmProApi"]
           ?? config["HrmProUrl"]
           ?? throw new InvalidOperationException("HRM Profile Service Address not configured");

       _logger = logger;
       _logger?.LogInformation($"🔗 Connecting to HRM Profile at: {_servUrl}");

       _channel = GrpcChannel.ForAddress(_servUrl, new GrpcChannelOptions
       {
           HttpHandler = new HttpClientHandler
           {
               ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
           }
       });
   }
      public async Task<HrmProResCodeList> GetEmpCodeList(CancellationToken ct = default)
        {
            using var channel = GrpcChannel.ForAddress(_servUrl);
            var client = new HrmProfileService.HrmProfileServiceClient(channel);
            var req = new HrmProListRqst();
            return await client.GetEmpCodeListAsync(req);
        }

 public async Task<EmpBasicInfoRes> GetEmpBasicInfo(string id, CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_servUrl);
        var client = new HrmProfileService.HrmProfileServiceClient(channel);
        var req = new HrmProRqst { Id = id };
        return await client.GetEmpBasicInfoAsync(req);
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
            throw;
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
         throw;
     }
 }
    public async Task<HrmProRes> GetEmp(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new HrmProfileService.HrmProfileServiceClient(_channel);
            var req = new HrmProRqst { Id = id };
            return await client.GetEmpAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetEmp for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<HrmProListRes> GetListEmp(CancellationToken ct = default)
    {
        try
        {
            var client = new HrmProfileService.HrmProfileServiceClient(_channel);
            var req = new HrmProListRqst();
            return await client.GetListEmpAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetListEmp: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            throw;
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
            throw;
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
            throw;
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
            throw;
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
            throw;
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
            throw;
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
            throw;
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