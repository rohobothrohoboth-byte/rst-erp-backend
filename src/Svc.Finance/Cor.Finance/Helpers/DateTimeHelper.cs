// Helpers/DateTimeHelper.cs
namespace Cor.Finance.Helpers;

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



        public static DateTime ToUtc(this DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
                return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            return dateTime.ToUniversalTime();
        }

        public static DateTime? ToUtc(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return null;
            return dateTime.Value.ToUtc();
        }

}