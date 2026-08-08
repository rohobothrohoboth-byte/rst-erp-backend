namespace Svc.Auth.Models.Entities;

public class PerMenu : BaseEntity
{
    public string Key { get; set; } = default!;
    public string Label { get; set; } = default!;
    public string Path { get; set; } = default!;
    public string Icon { get; set; } = default!;
    public bool IsChild { get; set; } = false!;
    public int Order { get; set; }
    public Guid PerModuleId { get; set; }
    public Guid? ParentId { get; set; }

    //******************************************//



        public PerModule PerModule { get; set; } = null!;
        public PerMenu? Parent { get; set; } = null!; // Made nullable
        public ICollection<PerMenu> Children { get; set; } = new List<PerMenu>();
        public ICollection<PerApi> PerApis { get; set; } = new List<PerApi>();
        public ICollection<UserPerMenu> UserPerMenus { get; set; } = new List<UserPerMenu>();
        public ICollection<PositionPerMenu> PositionPerMenus { get; set; } = new List<PositionPerMenu>();

}