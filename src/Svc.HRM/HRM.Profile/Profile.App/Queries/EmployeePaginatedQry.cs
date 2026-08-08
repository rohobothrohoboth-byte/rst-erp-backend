using MediatR;
using Profile.Domain.DTOs;

namespace Profile.App.Queries;

public class EmployeePaginatedQry : IRequest<PaginatedResult<EmployeeListDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "CreatedAt";
    public string? SortOrder { get; set; } = "desc";
    public string? SearchTerm { get; set; }
    public string? Department { get; set; }
    public string? Branch { get; set; }
    public string? EmpState { get; set; }
    public string? EmpNature { get; set; }
    public string? Gender { get; set; }
}