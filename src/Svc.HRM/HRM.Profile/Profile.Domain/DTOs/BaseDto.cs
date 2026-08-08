using System.Text.Json.Serialization;
using EthiopianCalendar;

namespace Profile.Domain.DTOs;

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

    // ✅ Fixed: Handle null/empty dates safely
    public string CreatedAt
    {
        get
        {
            try
            {
                // Check if DateAdd is not the default value
                if (DateAdd == DateTime.MinValue || DateAdd == default)
                    return "";
                return DateAdd.ToString("MMMM dd, yyyy");
            }
            catch
            {
                return "";
            }
        }
    }

    public string CreatedAtAm
    {
        get
        {
            try
            {
                // Check if DateAdd is not the default value
                if (DateAdd == DateTime.MinValue || DateAdd == default)
                    return "";

                // Ethiopian calendar only works for dates after 1892-09-11
                if (DateAdd < new DateTime(1892, 9, 11))
                    return DateAdd.ToString("MMMM dd, yyyy");

                return DateAdd.ToEthiopianDateString("MMMM dd, yyyy");
            }
            catch (Exception)
            {
                // If conversion fails, return English format or empty
                try
                {
                    return DateAdd != DateTime.MinValue ? DateAdd.ToString("MMMM dd, yyyy") : "";
                }
                catch
                {
                    return "";
                }
            }
        }
    }

    public string ModifiedAt
    {
        get
        {
            try
            {
                return DateMod.HasValue && DateMod.Value != DateTime.MinValue
                    ? DateMod.Value.ToString("MMMM dd, yyyy")
                    : "";
            }
            catch
            {
                return "";
            }
        }
    }

    public string ModifiedAtAm
    {
        get
        {
            try
            {
                if (!DateMod.HasValue || DateMod.Value == DateTime.MinValue)
                    return "";

                if (DateMod.Value < new DateTime(1892, 9, 11))
                    return DateMod.Value.ToString("MMMM dd, yyyy");

                return DateMod.Value.ToEthiopianDateString("MMMM dd, yyyy");
            }
            catch (Exception)
            {
                try
                {
                    return DateMod.HasValue && DateMod.Value != DateTime.MinValue
                        ? DateMod.Value.ToString("MMMM dd, yyyy")
                        : "";
                }
                catch
                {
                    return "";
                }
            }
        }
    }
}