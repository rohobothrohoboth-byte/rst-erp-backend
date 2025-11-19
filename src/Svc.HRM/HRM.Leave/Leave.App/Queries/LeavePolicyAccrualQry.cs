using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using Leave.Domain.Enums;
using MediatR;

namespace Leave.App.Queries;

public class LeavePolicyAccrualAllQry : IRequest<List<LeavePolicyAccrualListDto>> { }
public class LeavePolicyAccrualByIdQry : IRequest<LeavePolicyAccrualListDto?> { public Guid Id { get; set; } }

public class LeavePolicyAccrualAllQryHandler : IRequestHandler<LeavePolicyAccrualAllQry, List<LeavePolicyAccrualListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public LeavePolicyAccrualAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<LeavePolicyAccrualListDto>> Handle(LeavePolicyAccrualAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<LeavePolicyAccrual>().GetAll();
        var dataL = new List<LeavePolicyAccrualListDto>();
        var lpoL = await _unitOfWork.Repository<LeavePolicy>().GetAll();

        foreach (var data in dbData)
        {
            var lpo = lpoL.FirstOrDefault(t => t.Id == data.LeavePolicyId);
            var c = new LeavePolicyAccrualListDto
            {
                Id = data.Id,
                LeavePolicyId = data.LeavePolicyId,
                Entitlement = data.Entitlement,
                Frequency = data.Frequency,
                AccrualRate = data.AccrualRate,
                MinServiceMonths = data.MinServiceMonths,
                MaxCarryoverDays = data.MaxCarryoverDays,
                CarryoverExpiryDays = data.CarryoverExpiryDays,
                FrequencyStr = ((AccrualFrequency)Enum.Parse(typeof(AccrualFrequency), data.Frequency)).ToDisplayName(),
                LeavePolicy = lpo != null ? lpo.Name : "NOT AVAILABLE",
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

public class LeavePolicyAccrualByIdQryHandler : IRequestHandler<LeavePolicyAccrualByIdQry, LeavePolicyAccrualListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public LeavePolicyAccrualByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<LeavePolicyAccrualListDto?> Handle(LeavePolicyAccrualByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<LeavePolicyAccrual>().GetById(request.Id);
        if (data == null) { return null; }
        var lpo = await _unitOfWork.Repository<LeavePolicy>().GetById(data.LeavePolicyId);

        var c = new LeavePolicyAccrualListDto
        {
            Id = data.Id,
            LeavePolicyId = data.LeavePolicyId,
            Entitlement = data.Entitlement,
            Frequency = data.Frequency,
            AccrualRate = data.AccrualRate,
            MinServiceMonths = data.MinServiceMonths,
            MaxCarryoverDays = data.MaxCarryoverDays,
            CarryoverExpiryDays = data.CarryoverExpiryDays,
            FrequencyStr = ((AccrualFrequency)Enum.Parse(typeof(AccrualFrequency), data.Frequency)).ToDisplayName(),
            LeavePolicy = lpo != null ? lpo.Name : "NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}