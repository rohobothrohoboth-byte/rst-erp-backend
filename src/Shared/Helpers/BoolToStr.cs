namespace Helpers;

public static class BoolToStr
{
    public static string FormatBool(bool t)
    {
        return t ? "Yes" : "No";
    }

    public static string FormatStat(bool t)
    {
        return t ? "Active" : "In-active";
    }

    public static string EnumToString<T>(T value) where T : Enum
    {
        return Convert.ToInt32(value).ToString();
    }

    public static string ToLvReqDay(double days, bool isHalfDay)
    {
        if (isHalfDay) { return "1/2 day"; }
        return days == 1 ? "1 day" : $"{days:#,##0.##} days";
    }

    public static string FormatDate(DateTime date) => date.ToString("MMMM dd, yyyy");
}