// Cor.CRM/Commands/CampaignCmd.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Cor.CRM.Commands;

// ============================================================
// COMMANDS
// ============================================================

public class CampaignAddCmd : IRequest<CampaignDto>
{
    public CreateCampaignDto Dto { get; set; } = default!;
}

public class CampaignModCmd : IRequest<CampaignDto>
{
    public Guid Id { get; set; }
    public UpdateCampaignDto Dto { get; set; } = default!;
}

public class CampaignDelCmd : IRequest
{
    public Guid Id { get; set; }
}

public class CampaignAddLeadsCmd : IRequest
{
    public Guid CampaignId { get; set; }
    public List<Guid> LeadIds { get; set; } = new();
}

// ============================================================
// CAMPAIGN STATUS ACTIONS
// ============================================================

public class CampaignStartCmd : IRequest<CampaignDto>
{
    public Guid Id { get; set; }
}

public class CampaignPauseCmd : IRequest<CampaignDto>
{
    public Guid Id { get; set; }
}

public class CampaignResumeCmd : IRequest<CampaignDto>
{
    public Guid Id { get; set; }
}

public class CampaignArchiveCmd : IRequest<CampaignDto>
{
    public Guid Id { get; set; }
}

public class CampaignCancelCmd : IRequest<CampaignDto>
{
    public Guid Id { get; set; }
}

public class CampaignDuplicateCmd : IRequest<CampaignDto>
{
    public Guid Id { get; set; }
}

// ============================================================
// ADD CAMPAIGN
// ============================================================

public class CampaignAddHandler : IRequestHandler<CampaignAddCmd, CampaignDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public CampaignAddHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<CampaignDto> Handle(CampaignAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var campaign = new Campaign
            {
                Id = Guid.CreateVersion7(),
                Name = request.Dto.Name,
                Description = request.Dto.Description,
                Type = Enum.Parse<CampaignType>(request.Dto.Type),
                Status = string.IsNullOrEmpty(request.Dto.Status) ? CampaignStatus.Draft : Enum.Parse<CampaignStatus>(request.Dto.Status),
                StartDate = request.Dto.StartDate,
                EndDate = request.Dto.EndDate,
                Budget = request.Dto.Budget,
                ExpectedRevenue = request.Dto.ExpectedRevenue,
                TargetAudience = request.Dto.TargetAudience,
                TargetIndustry = request.Dto.TargetIndustry,
                TargetLocation = request.Dto.TargetLocation,
                TargetCount = request.Dto.TargetCount,
                Channel = request.Dto.Channel,
                MetricsJson = request.Dto.MetricsJson,
                ContentJson = request.Dto.ContentJson,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _uow.Add(campaign, ct);
            await _uow.Commit(ct);

            _logger.LogInformation("Campaign created: {CampaignId} - {CampaignName}", campaign.Id, campaign.Name);

            return MapToDto(campaign);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private CampaignDto MapToDto(Campaign campaign)
    {
        return new CampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Description = campaign.Description,
            Type = campaign.Type.ToString(),
            Status = campaign.Status.ToString(),
            StartDate = campaign.StartDate,
            EndDate = campaign.EndDate,
            Budget = campaign.Budget,
            ActualCost = campaign.ActualCost,
            ExpectedRevenue = campaign.ExpectedRevenue,
            ActualRevenue = campaign.ActualRevenue,
            TargetAudience = campaign.TargetAudience,
            TargetIndustry = campaign.TargetIndustry,
            TargetLocation = campaign.TargetLocation,
            TargetCount = campaign.TargetCount,
            ReachCount = campaign.ReachCount,
            EngagementCount = campaign.EngagementCount,
            ConversionCount = campaign.ConversionCount,
            ConversionRate = campaign.ConversionRate,
            EngagementRate = campaign.EngagementRate,
            Channel = campaign.Channel,
            MetricsJson = campaign.MetricsJson,
            ContentJson = campaign.ContentJson,
            IsActive = campaign.IsActive,
            CreatedAt = campaign.CreatedAt,
            UpdatedAt = campaign.UpdatedAt
        };
    }
}

