namespace Cor.HRMM.Models.DTOs;

public class PositionListDto : BaseDto
{
    public Guid DepartmentId { get; set; } = default!; //Cor.Module.Department
    public string IsVacant { get; set; } = default!; //enum.YesNo (0/1)
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public int NoOfPosition { get; set; } = default!;
    public string IsVacantStr { get; set; } = default!;
    public string Department { get; set; } = default!;
}

public class PositionAddDto
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public int NoOfPosition { get; set; } = default!;
    public string IsVacant { get; set; } = default!; //enum.YesNo (0/1)
    public Guid DepartmentId { get; set; } = default!; //Cor.Module.Department
}

public class PositionModDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public int NoOfPosition { get; set; } = default!;
    public string IsVacant { get; set; } = default!; //enum.YesNo (0/1)
    public Guid DepartmentId { get; set; } = default!; //Cor.Module.Department
    public string RowVersion { get; set; } = default!;
}