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
}