using System.Text.Json.Serialization;

namespace Cor.HRMM.Models.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    [JsonIgnore]
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    [JsonIgnore]
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; } = false;
    public uint xmin { get; private set; }

    public void SetRowVersion(uint version)
    {
        xmin = version;
    }
}