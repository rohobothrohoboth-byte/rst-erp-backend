namespace Cor.HRMM.Models.Entities;

public class Position: BaseEntity
{
    public string Name { get; set; } = default!;
    public string NameA { get; set; } = default!;
    public int NoOfPosition { get; set; } = default!;
    public string IsVacant { get; set; } = default!; //YesNo
    public Guid OfficeId { get; set; } = default!; //Cor.Module.Office
}
