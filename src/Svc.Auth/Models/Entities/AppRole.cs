using Microsoft.AspNetCore.Identity;

namespace Svc.Auth.Models.Entities;

public class AppRole : IdentityRole
{
    public string Desc { get; set; } = default!;

      public Guid? PositionId { get; set; }


}