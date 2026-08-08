using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Cor.CRM.Services;
using Helpers;
using Cor.CRM.Queries;
using Task = System.Threading.Tasks.Task;

namespace Cor.CRM.Commands;

public class LeadAddCmd : IRequest<LeadDto> { public CreateLeadDto AddDto { get; set; } = default!; }
public class LeadModCmd : IRequest<LeadDto> { public UpdateLeadDto ModDto { get; set; } = default!; }
public class LeadDelCmd : IRequest { public Guid Id { get; set; } }
public class LeadConvertCmd : IRequest<LeadDto> { public Guid Id { get; set; } }
public class LeadAssignCmd : IRequest<LeadDto> { public Guid Id { get; set; } public Guid UserId { get; set; } }
public class LeadBulkActionCmd : IRequest<bool> { public LeadBulkActionDto BulkDto { get; set; } = default!; }
public class LeadBulkAssignCmd : IRequest<bool> { public List<Guid> LeadIds { get; set; } = new(); public Guid UserId { get; set; } }

// ==================== ADD LEAD ====================
public class LeadAddHandler : IRequestHandler<LeadAddCmd, LeadDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _mediator;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogService _logger;
    private readonly ILeadScoringService _scoringService;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public LeadAddHandler(
        IUnitOfWork unitOfWork,
        IMediator mediator,
        IEventPublisher eventPublisher,
        ILogService logger,
        ILeadScoringService scoringService,
        IServiceScopeFactory serviceScopeFactory)
    {
        _uow = unitOfWork;
        _mediator = mediator;
        _eventPublisher = eventPublisher;
        _logger = logger;
        _scoringService = scoringService;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<LeadDto> Handle(LeadAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = new Lead
            {
                Id = Guid.CreateVersion7(),
                FirstName = request.AddDto.FirstName,
                LastName = request.AddDto.LastName,
                CompanyName = request.AddDto.CompanyName,
                Email = request.AddDto.Email,
                Phone = request.AddDto.Phone,
                Mobile = request.AddDto.Mobile,
                Address = request.AddDto.Address,
                City = request.AddDto.City,
                State = request.AddDto.State,
                Country = request.AddDto.Country,
                Status = Enum.Parse<LeadStatus>(request.AddDto.Status ?? "New"),
                Source = Enum.Parse<LeadSource>(request.AddDto.Source ?? "Website"),
                Priority = Enum.Parse<LeadPriority>(request.AddDto.Priority ?? "Medium"),
                Industry = request.AddDto.Industry != null ? Enum.Parse<Industry>(request.AddDto.Industry) : null,
                Title = request.AddDto.Title,
                Description = request.AddDto.Description,
                Budget = request.AddDto.Budget,
                EstimatedValue = request.AddDto.EstimatedValue,
                ExpectedCloseDate = request.AddDto.ExpectedCloseDate,
                AssignedToUserId = request.AddDto.AssignedToUserId,
                Tags = request.AddDto.Tags,
                IsActive = true,
                IsConverted = false,
                CreatedAt = DateTime.UtcNow,
                // Industry specific
                PropertyType = request.AddDto.PropertyType,
                PropertyPrice = request.AddDto.PropertyPrice,
                PropertyLocation = request.AddDto.PropertyLocation,
                PropertySize = request.AddDto.PropertySize,
                ProductCategory = request.AddDto.ProductCategory,
                OrderQuantity = request.AddDto.OrderQuantity,
                RequiredDeliveryDate = request.AddDto.RequiredDeliveryDate,
                TenderNumber = request.AddDto.TenderNumber,
                TenderDeadline = request.AddDto.TenderDeadline,
                Department = request.AddDto.Department
            };

            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            // ✅ FIXED: Calculate score in background without blocking the transaction
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var scoringService = scope.ServiceProvider.GetRequiredService<ILeadScoringService>();
                    await scoringService.CalculateLeadScoreAsync(data.Id, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Background scoring failed for lead: {LeadId}", data.Id);
                }
            });

            // Publish event
            await _eventPublisher.PublishAsync("Lead", "CREATED", new
            {
                LeadId = data.Id,
                data.FirstName,
                data.LastName,
                data.Email,
                data.CompanyName,
                data.Status,
                data.Priority,
                data.EstimatedValue,
                data.AssignedToUserId,
                Timestamp = DateTime.UtcNow
            }, ct);

            _logger.LogInformation("Lead created and event published: {LeadId}", data.Id);

            var response = await _mediator.Send(new LeadByIdQry { Id = data.Id }, ct);
            return response ?? new LeadDto();
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ==================== MODIFY LEAD ====================
public class LeadModHandler : IRequestHandler<LeadModCmd, LeadDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _mediator;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogService _logger;
    private readonly ILeadScoringService _scoringService;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public LeadModHandler(
        IUnitOfWork unitOfWork,
        IMediator mediator,
        IEventPublisher eventPublisher,
        ILogService logger,
        ILeadScoringService scoringService,
        IServiceScopeFactory serviceScopeFactory)
    {
        _uow = unitOfWork;
        _mediator = mediator;
        _eventPublisher = eventPublisher;
        _logger = logger;
        _scoringService = scoringService;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<LeadDto> Handle(LeadModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<Lead>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null)
            {
                throw new DomainException($"LEAD with Id {request.ModDto.Id} NOT FOUND.");
            }

            // Update fields
            oldData.FirstName = request.ModDto.FirstName ?? oldData.FirstName;
            oldData.LastName = request.ModDto.LastName ?? oldData.LastName;
            oldData.CompanyName = request.ModDto.CompanyName ?? oldData.CompanyName;
            oldData.Email = request.ModDto.Email ?? oldData.Email;
            oldData.Phone = request.ModDto.Phone ?? oldData.Phone;
            oldData.Mobile = request.ModDto.Mobile ?? oldData.Mobile;
            oldData.Address = request.ModDto.Address ?? oldData.Address;
            oldData.City = request.ModDto.City ?? oldData.City;
            oldData.State = request.ModDto.State ?? oldData.State;
            oldData.Country = request.ModDto.Country ?? oldData.Country;

            if (request.ModDto.Status != null)
                oldData.Status = Enum.Parse<LeadStatus>(request.ModDto.Status);
            if (request.ModDto.Source != null)
                oldData.Source = Enum.Parse<LeadSource>(request.ModDto.Source);
            if (request.ModDto.Priority != null)
                oldData.Priority = Enum.Parse<LeadPriority>(request.ModDto.Priority);
            if (request.ModDto.Industry != null)
                oldData.Industry = Enum.Parse<Industry>(request.ModDto.Industry);

            oldData.Title = request.ModDto.Title ?? oldData.Title;
            oldData.Description = request.ModDto.Description ?? oldData.Description;
            oldData.Budget = request.ModDto.Budget ?? oldData.Budget;
            oldData.EstimatedValue = request.ModDto.EstimatedValue ?? oldData.EstimatedValue;
            oldData.ExpectedCloseDate = request.ModDto.ExpectedCloseDate ?? oldData.ExpectedCloseDate;
            oldData.AssignedToUserId = request.ModDto.AssignedToUserId ?? oldData.AssignedToUserId;
            oldData.Tags = request.ModDto.Tags ?? oldData.Tags;
            oldData.UpdatedAt = DateTime.UtcNow;

            // Industry specific
            oldData.PropertyType = request.ModDto.PropertyType ?? oldData.PropertyType;
            oldData.PropertyPrice = request.ModDto.PropertyPrice ?? oldData.PropertyPrice;
            oldData.PropertyLocation = request.ModDto.PropertyLocation ?? oldData.PropertyLocation;
            oldData.PropertySize = request.ModDto.PropertySize ?? oldData.PropertySize;
            oldData.ProductCategory = request.ModDto.ProductCategory ?? oldData.ProductCategory;
            oldData.OrderQuantity = request.ModDto.OrderQuantity ?? oldData.OrderQuantity;
            oldData.RequiredDeliveryDate = request.ModDto.RequiredDeliveryDate ?? oldData.RequiredDeliveryDate;
            oldData.TenderNumber = request.ModDto.TenderNumber ?? oldData.TenderNumber;
            oldData.TenderDeadline = request.ModDto.TenderDeadline ?? oldData.TenderDeadline;
            oldData.Department = request.ModDto.Department ?? oldData.Department;

            await _uow.Update(oldData);
            await _uow.Commit(ct);

            // ✅ FIXED: Recalculate score in background
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var scoringService = scope.ServiceProvider.GetRequiredService<ILeadScoringService>();
                    await scoringService.CalculateLeadScoreAsync(oldData.Id, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Background scoring failed for lead: {LeadId}", oldData.Id);
                }
            });

            // Publish update event
            await _eventPublisher.PublishAsync("Lead", "UPDATED", new
            {
                LeadId = oldData.Id,
                oldData.FirstName,
                oldData.LastName,
                oldData.Email,
                oldData.Status,
                oldData.Priority,
                oldData.EstimatedValue,
                oldData.AssignedToUserId,
                Timestamp = DateTime.UtcNow
            }, ct);

            _logger.LogInformation("Lead updated and event published: {LeadId}", oldData.Id);

            var response = await _mediator.Send(new LeadByIdQry { Id = request.ModDto.Id }, ct);
            return response ?? new LeadDto();
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ==================== DELETE LEAD ====================
public class LeadDelHandler : IRequestHandler<LeadDelCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogService _logger;

    public LeadDelHandler(
        IUnitOfWork unitOfWork,
        IEventPublisher eventPublisher,
        ILogService logger)
    {
        _uow = unitOfWork;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task Handle(LeadDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<Lead>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null)
            {
                throw new DomainException($"LEAD with id [{request.Id}] NOT FOUND.");
            }

            await _uow.Delete(data);
            await _uow.Commit(ct);

            // Publish delete event
            await _eventPublisher.PublishAsync("Lead", "DELETED", new
            {
                LeadId = data.Id,
                data.FirstName,
                data.LastName,
                data.Email,
                Timestamp = DateTime.UtcNow
            }, ct);

            _logger.LogInformation("Lead deleted and event published: {LeadId}", data.Id);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ==================== CONVERT LEAD TO CUSTOMER ====================
public class LeadConvertHandler : IRequestHandler<LeadConvertCmd, LeadDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _mediator;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogService _logger;

    public LeadConvertHandler(
        IUnitOfWork unitOfWork,
        IMediator mediator,
        IEventPublisher eventPublisher,
        ILogService logger)
    {
        _uow = unitOfWork;
        _mediator = mediator;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<LeadDto> Handle(LeadConvertCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var lead = await _uow.Set<Lead>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (lead == null)
            {
                throw new DomainException($"LEAD with id [{request.Id}] NOT FOUND.");
            }

            if (lead.IsConverted)
            {
                throw new DomainException($"LEAD with id [{request.Id}] is already converted.");
            }

            // Create customer from lead
            var customer = new Customer
            {
                Id = Guid.CreateVersion7(),
                Name = $"{lead.FirstName} {lead.LastName}",
                CompanyName = lead.CompanyName,
                Email = lead.Email,
                Phone = lead.Phone,
                Mobile = lead.Mobile,
                Address = lead.Address,
                City = lead.City,
                State = lead.State,
                Country = lead.Country,
                Status = CustomerStatus.Active,
                Type = CustomerType.Individual,
                Industry = lead.Industry,
                Description = $"Converted from lead: {lead.Id}",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _uow.Add(customer, ct);

            // Update lead
            lead.IsConverted = true;
            lead.ConvertedDate = DateTime.UtcNow;
            lead.ConvertedCustomerId = customer.Id;
            lead.Status = LeadStatus.Converted;
            lead.UpdatedAt = DateTime.UtcNow;

            await _uow.Update(lead);
            await _uow.Commit(ct);

            // Publish conversion event
            await _eventPublisher.PublishAsync("Lead", "CONVERTED", new
            {
                LeadId = lead.Id,
                CustomerId = customer.Id,
                CustomerName = customer.Name,
                Timestamp = DateTime.UtcNow
            }, ct);

            _logger.LogInformation("Lead converted to customer: {LeadId} -> {CustomerId}", lead.Id, customer.Id);

            var response = await _mediator.Send(new LeadByIdQry { Id = request.Id }, ct);
            return response ?? new LeadDto();
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ==================== ASSIGN LEAD ====================
public class LeadAssignHandler : IRequestHandler<LeadAssignCmd, LeadDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _mediator;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogService _logger;

    public LeadAssignHandler(
        IUnitOfWork unitOfWork,
        IMediator mediator,
        IEventPublisher eventPublisher,
        ILogService logger)
    {
        _uow = unitOfWork;
        _mediator = mediator;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<LeadDto> Handle(LeadAssignCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<Lead>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null)
            {
                throw new DomainException($"LEAD with id [{request.Id}] NOT FOUND.");
            }

            data.AssignedToUserId = request.UserId;
            data.UpdatedAt = DateTime.UtcNow;
            await _uow.Update(data);
            await _uow.Commit(ct);

            await _eventPublisher.PublishAsync("Lead", "ASSIGNED", new
            {
                LeadId = data.Id,
                data.FirstName,
                data.LastName,
                AssignedToUserId = request.UserId,
                Timestamp = DateTime.UtcNow
            }, ct);

            _logger.LogInformation("Lead assigned: {LeadId} -> {UserId}", data.Id, request.UserId);

            var response = await _mediator.Send(new LeadByIdQry { Id = request.Id }, ct);
            return response ?? new LeadDto();
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ==================== BULK ACTION ====================
public class LeadBulkActionHandler : IRequestHandler<LeadBulkActionCmd, bool>
{
    private readonly IUnitOfWork _uow;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogService _logger;

    public LeadBulkActionHandler(
        IUnitOfWork unitOfWork,
        IEventPublisher eventPublisher,
        ILogService logger)
    {
        _uow = unitOfWork;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<bool> Handle(LeadBulkActionCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var leads = await _uow.Set<Lead>()
                .Where(l => request.BulkDto.LeadIds.Contains(l.Id))
                .ToListAsync(ct);

            if (!leads.Any())
                return false;

            switch (request.BulkDto.Action?.ToLower())
            {
                case "assign":
                    var userId = Guid.Parse(request.BulkDto.Data?.ToString() ?? throw new DomainException("User ID is required"));
                    foreach (var lead in leads)
                    {
                        lead.AssignedToUserId = userId;
                        lead.UpdatedAt = DateTime.UtcNow;
                    }
                    await _uow.UpdateRange(leads);
                    break;

                case "changestatus":
                    var status = Enum.Parse<LeadStatus>(request.BulkDto.Data?.ToString() ?? "New");
                    foreach (var lead in leads)
                    {
                        lead.Status = status;
                        lead.UpdatedAt = DateTime.UtcNow;
                    }
                    await _uow.UpdateRange(leads);
                    break;

                case "addtags":
                    var tags = request.BulkDto.Data?.ToString() ?? "";
                    foreach (var lead in leads)
                    {
                        lead.Tags = string.IsNullOrEmpty(lead.Tags) ? tags : $"{lead.Tags},{tags}";
                        lead.UpdatedAt = DateTime.UtcNow;
                    }
                    await _uow.UpdateRange(leads);
                    break;

                case "delete":
                    foreach (var lead in leads)
                    {
                        await _uow.Delete(lead);
                    }
                    break;

                default:
                    throw new DomainException($"Unknown action: {request.BulkDto.Action}");
            }

            await _uow.Commit(ct);

            await _eventPublisher.PublishAsync("Lead", "BULK_ACTION", new
            {
                Action = request.BulkDto.Action,
                Count = leads.Count,
                LeadIds = leads.Select(l => l.Id),
                Timestamp = DateTime.UtcNow
            }, ct);

            _logger.LogInformation("Bulk action completed: {Action} on {Count} leads",
                request.BulkDto.Action, leads.Count);

            return true;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ==================== BULK ASSIGN ====================
public class LeadBulkAssignHandler : IRequestHandler<LeadBulkAssignCmd, bool>
{
    private readonly IUnitOfWork _uow;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogService _logger;

    public LeadBulkAssignHandler(
        IUnitOfWork unitOfWork,
        IEventPublisher eventPublisher,
        ILogService logger)
    {
        _uow = unitOfWork;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<bool> Handle(LeadBulkAssignCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var leads = await _uow.Set<Lead>()
                .Where(l => request.LeadIds.Contains(l.Id))
                .ToListAsync(ct);

            if (!leads.Any())
                return false;

            foreach (var lead in leads)
            {
                lead.AssignedToUserId = request.UserId;
                lead.UpdatedAt = DateTime.UtcNow;
            }

            await _uow.UpdateRange(leads);
            await _uow.Commit(ct);

            await _eventPublisher.PublishAsync("Lead", "BULK_ASSIGN", new
            {
                UserId = request.UserId,
                Count = leads.Count,
                LeadIds = leads.Select(l => l.Id),
                Timestamp = DateTime.UtcNow
            }, ct);

            _logger.LogInformation("Bulk assign completed: {Count} leads assigned to {UserId}",
                leads.Count, request.UserId);

            return true;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}