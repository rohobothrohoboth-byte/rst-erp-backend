using Contracts;
using Grpc.Core;
using MediatR;
using Profile.App.Queries;

namespace Profile.API.gRPCService;

public class HrmProService : HrmProfileService.HrmProfileServiceBase
{
    private readonly IMediator _med;
    public HrmProService(IMediator med) => _med = med;

    public override async Task<HrmProResCode> GetEmpCode(HrmProRqst request, ServerCallContext context)
    {
        var res = new HrmProResCode();
        var response = await _med.Send(new EmpCodeByIdQry { Id = Guid.Parse(request.Id) });
        if (response != null && response.Length > 0) { res.Code = response; }
        return res;
    }

    public override async Task<HrmProRes> GetEmp(HrmProRqst request, ServerCallContext context)
    {
        var res = new HrmProRes();
        var response = await _med.Send(new EmpNameByIdQry { Id = Guid.Parse(request.Id) });
        if (response == null)
        {
            var nList = new HrmProList { Id = null, Name = null };
            res.Res = nList;
            return res;
        }
        var nList2 = new HrmProList { Id = response.Id.ToString(), Name = response.Name, };
        res.Res = nList2;
        return res;
    }

    public override async Task<EmpPosRes> GetPosEmp(HrmProRqst request, ServerCallContext context)
    {
        var res = new EmpPosRes();
        var response = await _med.Send(new GetPosEmpQry { Id = Guid.Parse(request.Id) });
        if (response == null)
        {
            res = new EmpPosRes { Id = null, PositionId = null, SaturdayWorkOption = null, SundayWorkOption = null };
        }
        res.Id = response!.Id.ToString();
        res.PositionId = response.PositionId.ToString();
        res.SaturdayWorkOption = response.SaturdayWorkOption;
        res.SundayWorkOption = response.SundayWorkOption;
        return res;
    }

    public override async Task<HrmProListRes> GetListEmp(HrmProListRqst request, ServerCallContext context)
    {
        var res = new HrmProListRes();
        var response = (await _med.Send(new EmpNameAllQry())).ToList();
        if (response.Count <= 0)
        {
            var nList = new HrmProList { Id = null, Name = null };
            res.Res.Add(nList);
            return res;
        }

        foreach (var dbItem in response)
        {
            res.Res.Add(new HrmProList { Id = dbItem.Id.ToString(), Name = dbItem.Name });
        }

        return res;
    }

    public override async Task<HrmProListPlcy> GetListEmpPolicy(HrmProListRqst request, ServerCallContext context)
    {
        var res = new HrmProListPlcy();
        var response = (await _med.Send(new EmpPolicyAllQry())).ToList();
        if (response.Count <= 0)
        {
            var nList = new HrmEmpPlcy { Id = null, Name = null, SerYear = null, EmpType = null, WorkAr = null, Gender = null, Jg = null };
            res.Res.Add(nList);
            return res;
        }

        foreach (var dbItem in response)
        {
            res.Res.Add(new HrmEmpPlcy
            {
                Id = dbItem.EmployeeId.ToString(),
                Name = dbItem.Name,
                SerYear = dbItem.SerYear.ToString(),
                EmpType = dbItem.EmpType,
                WorkAr = dbItem.WorkAr,
                Gender = dbItem.Gender,
                Jg = dbItem.Jg
            });
        }

        return res;
    }

    public override async Task<HrmEmpPlcy> GetEmpPolicy(HrmProRqst request, ServerCallContext context)
    {
        var res = new HrmEmpPlcy();
        var response = await _med.Send(new EmpPolicyByIdQry { Id = Guid.Parse(request.Id) });
        if (response == null)
        {
            res = new HrmEmpPlcy { Id = null, Name = null, SerYear = null, EmpType = null, WorkAr = null, Gender = null, Jg = null };
        }
        res.Id = response!.EmployeeId.ToString();
        res.Name = response.Name;
        res.SerYear = response.SerYear.ToString();
        res.EmpType = response.EmpType;
        res.WorkAr = response.WorkAr;
        res.Gender = response.Gender;
        res.Jg = response.Jg.ToString();
        return res;
    }


}