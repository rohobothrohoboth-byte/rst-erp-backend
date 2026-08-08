namespace Cor.HRMM.Models.Entities;

public class PositionReq : BaseEntity
{
    public string Gender { get; set; } = default!; //enum.PositionGender
    public string ProfessionType { get; set; } = default!; //enum.ProfessionType
    public string SaturdayWorkOption { get; set; } = default!; //enum.WorkOption
    public string SundayWorkOption { get; set; } = default!; //enum.WorkOption
    public double WorkingHours { get; set; } = default!;
    public Guid PositionId { get; set; } = default!; // Position

    //******************************************//

    public Position Position { get; set; } = null!;
}
