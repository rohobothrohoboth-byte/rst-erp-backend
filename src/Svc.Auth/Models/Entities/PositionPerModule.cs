namespace Svc.Auth.Models.Entities;

public class PositionPerModule : BaseEntity
{
    public Guid PositionId { get; set; }
    public Guid PerModuleId { get; set; }

    //******************************************//


    public PerModule PerModule { get; set; } = null!;
}

public class PositionPerMenu : BaseEntity
{
    public Guid PositionId { get; set; }
    public Guid PerMenuId { get; set; }

    //******************************************//


    public PerMenu PerMenu { get; set; } = null!;
}

public class PositionPerApi : BaseEntity
{
    public Guid PositionId { get; set; }
    public Guid PerApiId { get; set; }

    //******************************************//


    public PerApi PerApi { get; set; } = null!;
}