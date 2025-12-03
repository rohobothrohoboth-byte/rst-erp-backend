namespace Svc.AuthMgr.Dtos;

public class RegisterDto
{
    public required string EmpCode { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
}