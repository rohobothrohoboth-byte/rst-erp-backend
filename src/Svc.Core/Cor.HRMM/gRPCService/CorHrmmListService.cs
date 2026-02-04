using Contracts;
using Cor.HRMM.Queries;
using Grpc.Core;
using MediatR;

namespace Cor.HRMM.gRPCService;

public class CorHrmmListService : CorHrmmService.CorHrmmServiceBase
{
    private readonly IMediator _med;
    public CorHrmmListService(IMediator med) => _med = med;

    public override async Task<CorHrmmListRes> GetListJobGrade(CorHrmmListRqst request, ServerCallContext context)
    {
        var res = new CorHrmmListRes();
        var response = (await _med.Send(new JobGradeNameAllQry())).ToList();
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
        var response = await _med.Send(new JobGradeNameByIdQry { Id = Guid.Parse(request.Id) });
        if (response == null)
        {
            var nList = new CorHrmmList { Id = null, Name = null};
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
        var response = (await _med.Send(new PositionNameAllQry())).ToList();
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
        var response = await _med.Send(new PositionNameByIdQry { Id = Guid.Parse(request.Id) });
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
        var response = await _med.Send(new PosReqByPosIdQry { Id = Guid.Parse(request.Id) });
        if (response == null)
        {
            res = new PosReqRes { Id = null, PositionId = null, Gender = null, ProfessionType = null, SaturdayWorkOption = null, SundayWorkOption = null, WorkingHours = null };
        };
        res.Id = response!.Id.ToString();
        res.PositionId = response.PositionId.ToString();
        res.Gender = response.Gender;
        res.ProfessionType = response.ProfessionType;
        res.SaturdayWorkOption = response.SaturdayWorkOption;
        res.SundayWorkOption = response.SaturdayWorkOption;
        res.WorkingHours = response.WorkingHours.ToString();
        return res;
    }




}