// ============================================================
// UPDATE CAMPAIGN
// ============================================================

public class CampaignModHandler : IRequestHandler<CampaignModCmd, CampaignDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public CampaignModHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<CampaignDto> Handle(CampaignModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var campaign = await _uow.Set<Campaign>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (campaign == null)
            {
                throw new DomainException($"Campaign with id [{request.Id}] NOT FOUND.");
            }

            if (request.Dto.Name != null)
                campaign.Name = request.Dto.Name;
            if (request.Dto.Description != null)
                campaign.Description = request.Dto.Description;
            if (request.Dto.Type != null)
                campaign.Type = Enum.Parse<CampaignType>(request.Dto.Type);
            if (request.Dto.Status != null)
                campaign.Status = Enum.Parse<CampaignStatus>(request.Dto.Status);
            if (request.Dto.StartDate.HasValue)
                campaign.StartDate = request.Dto.StartDate;
            if (request.Dto.EndDate.HasValue)
                campaign.EndDate = request.Dto.EndDate;
            if (request.Dto.Budget.HasValue)
                campaign.Budget = request.Dto.Budget;
            if (request.Dto.ActualCost.HasValue)
                campaign.ActualCost = request.Dto.ActualCost;
            if (request.Dto.ExpectedRevenue.HasValue)
                campaign.ExpectedRevenue = request.Dto.ExpectedRevenue;
            if (request.Dto.ActualRevenue.HasValue)
                campaign.ActualRevenue = request.Dto.ActualRevenue;
            if (request.Dto.TargetAudience != null)
                campaign.TargetAudience = request.Dto.TargetAudience;
            if (request.Dto.TargetIndustry != null)
                campaign.TargetIndustry = request.Dto.TargetIndustry;
            if (request.Dto.TargetLocation != null)
                campaign.TargetLocation = request.Dto.TargetLocation;
            if (request.Dto.TargetCount.HasValue)
                campaign.TargetCount = request.Dto.TargetCount.Value;
            if (request.Dto.ReachCount.HasValue)
                campaign.ReachCount = request.Dto.ReachCount.Value;
            if (request.Dto.EngagementCount.HasValue)
                campaign.EngagementCount = request.Dto.EngagementCount.Value;
            if (request.Dto.ConversionCount.HasValue)
                campaign.ConversionCount = request.Dto.ConversionCount.Value;
            if (request.Dto.ConversionRate.HasValue)
                campaign.ConversionRate = request.Dto.ConversionRate;
            if (request.Dto.EngagementRate.HasValue)
                campaign.EngagementRate = request.Dto.EngagementRate;
            if (request.Dto.Channel != null)
                campaign.Channel = request.Dto.Channel;
            if (request.Dto.MetricsJson != null)
                campaign.MetricsJson = request.Dto.MetricsJson;
            if (request.Dto.ContentJson != null)
                campaign.ContentJson = request.Dto.ContentJson;
            if (request.Dto.IsActive.HasValue)
                campaign.IsActive = request.Dto.IsActive.Value;

            campaign.UpdatedAt = DateTime.UtcNow;

            await _uow.Update(campaign);
            await _uow.Commit(ct);

            _logger.LogInformation("Campaign updated: {CampaignId} - {CampaignName}", campaign.Id, campaign.Name);

            return MapToDto(campaign);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private CampaignDto MapToDto(Campaign campaign)
    {
        return new CampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Description = campaign.Description,
            Type = campaign.Type.ToString(),
            Status = campaign.Status.ToString(),
            StartDate = campaign.StartDate,
            EndDate = campaign.EndDate,
            Budget = campaign.Budget,
            ActualCost = campaign.ActualCost,
            ExpectedRevenue = campaign.ExpectedRevenue,
            ActualRevenue = campaign.ActualRevenue,
            TargetAudience = campaign.TargetAudience,
            TargetIndustry = campaign.TargetIndustry,
            TargetLocation = campaign.TargetLocation,
            TargetCount = campaign.TargetCount,
            ReachCount = campaign.ReachCount,
            EngagementCount = campaign.EngagementCount,
            ConversionCount = campaign.ConversionCount,
            ConversionRate = campaign.ConversionRate,
            EngagementRate = campaign.EngagementRate,
            Channel = campaign.Channel,
            MetricsJson = campaign.MetricsJson,
            ContentJson = campaign.ContentJson,
            IsActive = campaign.IsActive,
            CreatedAt = campaign.CreatedAt,
            UpdatedAt = campaign.UpdatedAt
        };
    }
}

