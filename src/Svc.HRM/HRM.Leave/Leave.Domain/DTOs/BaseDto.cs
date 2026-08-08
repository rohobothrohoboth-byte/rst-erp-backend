using EthiopianCalendar;
using System.Text.Json.Serialization;

namespace Leave.Domain.DTOs;

public abstract class RawBaseDto
{
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; } = default!;
    public string RowVersion { get; set; } = default!;
    [JsonIgnore]
    public uint xmin { get; internal set; }
}
public abstract class BaseDto
{
    public Guid Id { get; set; }
    [JsonIgnore]
    public DateTime DateAdd { get; set; }
    [JsonIgnore]
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; } = default!;
    public string RowVersion { get; set; } = default!;
    [JsonIgnore]
    public uint xmin { get; internal set; }
    public string CreatedAt => $"{DateAdd:MMMM dd, yyyy}";

    public string CreatedAtAm
    {
        get
        {
            try
            {
                return DateAdd.ToEthiopianDateString("MMMM dd, yyyy");
            }
            catch
            {
                return "";
            }
        }
    }

    public string ModifiedAt => DateMod.HasValue ? $"{DateMod:MMMM dd, yyyy}" : "";

    public string ModifiedAtAm
    {
        get
        {
            try
            {
                return DateMod.HasValue ? DateMod.Value.ToEthiopianDateString("MMMM dd, yyyy") : "";
            }
            catch
            {
                return "";
            }
        }
    }
}