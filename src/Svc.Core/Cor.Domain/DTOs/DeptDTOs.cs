using Cor.Domain.Enums;

namespace Cor.Domain.DTOs;
public class DeptListDto
{
    public Guid Id { get; set; }
    public Guid BranchId { get; set; }
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string Branch { get; set; } = default!;
    public string BranchAm { get; set; } = default!;
    public string CreatedAt { get; set; } = default!;
    public string CreatedAtAm { get; set; } = default!;
    public string ModifiedAt { get; set; } = default!;
    public string ModifiedAtAm { get; set; } = default!;
    public string RowVersion { get; set; } = default!;
}

public class AddDeptDto
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public DeptStat Status { get; set; } = DeptStat.Active;
    public Guid BranchId { get; set; }
}

public class EdtDeptDto
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public Guid BranchId { get; set; }
    public string RowVersion { get; set; } = default!;
}