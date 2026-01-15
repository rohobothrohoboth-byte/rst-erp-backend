using Common;
using Leave.App.Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using Leave.Domain.Enums;
using MediatR;

namespace Leave.App.Queries;

public class PolicyConfigByPolicyIdQry : IRequest<List<LeavePolicyConfigListDto>> { public Guid Id { get; set; } }
public class LeavePolicyConfigByIdQry : IRequest<LeavePolicyConfigListDto?> { public Guid Id { get; set; } }

public class PolicyConfigByPolicyIdHandler : IRequestHandler<PolicyConfigByPolicyIdQry, List<LeavePolicyConfigListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICorModClient _corModClient;

    public PolicyConfigByPolicyIdHandler(IUnitOfWork unitOfWork, ICorModClient corModClient)
    {
        _unitOfWork = unitOfWork;
        _corModClient = corModClient;
    }

    public async Task<List<LeavePolicyConfigListDto>> Handle(PolicyConfigByPolicyIdQry request, CancellationToken cancellationToken)
    {
        var dbData = (await _unitOfWork.Repository<LeavePolicyConfig>().Find(c => c.LeavePolicyId == request.Id)).ToList();
        var dataL = new List<LeavePolicyConfigListDto>();
        if (dbData.Count <= 0) { return dataL; }
        var lvPo = await _unitOfWork.Repository<LeavePolicy>().GetById(request.Id);
        var fyL = await _corModClient.GetListFiscalYear(cancellationToken);

        foreach (var data in dbData)
        {
            var fy = fyL.Res.FirstOrDefault(f => f.Id == data.FiscalYearId.ToString());
            var c = new LeavePolicyConfigListDto
            {
                Id = data.Id,
                AnnualEntitlement = data.AnnualEntitlement,
                AccrualFrequency = data.AccrualFrequency,
                AccrualRate = data.AccrualRate,
                MaxDaysPerReq = data.MaxDaysPerReq,
                MaxCarryOverDays = data.MaxCarryOverDays,
                MinServiceMonths = data.MinServiceMonths,
                IsActive = data.IsActive,
                AnnualEntitlementStr = $"{data.AnnualEntitlement} day/s",
                AccrualFrequencyStr = ((AccrualFrequency)Enum.Parse(typeof(AccrualFrequency), data.AccrualFrequency)).ToDisplayName(),
                AccrualRateStr = $"{data.AccrualRate} day/s",
                MaxDaysPerReqStr = $"{data.MaxDaysPerReq} day/s",
                MaxCarryOverDaysStr = $"{data.MaxCarryOverDays} day/s",
                MinServiceMonthsStr = $"{data.MinServiceMonths} month/s",
                IsActiveStr = BoolToStr.FormatBool(data.IsActive),
                LeavePolicy = lvPo!.Name,
                FiscalYear = fy!.Name != null ? fy.Name : "NOT AVAILABLE",
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

public class LeavePolicyConfigByIdHandler : IRequestHandler<LeavePolicyConfigByIdQry, LeavePolicyConfigListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICorModClient _corModClient;

    public LeavePolicyConfigByIdHandler(IUnitOfWork unitOfWork, ICorModClient corModClient)
    {
        _unitOfWork = unitOfWork;
        _corModClient = corModClient;
    }

    public async Task<LeavePolicyConfigListDto?> Handle(LeavePolicyConfigByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<LeavePolicyConfig>().GetById(request.Id);
        if (data == null) { return null; }
        var lvPo = await _unitOfWork.Repository<LeavePolicy>().GetById(data.LeavePolicyId);
        var fy = await _corModClient.GetFiscalYear(data.FiscalYearId.ToString(), cancellationToken);

        var c = new LeavePolicyConfigListDto
        {
            Id = data.Id,
            AnnualEntitlement = data.AnnualEntitlement,
            AccrualFrequency = data.AccrualFrequency,
            AccrualRate = data.AccrualRate,
            MaxDaysPerReq = data.MaxDaysPerReq,
            MaxCarryOverDays = data.MaxCarryOverDays,
            MinServiceMonths = data.MinServiceMonths,
            IsActive = data.IsActive,
            AnnualEntitlementStr = $"{data.AnnualEntitlement} day/s",
            AccrualFrequencyStr = ((AccrualFrequency)Enum.Parse(typeof(AccrualFrequency), data.AccrualFrequency)).ToDisplayName(),
            AccrualRateStr = $"{data.AccrualRate} day/s",
            MaxDaysPerReqStr = $"{data.MaxDaysPerReq} day/s",
            MaxCarryOverDaysStr = $"{data.MaxCarryOverDays} day/s",
            MinServiceMonthsStr = $"{data.MinServiceMonths} month/s",
            IsActiveStr = BoolToStr.FormatBool(data.IsActive),
            LeavePolicy = lvPo!.Name,
            FiscalYear = fy.Res.Name != null ? fy.Res.Name : "NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}
