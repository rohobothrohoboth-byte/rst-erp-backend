namespace Svc.Identity.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public List<string> Permissions { get; set; } = default!;
}