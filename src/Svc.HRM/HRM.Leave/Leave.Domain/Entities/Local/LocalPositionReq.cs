using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace Leave.Domain.Entities.Local;

public class LocalPositionReq : LocalBaseEntity
{
    public string Gender { get; set; } = default!; //enum.PositionGender
    public string ProfessionType { get; set; } = default!; //enum.ProfessionType
    public string SaturdayWorkOption { get; set; } = default!; //enum.WorkOption
    public string SundayWorkOption { get; set; } = default!; //enum.WorkOption
    public double WorkingHours { get; set; } = default!;
    public Guid PositionId { get; set; } = default!; // Position

    //******************************************//

    public LocalPosition Position { get; set; } = null!;
}
