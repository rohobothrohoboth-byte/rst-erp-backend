using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Svc.Lup.Queries;

namespace Svc.Lup.Controllers;

/// <summary>
/// LookUp Tables end points
/// <para>Will be converted to list in UI</para>
/// </summary>

//[Authorize]
[ApiController]
[Route("api/lup/v{version:apiVersion}")]
[ApiVersion("1.0")]
public class lupTablesController(IMediator med) : ControllerBase
{
    [HttpGet("AbsentReason")]
    public async Task<IActionResult> AbsentReason()
    {
        var lups = await med.Send(new AbsentReasonQry());
        return Ok(lups);
    }

    [HttpGet("AbsentReason/{id:guid}")]
    public async Task<IActionResult> AbsentReason(Guid id)
    {
        var lup = await med.Send(new AbsentReasonGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }
    
    [HttpGet("AdmissionType")]
    public async Task<IActionResult> AdmissionType()
    {
        var lups = await med.Send(new AdmissionTypeQry());
        return Ok(lups);
    }

    [HttpGet("AdmissionType/{id:guid}")]
    public async Task<IActionResult> AdmissionType(Guid id)
    {
        var lup = await med.Send(new AdmissionTypeGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }

    [HttpGet("AwardReason")]
    public async Task<IActionResult> AwardReason()
    {
        var lups = await med.Send(new AwardReasonQry());
        return Ok(lups);
    }

    [HttpGet("AwardReason/{id:guid}")]
    public async Task<IActionResult> AwardReason(Guid id)
    {
        var lup = await med.Send(new AwardReasonGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }

    [HttpGet("AwardType")]
    public async Task<IActionResult> AwardType()
    {
        var lups = await med.Send(new AwardTypeQry());
        return Ok(lups);
    }

    [HttpGet("AwardType/{id:guid}")]
    public async Task<IActionResult> AwardType(Guid id)
    {
        var lup = await med.Send(new AwardTypeGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }

    [HttpGet("CommitteeRole")]
    public async Task<IActionResult> CommitteeRole()
    {
        var lups = await med.Send(new CommitteeRoleQry());
        return Ok(lups);
    }

    [HttpGet("CommitteeRole/{id:guid}")]
    public async Task<IActionResult> CommitteeRole(Guid id)
    {
        var lup = await med.Send(new CommitteeRoleGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }

    [HttpGet("CriterionType")]
    public async Task<IActionResult> CriterionType()
    {
        var lups = await med.Send(new CriterionTypeQry());
        return Ok(lups);
    }

    [HttpGet("CriterionType/{id:guid}")]
    public async Task<IActionResult> CriterionType(Guid id)
    {
        var lup = await med.Send(new CriterionTypeGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }
    
    [HttpGet("EducationLevel")]
    public async Task<IActionResult> EducationLevel()
    {
        var lups = await med.Send(new EducationLevelQry());
        return Ok(lups);
    }

    [HttpGet("EducationLevel/{id:guid}")]
    public async Task<IActionResult> EducationLevel(Guid id)
    {
        var lup = await med.Send(new EducationLevelGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }

    [HttpGet("HolidayCondition")]
    public async Task<IActionResult> HolidayCondition()
    {
        var lups = await med.Send(new HolidayConditionQry());
        return Ok(lups);
    }

    [HttpGet("HolidayCondition/{id:guid}")]
    public async Task<IActionResult> HolidayCondition(Guid id)
    {
        var lup = await med.Send(new HolidayConditionGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }

    [HttpGet("LanguageSkill")]
    public async Task<IActionResult> LanguageSkill()
    {
        var lups = await med.Send(new LanguageSkillQry());
        return Ok(lups);
    }

    [HttpGet("LanguageSkill/{id:guid}")]
    public async Task<IActionResult> LanguageSkill(Guid id)
    {
        var lup = await med.Send(new LanguageSkillGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }

    [HttpGet("LeaveCondition")]
    public async Task<IActionResult> LeaveCondition()
    {
        var lups = await med.Send(new LeaveConditionQry());
        return Ok(lups);
    }

    [HttpGet("LeaveCondition/{id:guid}")]
    public async Task<IActionResult> LeaveCondition(Guid id)
    {
        var lup = await med.Send(new LeaveConditionGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }

    [HttpGet("LeaveType")]
    public async Task<IActionResult> LeaveType()
    {
        var lups = await med.Send(new LeaveTypeQry());
        return Ok(lups);
    }

    [HttpGet("LeaveType/{id:guid}")]
    public async Task<IActionResult> LeaveType(Guid id)
    {
        var lup = await med.Send(new LeaveTypeGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }

    [HttpGet("LeaveUsage")]
    public async Task<IActionResult> LeaveUsage()
    {
        var lups = await med.Send(new LeaveUsageQry());
        return Ok(lups);
    }

    [HttpGet("LeaveUsage/{id:guid}")]
    public async Task<IActionResult> LeaveUsage(Guid id)
    {
        var lup = await med.Send(new LeaveUsageGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }
    
    [HttpGet("MeasureTaken")]
    public async Task<IActionResult> MeasureTaken()
    {
        var lups = await med.Send(new MeasureTakenQry());
        return Ok(lups);
    }

    [HttpGet("MeasureTaken/{id:guid}")]
    public async Task<IActionResult> MeasureTaken(Guid id)
    {
        var lup = await med.Send(new MeasureTakenGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }

    [HttpGet("MeasureType")]
    public async Task<IActionResult> MeasureType()
    {
        var lups = await med.Send(new MeasureTypeQry());
        return Ok(lups);
    }

    [HttpGet("MeasureType/{id:guid}")]
    public async Task<IActionResult> MeasureType(Guid id)
    {
        var lup = await med.Send(new MeasureTypeGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }

    [HttpGet("PerformanceEvaluation")]
    public async Task<IActionResult> PerformanceEvaluation()
    {
        var lups = await med.Send(new PerformanceEvaluationQry());
        return Ok(lups);
    }

    [HttpGet("PerformanceEvaluation/{id:guid}")]
    public async Task<IActionResult> PerformanceEvaluation(Guid id)
    {
        var lup = await med.Send(new PerformanceEvaluationGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }

    [HttpGet("PositionChangeReason")]
    public async Task<IActionResult> PositionChangeReason()
    {
        var lups = await med.Send(new PositionChangeReasonQry());
        return Ok(lups);
    }

    [HttpGet("PositionChangeReason/{id:guid}")]
    public async Task<IActionResult> PositionChangeReason(Guid id)
    {
        var lup = await med.Send(new PositionChangeReasonGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }
    
    [HttpGet("Rating")]
    public async Task<IActionResult> Rating()
    {
        var lups = await med.Send(new RatingQry());
        return Ok(lups);
    }

    [HttpGet("Rating/{id:guid}")]
    public async Task<IActionResult> Rating(Guid id)
    {
        var lup = await med.Send(new RatingGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }
    
    [HttpGet("Relation")]
    public async Task<IActionResult> Relation()
    {
        var lups = await med.Send(new RelationQry());
        return Ok(lups);
    }

    [HttpGet("Relation/{id:guid}")]
    public async Task<IActionResult> Relation(Guid id)
    {
        var lup = await med.Send(new RelationGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }

    [HttpGet("ReportType")]
    public async Task<IActionResult> ReportType()
    {
        var lups = await med.Send(new ReportTypeQry());
        return Ok(lups);
    }

    [HttpGet("ReportType/{id:guid}")]
    public async Task<IActionResult> ReportType(Guid id)
    {
        var lup = await med.Send(new ReportTypeGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }

    [HttpGet("SalaryChangeReason")]
    public async Task<IActionResult> SalaryChangeReason()
    {
        var lups = await med.Send(new SalaryChangeReasonQry());
        return Ok(lups);
    }

    [HttpGet("SalaryChangeReason/{id:guid}")]
    public async Task<IActionResult> SalaryChangeReason(Guid id)
    {
        var lup = await med.Send(new SalaryChangeReasonGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }

    [HttpGet("SkillLevel")]
    public async Task<IActionResult> SkillLevel()
    {
        var lups = await med.Send(new SkillLevelQry());
        return Ok(lups);
    }

    [HttpGet("SkillLevel/{id:guid}")]
    public async Task<IActionResult> SkillLevel(Guid id)
    {
        var lup = await med.Send(new SkillLevelGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }

    [HttpGet("SponsorType")]
    public async Task<IActionResult> SponsorType()
    {
        var lups = await med.Send(new SponsorTypeQry());
        return Ok(lups);
    }

    [HttpGet("SponsorType/{id:guid}")]
    public async Task<IActionResult> SponsorType(Guid id)
    {
        var lup = await med.Send(new SponsorTypeGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }

    [HttpGet("TerminationReason")]
    public async Task<IActionResult> TerminationReason()
    {
        var lups = await med.Send(new TerminationReasonQry());
        return Ok(lups);
    }

    [HttpGet("TerminationReason/{id:guid}")]
    public async Task<IActionResult> TerminationReason(Guid id)
    {
        var lup = await med.Send(new TerminationReasonGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }

    [HttpGet("TrainingSource")]
    public async Task<IActionResult> TrainingSource()
    {
        var lups = await med.Send(new TrainingSourceQry());
        return Ok(lups);
    }

    [HttpGet("TrainingSource/{id:guid}")]
    public async Task<IActionResult> TrainingSource(Guid id)
    {
        var lup = await med.Send(new TrainingSourceGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }

    [HttpGet("TrainingType")]
    public async Task<IActionResult> TrainingType()
    {
        var lups = await med.Send(new TrainingTypeQry());
        return Ok(lups);
    }

    [HttpGet("TrainingType/{id:guid}")]
    public async Task<IActionResult> TrainingType(Guid id)
    {
        var lup = await med.Send(new TrainingTypeGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }

    [HttpGet("TransferReason")]
    public async Task<IActionResult> TransferReason()
    {
        var lups = await med.Send(new TransferReasonQry());
        return Ok(lups);
    }

    [HttpGet("TransferReason/{id:guid}")]
    public async Task<IActionResult> TransferReason(Guid id)
    {
        var lup = await med.Send(new TransferReasonGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }

    [HttpGet("VoucherType")]
    public async Task<IActionResult> VoucherType()
    {
        var lups = await med.Send(new VoucherTypeQry());
        return Ok(lups);
    }

    [HttpGet("VoucherType/{id:guid}")]
    public async Task<IActionResult> VoucherType(Guid id)
    {
        var lup = await med.Send(new VoucherTypeGetQry { Id = id });
        if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
        return Ok(lup);
    }

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}

    //[HttpGet("Add")]
    //public async Task<IActionResult> Add()
    //{
    //    var lups = await med.Send(new Qry());
    //    return Ok(lups);
    //}

    //[HttpGet("Add/{id:guid}")]
    //public async Task<IActionResult> Add(Guid id)
    //{
    //    var lup = await med.Send(new GetQry { Id = id });
    //    if (lup == null) { return NotFound(new { Error = $"Data with Id {id} not found" }); }
    //    return Ok(lup);
    //}



}
