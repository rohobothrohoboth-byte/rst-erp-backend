namespace Cor.HRMM.Models.DTOs;

public class EducationQualListDto : BaseDto
{
    public string Name { get; set; } = default!;
}

public class EducationQualAddDto
{
    public string Name { get; set; } = default!;
}

public class EducationQualModDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string RowVersion { get; set; } = default!;
}