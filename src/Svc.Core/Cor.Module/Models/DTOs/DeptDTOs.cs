namespace Cor.Module.Models.DTOs;

public class DeptListDto : BaseDTO
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
     public Guid BranchId { get; set; }
    public string DeptStat { get; set; } = default!;  // enum.DeptStat (0/1)
    public string Branch { get; set; } = default!;
    public string BranchAm { get; set; } = default!;
    public string DeptStatStr { get; set; } = default!;
    public string? ManagerName { get; set; }
    public string? Description { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

public class AddDeptDto
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public Guid BranchId { get; set; }
    public string? ManagerName { get; set; }
    public string? Description { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

public class EditDeptDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string DeptStat { get; set; } = default!;  // enum.DeptStat (0/1)
    public Guid BranchId { get; set; }
    public string RowVersion { get; set; } = default!;
    public string? ManagerName { get; set; }
    public string? Description { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}
public class DeptDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }

    public Guid? BranchId { get; set; }
    public string? DeptStat { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public string? ManagerName { get; set; }
    public string? Description { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}






