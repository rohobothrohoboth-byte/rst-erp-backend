using Contracts;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.gRPCService;

public class FinanceGrpcService : FinanceService.FinanceServiceBase
{
    private readonly ILogger<FinanceGrpcService> _logger;
    private readonly FinanceDbContext _context;

    public FinanceGrpcService(ILogger<FinanceGrpcService> logger, FinanceDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    // ==================== COMPANY ====================

    public override async Task<CompanyResponse> GetCompany(CompanyRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC: GetCompany called for ID: {Id}", request.Id);

        try
        {
            if (string.IsNullOrEmpty(request.Id) || !Guid.TryParse(request.Id, out var companyId))
            {
                return new CompanyResponse { Id = null, Name = null, NameAm = null };
            }

            var company = await _context.LocalCompanies
                .FirstOrDefaultAsync(x => x.Id == companyId && !x.IsDeleted, context.CancellationToken);

            if (company == null)
            {
                return new CompanyResponse { Id = null, Name = null, NameAm = null };
            }

            return MapToCompanyResponse(company);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting company {Id}", request.Id);
            throw new RpcException(new Status(StatusCode.Internal, "Error retrieving company"));
        }
    }

    public override async Task<CompanyListResponse> GetAllCompanies(CompanyListRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC: GetAllCompanies called");

        try
        {
            var query = _context.LocalCompanies
                .Where(x => !x.IsDeleted);

            // Apply pagination
            var totalCount = await query.CountAsync(context.CancellationToken);
            var page = request.Page > 0 ? request.Page : 1;
            var pageSize = request.PageSize > 0 ? request.PageSize : 50;

            var companies = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(context.CancellationToken);

            var response = new CompanyListResponse { TotalCount = totalCount };

            foreach (var company in companies)
            {
                response.Companies.Add(MapToCompanyResponse(company));
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all companies");
            throw new RpcException(new Status(StatusCode.Internal, "Error retrieving companies"));
        }
    }

    // ==================== BRANCH ====================

    public override async Task<BranchResponse> GetBranch(BranchRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC: GetBranch called for ID: {Id}", request.Id);

        try
        {
            if (string.IsNullOrEmpty(request.Id) || !Guid.TryParse(request.Id, out var branchId))
            {
                return new BranchResponse { Id = null, Name = null, NameAm = null };
            }

            var branch = await _context.LocalBranches
                .FirstOrDefaultAsync(x => x.Id == branchId && !x.IsDeleted, context.CancellationToken);

            if (branch == null)
            {
                return new BranchResponse { Id = null, Name = null, NameAm = null };
            }

            return MapToBranchResponse(branch);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting branch {Id}", request.Id);
            throw new RpcException(new Status(StatusCode.Internal, "Error retrieving branch"));
        }
    }

    public override async Task<BranchListResponse> GetAllBranches(BranchListRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC: GetAllBranches called");

        try
        {
            var query = _context.LocalBranches
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(request.CompanyId) && Guid.TryParse(request.CompanyId, out var companyId))
            {
                query = query.Where(x => x.CompId == companyId);
            }

            var totalCount = await query.CountAsync(context.CancellationToken);
            var page = request.Page > 0 ? request.Page : 1;
            var pageSize = request.PageSize > 0 ? request.PageSize : 50;

            var branches = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(context.CancellationToken);

            var response = new BranchListResponse { TotalCount = totalCount };

            foreach (var branch in branches)
            {
                response.Branches.Add(MapToBranchResponse(branch));
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all branches");
            throw new RpcException(new Status(StatusCode.Internal, "Error retrieving branches"));
        }
    }

    // ==================== DEPARTMENT ====================

    public override async Task<DepartmentResponse> GetDepartment(DepartmentRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC: GetDepartment called for ID: {Id}", request.Id);

        try
        {
            if (string.IsNullOrEmpty(request.Id) || !Guid.TryParse(request.Id, out var deptId))
            {
                return new DepartmentResponse { Id = null, Name = null, NameAm = null };
            }

            var department = await _context.LocalDepartments
                .FirstOrDefaultAsync(x => x.Id == deptId && !x.IsDeleted, context.CancellationToken);

            if (department == null)
            {
                return new DepartmentResponse { Id = null, Name = null, NameAm = null };
            }

            return MapToDepartmentResponse(department);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting department {Id}", request.Id);
            throw new RpcException(new Status(StatusCode.Internal, "Error retrieving department"));
        }
    }

    public override async Task<DepartmentListResponse> GetAllDepartments(DepartmentListRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC: GetAllDepartments called");

        try
        {
            var query = _context.LocalDepartments
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(request.BranchId) && Guid.TryParse(request.BranchId, out var branchId))
            {
                query = query.Where(x => x.BranchId == branchId);
            }

            var totalCount = await query.CountAsync(context.CancellationToken);
            var page = request.Page > 0 ? request.Page : 1;
            var pageSize = request.PageSize > 0 ? request.PageSize : 50;

            var departments = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(context.CancellationToken);

            var response = new DepartmentListResponse { TotalCount = totalCount };

            foreach (var dept in departments)
            {
                response.Departments.Add(MapToDepartmentResponse(dept));
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all departments");
            throw new RpcException(new Status(StatusCode.Internal, "Error retrieving departments"));
        }
    }

    // ==================== POSITION ====================

    public override async Task<PositionResponse> GetPosition(PositionRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC: GetPosition called for ID: {Id}", request.Id);

        try
        {
            if (string.IsNullOrEmpty(request.Id) || !Guid.TryParse(request.Id, out var positionId))
            {
                return new PositionResponse { Id = null, Name = null, NameAm = null };
            }

            var position = await _context.LocalPositions
                .Include(x => x.Department)
                .Include(x => x.JobGrade)
                .FirstOrDefaultAsync(x => x.Id == positionId && !x.IsDeleted, context.CancellationToken);

            if (position == null)
            {
                return new PositionResponse { Id = null, Name = null, NameAm = null };
            }

            return MapToPositionResponse(position);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting position {Id}", request.Id);
            throw new RpcException(new Status(StatusCode.Internal, "Error retrieving position"));
        }
    }

    public override async Task<PositionListResponse> GetAllPositions(PositionListRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC: GetAllPositions called");

        try
        {
            var query = _context.LocalPositions
                .Include(x => x.Department)
                .Include(x => x.JobGrade)
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(request.DepartmentId) && Guid.TryParse(request.DepartmentId, out var deptId))
            {
                query = query.Where(x => x.DepartmentId == deptId);
            }

            var totalCount = await query.CountAsync(context.CancellationToken);
            var page = request.Page > 0 ? request.Page : 1;
            var pageSize = request.PageSize > 0 ? request.PageSize : 50;

            var positions = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(context.CancellationToken);

            var response = new PositionListResponse { TotalCount = totalCount };

            foreach (var position in positions)
            {
                response.Positions.Add(MapToPositionResponse(position));
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all positions");
            throw new RpcException(new Status(StatusCode.Internal, "Error retrieving positions"));
        }
    }

    // ==================== JOB GRADE ====================

    public override async Task<JobGradeResponse> GetJobGrade(JobGradeRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC: GetJobGrade called for ID: {Id}", request.Id);

        try
        {
            if (string.IsNullOrEmpty(request.Id) || !Guid.TryParse(request.Id, out var jobGradeId))
            {
                return new JobGradeResponse { Id = null, Name = null };
            }

            var jobGrade = await _context.LocalJobGrades
                .FirstOrDefaultAsync(x => x.Id == jobGradeId && !x.IsDeleted, context.CancellationToken);

            if (jobGrade == null)
            {
                return new JobGradeResponse { Id = null, Name = null };
            }

            return MapToJobGradeResponse(jobGrade);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting job grade {Id}", request.Id);
            throw new RpcException(new Status(StatusCode.Internal, "Error retrieving job grade"));
        }
    }

    public override async Task<JobGradeListResponse> GetAllJobGrades(JobGradeListRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC: GetAllJobGrades called");

        try
        {
            var query = _context.LocalJobGrades
                .Where(x => !x.IsDeleted);

            var totalCount = await query.CountAsync(context.CancellationToken);
            var page = request.Page > 0 ? request.Page : 1;
            var pageSize = request.PageSize > 0 ? request.PageSize : 50;

            var jobGrades = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(context.CancellationToken);

            var response = new JobGradeListResponse { TotalCount = totalCount };

            foreach (var jobGrade in jobGrades)
            {
                response.JobGrades.Add(MapToJobGradeResponse(jobGrade));
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all job grades");
            throw new RpcException(new Status(StatusCode.Internal, "Error retrieving job grades"));
        }
    }

    // ==================== EMPLOYEE ====================

    public override async Task<EmployeeResponse> GetEmployee(EmployeeRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC: GetEmployee called for ID: {Id}", request.Id);

        try
        {
            if (string.IsNullOrEmpty(request.Id) || !Guid.TryParse(request.Id, out var employeeId))
            {
                return new EmployeeResponse { Id = null, FirstName = null, LastName = null };
            }

            var employee = await _context.LocalEmployees
                .Include(x => x.Position)
                .ThenInclude(x => x.Department)
                .Include(x => x.Position)
                .ThenInclude(x => x.JobGrade)
                .Include(x => x.Department)
                .Include(x => x.JobGrade)
                .FirstOrDefaultAsync(x => x.Id == employeeId && !x.IsDeleted, context.CancellationToken);

            if (employee == null)
            {
                return new EmployeeResponse { Id = null, FirstName = null, LastName = null };
            }

            return MapToEmployeeResponse(employee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting employee {Id}", request.Id);
            throw new RpcException(new Status(StatusCode.Internal, "Error retrieving employee"));
        }
    }

    public override async Task<EmployeeListResponse> GetAllEmployees(EmployeeListRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC: GetAllEmployees called");

        try
        {
            var query = _context.LocalEmployees
                .Include(x => x.Position)
                .ThenInclude(x => x.Department)
                .Include(x => x.Position)
                .ThenInclude(x => x.JobGrade)
                .Include(x => x.Department)
                .Include(x => x.JobGrade)
                .Where(x => !x.IsDeleted);

            var totalCount = await query.CountAsync(context.CancellationToken);
            var page = request.Page > 0 ? request.Page : 1;
            var pageSize = request.PageSize > 0 ? request.PageSize : 50;

            var employees = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(context.CancellationToken);

            var response = new EmployeeListResponse { TotalCount = totalCount };

            foreach (var employee in employees)
            {
                response.Employees.Add(MapToEmployeeResponse(employee));
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all employees");
            throw new RpcException(new Status(StatusCode.Internal, "Error retrieving employees"));
        }
    }

    // ==================== MAPPING METHODS ====================

    private static CompanyResponse MapToCompanyResponse(Models.Entities.Local.LocalCompany company)
    {
        return new CompanyResponse
        {
            Id = company.Id.ToString(),
            Name = company.Name,
            NameAm = company.NameAm,
            TaxId = company.TaxId ?? string.Empty,
            Phone = company.Phone ?? string.Empty,
            Email = company.Email ?? string.Empty,
            Address = company.Address ?? string.Empty,
            LogoUrl = string.Empty,
            IsDeleted = company.IsDeleted,
            SyncedAt = company.SyncedAt.ToString("O")
        };
    }

    private static BranchResponse MapToBranchResponse(Models.Entities.Local.LocalBranch branch)
    {
        return new BranchResponse
        {
            Id = branch.Id.ToString(),
            Name = branch.Name,
            NameAm = branch.NameAm,
            Code = branch.Code,
            Location = branch.Location ?? string.Empty,
            CompId = branch.CompId?.ToString() ?? string.Empty,
            IsDeleted = branch.IsDeleted,
            SyncedAt = branch.SyncedAt.ToString("O")
        };
    }

    private static DepartmentResponse MapToDepartmentResponse(Models.Entities.Local.LocalDepartment department)
    {
        return new DepartmentResponse
        {
            Id = department.Id.ToString(),
            Name = department.Name,
            NameAm = department.NameAm,
            BranchId = department.BranchId?.ToString() ?? string.Empty,
            BranchName = string.Empty,
            IsDeleted = department.IsDeleted,
            SyncedAt = department.SyncedAt.ToString("O")
        };
    }

    private static PositionResponse MapToPositionResponse(Models.Entities.Local.LocalPosition position)
    {
        return new PositionResponse
        {
            Id = position.Id.ToString(),
            Name = position.Name,
            NameAm = position.NameAm,
            NoOfPosition = position.NoOfPosition,
            IsVacant = position.IsVacant,
            DepartmentId = position.DepartmentId.ToString(),
            DepartmentName = position.Department?.Name ?? string.Empty,
            JobGradeId = position.JobGradeId?.ToString() ?? string.Empty,
            JobGradeName = position.JobGrade?.Name ?? string.Empty,
            IsDeleted = position.IsDeleted,
            SyncedAt = position.SyncedAt.ToString("O")
        };
    }

    private static JobGradeResponse MapToJobGradeResponse(Models.Entities.Local.LocalJobGrade jobGrade)
    {
        return new JobGradeResponse
        {
            Id = jobGrade.Id.ToString(),
            Name = jobGrade.Name,
            StartSalary = jobGrade.StartSalary,
            MaxSalary = jobGrade.MaxSalary,
            IsDeleted = jobGrade.IsDeleted,
            SyncedAt = jobGrade.SyncedAt.ToString("O")
        };
    }

    private static EmployeeResponse MapToEmployeeResponse(Models.Entities.Local.LocalEmployee employee)
    {
        return new EmployeeResponse
        {
            Id = employee.Id.ToString(),
            Code = employee.Code,
            FirstName = employee.FirstName,
            FirstNameAm = employee.FirstNameAm,
            MiddleName = employee.MiddleName,
            MiddleNameAm = employee.MiddleNameAm,
            LastName = employee.LastName,
            LastNameAm = employee.LastNameAm,
            Gender = employee.Gender,
            Nationality = employee.Nationality,
            Email = employee.Email ?? string.Empty,
            Phone = employee.Phone ?? string.Empty,
            PositionId = employee.PositionId.ToString(),
            PositionName = employee.Position?.Name ?? string.Empty,
            DepartmentId = employee.DepartmentId.ToString(),
            DepartmentName = employee.Department?.Name ?? string.Empty,
            JobGradeId = employee.JobGradeId.ToString(),
            JobGradeName = employee.JobGrade?.Name ?? string.Empty,
            BranchName = string.Empty,
            PersonId = employee.PersonId.ToString(),
            AppUserId = employee.AppUserId?.ToString() ?? string.Empty,
            EmpState = employee.EmpState,
            EmploymentType = employee.EmploymentType,
            EmploymentNature = employee.EmploymentNature,
            WorkArrangement = employee.WorkArrangement,
            EmploymentDate = employee.EmploymentDate.ToString("O"),
            HasAccount = employee.AppUserId.HasValue,
            IsAccountActive = employee.AppUserId.HasValue,
            IsDeleted = employee.IsDeleted,
            DateAdd = employee.DateAdd.ToString("O"),
            DateMod = employee.DateMod?.ToString("O") ?? string.Empty,
            SyncedAt = employee.SyncedAt.ToString("O")
        };
    }

}
