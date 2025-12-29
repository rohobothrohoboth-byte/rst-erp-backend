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
        if (response != null && response.Length > 0)
        {
            res.Code = response;
        }

        return res;
    }



}