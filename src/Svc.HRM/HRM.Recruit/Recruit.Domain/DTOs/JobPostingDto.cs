namespace Recruit.Domain.DTOs;

public class JobPostingListDto : BaseDto
{
    public DateTime PublishedDate { get; set; } = DateTime.UtcNow;
    public DateTime DeadlineDate { get; set; }
    public DateTime? ClosedDate { get; set; }
    public string PostNumber { get; set; } = default!;
    public string ReqNumber { get; set; } = default!;
    public string StatusStr { get; set; } = default!;
    public string PostTypeStr { get; set; } = default!;
    public string ReqAppQuan { get; set; } = default!;
    public string PublishedDateStr => $"{PublishedDate:MMMM dd, yyyy}";
    public string DeadlineDateStr => $"{DeadlineDate:MMMM dd, yyyy}";
    public string ClosedDateStr => ClosedDate.HasValue ? $"{ClosedDate:MMMM dd, yyyy}" : "";
    public string Status { get; set; } = default!; // enum.PostingStatus(0/1)
    public string PostType { get; set; } = default!; // enum.JobPostingType(0/1) 
}

public class JobPostingAddDto
{
    public string PostType { get; set; } = default!; // enum.JobPostingType(0/1) 
    public DateTime DeadlineDate { get; set; }
    public Guid Id { get; set; } // JobRequisition/WorkforcePlan
}

public class JobPostingModDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = default!; // enum.PostStatus(0/1)
    public string PostType { get; set; } = default!; // enum.JobPostingType(0/1) 
    public DateTime DeadlineDate { get; set; }
    public string RowVersion { get; set; } = default!;
}

public class JobPostingViewDto : BaseDto
{
    public DateTime PublishedDate { get; set; } = DateTime.UtcNow;
    public DateTime DeadlineDate { get; set; }
    public DateTime? ClosedDate { get; set; }
    public string PublishedDateStr => $"{PublishedDate:MMMM dd, yyyy}";
    public string DeadlineDateStr => $"{DeadlineDate:MMMM dd, yyyy}";
    public string ClosedDateStr => ClosedDate.HasValue ? $"{ClosedDate:MMMM dd, yyyy}" : "";
    public string PostNumber { get; set; } = default!;
    public string ReqNumber { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string PostType { get; set; } = default!;
    public string ReqReason { get; set; } = default!;
    public int ReqQuantity { get; set; }
    public int AppQuantity { get; set; }
    public string BudgetCode { get; set; } = default!;
    public string Position { get; set; } = default!; // Cor.HRMM.Position
    public string JgStep { get; set; } = default!; // Cor.HRMM.JgStep
    public string Department { get; set; } = default!;  // Cor.Module.Department
    public string Period { get; set; } = default!;  // Cor.Module.Period (or FiscalYear)
    public string RequistionBy { get; set; } = default!; // HRM.Profile.Employee

    public string Title { get; set; } = default!;
    public string Desc { get; set; } = default!;
    public string Qualification { get; set; } = default!;
    public string KeySkills { get; set; } = default!;
    public string WorkLocation { get; set; } = default!;
    public string PreGender { get; set; } = default!; // enum.Gender
    public string ContractType { get; set; } = default!; // enum.EmpNature
}