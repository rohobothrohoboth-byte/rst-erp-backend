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
            var nList = new CorModList { Id = null, Name = null };
            res.Res.Add(nList);
            return res;
        }

        foreach (var dbItem in response)
        {
            res.Res.Add(new CorModList { Id = dbItem.Id.ToString(), Name = dbItem.Name });
        }

        return res;
    }

    public override async Task<CorModuleRes> GetFiscalYear(CorModuleRqst request, ServerCallContext context)
    {
        var res = new CorModuleRes();
        var response = await _med.Send(new FiscalYearNameByIdQry { Id = Guid.Parse(request.Id) });
        if (response == null)
        {
            var nList = new CorModList { Id = null, Name = null };
            res.Res = nList;
            return res;
        }
        var nList2 = new CorModList { Id = response.Id.ToString(), Name = response.Name };
        res.Res = nList2;
        return res;
    }

    public override async Task<ActFiscalYear> GetFiscYearDesc(CorModuleRqst request, ServerCallContext context)
    {
        var res = new ActFiscalYear();
        var response = await _med.Send(new FiscalYearByIdQry { Id = Guid.Parse(request.Id) });
        if (response == null)
        {
            res = new ActFiscalYear { Id = null, Name = null, StartDate = null, EndDate = null };
        }

        res.Id = response!.Id.ToString();
        res.Name = response.Name.ToString();
        res.StartDate = response.DateStart.ToString();
        res.EndDate = response.DateEnd.ToString();
        return res;
    }

    public override async Task<ActFiscalYear> GetActiveFiscal(CorModuleListRqst request, ServerCallContext context)
    {
        var res = new ActFiscalYear();
        var response = await _med.Send(new ActiveFiscalYearQry());
        if (response == null)
        {
            res = new ActFiscalYear { Id = null, Name = null, StartDate = null, EndDate = null };
        }
        res.Id = response!.Id.ToString();
        res.Name = response!.Name;
        res.StartDate = response.DateStart.ToString();
        res.EndDate = response.DateEnd.ToString();
        return res;
    }

    public override async Task<HoDayListRes> GetListHoDay(CorModuleListRqst request, ServerCallContext context)
    {
        var res = new HoDayListRes();
        var response = (await _med.Send(new AllHolidayQry())).ToList();
        if (response.Count <= 0)
        {
            var nList = new HoDayRes { Id = null, Name = null, Date = null, IsPublic = false, FiscalYearId = null };
            res.Res.Add(nList);
            return res;
        }

        foreach (var dbItem in response)
        {
            res.Res.Add(new HoDayRes
            {
                Id = dbItem!.Id.ToString(),
                Name = dbItem.Name,
                Date = dbItem.Date.ToString(),
                IsPublic = dbItem.IsPublic,
                FiscalYearId = dbItem.FiscalYearId.ToString()
            });
        }

        return res;
    }

    public override async Task<HoDayRes> GetHoDay(CorModuleRqst request, ServerCallContext context)
    {
        var res = new HoDayRes();
        var response = await _med.Send(new HolidayByIdQry { Id = Guid.Parse(request.Id) });
        if (response == null)
        {
            res = new HoDayRes { Id = null, Name = null, Date = null, IsPublic = false, FiscalYearId = null };
        }
        res.Id = response!.Id.ToString();
        res.Name = response.Name;
        res.Date = response.Date.ToString();
        res.IsPublic = response.IsPublic;
        res.FiscalYearId = response.FiscalYearId.ToString();
        return res;
    }




}