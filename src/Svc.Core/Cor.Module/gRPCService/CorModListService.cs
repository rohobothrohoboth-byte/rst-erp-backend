using Contracts;
using Cor.Module.Queries;
using Grpc.Core;
using MediatR;

namespace Cor.Module.gRPCService;

public class CorModListService : CorModuleService.CorModuleServiceBase
{
    private readonly IMediator _med;
    public CorModListService(IMediator med) => _med = med;

    public override async Task<CorModuleListResAm> GetListDept(CorModuleListRqst request, ServerCallContext context)
    {
        var res = new CorModuleListResAm();
        var response = (await _med.Send(new DeptAllNameQry())).ToList();
        if (response.Count <= 0)
        {
            var nList = new CorModListAm { Id = null, Name = null, NameAm = null };
            res.Res.Add(nList);
            return res;
        }

        foreach (var dbItem in response)
        {
            res.Res.Add(new CorModListAm { Id = dbItem.Id.ToString(), Name = dbItem.Name, NameAm = dbItem.NameAm });
        }

        return res;
    }

    public override async Task<CorModuleResAm> GetDept(CorModuleRqst request, ServerCallContext context)
    {
        var res = new CorModuleResAm();
        var response = await _med.Send(new DeptNameByIdQry { Id = Guid.Parse(request.Id) });
        if (response == null)
        {
            var nList = new CorModListAm { Id = null, Name = null, NameAm = null };
            res.Res = nList;
            return res;
        }
        var nList2 = new CorModListAm { Id = response.Id.ToString(), Name = response.Name, NameAm = response.NameAm };
        res.Res = nList2;
        return res;
    }

    public override async Task<CorModuleListRes> GetListFiscalYear(CorModuleListRqst request, ServerCallContext context)
    {
        var res = new CorModuleListRes();
        var response = (await _med.Send(new FiscalYearAllNameQry())).ToList();
        if (response.Count <= 0)
        {
            var nList = new CorModList { Id = null, Name = null};
            res.Res.Add(nList);
            return res;
        }

        foreach (var dbItem in response)
        {
            res.Res.Add(new CorModList { Id = dbItem.Id.ToString(), Name = dbItem.Name});
        }

        return res;
    }

    public override async Task<CorModuleRes> GetFiscalYear(CorModuleRqst request, ServerCallContext context)
    {
        var res = new CorModuleRes();
        var response = await _med.Send(new FiscalYearNameByIdQry { Id = Guid.Parse(request.Id) });
        if (response == null)
        {
            var nList = new CorModList { Id = null, Name = null};
            res.Res = nList;
            return res;
        }
        var nList2 = new CorModList { Id = response.Id.ToString(), Name = response.Name};
        res.Res = nList2;
        return res;
    }




}