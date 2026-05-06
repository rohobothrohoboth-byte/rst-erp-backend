using Contracts;
using Cor.HRMM.Queries;
using Grpc.Core;
using MediatR;

namespace Cor.HRMM.gRPCService;

public class CorHrmmListService(IMediator med) : CorHrmmService.CorHrmmServiceBase
{
    public override async Task<CorHrmmListRes> GetListJgStep(CorHrmmListRqst request, ServerCallContext context)
    {
        var res = new CorHrmmListRes();
        var response = (await med.Send(new JgStepNameAllQry())).ToList();
        if (response.Count <= 0)
        {
            var nList = new CorHrmmList { Id = null, Name = null };
            res.Res.Add(nList);
            return res;
        }

        foreach (var dbItem in response)
        {
            res.Res.Add(new CorHrmmList { Id = dbItem.Id.ToString(), Name = dbItem.Name });
        }

        return res;
    }

    public override async Task<CorHrmmRes> GetJgStep(CorHrmmRqst request, ServerCallContext context)
    {
        var res = new CorHrmmRes();
        var response = await med.Send(new JgStepNameByIdQry { Id = Guid.Parse(request.Id) });
        if (response == null)
        {
            var nList = new CorHrmmList { Id = null, Name = null };
            res.Res = nList;
            return res;
        }
        var nList2 = new CorHrmmList { Id = response.Id.ToString(), Name = response.Name, };
        res.Res = nList2;
        return res;
    }

    public override async Task<CorHrmmListRes> GetListJobGrade(CorHrmmListRqst request, ServerCallContext context)
    {
        var res = new CorHrmmListRes();
        var response = (await med.Send(new JobGradeNameAllQry())).ToList();
        if (response.Count <= 0)
        {
            var nList = new CorHrmmList { Id = null, Name = null };
            res.Res.Add(nList);
            return res;
        }

        foreach (var dbItem in response)
        {
            res.Res.Add(new CorHrmmList { Id = dbItem.Id.ToString(), Name = dbItem.Name });
        }

        return res;
    }

    public override async Task<CorHrmmRes> GetJobGrade(CorHrmmRqst request, ServerCallContext context)
    {
        var res = new CorHrmmRes();
        var response = await med.Send(new JobGradeNameByIdQry { Id = Guid.Parse(request.Id) });
        if (response == null)
        {
            var nList = new CorHrmmList { Id = null, Name = null };
            res.Res = nList;
            return res;
        }
        var nList2 = new CorHrmmList { Id = response.Id.ToString(), Name = response.Name, };
        res.Res = nList2;
        return res;
    }

    public override async Task<CorHrmmListRes> GetListPosition(CorHrmmListRqst request, ServerCallContext context)
    {
        var res = new CorHrmmListRes();
        var response = (await med.Send(new PosNameAllQry())).ToList();
        if (response.Count <= 0)
        {
            var nList = new CorHrmmList { Id = null, Name = null };
            res.Res.Add(nList);
            return res;
        }

        foreach (var dbItem in response)
        {
            res.Res.Add(new CorHrmmList { Id = dbItem.Id.ToString(), Name = dbItem.Name });
        }

        return res;
    }

    public override async Task<CorHrmmRes> GetPosition(CorHrmmRqst request, ServerCallContext context)
    {
        var res = new CorHrmmRes();
        var response = await med.Send(new PosNameByIdQry { Id = Guid.Parse(request.Id) });
        if (response == null)
        {
            var nList = new CorHrmmList { Id = null, Name = null };
            res.Res = nList;
            return res;
        }
        var nList2 = new CorHrmmList { Id = response.Id.ToString(), Name = response.Name, };
        res.Res = nList2;
        return res;
    }

    public override async Task<PosReqRes> GetPosReq(CorHrmmRqst request, ServerCallContext context)
    {
        var res = new PosReqRes();
        var response = await med.Send(new PosReqByPosIdQry { Id = Guid.Parse(request.Id) });
        if (response == null)
        {
            res = new PosReqRes { Id = null, PositionId = null, Gender = null, ProfessionType = null, SaturdayWorkOption = null, SundayWorkOption = null, WorkingHours = null };
        }
        res.Id = response!.Id.ToString();
        res.PositionId = response.PositionId.ToString();
        res.Gender = response.Gender;
        res.ProfessionType = response.ProfessionType;
        res.SaturdayWorkOption = response.SaturdayWorkOption;
        res.SundayWorkOption = response.SaturdayWorkOption;
        res.WorkingHours = response.WorkingHours.ToString();
        return res;
    }

    public override async Task<JgStepSalary> GetJgStepSalary(CorHrmmRqst request, ServerCallContext context)
    {
        var res = new JgStepSalary();
        var response = await med.Send(new SalaryQry { Id = Guid.Parse(request.Id) });
        if (response == null)
        {
            res = new JgStepSalary { Salary = null };
        }
        res.Salary = response;
        return res;
    }

    public override async Task<SalaryJgs> GetSalaryJgs(CorHrmmRqst request, ServerCallContext context)
    {
        var res = new SalaryJgs();
        var response = await med.Send(new JgStepByIdQry { Id = Guid.Parse(request.Id) });
        if (response == null)
        {
            res = new SalaryJgs { Name = null, Salary = null, SalaryStr = null, Currency = null, SalaryPayFreq = null, JobGrade = null };
        }
        res.Name = response!.Name;
        res.Salary = response.Salary.ToString();
        res.SalaryStr = response.SalaryStr;
        res.Currency = response.CurrencyStr;
        res.SalaryPayFreq = response.SalaryPayFreqStr;
        res.JobGrade = response.JobGrade;
        return res;
    }




}