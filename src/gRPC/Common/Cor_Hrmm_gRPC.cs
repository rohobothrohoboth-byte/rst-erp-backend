using Contracts;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Common;

public interface ICorHrmmClient
{
    Task<CorHrmmListRes> GetListJgStep(CancellationToken ct = default);
    Task<CorHrmmRes> GetJgStep(string id, CancellationToken ct = default);
    Task<CorHrmmListRes> GetListJobGrade(CancellationToken ct = default);
    Task<CorHrmmRes> GetJobGrade(string id, CancellationToken ct = default);
    Task<CorHrmmListRes> GetListPosition(CancellationToken ct = default);
    Task<CorHrmmRes> GetPosition(string id, CancellationToken ct = default);
    Task<PosReqRes> GetPosReq(string id, CancellationToken ct = default);
    Task<JgStepSalary> GetJgStepSalary(string id, CancellationToken ct = default);
    Task<SalaryJgs> GetSalaryJgs(string id, CancellationToken ct = default);
}

public class CorHrmmClient : ICorHrmmClient
{
    private readonly string _servUrl;
    private readonly ILogger<CorHrmmClient>? _logger;
    private readonly GrpcChannel _channel;

    public CorHrmmClient(IConfiguration config, ILogger<CorHrmmClient>? logger = null)
    {
        // Accept whichever key the host service configured (ServiceUrls:CoreHRMMApi is the
        // modern one; CorHrmmUrl is the legacy gRPC-common one). Fall back to the standard
        // local port so a missing key degrades gracefully instead of failing every request.
        _servUrl = config["ServiceUrls:CoreHRMMApi"]
            ?? config["ServiceUrls:CorHrmmApi"]
            ?? config["CorHrmmUrl"]
            ?? "https://localhost:7001";
        _logger = logger;

        // ? Create a single channel that will be reused
        _channel = GrpcChannel.ForAddress(_servUrl, new GrpcChannelOptions
        {
            // Bound the connection attempt so an unreachable Core HRMM fails fast.
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
    public CorHrmmClient(string url, ILogger<CorHrmmClient>? logger = null)
    {
        _servUrl = url ?? throw new ArgumentNullException(nameof(url));
        _logger = logger;
        _logger?.LogInformation($"?? Connecting to Core HRMM at: {url}");

        _channel = GrpcChannel.ForAddress(_servUrl, new GrpcChannelOptions
        {
            // Bound the connection attempt so an unreachable Core HRMM fails fast.
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

    public async Task<CorHrmmListRes> GetListJgStep(CancellationToken ct = default)
    {
        try
        {
            var client = new CorHrmmService.CorHrmmServiceClient(_channel);
            var req = new CorHrmmListRqst();
            return await client.GetListJgStepAsync(req, deadline: DateTime.UtcNow.AddSeconds(6), cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetListJgStep: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            return new CorHrmmListRes();
        }
    }

    public async Task<CorHrmmRes> GetJgStep(string id, CancellationToken ct = default)
    {
        if (IsEmptyId(id)) { return new CorHrmmRes(); }
        try
        {
            var client = new CorHrmmService.CorHrmmServiceClient(_channel);
            var req = new CorHrmmRqst { Id = id };
            return await client.GetJgStepAsync(req, deadline: DateTime.UtcNow.AddSeconds(6), cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetJgStep for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            return new CorHrmmRes();
        }
    }

    public async Task<CorHrmmListRes> GetListJobGrade(CancellationToken ct = default)
    {
        try
        {
            var client = new CorHrmmService.CorHrmmServiceClient(_channel);
            var req = new CorHrmmListRqst();
            return await client.GetListJobGradeAsync(req, deadline: DateTime.UtcNow.AddSeconds(6), cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetListJobGrade: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            return new CorHrmmListRes();
        }
    }

    public async Task<CorHrmmRes> GetJobGrade(string id, CancellationToken ct = default)
    {
        if (IsEmptyId(id)) { return new CorHrmmRes(); }
        try
        {
            var client = new CorHrmmService.CorHrmmServiceClient(_channel);
            var req = new CorHrmmRqst { Id = id };
            return await client.GetJobGradeAsync(req, deadline: DateTime.UtcNow.AddSeconds(6), cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetJobGrade for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            return new CorHrmmRes();
        }
    }

    public async Task<CorHrmmListRes> GetListPosition(CancellationToken ct = default)
    {
        try
        {
            var client = new CorHrmmService.CorHrmmServiceClient(_channel);
            var req = new CorHrmmListRqst();
            return await client.GetListPositionAsync(req, deadline: DateTime.UtcNow.AddSeconds(6), cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetListPosition: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            return new CorHrmmListRes();
        }
    }

    public async Task<CorHrmmRes> GetPosition(string id, CancellationToken ct = default)
    {
        if (IsEmptyId(id)) { return new CorHrmmRes(); }
        try
        {
            var client = new CorHrmmService.CorHrmmServiceClient(_channel);
            var req = new CorHrmmRqst { Id = id };
            return await client.GetPositionAsync(req, deadline: DateTime.UtcNow.AddSeconds(6), cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetPosition for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            return new CorHrmmRes();
        }
    }

    public async Task<PosReqRes> GetPosReq(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new CorHrmmService.CorHrmmServiceClient(_channel);
            var req = new CorHrmmRqst { Id = id };
            return await client.GetPosReqAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetPosReq for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<JgStepSalary> GetJgStepSalary(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new CorHrmmService.CorHrmmServiceClient(_channel);
            var req = new CorHrmmRqst { Id = id };
            return await client.GetJgStepSalaryAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetJgStepSalary for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<SalaryJgs> GetSalaryJgs(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new CorHrmmService.CorHrmmServiceClient(_channel);
            var req = new CorHrmmRqst { Id = id };
            return await client.GetSalaryJgsAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetSalaryJgs for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }
}