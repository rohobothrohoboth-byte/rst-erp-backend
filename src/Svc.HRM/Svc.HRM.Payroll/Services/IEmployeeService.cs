using System.Text.Json;

namespace Svc.HRM.Payroll.Services;

public interface IEmployeeService
{
    Task<List<EmployeeDto>> GetAllEmployeesAsync(CancellationToken ct = default);
    Task<EmployeeDto?> GetEmployeeAsync(Guid id, CancellationToken ct = default);
    Task<List<EmployeeDto>> GetEmployeesByDepartmentAsync(Guid departmentId, CancellationToken ct = default);
}

public class EmployeeDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = default!;
    public Guid PositionId { get; set; }
    public string PositionName { get; set; } = default!;
    public DateTime EmploymentDate { get; set; }
    public bool IsActive { get; set; }
}

public class EmployeeAttendanceDto
{
    public Guid EmployeeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDays { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public int LeaveDays { get; set; }
    public int HolidayDays { get; set; }
    public double OvertimeHours { get; set; }
    public List<DailyAttendanceDto> DailyRecords { get; set; } = new();
}

public class DailyAttendanceDto
{
    public DateTime Date { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public string Status { get; set; } = default!; // Present, Absent, Late, Leave, Holiday
    public double HoursWorked { get; set; }
    public double OvertimeHours { get; set; }
}

public class EmployeeService : IEmployeeService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(HttpClient httpClient, ILogger<EmployeeService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<EmployeeDto>> GetAllEmployeesAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/hrm/profile/v1/Employee/AllEmployee", ct);
            if (!response.IsSuccessStatusCode)
                return new List<EmployeeDto>();

            var json = await response.Content.ReadAsStringAsync(ct);
            var result = JsonSerializer.Deserialize<ApiResponse<List<EmployeeDto>>>(json);
            return result?.Data ?? new List<EmployeeDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching employees");
            return new List<EmployeeDto>();
        }
    }

    public async Task<EmployeeDto?> GetEmployeeAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/hrm/profile/v1/Employee/GetEmployee/{id}", ct);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync(ct);
            var result = JsonSerializer.Deserialize<ApiResponse<EmployeeDto>>(json);
            return result?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching employee {Id}", id);
            return null;
        }
    }

    public async Task<List<EmployeeDto>> GetEmployeesByDepartmentAsync(Guid departmentId, CancellationToken ct = default)
    {
        try
        {
            var allEmployees = await GetAllEmployeesAsync(ct);
            return allEmployees.Where(x => x.DepartmentId == departmentId).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching employees by department");
            return new List<EmployeeDto>();
        }
    }

    private class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
    }
}