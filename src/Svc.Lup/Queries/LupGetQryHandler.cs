using MediatR;
using Svc.Lup.Interfaces;
using Svc.Lup.Models;

namespace Svc.Lup.Queries;

public class AbsentReasonGetQryHandler : IRequestHandler<AbsentReasonGetQry, LupCodeListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public AbsentReasonGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupCodeListDto?> Handle(AbsentReasonGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<AbsentReason>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupCodeListDto { Id = lup.Id, Name = lup.Name, Code = lup.Code };
        return l;
    }
}

public class AdmissionTypeGetQryHandler : IRequestHandler<AdmissionTypeGetQry, LupListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public AdmissionTypeGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupListDto?> Handle(AdmissionTypeGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<AdmissionType>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
        return l;
    }
}

public class AwardReasonGetQryHandler : IRequestHandler<AwardReasonGetQry, LupListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public AwardReasonGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupListDto?> Handle(AwardReasonGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<AwardReason>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
        return l;
    }
}

public class AwardTypeGetQryHandler : IRequestHandler<AwardTypeGetQry, LupListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public AwardTypeGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupListDto?> Handle(AwardTypeGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<AwardType>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
        return l;
    }
}

public class CommitteeRoleGetQryHandler : IRequestHandler<CommitteeRoleGetQry, LupListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public CommitteeRoleGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupListDto?> Handle(CommitteeRoleGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<CommitteeRole>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
        return l;
    }
}

public class CriterionTypeGetQryHandler : IRequestHandler<CriterionTypeGetQry, LupListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public CriterionTypeGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupListDto?> Handle(CriterionTypeGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<CriterionType>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
        return l;
    }
}

public class EducationLevelGetQryHandler : IRequestHandler<EducationLevelGetQry, LupListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public EducationLevelGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupListDto?> Handle(EducationLevelGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<EducationLevel>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
        return l;
    }
}

public class HolidayConditionGetQryHandler : IRequestHandler<HolidayConditionGetQry, LupListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public HolidayConditionGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupListDto?> Handle(HolidayConditionGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<HolidayCondition>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
        return l;
    }
}

public class LanguageSkillGetQryHandler : IRequestHandler<LanguageSkillGetQry, LupListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public LanguageSkillGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupListDto?> Handle(LanguageSkillGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<LanguageSkill>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
        return l;
    }
}

public class LeaveConditionGetQryHandler : IRequestHandler<LeaveConditionGetQry, LupListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public LeaveConditionGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupListDto?> Handle(LeaveConditionGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<LeaveCondition>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
        return l;
    }
}

public class LeaveTypeGetQryHandler : IRequestHandler<LeaveTypeGetQry, LupListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public LeaveTypeGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupListDto?> Handle(LeaveTypeGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<LeaveType>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
        return l;
    }
}

public class LeaveUsageGetQryHandler : IRequestHandler<LeaveUsageGetQry, LupListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public LeaveUsageGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupListDto?> Handle(LeaveUsageGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<LeaveUsage>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
        return l;
    }
}

public class MeasureTakenGetQryHandler : IRequestHandler<MeasureTakenGetQry, LupListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public MeasureTakenGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupListDto?> Handle(MeasureTakenGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<MeasureTaken>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
        return l;
    }
}

public class MeasureTypeGetQryHandler : IRequestHandler<MeasureTypeGetQry, LupListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public MeasureTypeGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupListDto?> Handle(MeasureTypeGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<MeasureType>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
        return l;
    }
}

public class PerformanceEvaluationGetQryHandler : IRequestHandler<PerformanceEvaluationGetQry, LupListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public PerformanceEvaluationGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupListDto?> Handle(PerformanceEvaluationGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<PerformanceEvaluation>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
        return l;
    }
}

public class PositionChangeReasonGetQryHandler : IRequestHandler<PositionChangeReasonGetQry, LupListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public PositionChangeReasonGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupListDto?> Handle(PositionChangeReasonGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<PositionChangeReason>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
        return l;
    }
}

public class RatingGetQryHandler : IRequestHandler<RatingGetQry, LupListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public RatingGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupListDto?> Handle(RatingGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<Rating>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
        return l;
    }
}

public class RelationGetQryHandler : IRequestHandler<RelationGetQry, LupListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public RelationGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupListDto?> Handle(RelationGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<Relation>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
        return l;
    }
}

public class ReportTypeGetQryHandler : IRequestHandler<ReportTypeGetQry, LupListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public ReportTypeGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupListDto?> Handle(ReportTypeGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<ReportType>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
        return l;
    }
}

public class SalaryChangeReasonGetQryHandler : IRequestHandler<SalaryChangeReasonGetQry, LupListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public SalaryChangeReasonGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupListDto?> Handle(SalaryChangeReasonGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<SalaryChangeReason>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
        return l;
    }
}

public class SkillLevelGetQryHandler : IRequestHandler<SkillLevelGetQry, LupListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public SkillLevelGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupListDto?> Handle(SkillLevelGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<SkillLevel>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
        return l;
    }
}

public class SponsorTypeGetQryHandler : IRequestHandler<SponsorTypeGetQry, LupListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public SponsorTypeGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupListDto?> Handle(SponsorTypeGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<SponsorType>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
        return l;
    }
}

public class TerminationReasonGetQryHandler : IRequestHandler<TerminationReasonGetQry, LupListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public TerminationReasonGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupListDto?> Handle(TerminationReasonGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<TerminationReason>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
        return l;
    }
}

public class TrainingSourceGetQryHandler : IRequestHandler<TrainingSourceGetQry, LupListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public TrainingSourceGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupListDto?> Handle(TrainingSourceGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<TrainingSource>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
        return l;
    }
}

public class TrainingTypeGetQryHandler : IRequestHandler<TrainingTypeGetQry, LupListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public TrainingTypeGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupListDto?> Handle(TrainingTypeGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<TrainingType>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
        return l;
    }
}

public class TransferReasonGetQryHandler : IRequestHandler<TransferReasonGetQry, LupListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public TransferReasonGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupListDto?> Handle(TransferReasonGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<TransferReason>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
        return l;
    }
}

public class VoucherTypeGetQryHandler : IRequestHandler<VoucherTypeGetQry, LupListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public VoucherTypeGetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<LupListDto?> Handle(VoucherTypeGetQry request, CancellationToken cancellationToken)
    {
        var lup = await _unitOfWork.Repository<VoucherType>().GetById(request.Id);
        if (lup == null) { return null; }
        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
        return l;
    }
}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}

//public class GetQryHandler : IRequestHandler<GetQry, LupListDto?>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public GetQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
//    public async Task<LupListDto?> Handle(GetQry request, CancellationToken cancellationToken)
//    {
//        var lup = await _unitOfWork.Repository<Add>().GetById(request.Id);
//        if (lup == null) { return null; }
//        var l = new LupListDto { Id = lup.Id, Name = lup.Name };
//        return l;
//    }
//}








