namespace Leave.App.Helpers;

public static class BoolToStr
{
    public static string FormatBool(bool t)
    {
        return t ? "YES" : "NO";
    }

    public static string EnumToString<T>(T value) where T : Enum
    {
        return Convert.ToInt32(value).ToString();
    }
}