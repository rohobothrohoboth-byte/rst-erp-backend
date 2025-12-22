using Contracts;
using Grpc.Core;
using MediatR;
using Svc.Lup.Queries;

namespace Svc.Lup.gRPCService;

public class LupListService : LupService.LupServiceBase
{
    private readonly IMediator _med;
    public LupListService(IMediator med) => _med = med;

    public override async Task<GetRelListRes> GetRelList(GetRelListRqst request, ServerCallContext context)
    {
        var res = new GetRelListRes();
        var response = (await _med.Send(new RelationQry())).ToList();
        if (response.Count <= 0)
        {
            var nList = new NameList { Id = null, Name = null };
            res.Res.Add(nList);
            return res;
        }

        foreach (var dbItem in response)
        {
            res.Res.Add(new NameList { Id = dbItem.Id.ToString(), Name = dbItem.Name });
        }

        return res;
    }

    public override async Task<GetRelRes> GetRel(GetRelRqst request, ServerCallContext context)
    {
        var res = new GetRelRes();
        var response = await _med.Send(new RelationGetQry { Id = Guid.Parse(request.Id) });
        if (response == null)
        {
            var nList = new NameList { Id = null, Name = null };
            res.Res = nList;
            return res;
        }
        var nList2 = new NameList { Id = response.Id.ToString(), Name = response.Name };
        res.Res = nList2;
        return res;
    }


}