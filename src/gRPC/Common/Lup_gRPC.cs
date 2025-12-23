using Contracts;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

namespace Common;

public interface ILupClient
{
    Task<LupListRes> GetListEduLevel(CancellationToken ct = default);
    Task<LupRes> GetEduLevel(string id, CancellationToken ct = default);
    Task<LupListRes> GetRelList(CancellationToken ct = default);
    Task<LupRes> GetRel(string id, CancellationToken ct = default);
}


public class LupClient : ILupClient
{
    private readonly string _servUrl;

    public LupClient(IConfiguration config)
    {
        _servUrl = config["LupUrl"] ?? throw new InvalidOperationException("Lup Service Address not configured");
    }

    public async Task<LupListRes> GetListEduLevel(CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_servUrl);
        var client = new LupService.LupServiceClient(channel);
        var req = new LupListRqst();
        return await client.GetListEduLevelAsync(req);
    }

    public async Task<LupRes> GetEduLevel(string id, CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_servUrl);
        var client = new LupService.LupServiceClient(channel);
        var req = new LupRqst { Id = id };
        return await client.GetEduLevelAsync(req);
    }

    public async Task<LupListRes> GetRelList(CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_servUrl);
        var client = new LupService.LupServiceClient(channel);
        var req = new LupListRqst();
        return await client.GetRelListAsync(req);
    }

    public async Task<LupRes> GetRel(string id, CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_servUrl);
        var client = new LupService.LupServiceClient(channel);
        var req = new LupRqst { Id = id };
        return await client.GetRelAsync(req);
    }



}