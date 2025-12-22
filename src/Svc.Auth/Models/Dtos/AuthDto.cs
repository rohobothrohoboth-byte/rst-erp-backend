namespace Svc.Auth.Models.Dtos;

public class RegisterStep1
{
    public Guid EmployeeId { get; set; }
    public string Password { get; set; } = default!;
    public Guid RoleId { get; set; }
    public List<Guid> PerModules { get; set; } = default!;
}

public class RegisterStep2
{
    public Guid EmployeeId { get; set; }
    public List<Guid> PerMenus { get; set; } = default!;
}

public class RegisterStep3
{
    public Guid EmployeeId { get; set; }
    public List<Guid> PerAccess { get; set; } = default!;
}

public class LoginDto
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}

public class LoginResDto
{
    public string AccessToken { get; set; } = default;
    public string RefreshToken { get; set; } = default;
    public DateTime ExpiresDate { get; set; }
}

public class UserDto
{
    public Guid? EmployeeId { get; set; }
    public string UserId { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string Role { get; set; } = default!;
    public List<string> PerModule { get; set; } = null;
    public List<string> PerMenu { get; set; } = null;
    public List<string> PerApi { get; set; } = null;
}

public class TokenDto
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}

public class RefreshTokenDto
{
    public string Token { get; set; } = default!;
}

public class NameList
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default;
}

public class RoleListDto
{
    public string Id { get; set; } = default;
    public string Role { get; set; } = default;
}

public class ModuleListDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = default;
    public string Name { get; set; } = default;
}