// ============================================================
// DELETE CAMPAIGN
// ============================================================

public class CampaignDelHandler : IRequestHandler<CampaignDelCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public CampaignDelHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task Handle(CampaignDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var campaign = await _uow.Set<Campaign>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (campaign == null)
            {
                throw new DomainException($"Campaign with id [{request.Id}] NOT FOUND.");
            }

            // Soft delete
            campaign.IsDeleted = true;
            campaign.UpdatedAt = DateTime.UtcNow;

            await _uow.Update(campaign);
            await _uow.Commit(ct);

            _logger.LogInformation("Campaign deleted: {CampaignId} - {CampaignName}", campaign.Id, campaign.Name);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// ADD LEADS TO CAMPAIGN
// ============================================================

public class CampaignAddLeadsHandler : IRequestHandler<CampaignAddLeadsCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public CampaignAddLeadsHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task Handle(CampaignAddLeadsCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var campaign = await _uow.Set<Campaign>()
                .FirstOrDefaultAsync(x => x.Id == request.CampaignId && !x.IsDeleted, ct);

            if (campaign == null)
            {
                throw new DomainException($"Campaign with id [{request.CampaignId}] NOT FOUND.");
            }

            foreach (var leadId in request.LeadIds)
            {
                var campaignLead = new CampaignLead
                {
                    CampaignId = request.CampaignId,
                    LeadId = leadId,
                    AddedAt = DateTime.UtcNow
                };
                await _uow.Add(campaignLead, ct);
            }

            await _uow.Commit(ct);

            _logger.LogInformation("Added {Count} leads to campaign: {CampaignId}", request.LeadIds.Count, request.CampaignId);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// START CAMPAIGN
// ============================================================

public class CampaignStartHandler : IRequestHandler<CampaignStartCmd, CampaignDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public CampaignStartHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<CampaignDto> Handle(CampaignStartCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var campaign = await _uow.Set<Campaign>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (campaign == null)
            {
                throw new DomainException($"Campaign with id [{request.Id}] NOT FOUND.");
            }

            // ✅ Only allow starting if status is Draft (1) or Scheduled (7)
            if (campaign.Status != CampaignStatus.Draft && campaign.Status != CampaignStatus.Scheduled)
            {
                throw new DomainException($"Cannot start campaign with status [{campaign.Status}]. Only Draft or Scheduled campaigns can be started.");
            }

            campaign.Status = CampaignStatus.Active;
            campaign.IsActive = true;
            campaign.StartDate = DateTime.UtcNow;
            campaign.UpdatedAt = DateTime.UtcNow;

            await _uow.Update(campaign);
            await _uow.Commit(ct);

            _logger.LogInformation("Campaign started: {CampaignId} - {CampaignName}", campaign.Id, campaign.Name);

            return MapToDto(campaign);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private CampaignDto MapToDto(Campaign campaign)
    {
        return new CampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Description = campaign.Description,
            Type = campaign.Type.ToString(),
            Status = campaign.Status.ToString(),
            StartDate = campaign.StartDate,
            EndDate = campaign.EndDate,
            Budget = campaign.Budget,
            ActualCost = campaign.ActualCost,
            ExpectedRevenue = campaign.ExpectedRevenue,
            ActualRevenue = campaign.ActualRevenue,
            TargetAudience = campaign.TargetAudience,
            TargetIndustry = campaign.TargetIndustry,
            TargetLocation = campaign.TargetLocation,
            TargetCount = campaign.TargetCount,
            ReachCount = campaign.ReachCount,
            EngagementCount = campaign.EngagementCount,
            ConversionCount = campaign.ConversionCount,
            ConversionRate = campaign.ConversionRate,
            EngagementRate = campaign.EngagementRate,
            Channel = campaign.Channel,
            MetricsJson = campaign.MetricsJson,
            ContentJson = campaign.ContentJson,
            IsActive = campaign.IsActive,
            CreatedAt = campaign.CreatedAt,
            UpdatedAt = campaign.UpdatedAt
        };
    }
}

// ============================================================
// PAUSE CAMPAIGN
// ============================================================

public class CampaignPauseHandler : IRequestHandler<CampaignPauseCmd, CampaignDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public CampaignPauseHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<CampaignDto> Handle(CampaignPauseCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var campaign = await _uow.Set<Campaign>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (campaign == null)
            {
                throw new DomainException($"Campaign with id [{request.Id}] NOT FOUND.");
            }

            if (campaign.Status != CampaignStatus.Active)
            {
                throw new DomainException($"Cannot pause campaign with status [{campaign.Status}]. Only Active campaigns can be paused.");
            }

            campaign.Status = CampaignStatus.Paused;
            campaign.UpdatedAt = DateTime.UtcNow;

            await _uow.Update(campaign);
            await _uow.Commit(ct);

            _logger.LogInformation("Campaign paused: {CampaignId} - {CampaignName}", campaign.Id, campaign.Name);

            return MapToDto(campaign);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private CampaignDto MapToDto(Campaign campaign)
    {
        return new CampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Description = campaign.Description,
            Type = campaign.Type.ToString(),
            Status = campaign.Status.ToString(),
            StartDate = campaign.StartDate,
            EndDate = campaign.EndDate,
            Budget = campaign.Budget,
            ActualCost = campaign.ActualCost,
            ExpectedRevenue = campaign.ExpectedRevenue,
            ActualRevenue = campaign.ActualRevenue,
            TargetAudience = campaign.TargetAudience,
            TargetIndustry = campaign.TargetIndustry,
            TargetLocation = campaign.TargetLocation,
            TargetCount = campaign.TargetCount,
            ReachCount = campaign.ReachCount,
            EngagementCount = campaign.EngagementCount,
            ConversionCount = campaign.ConversionCount,
            ConversionRate = campaign.ConversionRate,
            EngagementRate = campaign.EngagementRate,
            Channel = campaign.Channel,
            MetricsJson = campaign.MetricsJson,
            ContentJson = campaign.ContentJson,
            IsActive = campaign.IsActive,
            CreatedAt = campaign.CreatedAt,
            UpdatedAt = campaign.UpdatedAt
        };
    }
}

