using Contracts;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Common;

public interface ICrmClient
{
    // Lead
    Task<LeadResponse> GetLead(string id, CancellationToken ct = default);
    Task<LeadListResponse> GetAllLeads(CancellationToken ct = default);
    Task<LeadListResponse> GetLeadsByStatus(string status, CancellationToken ct = default);
    Task<LeadListResponse> GetLeadsByAssignedUser(string userId, CancellationToken ct = default);

    // Customer
    Task<CustomerResponse> GetCustomer(string id, CancellationToken ct = default);
    Task<CustomerListResponse> GetAllCustomers(CancellationToken ct = default);

    // Opportunity
    Task<OpportunityResponse> GetOpportunity(string id, CancellationToken ct = default);
    Task<OpportunityListResponse> GetAllOpportunities(CancellationToken ct = default);

    // Activity
    Task<ActivityResponse> GetActivity(string id, CancellationToken ct = default);
    Task<ActivityListResponse> GetAllActivities(CancellationToken ct = default);

    // Task
    Task<TaskResponse> GetTask(string id, CancellationToken ct = default);
    Task<TaskListResponse> GetAllTasks(CancellationToken ct = default);

    // Campaign
    Task<CampaignResponse> GetCampaign(string id, CancellationToken ct = default);
    Task<CampaignListResponse> GetAllCampaigns(CancellationToken ct = default);
}

public class CrmClient : ICrmClient
{
    private readonly string _servUrl;
    private readonly ILogger<CrmClient>? _logger;
    private readonly GrpcChannel _channel;

    public CrmClient(IConfiguration config, ILogger<CrmClient>? logger = null)
    {
        _servUrl = config["ServiceUrls:CoreCRMApi"] ?? throw new InvalidOperationException("CRM Service Address not configured");
        _logger = logger;

        _channel = GrpcChannel.ForAddress(_servUrl, new GrpcChannelOptions
        {
            HttpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            }
        });
    }

    // ==================== LEAD ====================

    public async Task<LeadResponse> GetLead(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new CrmService.CrmServiceClient(_channel);
            var req = new LeadRequest { Id = id };
            return await client.GetLeadAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetLead for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<LeadListResponse> GetAllLeads(CancellationToken ct = default)
    {
        try
        {
            var client = new CrmService.CrmServiceClient(_channel);
            var req = new LeadListRequest();
            return await client.GetAllLeadsAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetAllLeads: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<LeadListResponse> GetLeadsByStatus(string status, CancellationToken ct = default)
    {
        try
        {
            var client = new CrmService.CrmServiceClient(_channel);
            var req = new LeadsByStatusRequest { Status = status };
            return await client.GetLeadsByStatusAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetLeadsByStatus for Status: {Status}, {Detail}", status, ex.Status.Detail);
            throw;
        }
    }

    public async Task<LeadListResponse> GetLeadsByAssignedUser(string userId, CancellationToken ct = default)
    {
        try
        {
            var client = new CrmService.CrmServiceClient(_channel);
            var req = new LeadsByAssignedUserRequest { UserId = userId };
            return await client.GetLeadsByAssignedUserAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetLeadsByAssignedUser for UserId: {UserId}, {Detail}", userId, ex.Status.Detail);
            throw;
        }
    }

    // ==================== CUSTOMER ====================

    public async Task<CustomerResponse> GetCustomer(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new CrmService.CrmServiceClient(_channel);
            var req = new CustomerRequest { Id = id };
            return await client.GetCustomerAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetCustomer for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<CustomerListResponse> GetAllCustomers(CancellationToken ct = default)
    {
        try
        {
            var client = new CrmService.CrmServiceClient(_channel);
            var req = new CustomerListRequest();
            return await client.GetAllCustomersAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetAllCustomers: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    // ==================== OPPORTUNITY ====================

    public async Task<OpportunityResponse> GetOpportunity(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new CrmService.CrmServiceClient(_channel);
            var req = new OpportunityRequest { Id = id };
            return await client.GetOpportunityAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetOpportunity for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<OpportunityListResponse> GetAllOpportunities(CancellationToken ct = default)
    {
        try
        {
            var client = new CrmService.CrmServiceClient(_channel);
            var req = new OpportunityListRequest();
            return await client.GetAllOpportunitiesAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetAllOpportunities: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    // ==================== ACTIVITY ====================

    public async Task<ActivityResponse> GetActivity(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new CrmService.CrmServiceClient(_channel);
            var req = new ActivityRequest { Id = id };
            return await client.GetActivityAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetActivity for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<ActivityListResponse> GetAllActivities(CancellationToken ct = default)
    {
        try
        {
            var client = new CrmService.CrmServiceClient(_channel);
            var req = new ActivityListRequest();
            return await client.GetAllActivitiesAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetAllActivities: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    // ==================== TASK ====================

    public async Task<TaskResponse> GetTask(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new CrmService.CrmServiceClient(_channel);
            var req = new TaskRequest { Id = id };
            return await client.GetTaskAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetTask for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<TaskListResponse> GetAllTasks(CancellationToken ct = default)
    {
        try
        {
            var client = new CrmService.CrmServiceClient(_channel);
            var req = new TaskListRequest();
            return await client.GetAllTasksAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetAllTasks: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    // ==================== CAMPAIGN ====================

    public async Task<CampaignResponse> GetCampaign(string id, CancellationToken ct = default)
    {
        try
        {
            var client = new CrmService.CrmServiceClient(_channel);
            var req = new CampaignRequest { Id = id };
            return await client.GetCampaignAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetCampaign for ID: {Id}, Status: {Status}, Detail: {Detail}", id, ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<CampaignListResponse> GetAllCampaigns(CancellationToken ct = default)
    {
        try
        {
            var client = new CrmService.CrmServiceClient(_channel);
            var req = new CampaignListRequest();
            return await client.GetAllCampaignsAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            _logger?.LogError(ex, "gRPC error in GetAllCampaigns: {Status}, {Detail}", ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }
}