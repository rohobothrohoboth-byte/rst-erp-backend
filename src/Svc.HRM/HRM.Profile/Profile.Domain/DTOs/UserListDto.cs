// Profile.Domain/DTOs/UserListDto.cs
namespace Profile.Domain.DTOs;

public class UserListDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = default!;
    public string EmpFullName { get; set; } = default!;
    public string EmpFullNameAm { get; set; } = default!;
    public string Gender { get; set; } = default!;
    public string Department { get; set; } = default!;
    public string Position { get; set; } = default!;
    public string Branch { get; set; } = default!;
    public string EmpState { get; set; } = default!;
    public bool HasAccount { get; set; }
    public bool IsAccountActive { get; set; }
    public Guid? UserId { get; set; }
}