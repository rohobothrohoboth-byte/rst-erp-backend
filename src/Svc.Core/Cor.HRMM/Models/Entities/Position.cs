namespace Cor.HRMM.Models.Entities;

public class Position: BaseEntity
{
    public string Code { get; set; } = default!;
    public string IsVacant { get; set; } = default!; //YesNo
    public string SaturdayWorkOption { get; set; } = default!; //WorkOption
    public string SundayWorkOption { get; set; } = default!; //WorkOption
    public Guid PositionClassId { get; set; } = default!; //PositionClass

    //******************************************//

    public PositionClass PositionClass { get; set; } = null!;
}
