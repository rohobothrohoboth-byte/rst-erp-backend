using Contracts;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

namespace Common;

public interface ILupClient
{
    Task<GetRelListRes> GetRelList(CancellationToken ct = default);
    Task<GetRelRes> GetRel(string id, CancellationToken ct = default);
}


public class LupClient : ILupClient
{
    private readonly string _lupUrl;

    public LupClient(IConfiguration config)
    {
        _lupUrl = config["LupUrl"] ?? throw new InvalidOperationException("Lup Service Address not configured");
    }

    public async Task<GetRelListRes> GetRelList(CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_lupUrl);
        var client = new LupService.LupServiceClient(channel);
        var req = new GetRelListRqst();
        return await client.GetRelListAsync(req);
    }

    public async Task<GetRelRes> GetRel(string id, CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_lupUrl);
        var client = new LupService.LupServiceClient(channel);
        var req = new GetRelRqst { Id = id };
        return await client.GetRelAsync(req);
    }

    //public async Task<bool> ValidateToken(string token, CancellationToken ct = default)
    //{
    //    using var channel = GrpcChannel.ForAddress(_authUrl);
    //    var client = new AuthValidator.AuthValidatorClient(channel);
    //    var request = new ValidateTokenRequest { Token = token };
    //    var res = await client.ValidateTokenAsync(request);
    //    return res.IsValid;
    //}



}