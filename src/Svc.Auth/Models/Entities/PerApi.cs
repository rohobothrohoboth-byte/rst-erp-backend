namespace Svc.Auth.Models.Entities;

public class PerApi : BaseEntity
{
    public string Key { get; set; } = default!;
    public string Desc { get; set; } = default!;
}