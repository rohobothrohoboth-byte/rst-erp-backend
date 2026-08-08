using System.Net.Http.Json;
using Leave.Domain.Entities.Local;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Helpers;
using System.Text.Json;
using Leave.Domain.DTOs;
namespace Leave.App.Services;

public interface IHrmProApiService
{
    // Return DTOs - this matches your implementation
    Task<List<EmployeeDto>> GetAllEmployeesAsync(CancellationToken ct = default);
    Task<EmployeeDto?> GetEmployeeAsync(Guid id, CancellationToken ct = default);
    Task<bool> UpdateEmployeeAppUserIdAsync(Guid employeeId, string appUserId, CancellationToken ct = default);
}