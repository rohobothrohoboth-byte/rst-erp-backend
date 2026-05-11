using System.Text.Json.Serialization;

namespace Profile.Domain.DTOs;

public abstract class RawBaseDto
{
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; } = default!;
    public string RowVersion { get; set; } = default!; 
    [JsonIgnore]
    public uint xmin { get; internal set; }
}
