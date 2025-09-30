
namespace Cor.HRMM.Models.DTOs;

public class JobGradeListDto : BaseDto
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
}

public class JobGradeAddDto
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
}

public class JobGradeModDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string RowVersion { get; set; } = default!;
}