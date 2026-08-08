using Contracts;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Persistence;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Cor.CRM.Models.Entities;
namespace Cor.CRM.gRPCService;

public class CrmGrpcService : CrmService.CrmServiceBase
{
    private readonly ILogger<CrmGrpcService> _logger;
    private readonly CrmDbContext _context;

    public CrmGrpcService(ILogger<CrmGrpcService> logger, CrmDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    // ==================== LEAD ====================
    public override async Task<LeadResponse> GetLead(LeadRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC: GetLead called for ID: {Id}", request.Id);

        try
        {
            var leadId = Guid.Parse(request.Id);
            var lead = await _context.Leads
                .FirstOrDefaultAsync(x => x.Id == leadId && !x.IsDeleted, context.CancellationToken);

            if (lead == null)
            {
                return new LeadResponse { Id = null, FirstName = null, LastName = null };
            }

            return MapToLeadResponse(lead);
        }
        catch (FormatException)
        {
            return new LeadResponse { Id = null, FirstName = null, LastName = null };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lead {Id}", request.Id);
            throw new RpcException(new Status(StatusCode.Internal, "Error retrieving lead"));
        }
    }

    public override async Task<LeadListResponse> GetAllLeads(LeadListRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC: GetAllLeads called");

        try
        {
            var leads = await _context.Leads
                .Where(x => !x.IsDeleted)
                .ToListAsync(context.CancellationToken);

            var response = new LeadListResponse { TotalCount = leads.Count };

            foreach (var lead in leads)
            {
                response.Leads.Add(MapToLeadResponse(lead));
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all leads");
            throw new RpcException(new Status(StatusCode.Internal, "Error retrieving leads"));
        }
    }

    // ==================== CUSTOMER ====================
    public override async Task<CustomerResponse> GetCustomer(CustomerRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC: GetCustomer called for ID: {Id}", request.Id);

        try
        {
            var customerId = Guid.Parse(request.Id);
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == customerId && !x.IsDeleted, context.CancellationToken);

            if (customer == null)
            {
                return new CustomerResponse { Id = null, Name = null };
            }

            return MapToCustomerResponse(customer);
        }
        catch (FormatException)
        {
            return new CustomerResponse { Id = null, Name = null };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer {Id}", request.Id);
            throw new RpcException(new Status(StatusCode.Internal, "Error retrieving customer"));
        }
    }

    public override async Task<CustomerListResponse> GetAllCustomers(CustomerListRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC: GetAllCustomers called");

        try
        {
            var customers = await _context.Customers
                .Where(x => !x.IsDeleted)
                .ToListAsync(context.CancellationToken);

            var response = new CustomerListResponse { TotalCount = customers.Count };

            foreach (var customer in customers)
            {
                response.Customers.Add(MapToCustomerResponse(customer));
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all customers");
            throw new RpcException(new Status(StatusCode.Internal, "Error retrieving customers"));
        }
    }

    // ==================== OPPORTUNITY ====================
    public override async Task<OpportunityResponse> GetOpportunity(OpportunityRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC: GetOpportunity called for ID: {Id}", request.Id);

        try
        {
            var oppId = Guid.Parse(request.Id);
            var opportunity = await _context.Opportunities
                .Include(x => x.Customer)
                .Include(x => x.Lead)
                .FirstOrDefaultAsync(x => x.Id == oppId && !x.IsDeleted, context.CancellationToken);

            if (opportunity == null)
            {
                return new OpportunityResponse { Id = null, Name = null };
            }

            return MapToOpportunityResponse(opportunity);
        }
        catch (FormatException)
        {
            return new OpportunityResponse { Id = null, Name = null };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting opportunity {Id}", request.Id);
            throw new RpcException(new Status(StatusCode.Internal, "Error retrieving opportunity"));
        }
    }

    // ==================== MAPPING METHODS ====================
    private static LeadResponse MapToLeadResponse(Lead lead)
    {
        return new LeadResponse
        {
            Id = lead.Id.ToString(),
            FirstName = lead.FirstName,
            LastName = lead.LastName,
            Email = lead.Email,
            CompanyName = lead.CompanyName ?? string.Empty,
            Status = lead.Status.ToString(),
            Score = lead.Score,
            IsConverted = lead.IsConverted,
            CreatedAt = lead.CreatedAt.ToString("o")
        };
    }

    private static CustomerResponse MapToCustomerResponse(Customer customer)
    {
        return new CustomerResponse
        {
            Id = customer.Id.ToString(),
            Name = customer.Name,
            CompanyName = customer.CompanyName ?? string.Empty,
            Email = customer.Email ?? string.Empty,
            Status = customer.Status.ToString(),
            Type = customer.Type.ToString(),
            CreatedAt = customer.CreatedAt.ToString("o")
        };
    }

    private static OpportunityResponse MapToOpportunityResponse(Opportunity opportunity)
    {
        return new OpportunityResponse
        {
            Id = opportunity.Id.ToString(),
            Name = opportunity.Name,
            Amount = (double)opportunity.Amount,
            Stage = opportunity.Stage.ToString(),
            WinProbability = (int)opportunity.WinProbability,
            CustomerId = opportunity.CustomerId?.ToString() ?? string.Empty,
            CustomerName = opportunity.Customer?.Name ?? string.Empty,
            LeadId = opportunity.LeadId?.ToString() ?? string.Empty,
            LeadName = opportunity.Lead != null ? $"{opportunity.Lead.FirstName} {opportunity.Lead.LastName}" : string.Empty,
            ExpectedCloseDate = opportunity.ExpectedCloseDate?.ToString("o") ?? string.Empty,
            CreatedAt = opportunity.CreatedAt.ToString("o")
        };
    }
}