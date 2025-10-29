namespace Cor.Module.Helpers;

public class NumToWord
{
    private static readonly Random Random = new();
    
    public string NumGenerator(int cnt)
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
}