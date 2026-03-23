namespace Svc.Auth.Models.Dtos;

public class LoginDto
{
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
}

public class LoginResDto
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime ExpiresDate { get; set; }
}

public class UserDto
{
    public Guid? EmployeeId { get; set; }
    public string UserId { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string Role { get; set; } = default!;
    public List<string> PerModule { get; set; } = null!;
    public List<string> PerMenu { get; set; } = null!;
    public List<string> PerApi { get; set; } = null!;
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
    public string Name { get; set; } = default!;
}

public class RoleListDto
{
    public string Id { get; set; } = default!;
    public string Role { get; set; } = default!;
}

public class ModuleListDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = default!;
    public string Name { get; set; } = default!;
}

public class FlatPermissionDto
{
    public string ModKey { get; set; } = default!;
    public string ModDesc { get; set; } = default!;

    public Guid MenuId { get; set; }
    public string MenuKey { get; set; } = default!;
    public string Label { get; set; } = default!;
    public string Path { get; set; } = default!;
    public string Icon { get; set; } = default!;
    public bool IsChild { get; set; } = false!;
    public int Order { get; set; }
    public Guid? ParentId { get; set; }

    public string ApiKey { get; set; } = default!;
}

public class ModuleTokenDto
{
    public string K { get; set; } = default!;
    public string? L { get; set; }
    public List<MenuTokenDto> M { get; set; } = new();
}

public class MenuTokenDto
{
    public string K { get; set; } = default!;
    public string L { get; set; } = default!;
    public string? P { get; set; }
    public string? I { get; set; }
    public int O { get; set; }
    public List<string> A { get; set; } = new();
    public List<MenuTokenDto>? C { get; set; }
}
