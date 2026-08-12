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

    // Returns Id + full name (+ Amharic) + code for every employee. Consumed by the
    // Leave service (e.g. leave history/report) to resolve EmployeeId -> name/code,
    // since employee identity lives in this service's DB, not the Leave DB.
    // Previously not overridden, so gRPC returned Unimplemented.
    public override async Task<HrmProResCodeList> GetEmpCodeList(HrmProListRqst request, ServerCallContext context)
    {
        var res = new HrmProResCodeList();
        var response = (await _med.Send(new EmpAllAdminQry())).ToList();

        foreach (var dbItem in response)
        {
            res.Res.Add(new HrmProResCode
            {
                Id = dbItem.Id.ToString(),
                Name = dbItem.EmpFullName ?? "",
                NameAm = dbItem.EmpFullNameAm ?? "",
                Code = dbItem.Code ?? ""
            });
        }

        return res;
    }

    // Full basic info for one employee. Consumed by the Leave service (e.g. leave
    // request detail). Previously not overridden -> gRPC returned Unimplemented.
    public override async Task<EmpBasicInfoRes> GetEmpBasicInfo(HrmProRqst request, ServerCallContext context)
    {
        var res = new EmpBasicInfoRes();
        var e = await _med.Send(new EmployeeByIdQry { Id = Guid.Parse(request.Id) });
        if (e == null) return res;

        res.Id = e.Id.ToString();
        res.EmpFullName = e.EmpFullName ?? "";
        res.EmpFullNameAm = e.EmpFullNameAm ?? "";
        res.Code = e.Code ?? "";
        res.Gender = e.Gender ?? "";
        res.EmpState = e.EmpState ?? "";
        res.Branch = e.Branch ?? "";
        res.Department = e.Department ?? "";
        res.Position = e.Position ?? "";
        res.JobGrade = e.JobGrade ?? "";
        res.EmpType = e.EmpType ?? "";
        res.EmpNature = e.EmpNature ?? "";
        res.WorkArr = e.WorkArr ?? "";
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

    public override async Task<HrmEmpId> GetEmpId(HrmProRqst request, ServerCallContext context)
    {
        var res = new HrmEmpId();
        var response = await _med.Send(new GetEmpIdByIdQry { Id = Guid.Parse(request.Id) });
        if (response == null)
        {
            res = new HrmEmpId { Id = null, PositionId = null, DeptId = null, BranchId = null, CompanyId = null, JgStepId = null, JgId = null };
        }

        res.Id = response!.Id.ToString();
        res.PositionId = response.PositionId.ToString();
        res.DeptId = response.DeptId.ToString();
        res.BranchId = response.BranchId.ToString();
        res.CompanyId = response.CompanyId.ToString();
        res.JgStepId = response.JgStepId.ToString();
        res.JgId = response.JgId.ToString();
        return res;
    }

    public override async Task<HrmListEmpId> GetListEmpId(HrmProListRqst request, ServerCallContext context)
    {
        var res = new HrmListEmpId();
        var response = (await _med.Send(new GetEmpIdAllQry())).ToList();
        if (response.Count <= 0)
        {
            var nList = new HrmEmpId { Id = null, PositionId = null, DeptId = null, BranchId = null, CompanyId = null, JgStepId = null, JgId = null };
            res.Res.Add(nList);
            return res;
        }

        foreach (var dbItem in response)
        {
            res.Res.Add(new HrmEmpId { Id = dbItem.Id.ToString(), PositionId = dbItem.PositionId.ToString(), DeptId = dbItem.DeptId.ToString(), BranchId = dbItem.BranchId.ToString(), CompanyId = dbItem.CompanyId.ToString(), JgStepId = dbItem.JgStepId.ToString(), JgId = dbItem.JgId.ToString() });
        }

        return res;
    }

    public override async Task<AdminEmpList> GetAdminEmpList(HrmProListRqst request, ServerCallContext context)
    {
        var res = new AdminEmpList();
        var response = (await _med.Send(new EmpAllAdminQry())).ToList();
        if (response.Count <= 0)
        {
            var nList = new AdminEmp { Id = null, Name = null, NameAm = null, Gender = null, Code = null, Branch = null, Dept = null, Position = null , Status = null  };
            res.Res.Add(nList);
            return res;
        }

        foreach (var dbItem in response)
        {
            res.Res.Add(new AdminEmp { Id = dbItem.Id.ToString(), Name = dbItem.EmpFullName, NameAm = dbItem.EmpFullNameAm, Gender = dbItem.Gender, Code = dbItem.Code, Branch = dbItem.Branch, Dept = dbItem.Department, Position = dbItem.Position, Status = dbItem.EmpState });
        }

        return res;
    }


}