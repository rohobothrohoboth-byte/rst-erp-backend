using EthiopianCalendar;

namespace Cor.Module.Models.DTOs;

// ? This DTO must exist
public class BranchDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string BranchType { get; set; } = default!;
    public string BranchStat { get; set; } = default!;
    public Guid CompId { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? ManagerName { get; set; }
    public DateTime OpenDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class BranchListDto : BaseDTO
{
    public Guid CompId { get; set; }
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string BranchType { get; set; } = default!;
    public string BranchStat { get; set; } = default!;
    public string Comp { get; set; } = default!;
    public string CompAm { get; set; } = default!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? ManagerName { get; set; }
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
    public string? Code { get; set; }
    public string Location { get; set; } = default!;
    public DateTime OpenDate { get; set; } = DateTime.UtcNow;
    public string BranchType { get; set; } = default!;
    public string BranchStat { get; set; } = default!;
    public Guid CompId { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? ManagerName { get; set; }
}

public class EditBranchDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string? Code { get; set; }
    public string Location { get; set; } = default!;
    public DateTime OpenDate { get; set; } = DateTime.UtcNow;
    public string BranchType { get; set; } = default!;
    public string BranchStat { get; set; } = default!;
    public Guid CompId { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? ManagerName { get; set; }
    public string RowVersion { get; set; } = default!;
}