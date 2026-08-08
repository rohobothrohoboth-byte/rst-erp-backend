// Recruit.Domain/DTOs/VacancyDto.cs

using System.Text.Json.Serialization;

namespace Recruit.Domain.DTOs;

public class VacancyListDto
{
    [JsonIgnore]
    public DateTime PublishedDate { get; set; }
    [JsonIgnore]
    public DateTime DeadlineDate { get; set; }
    [JsonIgnore]
    public Guid JobReqId { get; set; }
    [JsonIgnore]
    public Guid PositionId { get; set; }
    [JsonIgnore]
    public Guid JgStepId { get; set; }
    [JsonIgnore]
    public Guid WorkforcePlanId { get; set; }
    [JsonIgnore]
    public Guid JobDecId { get; set; }
    [JsonIgnore]
    public Guid DepartmentId { get; set; }
    [JsonIgnore]
    public string EmpNature { get; set; } = default!;
    [JsonIgnore]
    public string PreGender { get; set; } = default!;
    [JsonIgnore]
    public string PostType { get; set; } = default!;

    public Guid Id { get; set; }
    public int NumOpen { get; set; } = 0;
    public string PostNumber { get; set; } = default!;
    public string Position { get; set; } = default!;
    public string Department { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string EmpNatureStr { get; set; } = default!;
    public string PreGenderStr { get; set; } = default!;
    public string JobGrade { get; set; } = default!;
    public bool IsInternal { get; set; }
    public string PostTypeStr { get; set; } = default!;
    public string DatePosted => $"{PublishedDate:MMMM dd, yyyy}";
    public string Deadline => $"{DeadlineDate:MMMM dd, yyyy}";
}


public class VacancyDetailDto
{
    [JsonIgnore]
    public DateTime PublishedDate { get; set; }
    [JsonIgnore]
    public DateTime DeadlineDate { get; set; }
    [JsonIgnore]
    public Guid JobReqId { get; set; }
    [JsonIgnore]
    public Guid PositionId { get; set; }
    [JsonIgnore]
    public Guid JgStepId { get; set; }
    [JsonIgnore]
    public Guid WorkforcePlanId { get; set; }
    [JsonIgnore]
    public Guid JobDecId { get; set; }
    [JsonIgnore]
    public Guid DepartmentId { get; set; }
    [JsonIgnore]
    public string EmpNature { get; set; } = default!;
    [JsonIgnore]
    public string PreGender { get; set; } = default!;
    [JsonIgnore]
    public string WorkArr { get; set; } = default!;
    [JsonIgnore]
    public string PostType { get; set; } = default!;

    // ? Raw data from database - these will be parsed into lists
    [JsonIgnore]
    public string? KeyRespo { get; set; }  // Changed from KeyRespoRaw to KeyRespo
    [JsonIgnore]
    public string? ReqQual { get; set; }   // Changed from ReqQualRaw to ReqQual
    [JsonIgnore]
    public string? KeySkills { get; set; } // Changed from KeySkillsRaw to KeySkills

    public Guid Id { get; set; }
    public int NumOpen { get; set; } = 0;
    public string PostNumber { get; set; } = default!;
    public string Position { get; set; } = default!;
    public string Department { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string JobGrade { get; set; } = default!;
    public string Salary { get; set; } = default!;
    public string EmpNatureStr { get; set; } = default!;
    public string PreGenderStr { get; set; } = default!;
    public string WorkArrStr { get; set; } = default!;
    public bool IsInternal { get; set; }
    public string PostTypeStr { get; set; } = default!;
    public string DatePosted => $"{PublishedDate:MMMM dd, yyyy}";
    public string Deadline => $"{DeadlineDate:MMMM dd, yyyy}";
    public string JobDesc { get; set; } = default!;

    // ? These will be populated after parsing the raw strings
    public List<string> KeyRespoList { get; set; } = [];
    public List<string> ReqQualList { get; set; } = [];
    public List<string> KeySkillsList { get; set; } = [];
}