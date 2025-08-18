using EthiopianCalendar;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cor.Domain.Entities;
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
    public byte[] RowVersion { get; set; }

    public string CreatedAt => $"{DateAdd:MMMM dd, yyyy}";
    public string ModifiedAt => DateMod.HasValue ? $"{DateMod:MMMM dd, yyyy}" : "";

    public string CreatedAtAm => DateAdd.ToEthiopianDateString("MMMM dd, yyyy");
    public string ModifiedAtAm => DateMod.HasValue ? DateMod.Value.ToEthiopianDateString("MMMM dd, yyyy") : "";
}