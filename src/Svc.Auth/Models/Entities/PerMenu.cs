namespace Svc.Auth.Models.Entities;

public class PerMenu : BaseEntity
{
    public string Key { get; set; } = default!;
    public string Desc { get; set; } = default!;
    public Guid PerModuleId { get; set; }

    //******************************************//

    public PerModule PerModule { get; set; } = null;
}