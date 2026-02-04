using Contracts;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

namespace Common;

public interface ICorHrmmClient
{
    Task<CorHrmmListRes> GetListJobGrade(CancellationToken ct = default);
    Task<CorHrmmRes> GetJobGrade(string id, CancellationToken ct = default);
    Task<CorHrmmListRes> GetListPosition(CancellationToken ct = default);
    Task<CorHrmmRes> GetPosition(string id, CancellationToken ct = default);
    Task<PosReqRes> GetPosReq(string id, CancellationToken ct = default);

}


public class CorHrmmClient : ICorHrmmClient
{
    private readonly string _servUrl;

    public CorHrmmClient(IConfiguration config)
    {
        _servUrl = config["CorHrmmUrl"] ?? throw new InvalidOperationException("Core Module Service Address not configured");
    }

    public async Task<CorHrmmListRes> GetListJobGrade(CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_servUrl);
        var client = new CorHrmmService.CorHrmmServiceClient(channel);
        var req = new CorHrmmListRqst();
        return await client.GetListJobGradeAsync(req);
    }

    public async Task<CorHrmmRes> GetJobGrade(string id, CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_servUrl);
        var client = new CorHrmmService.CorHrmmServiceClient(channel);
        var req = new CorHrmmRqst { Id = id };
        return await client.GetJobGradeAsync(req);
    }

    public async Task<CorHrmmListRes> GetListPosition(CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_servUrl);
        var client = new CorHrmmService.CorHrmmServiceClient(channel);
        var req = new CorHrmmListRqst();
        return await client.GetListPositionAsync(req);
    }

    public async Task<CorHrmmRes> GetPosition(string id, CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_servUrl);
        var client = new CorHrmmService.CorHrmmServiceClient(channel);
        var req = new CorHrmmRqst { Id = id };
        return await client.GetPositionAsync(req);
    }

    public async Task<PosReqRes> GetPosReq(string id, CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_servUrl);
        var client = new CorHrmmService.CorHrmmServiceClient(channel);
        var req = new CorHrmmRqst { Id = id };
        return await client.GetPosReqAsync(req);
    }




}