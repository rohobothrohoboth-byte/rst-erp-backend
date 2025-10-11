using EthiopianCalendar;

namespace Module.Domain.DTOs;

public class BranchListDto : BaseDTO
{
    public Guid CompId { get; set; }
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string BranchType { get; set; } = default!;  // enum.BranchType (0/1)
    public string BranchStat { get; set; } = default!;  // enum.BranchStat (0/1)
    public string Comp { get; set; } = default!;
    public string CompAm { get; set; } = default!;
    public DateTime OpenDate { get; set; }
    public string BranchTypeStr { get; set; } = default!;
    public string BranchStatStr { get; set; } = default!;
    public string OpenDateStr => $"{OpenDate:MMMM dd, yyyy}";
    public string OpenDateStrAm => OpenDate.ToEthiopianDateString("MMMM dd, yyyy");
}

public class BranchCompListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
}

public class AddBranchDto
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string Location { get; set; } = default!;
    public DateTime OpenDate { get; set; } = DateTime.UtcNow;
    public string BranchType { get; set; } = default!;  // enum.BranchType (0/1)
    public Guid CompId { get; set; }
}

public class EditBranchDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string Location { get; set; } = default!;
    public DateTime OpenDate { get; set; } = DateTime.UtcNow;
    public string BranchType { get; set; } = default!;  // enum.BranchType (0/1)
    public string BranchStat { get; set; } = default!;  // enum.BranchStat (0/1)
    public Guid CompId { get; set; }
    public string RowVersion { get; set; } = default!;
}
