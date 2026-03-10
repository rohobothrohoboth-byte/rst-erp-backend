using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Recruit.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    [JsonIgnore]
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    [JsonIgnore]
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; } = false;
    [Timestamp]
    public byte[] RowVersion { get; set; } = [];
}