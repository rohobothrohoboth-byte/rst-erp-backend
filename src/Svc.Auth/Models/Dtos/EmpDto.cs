namespace Svc.Auth.Models.Dtos;

public class EmpListDto
{
    public Guid Id { get; set; }
    public string EmpFullName { get; set; } = default!;
    public string EmpFullNameAm { get; set; } = default!;
    public string Gender { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Branch { get; set; } = default!;
    public string Department { get; set; } = default!;
    public string Position { get; set; } = default!;
    public string EmpState { get; set; } = default!;
    public bool HasAccount { get; set; }
}