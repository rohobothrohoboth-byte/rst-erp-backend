namespace Cor.HRMM.Models.Entities;

public class PositionExp: BaseEntity
{
    public int SamePosExp { get; set; } = 0;
    public int OtherPosExp { get; set; } = 0;
    public int MinAge { get; set; } = default!;
    public int MaxAge { get; set; } = default!;
    public Guid PositionId { get; set; } = default!; // Position

    //******************************************//

    public Position Position { get; set; } = null!;
}

