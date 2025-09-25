namespace Cor.HRMM.Models.Entities;

public class PositionReq : BaseEntity
{
    public string Gender { get; set; } = default!; //PositionGender
    public string SaturdayWorkOption { get; set; } = default!; //WorkOption
    public string SundayWorkOption { get; set; } = default!; //WorkOption
    public decimal WorkingHours { get; set; } = default!;
    public Guid ProfessionType { get; set; } = default!; //Lup.ProfessionType
    public Guid PositionId { get; set; } = default!; // Position

    //******************************************//

    public Position Position { get; set; } = null!;
}