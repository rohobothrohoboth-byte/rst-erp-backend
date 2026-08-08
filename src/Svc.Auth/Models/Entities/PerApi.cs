namespace Svc.Auth.Models.Entities;

public class PerApi : BaseEntity
{
    public string Key { get; set; } = default!;
    public string Desc { get; set; } = default!;
    public Guid PerMenuId { get; set; }

    //******************************************//



        public PerMenu PerMenu { get; set; } = null!;
        public ICollection<UserPerApi> UserPerApis { get; set; } = new List<UserPerApi>();
        public ICollection<PositionPerApi> PositionPerApis { get; set; } = new List<PositionPerApi>();
}