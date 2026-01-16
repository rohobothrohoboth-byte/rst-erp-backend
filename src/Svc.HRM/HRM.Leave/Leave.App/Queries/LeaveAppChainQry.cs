using Common;
using Leave.App.Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Queries;

public class LeaveAppChainByPolicyIdQry : IRequest<List<LeaveAppChainListDto>> { public Guid Id { get; set; } }
public class LeaveAppChainByIdQry : IRequest<LeaveAppChainListDto?> { public Guid Id { get; set; } }
public class ActiveLeaveAppChainQry : IRequest<LeaveAppChainListDto?> { }

public class LeaveAppChainByPolicyIdHandler : IRequestHandler<LeaveAppChainByPolicyIdQry, List<LeaveAppChainListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public LeaveAppChainByPolicyIdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<LeaveAppChainListDto>> Handle(LeaveAppChainByPolicyIdQry request, CancellationToken cancellationToken)
    {
        var dbData = (await _unitOfWork.Repository<LeaveAppChain>().Find(c => c.LeavePolicyId == request.Id)).ToList();
        var dataL = new List<LeaveAppChainListDto>();
        if (dbData.Count <= 0) { return dataL; }
        var lvPo = await _unitOfWork.Repository<LeavePolicy>().GetById(request.Id);

        foreach (var data in dbData)
        {
            var c = new LeaveAppChainListDto
            {
                Id = data.Id,
                EffectiveFrom = data.EffectiveFrom,
                EffectiveTo = data.EffectiveTo,
                IsActive = data.IsActive,
                IsActiveStr = BoolToStr.FormatBool(data.IsActive),
                LeavePolicy = lvPo!.Name,
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = Convert.ToBase64String(data.RowVersion)
            };
            dataL.Add(c);
        }

        return dataL;
    }
}

public class LeaveAppChainByIdHandler : IRequestHandler<LeaveAppChainByIdQry, LeaveAppChainListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICorModClient _corModClient;

    public LeaveAppChainByIdHandler(IUnitOfWork unitOfWork, ICorModClient corModClient)
    {
        _unitOfWork = unitOfWork;
        _corModClient = corModClient;
    }

    public async Task<LeaveAppChainListDto?> Handle(LeaveAppChainByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<LeaveAppChain>().GetById(request.Id);
        if (data == null) { return null; }
        var lvPo = await _unitOfWork.Repository<LeavePolicy>().GetById(data.LeavePolicyId);

        var c = new LeaveAppChainListDto
        {
            Id = data.Id,
            EffectiveFrom = data.EffectiveFrom,
            EffectiveTo = data.EffectiveTo,
            IsActive = data.IsActive,
            IsActiveStr = BoolToStr.FormatBool(data.IsActive),
            LeavePolicy = lvPo!.Name,
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}

public class ActiveLeaveAppChainHandler : IRequestHandler<ActiveLeaveAppChainQry, LeaveAppChainListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICorModClient _corModClient;

    public ActiveLeaveAppChainHandler(IUnitOfWork unitOfWork, ICorModClient corModClient)
    {
        _unitOfWork = unitOfWork;
        _corModClient = corModClient;
    }

    public async Task<LeaveAppChainListDto?> Handle(ActiveLeaveAppChainQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<LeaveAppChain>().GetFoD(c => c.IsActive == true);
        if (data == null) { return null; }
        var lvPo = await _unitOfWork.Repository<LeavePolicy>().GetById(data.LeavePolicyId);

        var c = new LeaveAppChainListDto
        {
            Id = data.Id,
            EffectiveFrom = data.EffectiveFrom,
            EffectiveTo = data.EffectiveTo,
            IsActive = data.IsActive,
            IsActiveStr = BoolToStr.FormatBool(data.IsActive),
            LeavePolicy = lvPo!.Name,
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}