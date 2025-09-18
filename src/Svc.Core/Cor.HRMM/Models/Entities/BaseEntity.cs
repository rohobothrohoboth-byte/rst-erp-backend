using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cor.HRMM.Models.Entities;

public abstract class BaseEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    [JsonIgnore]
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    [JsonIgnore]
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; } = false;
    [Timestamp]
    public byte[] RowVersion { get; set; } = [];
}
