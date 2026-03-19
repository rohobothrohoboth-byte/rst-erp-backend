namespace Svc.Auth.Models.Dtos;

public class LoginDto
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
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

public class MenuNode
{
    public Guid Id { get; set; }
    public string Key { get; set; } = default!;
    public string Label { get; set; } = default!;
    public string Path { get; set; } = default!;
    public string Icon { get; set; } = default!;
    public int Order { get; set; }
    public Guid? ParentId { get; set; }

    public HashSet<string> ApiSet { get; set; } = new();
    public List<MenuNode> Children { get; set; } = new();
}

//public class MenuDto
//{
//    public Guid Id { get; set; }
//    public string Key { get; set; } = default!;
//    public string Label { get; set; } = default!;
//    public string Path { get; set; } = default!;
//    public string Icon { get; set; } = default!;
//    public int Order { get; set; }
//    public bool IsChild { get; set; }
//    public Guid? ParentId { get; set; }

//    public List<MenuDto> Childs { get; set; } = [];
//    public List<string> Apis { get; set; } = [];
//    public Guid PerModuleId { get; set; }
//}

public class FlatPermissionDto
{
    public string ModuleKey { get; set; } = default!;
    public string MenuKey { get; set; } = default!;
    public string ApiKey { get; set; } = default!;
}

public class ModuleDto
{
    public string Key { get; set; } = default!;
    public List<MenuDto> Menus { get; set; } = new();
}

public class MenuDto
{
    public string Key { get; set; } = default!;
    public List<string> Apis { get; set; } = new();
}