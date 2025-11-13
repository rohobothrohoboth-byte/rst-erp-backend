namespace Cor.Module.Models.Entities;

public class Holiday : BaseEntity
{
    public string Name { get; set; } = default!;
    public DateTime Date { get; set; } = default!;
    public bool IsPublic { get; set; } = true;
    public Guid FiscalYearId { get; set; }

    //******************************************//

    public FiscalYear FiscalYear { get; set; } = null!;
}