namespace Helpers;

public static class NumToWord
{
    private static readonly Random Random = new();
#pragma warning disable CA1822
    public static string IdGenerator(int cnt)
#pragma warning restore CA1822
    {
        var seed = Random.Next(1, int.MaxValue);
        const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
        var chars = new char[cnt];
        var rd = new Random(seed);

        for (var i = 0; i < cnt; i++)
        {
            chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
        }

        return new string(chars);
    }

    public static string NumGenerator(int cnt)
    {
        var seed = Random.Next(1, int.MaxValue);
        const string allowedChars = "0123456789";
        var chars = new char[cnt];
        var rd = new Random(seed);

        for (var i = 0; i < cnt; i++)
        {
            chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
        }

        return new string(chars);
    }

    public static int GetMonths(DateTime startDateUtc, DateTime asOfUtc)
    {
        if (asOfUtc < startDateUtc) { return 0; }
        var months = (asOfUtc.Year - startDateUtc.Year) * 12 + (asOfUtc.Month - startDateUtc.Month);
        if (asOfUtc.Day < startDateUtc.Day) { months--; }
        return Math.Max(0, months);
    }

    public static string FormatMonth(int totalMonths)
    {
        if (totalMonths < 0) { throw new ArgumentOutOfRangeException(nameof(totalMonths)); }
        if (totalMonths == 0) { return "Less than 1 month"; }
        int years = Math.DivRem(totalMonths, 12, out int months);
        static string Unit(int value, string name) => value == 1 ? $"{value} {name}" : $"{value} {name}s";
        if (years == 0) { return Unit(months, "month"); }
        if (months == 0) { return Unit(years, "year"); }

        return $"{Unit(years, "year")} {Unit(months, "month")}";
    }

    public static string FullServDur(this DateTime startDate, DateTime? endDate = null)
    {
        var end = endDate ?? DateTime.UtcNow;
        if (end < startDate) { return "0d"; }

        int years = 0;
        int months = 0;
        var temp = startDate;
        while (temp.AddYears(1) <= end)
        {
            temp = temp.AddYears(1);
            years++;
        }

        while (temp.AddMonths(1) <= end)
        {
            temp = temp.AddMonths(1);
            months++;
        }

        int days = (end - temp).Days;
        var parts = new List<string>();
        if (years > 0) { parts.Add($"{years}y"); }
        if (months > 0) { parts.Add($"{months}m"); }
        if (days > 0) { parts.Add($"{days}d"); }
        if (!parts.Any()) { return "0d"; }

        return string.Join(" ", parts);
    }
}