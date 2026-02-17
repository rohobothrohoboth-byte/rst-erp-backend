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
    Task<ActFiscalYear> GetFiscYearDesc(string id, CancellationToken ct = default);
    Task<ActFiscalYear> GetActiveFiscal(CancellationToken ct = default);
    Task<HoDayListRes> GetListHoDay(CancellationToken ct = default);
    Task<HoDayRes> GetHoDay(string id, CancellationToken ct = default);
    Task<PeriodListRes> GetListPeriod(CancellationToken ct = default);
    Task<PeriodRes> GetPeriod(string id, CancellationToken ct = default);
    Task<DbcListRes> GetListDbc(CancellationToken ct = default);
    Task<DbcRes> GetDbc(string id, CancellationToken ct = default);

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

    public async Task<ActFiscalYear> GetFiscYearDesc(string id, CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_servUrl);
        var client = new CorModuleService.CorModuleServiceClient(channel);
        var req = new CorModuleRqst { Id = id };
        return await client.GetFiscYearDescAsync(req);
    }

    public async Task<ActFiscalYear> GetActiveFiscal(CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_servUrl);
        var client = new CorModuleService.CorModuleServiceClient(channel);
        var req = new CorModuleListRqst();
        return await client.GetActiveFiscalAsync(req);
    }

    public async Task<HoDayListRes> GetListHoDay(CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_servUrl);
        var client = new CorModuleService.CorModuleServiceClient(channel);
        var req = new CorModuleListRqst();
        return await client.GetListHoDayAsync(req);
    }

    public async Task<HoDayRes> GetHoDay(string id, CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_servUrl);
        var client = new CorModuleService.CorModuleServiceClient(channel);
        var req = new CorModuleRqst { Id = id };
        return await client.GetHoDayAsync(req);
    }

    public async Task<PeriodListRes> GetListPeriod(CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_servUrl);
        var client = new CorModuleService.CorModuleServiceClient(channel);
        var req = new CorModuleListRqst();
        return await client.GetListPeriodAsync(req);
    }

    public async Task<PeriodRes> GetPeriod(string id, CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_servUrl);
        var client = new CorModuleService.CorModuleServiceClient(channel);
        var req = new CorModuleRqst { Id = id };
        return await client.GetPeriodAsync(req);
    }

    public async Task<DbcListRes> GetListDbc(CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_servUrl);
        var client = new CorModuleService.CorModuleServiceClient(channel);
        var req = new CorModuleListRqst();
        return await client.GetListDbcAsync(req);
    }

    public async Task<DbcRes> GetDbc(string id, CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_servUrl);
        var client = new CorModuleService.CorModuleServiceClient(channel);
        var req = new CorModuleRqst { Id = id };
        return await client.GetDbcAsync(req);
    }




}