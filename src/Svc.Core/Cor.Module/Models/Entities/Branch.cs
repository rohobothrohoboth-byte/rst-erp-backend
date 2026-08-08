namespace Cor.Module.Models.Entities;

public class Branch : BaseEntity
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
   public string Code { get; set; } = default!;
    public string Location { get; set; } = default!;
    public DateTime OpenDate { get; set; } = DateTime.UtcNow;
    public string BranchType { get; set; } = default!;
    public string BranchStat { get; set; } = default!;
    public Guid CompId { get; set; }

    //******************************************//

    public Company Comp { get; set; } = null!;



        public ICollection<Department> Departments { get; set; } = new List<Department>();


}