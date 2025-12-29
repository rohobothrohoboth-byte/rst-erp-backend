namespace Svc.Auth.Models.Dtos;

public class RegStep1
{
    public Guid EmployeeId { get; set; }
    public string Password { get; set; } = default!;
    public string RoleId { get; set; } = default!;
    public List<Guid> PerModules { get; set; } = default!;
}

public class RegStep2
{
    public string UserId { get; set; } = default!;
    public List<Guid> PerMenus { get; set; } = default!;
}

public class RegStep3
{
    public string UserId { get; set; } = default!;
    public List<Guid> PerAccess { get; set; } = default!;
}

public class RegRes
{
    public string UserId { get; set; } = default!; //User Id
}