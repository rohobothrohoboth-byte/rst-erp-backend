namespace Svc.Identity.Entities;

public class Permission : BaseEntity
{
    public string Name { get; set; } = default!;  // e.g., "ReadUser", "WritePost"
}