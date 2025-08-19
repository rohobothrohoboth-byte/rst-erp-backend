using Cor.Domain.Enums;

namespace Cor.Domain.Entities;
public class Branch : BaseEntity
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Location { get; set; } = default!;
    public BranchType Type { get; set; } = BranchType.HeadOff;
    public BranchStat Status { get; set; } = BranchStat.Active;
    public DateTime OpenDate { get; set; } = DateTime.UtcNow;
    public Guid CompId { get; set; }

    //******************************************//
    
    public Company Comp { get; set; } = default!;
}