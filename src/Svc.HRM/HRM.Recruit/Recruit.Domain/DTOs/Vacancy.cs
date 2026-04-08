using System.Text.Json.Serialization;

namespace Recruit.Domain.DTOs;

public class VacancyListDto
{
    [JsonIgnore]
    public DateTime PublishedDate { get; set; }
    [JsonIgnore]
    public DateTime DeadlineDate { get; set; }
    [JsonIgnore]
    public Guid JobReqId { get; set; } // JobRequisition
    [JsonIgnore]
    public Guid PositionId { get; set; } // Cor.HRMM.Position
    [JsonIgnore]
    public Guid JgStepId { get; set; } // Cor.HRMM.JgStep
    [JsonIgnore]
    public Guid WorkforcePlanId { get; set; } // WorkforcePlan
    [JsonIgnore]
    public Guid JobDecId { get; set; } // JobDec
    [JsonIgnore]
    public Guid DepartmentId { get; set; } // Cor.Module.Department
    [JsonIgnore]
    public string EmpNature { get; set; } = default!; // enum.EmpNature
    [JsonIgnore]
    public string PreGender { get; set; } = default!; // enum.PositionGender


    public Guid Id { get; set; }
    public int NumOpen { get; set; } = 0;
    public string PostNumber { get; set; } = default!;
    public string Position { get; set; } = default!;
    public string Department { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string EmpNatureStr { get; set; } = default!;
    public string PreGenderStr { get; set; } = default!;
    public string JobGrade { get; set; } = default!;
    public string DatePosted => $"{PublishedDate:MMMM dd, yyyy}";
    public string Deadline => $"{DeadlineDate:MMMM dd, yyyy}";
}

public class VacancyDetailDto
{
    public Guid PostId { get; set; }
    public int NumOpen { get; set; } = 0;
    public string Position { get; set; } = default!;
    public string Department { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string EmpNature { get; set; } = default!; // enum.EmpNature
    public string PreGender { get; set; } = default!; // enum.Gender
    public string WorkArr { get; set; } = default!; // enum.WorkArrangement
    public string DatePosted { get; set; } = default!;
    public string Deadline { get; set; } = default!;

    public string JobDesc { get; set; } = default!;
    public string KeyRespo { get; set; } = default!;
    public string ReqQual { get; set; } = default!;
    public string KeySkills { get; set; } = default!;
    public string JobGrade { get; set; } = default!;
}


