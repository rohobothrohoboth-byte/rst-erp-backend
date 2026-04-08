using System.Text.Json.Serialization;

namespace Recruit.Domain.DTOs;

public class WfpJobReqListDto : BaseDto
{
    [JsonIgnore]
    public Guid JgStepId { get; set; }
    [JsonIgnore]
    public Guid PositionId { get; set; }
    [JsonIgnore]
    public Guid JobDecId { get; set; } // JobDec
    [JsonIgnore]
    public string Status { get; set; } = default!; // enum.RequisitionStatus(0/1)

    public DateTime StartDate { get; set; }
    public string ReqNumber { get; set; } = default!;
    public string ReqReason { get; set; } = default!;
    public int ReqQuantity { get; set; }
    public string BudgetCode { get; set; } = default!;
    public string StatusStr { get; set; } = default!;
    public string Position { get; set; } = default!; // Cor.HRMM.Position
    public string JgStep { get; set; } = default!; // Cor.HRMM.JgStep
    public string StartDateStr => $"{StartDate:MMMM dd, yyyy}";
}

public class JobReqListDto : BaseDto
{
    [JsonIgnore]
    public Guid WorkforcePlanId { get; set; } // WorkforcePlan
    [JsonIgnore]
    public Guid JgStepId { get; set; }
    [JsonIgnore]
    public Guid PositionId { get; set; }
    [JsonIgnore]
    public Guid JobDecId { get; set; } // JobDec
    [JsonIgnore]
    public string Status { get; set; } = default!; // enum.RequisitionStatus(0/1)

    public DateTime StartDate { get; set; }
    public string ReqNumber { get; set; } = default!;
    public string ReqReason { get; set; } = default!;
    public int ReqQuantity { get; set; }
    public string BudgetCode { get; set; } = default!;
    public string StatusStr { get; set; } = default!;
    public string Position { get; set; } = default!; // Cor.HRMM.Position
    public string JgStep { get; set; } = default!; // Cor.HRMM.JgStep
    public string WfpCode { get; set; } = default!; // WorkforcePlan
    public string StartDateStr => $"{StartDate:MMMM dd, yyyy}";
}

public class JobReqAddDto
{
    public string ReqReason { get; set; } = default!;
    public int ReqPositions { get; set; }
    public string BudgetCode { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public Guid PositionId { get; set; } // Cor.HRMM.Position
    public Guid JgStepId { get; set; } // Cor.HRMM.JgStep
    public Guid WorkforcePlanId { get; set; } // WorkforcePlan

    public string KeyRespo { get; set; } = default!;
    public string Desc { get; set; } = default!;
    public string ReqQual { get; set; } = default!;
    public string KeySkills { get; set; } = default!;
    public string WorkLocation { get; set; } = default!;
    public string PreGender { get; set; } = default!; // enum.Gender
    public string EmpNature { get; set; } = default!; // enum.EmpNature
    public string WorkArr { get; set; } = default!; // enum.WorkArrangement
}

public class JobReqModDto
{
    public Guid Id { get; set; }
    public string ReqReason { get; set; } = default!;
    public int ReqPositions { get; set; }
    public string BudgetCode { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public Guid PositionId { get; set; } // Cor.HRMM.Position
    public Guid JgStepId { get; set; } // Cor.HRMM.JgStep

    public string KeyRespo { get; set; } = default!;
    public string Desc { get; set; } = default!;
    public string ReqQual { get; set; } = default!;
    public string KeySkills { get; set; } = default!;
    public string WorkLocation { get; set; } = default!;
    public string PreGender { get; set; } = default!; // enum.Gender
    public string EmpNature { get; set; } = default!; // enum.EmpNature
    public string WorkArr { get; set; } = default!; // enum.WorkArrangement
    public string RowVersion { get; set; } = default!;
}