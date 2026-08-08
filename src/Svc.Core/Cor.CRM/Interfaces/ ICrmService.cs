using Cor.CRM.Models.DTOs;

namespace Cor.CRM.Interfaces;

public interface ICrmService
{
    // Lead
    Task<LeadDto> GetLeadAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<LeadDto>> GetAllLeadsAsync(LeadFilterDto filter, CancellationToken ct = default);
    Task<LeadDto> CreateLeadAsync(CreateLeadDto dto, CancellationToken ct = default);
    Task<LeadDto> UpdateLeadAsync(Guid id, UpdateLeadDto dto, CancellationToken ct = default);
    Task<bool> DeleteLeadAsync(Guid id, CancellationToken ct = default);
    Task<LeadDto> ConvertLeadAsync(Guid id, CancellationToken ct = default);
    Task<bool> AssignLeadAsync(Guid id, Guid userId, CancellationToken ct = default);
    Task<LeadStatsDto> GetLeadStatsAsync(CancellationToken ct = default);

    // Customer
    Task<CustomerDto> GetCustomerAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync(CustomerFilterDto filter, CancellationToken ct = default);
    Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto dto, CancellationToken ct = default);
    Task<CustomerDto> UpdateCustomerAsync(Guid id, UpdateCustomerDto dto, CancellationToken ct = default);
    Task<bool> DeleteCustomerAsync(Guid id, CancellationToken ct = default);

    // Opportunity
    Task<OpportunityDto> GetOpportunityAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<OpportunityDto>> GetAllOpportunitiesAsync(OpportunityFilterDto filter, CancellationToken ct = default);
    Task<OpportunityDto> CreateOpportunityAsync(CreateOpportunityDto dto, CancellationToken ct = default);
    Task<OpportunityDto> UpdateOpportunityAsync(Guid id, UpdateOpportunityDto dto, CancellationToken ct = default);
    Task<bool> DeleteOpportunityAsync(Guid id, CancellationToken ct = default);
}