// ============================================================
// RESUME CAMPAIGN
// ============================================================

public class CampaignResumeHandler : IRequestHandler<CampaignResumeCmd, CampaignDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public CampaignResumeHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<CampaignDto> Handle(CampaignResumeCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var campaign = await _uow.Set<Campaign>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (campaign == null)
            {
                throw new DomainException($"Campaign with id [{request.Id}] NOT FOUND.");
            }

            if (campaign.Status != CampaignStatus.Paused)
            {
                throw new DomainException($"Cannot resume campaign with status [{campaign.Status}]. Only Paused campaigns can be resumed.");
            }

            campaign.Status = CampaignStatus.Active;
            campaign.UpdatedAt = DateTime.UtcNow;

            await _uow.Update(campaign);
            await _uow.Commit(ct);

            _logger.LogInformation("Campaign resumed: {CampaignId} - {CampaignName}", campaign.Id, campaign.Name);

            return MapToDto(campaign);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private CampaignDto MapToDto(Campaign campaign)
    {
        return new CampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Description = campaign.Description,
            Type = campaign.Type.ToString(),
            Status = campaign.Status.ToString(),
            StartDate = campaign.StartDate,
            EndDate = campaign.EndDate,
            Budget = campaign.Budget,
            ActualCost = campaign.ActualCost,
            ExpectedRevenue = campaign.ExpectedRevenue,
            ActualRevenue = campaign.ActualRevenue,
            TargetAudience = campaign.TargetAudience,
            TargetIndustry = campaign.TargetIndustry,
            TargetLocation = campaign.TargetLocation,
            TargetCount = campaign.TargetCount,
            ReachCount = campaign.ReachCount,
            EngagementCount = campaign.EngagementCount,
            ConversionCount = campaign.ConversionCount,
            ConversionRate = campaign.ConversionRate,
            EngagementRate = campaign.EngagementRate,
            Channel = campaign.Channel,
            MetricsJson = campaign.MetricsJson,
            ContentJson = campaign.ContentJson,
            IsActive = campaign.IsActive,
            CreatedAt = campaign.CreatedAt,
            UpdatedAt = campaign.UpdatedAt
        };
    }
}

