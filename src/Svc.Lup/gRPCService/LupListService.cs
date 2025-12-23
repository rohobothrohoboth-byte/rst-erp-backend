using Contracts;
using Grpc.Core;
using MediatR;
using Svc.Lup.Queries;

namespace Svc.Lup.gRPCService;

public class LupListService : LupService.LupServiceBase
{
    private readonly IMediator _med;
    public LupListService(IMediator med) => _med = med;

    public override async Task<LupListRes> GetListEduLevel(LupListRqst request, ServerCallContext context)
    {
        var res = new LupListRes();
        var response = (await _med.Send(new EducationLevelQry())).ToList();
        if (response.Count <= 0)
        {
            var nList = new LupList { Id = null, Name = null };
            res.Res.Add(nList);
            return res;
        }

        foreach (var dbItem in response)
        {
            res.Res.Add(new LupList { Id = dbItem.Id.ToString(), Name = dbItem.Name });
        }

        return res;
    }

    public override async Task<LupRes> GetEduLevel(LupRqst request, ServerCallContext context)
    {
        var res = new LupRes();
        var response = await _med.Send(new EducationLevelGetQry { Id = Guid.Parse(request.Id) });
        if (response == null)
        {
            var nList = new LupList { Id = null, Name = null };
            res.Res = nList;
            return res;
        }
        var nList2 = new LupList { Id = response.Id.ToString(), Name = response.Name };
        res.Res = nList2;
        return res;
    }

    public override async Task<LupListRes> GetRelList(LupListRqst request, ServerCallContext context)
    {
        var res = new LupListRes();
        var response = (await _med.Send(new RelationQry())).ToList();
        if (response.Count <= 0)
        {
            var nList = new LupList { Id = null, Name = null };
            res.Res.Add(nList);
            return res;
        }

        foreach (var dbItem in response)
        {
            res.Res.Add(new LupList { Id = dbItem.Id.ToString(), Name = dbItem.Name });
        }

        return res;
    }

    public override async Task<LupRes> GetRel(LupRqst request, ServerCallContext context)
    {
        var res = new LupRes();
        var response = await _med.Send(new RelationGetQry { Id = Guid.Parse(request.Id) });
        if (response == null)
        {
            var nList = new LupList { Id = null, Name = null };
            res.Res = nList;
            return res;
        }
        var nList2 = new LupList { Id = response.Id.ToString(), Name = response.Name };
        res.Res = nList2;
        return res;
    }


}