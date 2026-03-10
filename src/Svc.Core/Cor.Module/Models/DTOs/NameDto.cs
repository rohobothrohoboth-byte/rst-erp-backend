namespace Cor.Module.Models.DTOs;

public class NameList
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}

public class NameAmList
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
}

public class BranchDeptList
{
    public Guid Id { get; set; }
    public Guid BranchId { get; set; }
    public string Dept { get; set; } = default!;
    public string Branch { get; set; } = default!;
}

public class DbcResDto
{
    public Guid DeptId { get; set; }
    public Guid BranchId { get; set; }
    public Guid CompId { get; set; }
}