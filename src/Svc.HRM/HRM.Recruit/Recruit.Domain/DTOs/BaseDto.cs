using System.Text.Json.Serialization;
using EthiopianCalendar;

namespace Recruit.Domain.DTOs;

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

    // ? Fixed: Check if DateAdd is valid before formatting
    public string CreatedAt
    {
        get
        {
            if (DateAdd == DateTime.MinValue || DateAdd.Year < 1900)
                return "N/A";

            return $"{DateAdd:MMMM dd, yyyy}";
        }
    }

    // ? Fixed: Check if DateAdd is valid for Ethiopian conversion
    public string CreatedAtAm
    {
        get
        {
            if (DateAdd == DateTime.MinValue || DateAdd.Year < 1900)
                return "N/A";

            try
            {
                return DateAdd.ToEthiopianDateString("MMMM dd, yyyy");
            }
            catch
            {
                return $"{DateAdd:MMMM dd, yyyy}";
            }
        }
    }

    // ? Fixed: Check if DateMod has value and is valid
    public string ModifiedAt
    {
        get
        {
            if (!DateMod.HasValue || DateMod.Value == DateTime.MinValue || DateMod.Value.Year < 1900)
                return "N/A";

            return $"{DateMod.Value:MMMM dd, yyyy}";
        }
    }

    // ? Fixed: Check if DateMod has value and is valid for Ethiopian conversion
    public string ModifiedAtAm
    {
        get
        {
            if (!DateMod.HasValue || DateMod.Value == DateTime.MinValue || DateMod.Value.Year < 1900)
                return "N/A";

            try
            {
                return DateMod.Value.ToEthiopianDateString("MMMM dd, yyyy");
            }
            catch
            {
                return $"{DateMod.Value:MMMM dd, yyyy}";
            }
        }
    }
}