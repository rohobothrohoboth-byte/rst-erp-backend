namespace Svc.Auth.Models.Entities;

public class PerMenu : BaseEntity
{
    public string Key { get; set; } = default!;
    public string Label { get; set; } = default!;
    public string Path { get; set; } = default!;
    public string Icon { get; set; } = default!;
    public bool IsChild { get; set; } = false!;
    public string Parent { get; set; } = "";
    public int Order { get; set; }
    public Guid PerModuleId { get; set; }

    //******************************************//

    public PerModule PerModule { get; set; } = null;
}