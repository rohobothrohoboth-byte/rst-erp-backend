using MediatR;
using Profile.Domain.DTOs;

namespace Profile.App.Queries;

public class EmployeeFilterOptionsQry : IRequest<EmployeeFilterOptionsDto>
{
}

public class EmployeeFilterOptionsDto
{
    public List<IdNameDto> Departments { get; set; } = new();
    public List<IdNameDto> Branches { get; set; } = new();
    public List<string> EmpStates { get; set; } = new();
    public List<string> EmpNatures { get; set; } = new();
    public List<string> Genders { get; set; } = new();
}

public class IdNameDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}