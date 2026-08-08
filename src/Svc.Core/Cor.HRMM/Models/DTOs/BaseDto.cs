using System.Text.Json.Serialization;
using EthiopianCalendar;

namespace Cor.HRMM.Models.DTOs;

public class BaseDto
{
    public Guid Id { get; set; }

    [JsonIgnore]
    public DateTime DateAdd { get; set; }

    [JsonIgnore]
    public DateTime? DateMod { get; set; }

    public bool IsDeleted { get; set; } = default!;
    public string RowVersion { get; set; } = default!;

    [JsonIgnore]
    public uint xmin { get; set; }

    private string? _cachedCreatedAt;
    private string? _cachedCreatedAtAm;
    private string? _cachedModifiedAt;
    private string? _cachedModifiedAtAm;

    public string CreatedAt
    {
        get
        {
            if (_cachedCreatedAt == null)
            {
                _cachedCreatedAt = FormatDateSafe(DateAdd);
            }
            return _cachedCreatedAt;
        }
    }

    public string CreatedAtAm
    {
        get
        {
            if (_cachedCreatedAtAm == null)
            {
                _cachedCreatedAtAm = ToEthiopianDateSafe(DateAdd);
            }
            return _cachedCreatedAtAm;
        }
    }

    public string ModifiedAt
    {
        get
        {
            if (_cachedModifiedAt == null && DateMod.HasValue)
            {
                _cachedModifiedAt = FormatDateSafe(DateMod.Value);
            }
            return _cachedModifiedAt ?? "";
        }
    }

    public string ModifiedAtAm
    {
        get
        {
            if (_cachedModifiedAtAm == null && DateMod.HasValue)
            {
                _cachedModifiedAtAm = ToEthiopianDateSafe(DateMod.Value);
            }
            return _cachedModifiedAtAm ?? "";
        }
    }

    private static string FormatDateSafe(DateTime date)
    {
        try
        {
            if (date <= DateTime.MinValue || date > DateTime.MaxValue.AddYears(-1))
                return "Invalid Date";

            return date.ToString("MMMM dd, yyyy");
        }
        catch
        {
            return "Invalid Date";
        }
    }

    private static string ToEthiopianDateSafe(DateTime date)
    {
        try
        {
            // Ethiopian calendar valid range: approximately 1892-2090 Gregorian
            var minEthiopianDate = new DateTime(1892, 8, 29);
            var maxEthiopianDate = new DateTime(2090, 12, 31);

            if (date <= DateTime.MinValue ||
                date < minEthiopianDate ||
                date > maxEthiopianDate)
            {
                return date.ToString("MMMM dd, yyyy");
            }

            return date.ToEthiopianDateString("MMMM dd, yyyy");
        }
        catch
        {
            try
            {
                return date.ToString("MMMM dd, yyyy");
            }
            catch
            {
                return "Invalid Date";
            }
        }
    }
}