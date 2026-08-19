// Services/IntegrationService.cs
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Services
{
    public interface IIntegrationService
    {
        Task NotifyProjectCreatedAsync(Project project);
        Task NotifyProjectUpdatedAsync(Project project);
        Task<EmployeeDto> GetEmployeeAsync(Guid employeeId);
        Task<CustomerDto> GetCustomerAsync(Guid customerId);
        Task<FinancialDto> GetProjectBudgetAsync(Guid projectId);
        Task<ResourceDto> GetResourceAsync(Guid resourceId);
        Task<bool> ValidateBudgetAsync(Guid projectId, decimal amount);
    }

    public class IntegrationService : IIntegrationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<IntegrationService> _logger;
        private readonly IConfiguration _configuration;

        public IntegrationService(
            HttpClient httpClient,
            ILogger<IntegrationService> logger,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task NotifyProjectCreatedAsync(Project project)
        {
            try
            {
                // Notify HRM for resource allocation
                var hrmPayload = new
                {
                    ProjectId = project.Id,
                    ProjectName = project.Name,
                    ProjectManagerId = project.ProjectManagerId,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate
                };

                var hrmResponse = await _httpClient.PostAsync(
                    $"{_configuration["HrmApiUrl"]}/api/project-notifications",
                    new StringContent(JsonSerializer.Serialize(hrmPayload), Encoding.UTF8, "application/json")
                );

                // Notify Finance for budget tracking
                var financePayload = new
                {
                    ProjectId = project.Id,
                    ProjectName = project.Name,
                    Budget = project.Budget,
                    DepartmentId = project.DepartmentId
                };

                var financeResponse = await _httpClient.PostAsync(
                    $"{_configuration["FinanceApiUrl"]}/api/project-budget",
                    new StringContent(JsonSerializer.Serialize(financePayload), Encoding.UTF8, "application/json")
                );

                // Notify Core for user permissions
                var corePayload = new
                {
                    ProjectId = project.Id,
                    ProjectName = project.Name,
                    ProjectManagerId = project.ProjectManagerId
                };

                var coreResponse = await _httpClient.PostAsync(
                    $"{_configuration["CoreApiUrl"]}/api/project-permissions",
                    new StringContent(JsonSerializer.Serialize(corePayload), Encoding.UTF8, "application/json")
                );

                _logger.LogInformation("Project {ProjectId} notifications sent successfully", project.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending project notifications for {ProjectId}", project.Id);
                // Non-critical - don't throw
            }
        }

        public async Task NotifyProjectUpdatedAsync(Project project)
        {
            try
            {
                // Notify CRM if customer project
                if (project.CustomerId==null)
                {
                    var crmPayload = new
                    {
                        ProjectId = project.Id,
                        ProjectName = project.Name,
                        CustomerId = project.CustomerId,
                        Status = project.Status,
                        CompletionPercentage = project.CompletionPercentage
                    };

                    await _httpClient.PostAsync(
                        $"{_configuration["CrmApiUrl"]}/api/project-updates",
                        new StringContent(JsonSerializer.Serialize(crmPayload), Encoding.UTF8, "application/json")
                    );
                }

                // Notify Finance for cost updates
                var financePayload = new
                {
                    ProjectId = project.Id,
                    ActualCost = project.ActualCost,
                    TotalBilled = project.TotalBilled
                };

                await _httpClient.PostAsync(
                    $"{_configuration["FinanceApiUrl"]}/api/project-costs",
                    new StringContent(JsonSerializer.Serialize(financePayload), Encoding.UTF8, "application/json")
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending project update notifications for {ProjectId}", project.Id);
            }
        }

        public async Task<EmployeeDto> GetEmployeeAsync(Guid employeeId)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{_configuration["HrmApiUrl"]}/api/employees/{employeeId}"
                );

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<EmployeeDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee {EmployeeId}", employeeId);
                return null;
            }
        }

        public async Task<CustomerDto> GetCustomerAsync(Guid customerId)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{_configuration["CrmApiUrl"]}/api/customers/{customerId}"
                );

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<CustomerDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer {CustomerId}", customerId);
                return null;
            }
        }

        public async Task<FinancialDto> GetProjectBudgetAsync(Guid projectId)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{_configuration["FinanceApiUrl"]}/api/projects/{projectId}/budget"
                );

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<FinancialDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting budget for project {ProjectId}", projectId);
                return null;
            }
        }

        public async Task<ResourceDto> GetResourceAsync(Guid resourceId)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{_configuration["InventoryApiUrl"]}/api/resources/{resourceId}"
                );

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ResourceDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting resource {ResourceId}", resourceId);
                return null;
            }
        }

        public async Task<bool> ValidateBudgetAsync(Guid projectId, decimal amount)
        {
            try
            {
                var payload = new { ProjectId = projectId, Amount = amount };
                var response = await _httpClient.PostAsync(
                    $"{_configuration["FinanceApiUrl"]}/api/projects/validate-budget",
                    new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
                );

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<BudgetValidationResult>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return result?.IsValid ?? false;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating budget for project {ProjectId}", projectId);
                return false;
            }
        }
    }

    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
    }

    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class FinancialDto
    {
        public decimal Budget { get; set; }
        public decimal Spent { get; set; }
        public decimal Remaining { get; set; }
    }

    public class ResourceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal CostPerUnit { get; set; }
        public int AvailableQuantity { get; set; }
    }

    public class BudgetValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
    }
}