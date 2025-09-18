using MediatR;
using Svc.Lup.Interfaces;
using Svc.Lup.Models;

namespace Svc.Lup.Queries;

public class AbsentReasonQryHandler : IRequestHandler<AbsentReasonQry, List<LupCodeListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public AbsentReasonQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupCodeListDto>> Handle(AbsentReasonQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<AbsentReason>().GetAll();
        return lups.Select(lup => new LupCodeListDto { Id = lup.Id, Name = lup.Name, Code = lup.Code, }).ToList();
    }
}

public class AddressTypeQryHandler : IRequestHandler<AddressTypeQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public AddressTypeQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(AddressTypeQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<AddressType>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class AdmissionTypeQryHandler : IRequestHandler<AdmissionTypeQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public AdmissionTypeQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(AdmissionTypeQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<AdmissionType>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class AwardReasonQryHandler : IRequestHandler<AwardReasonQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public AwardReasonQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(AwardReasonQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<AwardReason>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class AwardTypeQryHandler : IRequestHandler<AwardTypeQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public AwardTypeQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(AwardTypeQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<AwardReason>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class CommitteeRoleQryHandler : IRequestHandler<CommitteeRoleQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public CommitteeRoleQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(CommitteeRoleQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<CommitteeRole>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class CriterionTypeQryHandler : IRequestHandler<CriterionTypeQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public CriterionTypeQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(CriterionTypeQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<CriterionType>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class EmploymentNatureQryHandler : IRequestHandler<EmploymentNatureQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public EmploymentNatureQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(EmploymentNatureQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<EmploymentNature>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class HolidayConditionQryHandler : IRequestHandler<HolidayConditionQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public HolidayConditionQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(HolidayConditionQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<HolidayCondition>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class LanguageSkillQryHandler : IRequestHandler<LanguageSkillQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public LanguageSkillQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(LanguageSkillQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<LanguageSkill>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class LeaveConditionQryHandler : IRequestHandler<LeaveConditionQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public LeaveConditionQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(LeaveConditionQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<LeaveCondition>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class LeaveTypeQryHandler : IRequestHandler<LeaveTypeQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public LeaveTypeQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(LeaveTypeQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<LeaveType>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class LeaveUsageQryHandler : IRequestHandler<LeaveUsageQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public LeaveUsageQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(LeaveUsageQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<LeaveUsage>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class MaritalStatusQryHandler : IRequestHandler<MaritalStatusQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public MaritalStatusQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(MaritalStatusQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<MaritalStatus>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class MeasureTakenQryHandler : IRequestHandler<MeasureTakenQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public MeasureTakenQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(MeasureTakenQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<MeasureTaken>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class MeasureTypeQryHandler : IRequestHandler<MeasureTypeQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public MeasureTypeQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(MeasureTypeQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<MeasureType>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class PerformanceEvaluationQryHandler : IRequestHandler<PerformanceEvaluationQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public PerformanceEvaluationQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(PerformanceEvaluationQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<PerformanceEvaluation>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class PositionChangeReasonQryHandler : IRequestHandler<PositionChangeReasonQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public PositionChangeReasonQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(PositionChangeReasonQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<PositionChangeReason>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class PositionClassTypeQryHandler : IRequestHandler<PositionClassTypeQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public PositionClassTypeQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(PositionClassTypeQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<PositionClassType>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class QuarterQryHandler : IRequestHandler<QuarterQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public QuarterQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(QuarterQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<Quarter>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class RatingQryHandler : IRequestHandler<RatingQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public RatingQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(RatingQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<Rating>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class RegionQryHandler : IRequestHandler<RegionQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public RegionQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(RegionQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<Region>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class RelationQryHandler : IRequestHandler<RelationQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public RelationQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(RelationQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<Relation>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class ReportTypeQryHandler : IRequestHandler<ReportTypeQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public ReportTypeQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(ReportTypeQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<ReportType>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class SalaryChangeReasonQryHandler : IRequestHandler<SalaryChangeReasonQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public SalaryChangeReasonQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(SalaryChangeReasonQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<SalaryChangeReason>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class SkillLevelQryHandler : IRequestHandler<SkillLevelQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public SkillLevelQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(SkillLevelQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<SkillLevel>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class SponsorTypeQryHandler : IRequestHandler<SponsorTypeQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public SponsorTypeQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(SponsorTypeQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<SponsorType>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class TerminationReasonQryHandler : IRequestHandler<TerminationReasonQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public TerminationReasonQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(TerminationReasonQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<TerminationReason>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class TrainingSourceQryHandler : IRequestHandler<TrainingSourceQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public TrainingSourceQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(TrainingSourceQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<TrainingSource>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class TrainingTypeQryHandler : IRequestHandler<TrainingTypeQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public TrainingTypeQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(TrainingTypeQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<TrainingType>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class TransferReasonQryHandler : IRequestHandler<TransferReasonQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public TransferReasonQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(TransferReasonQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<TransferReason>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

public class VoucherTypeQryHandler : IRequestHandler<VoucherTypeQry, List<LupListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public VoucherTypeQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<LupListDto>> Handle(VoucherTypeQry request, CancellationToken cancellationToken)
    {
        var lups = await _unitOfWork.Repository<VoucherType>().GetAll();
        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
    }
}

//public class QryHandler : IRequestHandler<Qry, List<LupListDto>>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public QryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<List<LupListDto>> Handle(Qry request, CancellationToken cancellationToken)
//    {
//        var lups = await _unitOfWork.Repository<Add>().GetAll();
//        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
//    }
//}

//public class QryHandler : IRequestHandler<Qry, List<LupListDto>>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public QryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<List<LupListDto>> Handle(Qry request, CancellationToken cancellationToken)
//    {
//        var lups = await _unitOfWork.Repository<Add>().GetAll();
//        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
//    }
//}

//public class QryHandler : IRequestHandler<Qry, List<LupListDto>>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public QryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<List<LupListDto>> Handle(Qry request, CancellationToken cancellationToken)
//    {
//        var lups = await _unitOfWork.Repository<Add>().GetAll();
//        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
//    }
//}

//public class QryHandler : IRequestHandler<Qry, List<LupListDto>>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public QryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<List<LupListDto>> Handle(Qry request, CancellationToken cancellationToken)
//    {
//        var lups = await _unitOfWork.Repository<Add>().GetAll();
//        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
//    }
//}

//public class QryHandler : IRequestHandler<Qry, List<LupListDto>>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public QryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<List<LupListDto>> Handle(Qry request, CancellationToken cancellationToken)
//    {
//        var lups = await _unitOfWork.Repository<Add>().GetAll();
//        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
//    }
//}

//public class QryHandler : IRequestHandler<Qry, List<LupListDto>>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public QryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<List<LupListDto>> Handle(Qry request, CancellationToken cancellationToken)
//    {
//        var lups = await _unitOfWork.Repository<Add>().GetAll();
//        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
//    }
//}

//public class QryHandler : IRequestHandler<Qry, List<LupListDto>>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public QryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<List<LupListDto>> Handle(Qry request, CancellationToken cancellationToken)
//    {
//        var lups = await _unitOfWork.Repository<Add>().GetAll();
//        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
//    }
//}

//public class QryHandler : IRequestHandler<Qry, List<LupListDto>>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public QryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<List<LupListDto>> Handle(Qry request, CancellationToken cancellationToken)
//    {
//        var lups = await _unitOfWork.Repository<Add>().GetAll();
//        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
//    }
//}

//public class QryHandler : IRequestHandler<Qry, List<LupListDto>>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public QryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<List<LupListDto>> Handle(Qry request, CancellationToken cancellationToken)
//    {
//        var lups = await _unitOfWork.Repository<Add>().GetAll();
//        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
//    }
//}

//public class QryHandler : IRequestHandler<Qry, List<LupListDto>>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public QryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<List<LupListDto>> Handle(Qry request, CancellationToken cancellationToken)
//    {
//        var lups = await _unitOfWork.Repository<Add>().GetAll();
//        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
//    }
//}

//public class QryHandler : IRequestHandler<Qry, List<LupListDto>>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public QryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<List<LupListDto>> Handle(Qry request, CancellationToken cancellationToken)
//    {
//        var lups = await _unitOfWork.Repository<Add>().GetAll();
//        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
//    }
//}

//public class QryHandler : IRequestHandler<Qry, List<LupListDto>>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public QryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<List<LupListDto>> Handle(Qry request, CancellationToken cancellationToken)
//    {
//        var lups = await _unitOfWork.Repository<Add>().GetAll();
//        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
//    }
//}

//public class QryHandler : IRequestHandler<Qry, List<LupListDto>>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public QryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<List<LupListDto>> Handle(Qry request, CancellationToken cancellationToken)
//    {
//        var lups = await _unitOfWork.Repository<Add>().GetAll();
//        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
//    }
//}

//public class QryHandler : IRequestHandler<Qry, List<LupListDto>>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public QryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<List<LupListDto>> Handle(Qry request, CancellationToken cancellationToken)
//    {
//        var lups = await _unitOfWork.Repository<Add>().GetAll();
//        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
//    }
//}

//public class QryHandler : IRequestHandler<Qry, List<LupListDto>>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public QryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<List<LupListDto>> Handle(Qry request, CancellationToken cancellationToken)
//    {
//        var lups = await _unitOfWork.Repository<Add>().GetAll();
//        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
//    }
//}

//public class QryHandler : IRequestHandler<Qry, List<LupListDto>>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public QryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<List<LupListDto>> Handle(Qry request, CancellationToken cancellationToken)
//    {
//        var lups = await _unitOfWork.Repository<Add>().GetAll();
//        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
//    }
//}

//public class QryHandler : IRequestHandler<Qry, List<LupListDto>>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public QryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<List<LupListDto>> Handle(Qry request, CancellationToken cancellationToken)
//    {
//        var lups = await _unitOfWork.Repository<Add>().GetAll();
//        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
//    }
//}

//public class QryHandler : IRequestHandler<Qry, List<LupListDto>>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public QryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<List<LupListDto>> Handle(Qry request, CancellationToken cancellationToken)
//    {
//        var lups = await _unitOfWork.Repository<Add>().GetAll();
//        return lups.Select(lup => new LupListDto { Id = lup.Id, Name = lup.Name }).ToList();
//    }
//}

