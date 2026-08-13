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
            HttpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
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
            HttpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            }
        });
    }

    public async Task<CorHrmmListRes> GetListJgStep(CancellationToken ct = default)
    {
        try
        {
            var client = new CorHrmmService.CorHrmmServiceClient(_channel);
            var req = new CorHrmmListRqst();
            return await client.GetListJgStepAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetListJgStep: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<CorHrmmRes> GetJgStep(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new CorHrmmService.CorHrmmServiceClient(_channel);
            var req = new CorHrmmRqst { Id = id };
            return await client.GetJgStepAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetJgStep for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<CorHrmmListRes> GetListJobGrade(CancellationToken ct = default)
    {
        try
        {
            var client = new CorHrmmService.CorHrmmServiceClient(_channel);
            var req = new CorHrmmListRqst();
            return await client.GetListJobGradeAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetListJobGrade: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<CorHrmmRes> GetJobGrade(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new CorHrmmService.CorHrmmServiceClient(_channel);
            var req = new CorHrmmRqst { Id = id };
            return await client.GetJobGradeAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetJobGrade for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<CorHrmmListRes> GetListPosition(CancellationToken ct = default)
    {
        try
        {
            var client = new CorHrmmService.CorHrmmServiceClient(_channel);
            var req = new CorHrmmListRqst();
            return await client.GetListPositionAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetListPosition: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<CorHrmmRes> GetPosition(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new CorHrmmService.CorHrmmServiceClient(_channel);
            var req = new CorHrmmRqst { Id = id };
            return await client.GetPositionAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetPosition for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            throw;
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