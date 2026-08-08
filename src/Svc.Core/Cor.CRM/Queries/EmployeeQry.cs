// Cor.CRM/Queries/EmployeeQry.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Dapper;
using Helpers;
using MediatR;

namespace Cor.CRM.Queries;

public class EmployeeAllQry : IRequest<List<EmployeeDto>> { }

public class EmployeeForAssignmentQry : IRequest<List<EmployeeAssignmentDto>> { }

public class EmployeeByIdQry : IRequest<EmployeeDto?>
{
    public Guid Id { get; set; }
}

public class EmployeeByAppUserQry : IRequest<EmployeeDto?>
{
    public Guid AppUserId { get; set; }
}

// ==================== GET ALL EMPLOYEES ====================
public class EmployeeAllHandler : IRequestHandler<EmployeeAllQry, List<EmployeeDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public EmployeeAllHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<EmployeeDto>> Handle(EmployeeAllQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    ""Id"",
                    ""Code"",
                    ""FirstName"",
                    ""FirstNameAm"",
                    ""MiddleName"",
                    ""MiddleNameAm"",
                    ""LastName"",
                    ""LastNameAm"",
                    ""Gender"",
                    ""Email"",
                    ""Phone"",
                    ""AppUserId"",
                    ""DepartmentId"",
                    ""PositionId"",
                    ""JobGradeId"",
                    ""EmploymentType"",
                    ""EmpState"",
                    ""EmploymentDate"",
                    ""DateAdd"",
                    ""DateMod"",
                    ""IsDeleted"",
                    ""SyncedAt""
                FROM ""LocalEmployees""
                WHERE ""IsDeleted"" = false
                ORDER BY ""FirstName"", ""LastName""
            ";

            var data = await _dapper.QueryAsync<EmployeeDto>(sql, new { }, ct);
            return data.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all employees");
            throw;
        }
    }
}

// ==================== GET EMPLOYEES FOR ASSIGNMENT ====================
// Cor.CRM/Queries/EmployeeQry.cs

// Cor.CRM/Queries/EmployeeQry.cs

public class EmployeeForAssignmentHandler : IRequestHandler<EmployeeForAssignmentQry, List<EmployeeAssignmentDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;
  public EmployeeForAssignmentHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }
    public async Task<List<EmployeeAssignmentDto>> Handle(EmployeeForAssignmentQry request, CancellationToken ct)
    {
        try
        {
            // ✅ Return ALL active employees, even without AppUserId
            var sql = @"
                SELECT
                    le.""Id"",
                    le.""AppUserId"",
                    le.""FirstName"",
                    le.""LastName"",
                    le.""Email"",
                    le.""Phone"",
                    le.""Code""
                FROM ""LocalEmployees"" le
                WHERE le.""IsDeleted"" = false
                    AND le.""EmpState"" = 'Active'
                ORDER BY le.""FirstName"", le.""LastName""
            ";

            var data = await _dapper.QueryAsync<EmployeeAssignmentDto>(sql, new { }, ct);
            return data.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get employees for assignment");
            throw;
        }
    }
}

// ==================== GET EMPLOYEE BY ID ====================
public class EmployeeByIdHandler : IRequestHandler<EmployeeByIdQry, EmployeeDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public EmployeeByIdHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<EmployeeDto?> Handle(EmployeeByIdQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    ""Id"",
                    ""Code"",
                    ""FirstName"",
                    ""FirstNameAm"",
                    ""MiddleName"",
                    ""MiddleNameAm"",
                    ""LastName"",
                    ""LastNameAm"",
                    ""Gender"",
                    ""Email"",
                    ""Phone"",
                    ""AppUserId"",
                    ""DepartmentId"",
                    ""PositionId"",
                    ""JobGradeId"",
                    ""EmploymentType"",
                    ""EmpState"",
                    ""EmploymentDate"",
                    ""DateAdd"",
                    ""DateMod"",
                    ""IsDeleted"",
                    ""SyncedAt""
                FROM ""LocalEmployees""
                WHERE ""Id"" = @Id AND ""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", request.Id);

            var data = await _dapper.QueryFirstOrDefaultAsync<EmployeeDto>(sql, parameters, ct);
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get employee by ID: {EmployeeId}", request.Id);
            throw;
        }
    }
}

// ==================== GET EMPLOYEE BY APPUSER ID ====================
// Cor.CRM/Queries/EmployeeQry.cs

// Cor.CRM/Queries/EmployeeQry.cs

public class EmployeeByAppUserHandler : IRequestHandler<EmployeeByAppUserQry, EmployeeDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public EmployeeByAppUserHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<EmployeeDto?> Handle(EmployeeByAppUserQry request, CancellationToken ct)
    {
        try
        {
            // ✅ FIX: Cast AppUserId to text for comparison
            var sql = @"
                SELECT
                    ""Id"",
                    ""Code"",
                    ""FirstName"",
                    ""FirstNameAm"",
                    ""MiddleName"",
                    ""MiddleNameAm"",
                    ""LastName"",
                    ""LastNameAm"",
                    ""Gender"",
                    ""Email"",
                    ""Phone"",
                    ""AppUserId"",
                    ""DepartmentId"",
                    ""PositionId"",
                    ""JobGradeId"",
                    ""EmploymentType"",
                    ""EmpState"",
                    ""EmploymentDate"",
                    ""DateAdd"",
                    ""DateMod"",
                    ""IsDeleted"",
                    ""SyncedAt""
                FROM ""LocalEmployees""
                WHERE CAST(""AppUserId"" AS text) = @AppUserId AND ""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@AppUserId", request.AppUserId.ToString());

            var data = await _dapper.QueryFirstOrDefaultAsync<EmployeeDto>(sql, parameters, ct);

            // ✅ If not found, return null (don't throw)
            if (data == null)
            {
                _logger.LogWarning("Employee with AppUserId [{AppUserId}] NOT FOUND.", request.AppUserId);
                return null;
            }

            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get employee by AppUser ID: {AppUserId}", request.AppUserId);
            throw;
        }
    }
}