// ============================================================
// ARCHIVE CAMPAIGN
// ============================================================

public class CampaignArchiveHandler : IRequestHandler<CampaignArchiveCmd, CampaignDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public CampaignArchiveHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<CampaignDto> Handle(CampaignArchiveCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var campaign = await _uow.Set<Campaign>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (campaign == null)
            {
                throw new DomainException($"Campaign with id [{request.Id}] NOT FOUND.");
            }

            if (campaign.Status == CampaignStatus.Archived)
            {
                throw new DomainException("Campaign is already archived.");
            }

            if (campaign.Status == CampaignStatus.Active)
            {
                throw new DomainException("Cannot archive an active campaign. Please pause or complete it first.");
            }

            campaign.Status = CampaignStatus.Archived;
            campaign.IsActive = false;
            campaign.UpdatedAt = DateTime.UtcNow;

            await _uow.Update(campaign);
            await _uow.Commit(ct);

            _logger.LogInformation("Campaign archived: {CampaignId} - {CampaignName}", campaign.Id, campaign.Name);

            return MapToDto(campaign);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private CampaignDto MapToDto(Campaign campaign)
    {
        return new CampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Description = campaign.Description,
            Type = campaign.Type.ToString(),
            Status = campaign.Status.ToString(),
            StartDate = campaign.StartDate,
            EndDate = campaign.EndDate,
            Budget = campaign.Budget,
            ActualCost = campaign.ActualCost,
            ExpectedRevenue = campaign.ExpectedRevenue,
            ActualRevenue = campaign.ActualRevenue,
            TargetAudience = campaign.TargetAudience,
            TargetIndustry = campaign.TargetIndustry,
            TargetLocation = campaign.TargetLocation,
            TargetCount = campaign.TargetCount,
            ReachCount = campaign.ReachCount,
            EngagementCount = campaign.EngagementCount,
            ConversionCount = campaign.ConversionCount,
            ConversionRate = campaign.ConversionRate,
            EngagementRate = campaign.EngagementRate,
            Channel = campaign.Channel,
            MetricsJson = campaign.MetricsJson,
            ContentJson = campaign.ContentJson,
            IsActive = campaign.IsActive,
            CreatedAt = campaign.CreatedAt,
            UpdatedAt = campaign.UpdatedAt
        };
    }
}

// ============================================================
// CANCEL CAMPAIGN
// ============================================================

