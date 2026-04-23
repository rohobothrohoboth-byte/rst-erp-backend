using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Recruit.Domain.DTOs;

public class JobAppListDto : BaseDto
{
    public DateTime AppliedDate { get; set; }
    public string Status { get; set; } = default!; // enum.ApplicationStatus(0/1)
     
    public string StatusStr { get; set; } = default!;
    public string Applicant { get; set; } = default!; // HRM.Profile.Employee OR Applicant
    public string JobPostingNum { get; set; } = default!;
    public string Position { get; set; } = default!;  // Cor.HRMM.Position
    public string Department { get; set; } = default!;  // Cor.Module.Department
    public string Period { get; set; } = default!; // Cor.Module.Period (or FiscalYear)
    public string AppliedDateStr => $"{AppliedDate:MMMM dd, yyyy}";
}

public class JobAppIntAddDto
{
    [JsonIgnore]
    public Guid EmployeeId { get; set; } // HRM.Profile.Employee
    public Guid JobPostingId { get; set; } // JobPosting
    public string CoverLetter { get; set; } = default!;
    public IFormFile? File { get; set; } = default!;
}

public class JobAppIntModDto
{
    public Guid Id { get; set; }
    public string CoverLetter { get; set; } = default!;
    public IFormFile? File { get; set; } = default!;
    public string RowVersion { get; set; } = default!;
}

public class JobAppIdDto
{
    public Guid? ApplicantId { get; set; } // Applicant
    public Guid? EmployeeId { get; set; } // HRM.Profile.Employee
    public Guid JobPostingId { get; set; } // JobPosting
    public Guid JobReqId { get; set; } // JobRequisition
    public Guid PositionId { get; set; } // Cor.HRMM.Position
    public Guid JgStepId { get; set; } // Cor.HRMM.JgStep
    public Guid WorkforcePlanId { get; set; } // WorkforcePlan
    public Guid JobDecId { get; set; } // JobDec
    public Guid DepartmentId { get; set; } // Cor.Module.Department
    public Guid? PeriodId { get; set; } // Cor.Module.Period (or FiscalYear)
}

public class ApplicantJoinRow
{
    public Guid Id { get; set; }
    public Guid PersonId { get; set; }

    public string FirstName { get; set; } = default!;
    public string MiddleName { get; set; } = default!;
    public string LastName { get; set; } = default!;
}

public class JobAppInfoDto
{
    public Guid Id { get; set; }
    public string PostType { get; set; } = default!; // enum.JobPostingType(0/1) 
    public Guid? ApplicantId { get; set; } // Applicant
    public Guid? EmployeeId { get; set; } // HRM.Profile.Employee
    public Guid PositionId { get; set; } // Cor.HRMM.Position
    public Guid JgStepId { get; set; } // Cor.HRMM.JgStep
    public Guid DepartmentId { get; set; } // Cor.Module.Department
    public Guid PeriodId { get; set; } // Cor.Module.Period (or FiscalYear)

    public Guid JobApplicationId { get; set; }
    public string Applicant { get; set; } = default!; // Applicant
    public string PostNumber { get; set; } = default!; // JobPosting
    public string ReqNumber { get; set; } = default!; // JobRequisition
    public string PlanCode { get; set; } = default!; // WorkforcePlan

    public string Title { get; set; } = default!;
    public string Desc { get; set; } = default!;
    public string Qualification { get; set; } = default!;
    public string KeySkills { get; set; } = default!;
    public string WorkLocation { get; set; } = default!;
    public string PreGender { get; set; } = default!; // enum.Gender
    public string ContractType { get; set; } = default!; // enum.EmpNature

    public string Position { get; set; } = default!; // Cor.HRMM.Position
    public string JgStep { get; set; } = default!; // Cor.HRMM.JgStep
    public string Department { get; set; } = default!; // Cor.Module.Department
    public string Period { get; set; } = default!; // Cor.Module.Period (or FiscalYear)
}