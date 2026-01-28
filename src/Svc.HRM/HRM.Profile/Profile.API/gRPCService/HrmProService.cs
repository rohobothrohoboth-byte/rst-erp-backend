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


}