// Helpers/DateTimeHelper.cs
namespace Helpers;

public static class DateTimeHelper
{
    public static DateTime EnsureUtc(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Unspecified)
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }
        return dateTime.ToUniversalTime();
    }

    public static DateTime? EnsureUtc(DateTime? dateTime)
    {
        if (dateTime.HasValue)
        {
            return EnsureUtc(dateTime.Value);
        }
        return null;
    }
}
