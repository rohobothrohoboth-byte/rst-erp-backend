using Contracts;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

namespace Common;

public interface ICorModClient
{
    Task<CorModuleListResAm> GetListDept(CancellationToken ct = default);
    Task<CorModuleResAm> GetDept(string id, CancellationToken ct = default);
    Task<CorModuleListRes> GetListFiscalYear(CancellationToken ct = default);
    Task<CorModuleRes> GetFiscalYear(string id, CancellationToken ct = default);

}


public class CorModClient : ICorModClient
{
    private readonly string _servUrl;

    public CorModClient(IConfiguration config)
    {
        _servUrl = config["CorModUrl"] ?? throw new InvalidOperationException("Core Module Service Address not configured");
    }

    public async Task<CorModuleListResAm> GetListDept(CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_servUrl);
        var client = new CorModuleService.CorModuleServiceClient(channel);
        var req = new CorModuleListRqst();
        return await client.GetListDeptAsync(req);
    }

    public async Task<CorModuleResAm> GetDept(string id, CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_servUrl);
        var client = new CorModuleService.CorModuleServiceClient(channel);
        var req = new CorModuleRqst { Id = id };
        return await client.GetDeptAsync(req);
    }

    public async Task<CorModuleListRes> GetListFiscalYear(CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_servUrl);
        var client = new CorModuleService.CorModuleServiceClient(channel);
        var req = new CorModuleListRqst();
        return await client.GetListFiscalYearAsync(req);
    }

    public async Task<CorModuleRes> GetFiscalYear(string id, CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_servUrl);
        var client = new CorModuleService.CorModuleServiceClient(channel);
        var req = new CorModuleRqst { Id = id };
        return await client.GetFiscalYearAsync(req);
    }




}