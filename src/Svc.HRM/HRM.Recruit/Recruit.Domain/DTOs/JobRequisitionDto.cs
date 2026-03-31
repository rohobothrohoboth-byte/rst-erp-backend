using System.Text.Json.Serialization;

namespace Recruit.Domain.DTOs;

public class JobReqListDto : BaseDto
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

public class JobReqAddDto
{
    //public string ReqNumber { get; set; } = default!;
    public string ReqReason { get; set; } = default!;
    public int ReqPositions { get; set; }
    public string BudgetCode { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public Guid PositionId { get; set; } // Cor.HRMM.Position
    public Guid JgStepId { get; set; } // Cor.HRMM.JgStep
    public Guid WorkforcePlanId { get; set; } // WorkforcePlan

    public string Title { get; set; } = default!;
    public string Desc { get; set; } = default!;
    public string Qualification { get; set; } = default!;
    public string KeySkills { get; set; } = default!;
    public string WorkLocation { get; set; } = default!;
    public string PreGender { get; set; } = default!; // enum.Gender
    public string ContractType { get; set; } = default!; // enum.EmpNature
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

    public string Title { get; set; } = default!;
    public string Desc { get; set; } = default!;
    public string Qualification { get; set; } = default!;
    public string KeySkills { get; set; } = default!;
    public string WorkLocation { get; set; } = default!;
    public string PreGender { get; set; } = default!; // enum.Gender
    public string ContractType { get; set; } = default!; // enum.EmpNature
    public string RowVersion { get; set; } = default!;
}