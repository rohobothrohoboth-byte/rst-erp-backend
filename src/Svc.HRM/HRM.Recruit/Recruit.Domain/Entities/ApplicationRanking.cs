namespace Recruit.Domain.Entities;

public class ApplicationRanking : BaseEntity
{
    public double TotalScore { get; set; }
    public int Rank { get; set; }
    public Guid JobAppId { get; set; } // JobApplication

    //******************************************//

    public JobApplication JobApp { get; set; } = null!;
}