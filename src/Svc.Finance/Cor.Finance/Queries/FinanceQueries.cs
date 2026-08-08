// Cor.Finance/Queries/FinanceQueries.cs
using MediatR;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using Microsoft.EntityFrameworkCore;
using Cor.Finance.Models.Entities;
namespace Cor.Finance.Queries;

// Company Queries
public class GetCompanyByIdQry : IRequest<CompanyDto?>
{
    public Guid Id { get; set; }
}



// Branch Queries
public class GetBranchByIdQry : IRequest<BranchDto?>
{
    public Guid Id { get; set; }
}

// Department Queries
public class GetDepartmentByIdQry : IRequest<DepartmentDto?>
{
    public Guid Id { get; set; }
}

// Position Queries
public class GetPositionByIdQry : IRequest<PositionDto?>
{
    public Guid Id { get; set; }
}

public class GetAllLocalPositionsQry : IRequest<List<PositionDto>> { }

// JobGrade Queries
public class GetJobGradeByIdQry : IRequest<JobGradeDto?>
{
    public Guid Id { get; set; }
}

public class GetAllLocalJobGradesQry : IRequest<List<JobGradeDto>> { }

// Employee Queries
public class GetEmployeeByIdQry : IRequest<EmployeeDto?>
{
    public Guid Id { get; set; }
}