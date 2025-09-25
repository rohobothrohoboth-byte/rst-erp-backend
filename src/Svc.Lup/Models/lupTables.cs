using System.ComponentModel.DataAnnotations.Schema;

namespace Svc.Lup.Models;

public class BaseCode
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
}
public class BaseName
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}

public class AbsentReason : BaseCode { }
public class AddressType : BaseName { }
public class AdmissionType : BaseName { }
public class AwardReason : BaseName { }
public class AwardType : BaseName { }
public class CommitteeRole : BaseName { }
public class CriterionType : BaseName { }
public class EmploymentNature : BaseName { }
public class EducationLevel : BaseName { }
public class HolidayCondition : BaseName { }
public class LanguageSkill : BaseName { }
public class LeaveCondition : BaseName { }
public class LeaveType : BaseName { }
public class LeaveUsage : BaseName { }
public class MaritalStatus : BaseName { }
public class MeasureTaken : BaseName { }
public class MeasureType : BaseName { }
public class PerformanceEvaluation : BaseName { }
public class PositionChangeReason : BaseName { }
public class ProfessionType : BaseName { }
public class Quarter : BaseName { }
public class Rating : BaseName { }
public class Region : BaseName { }
public class Relation : BaseName { }
public class ReportType : BaseName { }
public class SalaryChangeReason : BaseName { }
public class SkillLevel : BaseName { }
public class SponsorType : BaseName { }
public class TerminationReason : BaseName { }
public class TrainingSource : BaseName { }
public class TrainingType : BaseName { }
public class TransferReason : BaseName { }
public class VoucherType : BaseName { }
//public class LeaveUsage  : BaseName { }
//public class LeaveUsage  : BaseName { }
//public class LeaveUsage  : BaseName { }
//public class LeaveUsage  : BaseName { }




