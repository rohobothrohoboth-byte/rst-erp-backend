// Models/Analytics/RawDataDto.cs
namespace Cor.Finance.Models.Analytics;

public class RawDataDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Amount { get; set; }
    public int Count { get; set; }
}

