namespace Cor.HRMM.Models.Entities;

public class PositionExperience : BaseEntity
{
    public Guid PositionId { get; set; } = default!; // Position
    public int YearOfService { get; set; } = 0;

    //******************************************//

    public Position Position { get; set; } = null!;
}
