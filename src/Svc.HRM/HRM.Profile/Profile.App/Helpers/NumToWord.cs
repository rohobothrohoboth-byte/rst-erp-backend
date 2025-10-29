namespace Profile.App.Helpers;

public class NumToWord
{
    private static readonly Random Random = new();
#pragma warning disable CA1822
    public string IdGenerator(int cnt)
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
}