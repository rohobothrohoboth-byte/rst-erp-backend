namespace Cor.Domain.DTOs;

public class DeptListDto : BaseDTO
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string DeptStat { get; set; } = default!;
    public string Branch { get; set; } = default!;
    public string BranchAm { get; set; } = default!;
}

public class AddDeptDto
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string DeptStat { get; set; } = default!;
    public Guid BranchId { get; set; }
}

public class EdtDeptDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string DeptStat { get; set; } = default!;
    public Guid BranchId { get; set; }
    public string RowVersion { get; set; } = default!;
}