public class CampaignCancelHandler : IRequestHandler<CampaignCancelCmd, CampaignDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public CampaignCancelHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<CampaignDto> Handle(CampaignCancelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var campaign = await _uow.Set<Campaign>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (campaign == null)
            {
                throw new DomainException($"Campaign with id [{request.Id}] NOT FOUND.");
            }

            if (campaign.Status == CampaignStatus.Cancelled)
            {
                throw new DomainException("Campaign is already cancelled.");
            }

            if (campaign.Status == CampaignStatus.Completed)
            {
                throw new DomainException("Cannot cancel a completed campaign.");
            }

            campaign.Status = CampaignStatus.Cancelled;
            campaign.IsActive = false;
            campaign.UpdatedAt = DateTime.UtcNow;

            await _uow.Update(campaign);
            await _uow.Commit(ct);

            _logger.LogInformation("Campaign cancelled: {CampaignId} - {CampaignName}", campaign.Id, campaign.Name);

            return MapToDto(campaign);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private CampaignDto MapToDto(Campaign campaign)
    {
        return new CampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Description = campaign.Description,
            Type = campaign.Type.ToString(),
            Status = campaign.Status.ToString(),
            StartDate = campaign.StartDate,
            EndDate = campaign.EndDate,
            Budget = campaign.Budget,
            ActualCost = campaign.ActualCost,
            ExpectedRevenue = campaign.ExpectedRevenue,
            ActualRevenue = campaign.ActualRevenue,
            TargetAudience = campaign.TargetAudience,
            TargetIndustry = campaign.TargetIndustry,
            TargetLocation = campaign.TargetLocation,
            TargetCount = campaign.TargetCount,
            ReachCount = campaign.ReachCount,
            EngagementCount = campaign.EngagementCount,
            ConversionCount = campaign.ConversionCount,
            ConversionRate = campaign.ConversionRate,
            EngagementRate = campaign.EngagementRate,
            Channel = campaign.Channel,
            MetricsJson = campaign.MetricsJson,
            ContentJson = campaign.ContentJson,
            IsActive = campaign.IsActive,
            CreatedAt = campaign.CreatedAt,
            UpdatedAt = campaign.UpdatedAt
        };
    }
}

// ============================================================
// DUPLICATE CAMPAIGN
// ============================================================

public class CampaignDuplicateHandler : IRequestHandler<CampaignDuplicateCmd, CampaignDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public CampaignDuplicateHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<CampaignDto> Handle(CampaignDuplicateCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var original = await _uow.Set<Campaign>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (original == null)
            {
                throw new DomainException($"Campaign with id [{request.Id}] NOT FOUND.");
            }

            var duplicate = new Campaign
            {
                Id = Guid.CreateVersion7(),
                Name = $"{original.Name} (Copy)",
                Description = original.Description,
                Type = original.Type,
                Status = CampaignStatus.Draft,
                StartDate = original.StartDate,
                EndDate = original.EndDate,
                Budget = original.Budget,
                ExpectedRevenue = original.ExpectedRevenue,
                TargetAudience = original.TargetAudience,
                TargetIndustry = original.TargetIndustry,
                TargetLocation = original.TargetLocation,
                TargetCount = original.TargetCount,
                Channel = original.Channel,
                MetricsJson = original.MetricsJson,
                ContentJson = original.ContentJson,
                IsActive = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _uow.Add(duplicate, ct);
            await _uow.Commit(ct);

            _logger.LogInformation("Campaign duplicated: {OriginalId} -> {DuplicateId}",
                original.Id, duplicate.Id);

            return MapToDto(duplicate);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private CampaignDto MapToDto(Campaign campaign)
    {
        return new CampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Description = campaign.Description,
            Type = campaign.Type.ToString(),
            Status = campaign.Status.ToString(),
            StartDate = campaign.StartDate,
            EndDate = campaign.EndDate,
            Budget = campaign.Budget,
            ActualCost = campaign.ActualCost,
            ExpectedRevenue = campaign.ExpectedRevenue,
            ActualRevenue = campaign.ActualRevenue,
            TargetAudience = campaign.TargetAudience,
            TargetIndustry = campaign.TargetIndustry,
            TargetLocation = campaign.TargetLocation,
            TargetCount = campaign.TargetCount,
            ReachCount = campaign.ReachCount,
            EngagementCount = campaign.EngagementCount,
            ConversionCount = campaign.ConversionCount,
            ConversionRate = campaign.ConversionRate,
            EngagementRate = campaign.EngagementRate,
            Channel = campaign.Channel,
            MetricsJson = campaign.MetricsJson,
            ContentJson = campaign.ContentJson,
            IsActive = campaign.IsActive,
            CreatedAt = campaign.CreatedAt,
            UpdatedAt = campaign.UpdatedAt
        };
    }
}