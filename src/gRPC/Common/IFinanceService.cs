using Contracts;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Common;

public interface IFinanceClient
{
    // Company
    Task<CompanyResponse> GetCompany(string id, CancellationToken ct = default);
    Task<CompanyListResponse> GetAllCompanies(CancellationToken ct = default);

    // Branch
    Task<BranchResponse> GetBranch(string id, CancellationToken ct = default);
    Task<BranchListResponse> GetAllBranches(CancellationToken ct = default);

    // Department
    Task<DepartmentResponse> GetDepartment(string id, CancellationToken ct = default);
    Task<DepartmentListResponse> GetAllDepartments(CancellationToken ct = default);

    // Position
    Task<PositionResponse> GetPosition(string id, CancellationToken ct = default);
    Task<PositionListResponse> GetAllPositions(CancellationToken ct = default);

    // Job Grade
    Task<JobGradeResponse> GetJobGrade(string id, CancellationToken ct = default);
    Task<JobGradeListResponse> GetAllJobGrades(CancellationToken ct = default);

    // Employee
    Task<EmployeeResponse> GetEmployee(string id, CancellationToken ct = default);
    Task<EmployeeListResponse> GetAllEmployees(CancellationToken ct = default);
}

public class FinanceClient : IFinanceClient
{
    private readonly string _servUrl;
    private readonly ILogger<FinanceClient>? _logger;
    private readonly GrpcChannel _channel;

    public FinanceClient(IConfiguration config, ILogger<FinanceClient>? logger = null)
    {
        _servUrl = config["FinanceUrl"] ?? throw new InvalidOperationException("Finance Service Address not configured");
        _logger = logger;

        _channel = GrpcChannel.ForAddress(_servUrl, new GrpcChannelOptions
        {
            HttpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            }
        });
    }

    // ==================== COMPANY ====================

    public async Task<CompanyResponse> GetCompany(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new FinanceService.FinanceServiceClient(_channel);
            var req = new CompanyRequest { Id = id };
            return await client.GetCompanyAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetCompany for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<CompanyListResponse> GetAllCompanies(CancellationToken ct = default)
    {
        try
        {
            var client = new FinanceService.FinanceServiceClient(_channel);
            var req = new CompanyListRequest();
            return await client.GetAllCompaniesAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetAllCompanies: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    // ==================== BRANCH ====================

    public async Task<BranchResponse> GetBranch(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new FinanceService.FinanceServiceClient(_channel);
            var req = new BranchRequest { Id = id };
            return await client.GetBranchAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetBranch for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<BranchListResponse> GetAllBranches(CancellationToken ct = default)
    {
        try
        {
            var client = new FinanceService.FinanceServiceClient(_channel);
            var req = new BranchListRequest();
            return await client.GetAllBranchesAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetAllBranches: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    // ==================== DEPARTMENT ====================

    public async Task<DepartmentResponse> GetDepartment(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new FinanceService.FinanceServiceClient(_channel);
            var req = new DepartmentRequest { Id = id };
            return await client.GetDepartmentAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetDepartment for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<DepartmentListResponse> GetAllDepartments(CancellationToken ct = default)
    {
        try
        {
            var client = new FinanceService.FinanceServiceClient(_channel);
            var req = new DepartmentListRequest();
            return await client.GetAllDepartmentsAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetAllDepartments: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    // ==================== POSITION ====================

    public async Task<PositionResponse> GetPosition(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new FinanceService.FinanceServiceClient(_channel);
            var req = new PositionRequest { Id = id };
            return await client.GetPositionAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetPosition for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<PositionListResponse> GetAllPositions(CancellationToken ct = default)
    {
        try
        {
            var client = new FinanceService.FinanceServiceClient(_channel);
            var req = new PositionListRequest();
            return await client.GetAllPositionsAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetAllPositions: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    // ==================== JOB GRADE ====================

    public async Task<JobGradeResponse> GetJobGrade(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new FinanceService.FinanceServiceClient(_channel);
            var req = new JobGradeRequest { Id = id };
            return await client.GetJobGradeAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetJobGrade for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<JobGradeListResponse> GetAllJobGrades(CancellationToken ct = default)
    {
        try
        {
            var client = new FinanceService.FinanceServiceClient(_channel);
            var req = new JobGradeListRequest();
            return await client.GetAllJobGradesAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetAllJobGrades: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    // ==================== EMPLOYEE ====================

    public async Task<EmployeeResponse> GetEmployee(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new FinanceService.FinanceServiceClient(_channel);
            var req = new EmployeeRequest { Id = id };
            return await client.GetEmployeeAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetEmployee for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<EmployeeListResponse> GetAllEmployees(CancellationToken ct = default)
    {
        try
        {
            var client = new FinanceService.FinanceServiceClient(_channel);
            var req = new EmployeeListRequest();
            return await client.GetAllEmployeesAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetAllEmployees: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }
}