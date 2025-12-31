using Contracts;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

namespace Common;


public interface IHrmProfileClient
{
    Task<HrmProResCode> GetEmpCode(string id, CancellationToken ct = default);
    Task<HrmProRes> GetEmp(string id, CancellationToken ct = default);
    Task<HrmProListRes> GetListEmp(CancellationToken ct = default);


}


public class HrmProfileClient : IHrmProfileClient
{
    private readonly string _servUrl;

    public HrmProfileClient(IConfiguration config)
    {
        _servUrl = config["HrmProUrl"] ?? throw new InvalidOperationException("HRM Profile Service Address not configured");
    }

    public async Task<HrmProResCode> GetEmpCode(string id, CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_servUrl);
        var client = new HrmProfileService.HrmProfileServiceClient(channel);
        var req = new HrmProRqst { Id = id };
        return await client.GetEmpCodeAsync(req);
    }

    public async Task<HrmProRes> GetEmp(string id, CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_servUrl);
        var client = new HrmProfileService.HrmProfileServiceClient(channel);
        var req = new HrmProRqst { Id = id };
        return await client.GetEmpAsync(req);
    }

    public async Task<HrmProListRes> GetListEmp(CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_servUrl);
        var client = new HrmProfileService.HrmProfileServiceClient(channel);
        var req = new HrmProListRqst();
        return await client.GetListEmpAsync(req);
    }


}
