// Cor.CRM/Commands/SMSCampaignCommands.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Cor.CRM.Commands;

public class SMSCampaignAddCmd : IRequest<SMSCampaignDto>
{
    public CreateSMSCampaignDto Dto { get; set; } = default!;
}

public class SMSCampaignUpdateCmd : IRequest<SMSCampaignDto>
{
    public Guid Id { get; set; }
    public UpdateSMSCampaignDto Dto { get; set; } = default!;
}

public class SMSCampaignDeleteCmd : IRequest
{
    public Guid Id { get; set; }
}

public class SMSCampaignSendCmd : IRequest<SMSCampaignDto>
{
    public Guid Id { get; set; }
}

public class SMSCampaignDuplicateCmd : IRequest<SMSCampaignDto>
{
    public Guid Id { get; set; }
}

public class SMSCampaignPauseCmd : IRequest<SMSCampaignDto>
{
    public Guid Id { get; set; }
}

public class SMSCampaignResumeCmd : IRequest<SMSCampaignDto>
{
    public Guid Id { get; set; }
}

public class SMSCampaignCancelCmd : IRequest<SMSCampaignDto>
{
    public Guid Id { get; set; }
}

public class SMSCampaignAddHandler : IRequestHandler<SMSCampaignAddCmd, SMSCampaignDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public SMSCampaignAddHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<SMSCampaignDto> Handle(SMSCampaignAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var campaign = new SMSCampaign
            {
                Id = Guid.CreateVersion7(),
                Name = request.Dto.Name,
                Message = request.Dto.Message,
                Status = string.IsNullOrEmpty(request.Dto.Status)
                    ? SMSCampaignStatus.Draft
                    : Enum.Parse<SMSCampaignStatus>(request.Dto.Status),
                ScheduledDate = request.Dto.ScheduledDate.HasValue
                    ? DateTimeHelper.EnsureUtc(request.Dto.ScheduledDate.Value)
                    : null,
                FromNumber = request.Dto.FromNumber,
                CampaignId = request.Dto.CampaignId,
                CharacterCount = request.Dto.Message?.Length ?? 0,
                MessageParts = request.Dto.Message != null ? (int)Math.Ceiling((decimal)request.Dto.Message.Length / 160) : 1,
                CreatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                IsDeleted = false
            };

            await _uow.Add(campaign, ct);
            await _uow.Commit(ct);

            _logger.LogInformation("SMS campaign created: {CampaignId}", campaign.Id);

            return MapToDto(campaign);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private SMSCampaignDto MapToDto(SMSCampaign campaign)
    {
        return new SMSCampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Message = campaign.Message,
            Status = campaign.Status.ToString(),
            ScheduledDate = campaign.ScheduledDate,
            SentDate = campaign.SentDate,
            RecipientCount = campaign.RecipientCount,
            SentCount = campaign.SentCount,
            DeliveredCount = campaign.DeliveredCount,
            FailedCount = campaign.FailedCount,
            CharacterCount = campaign.CharacterCount,
            MessageParts = campaign.MessageParts,
            FromNumber = campaign.FromNumber,
            CampaignId = campaign.CampaignId,
            CreatedAt = campaign.CreatedAt,
            UpdatedAt = campaign.UpdatedAt
        };
    }
}

public class SMSCampaignUpdateHandler : IRequestHandler<SMSCampaignUpdateCmd, SMSCampaignDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public SMSCampaignUpdateHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<SMSCampaignDto> Handle(SMSCampaignUpdateCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var campaign = await _uow.Set<SMSCampaign>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (campaign == null)
                throw new DomainException($"SMS campaign with id [{request.Id}] NOT FOUND.");

            if (campaign.Status == SMSCampaignStatus.Sent || campaign.Status == SMSCampaignStatus.Sending)
                throw new DomainException($"Cannot update campaign with status [{campaign.Status}]");

            if (!string.IsNullOrEmpty(request.Dto.Name))
                campaign.Name = request.Dto.Name;

            if (!string.IsNullOrEmpty(request.Dto.Message))
            {
                campaign.Message = request.Dto.Message;
                campaign.CharacterCount = request.Dto.Message.Length;
                campaign.MessageParts = (int)Math.Ceiling((decimal)request.Dto.Message.Length / 160);
            }

            if (!string.IsNullOrEmpty(request.Dto.Status))
                campaign.Status = Enum.Parse<SMSCampaignStatus>(request.Dto.Status);

            if (request.Dto.ScheduledDate.HasValue)
                campaign.ScheduledDate = DateTimeHelper.EnsureUtc(request.Dto.ScheduledDate.Value);

            if (request.Dto.FromNumber != null)
                campaign.FromNumber = request.Dto.FromNumber;

            campaign.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(campaign);
            await _uow.Commit(ct);

            _logger.LogInformation("SMS campaign updated: {CampaignId}", campaign.Id);

            return MapToDto(campaign);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private SMSCampaignDto MapToDto(SMSCampaign campaign)
    {
        return new SMSCampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Message = campaign.Message,
            Status = campaign.Status.ToString(),
            ScheduledDate = campaign.ScheduledDate,
            SentDate = campaign.SentDate,
            RecipientCount = campaign.RecipientCount,
            SentCount = campaign.SentCount,
            DeliveredCount = campaign.DeliveredCount,
            FailedCount = campaign.FailedCount,
            CharacterCount = campaign.CharacterCount,
            MessageParts = campaign.MessageParts,
            FromNumber = campaign.FromNumber,
            CampaignId = campaign.CampaignId,
            CreatedAt = campaign.CreatedAt,
            UpdatedAt = campaign.UpdatedAt
        };
    }
}

public class SMSCampaignDeleteHandler : IRequestHandler<SMSCampaignDeleteCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public SMSCampaignDeleteHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task Handle(SMSCampaignDeleteCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var campaign = await _uow.Set<SMSCampaign>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (campaign == null)
                throw new DomainException($"SMS campaign with id [{request.Id}] NOT FOUND.");

            if (campaign.Status == SMSCampaignStatus.Sent || campaign.Status == SMSCampaignStatus.Sending)
                throw new DomainException($"Cannot delete campaign with status [{campaign.Status}]");

            campaign.IsDeleted = true;
            campaign.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(campaign);
            await _uow.Commit(ct);

            _logger.LogInformation("SMS campaign deleted: {CampaignId}", campaign.Id);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class SMSCampaignSendHandler : IRequestHandler<SMSCampaignSendCmd, SMSCampaignDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public SMSCampaignSendHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<SMSCampaignDto> Handle(SMSCampaignSendCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var campaign = await _uow.Set<SMSCampaign>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (campaign == null)
                throw new DomainException($"SMS campaign with id [{request.Id}] NOT FOUND.");

            if (campaign.Status == SMSCampaignStatus.Sent)
                throw new DomainException("Campaign is already sent");

            if (campaign.Status == SMSCampaignStatus.Sending)
                throw new DomainException("Campaign is already sending");

            campaign.Status = SMSCampaignStatus.Sending;
            campaign.SentDate = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
            campaign.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(campaign);
            await _uow.Commit(ct);

            _logger.LogInformation("SMS campaign sent: {CampaignId}", campaign.Id);

            return MapToDto(campaign);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private SMSCampaignDto MapToDto(SMSCampaign campaign)
    {
        return new SMSCampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Message = campaign.Message,
            Status = campaign.Status.ToString(),
            ScheduledDate = campaign.ScheduledDate,
            SentDate = campaign.SentDate,
            RecipientCount = campaign.RecipientCount,
            SentCount = campaign.SentCount,
            DeliveredCount = campaign.DeliveredCount,
            FailedCount = campaign.FailedCount,
            CharacterCount = campaign.CharacterCount,
            MessageParts = campaign.MessageParts,
            FromNumber = campaign.FromNumber,
            CampaignId = campaign.CampaignId,
            CreatedAt = campaign.CreatedAt,
            UpdatedAt = campaign.UpdatedAt
        };
    }
}

public class SMSCampaignDuplicateHandler : IRequestHandler<SMSCampaignDuplicateCmd, SMSCampaignDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public SMSCampaignDuplicateHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<SMSCampaignDto> Handle(SMSCampaignDuplicateCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var original = await _uow.Set<SMSCampaign>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (original == null)
                throw new DomainException($"SMS campaign with id [{request.Id}] NOT FOUND.");

            var duplicate = new SMSCampaign
            {
                Id = Guid.CreateVersion7(),
                Name = $"{original.Name} (Copy)",
                Message = original.Message,
                Status = SMSCampaignStatus.Draft,
                FromNumber = original.FromNumber,
                CampaignId = original.CampaignId,
                CharacterCount = original.CharacterCount,
                MessageParts = original.MessageParts,
                CreatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                IsDeleted = false
            };

            await _uow.Add(duplicate, ct);
            await _uow.Commit(ct);

            _logger.LogInformation("SMS campaign duplicated: {OriginalId} -> {DuplicateId}", original.Id, duplicate.Id);

            return MapToDto(duplicate);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private SMSCampaignDto MapToDto(SMSCampaign campaign)
    {
        return new SMSCampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Message = campaign.Message,
            Status = campaign.Status.ToString(),
            ScheduledDate = campaign.ScheduledDate,
            SentDate = campaign.SentDate,
            RecipientCount = campaign.RecipientCount,
            SentCount = campaign.SentCount,
            DeliveredCount = campaign.DeliveredCount,
            FailedCount = campaign.FailedCount,
            CharacterCount = campaign.CharacterCount,
            MessageParts = campaign.MessageParts,
            FromNumber = campaign.FromNumber,
            CampaignId = campaign.CampaignId,
            CreatedAt = campaign.CreatedAt,
            UpdatedAt = campaign.UpdatedAt
        };
    }
}

public class SMSCampaignPauseHandler : IRequestHandler<SMSCampaignPauseCmd, SMSCampaignDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public SMSCampaignPauseHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<SMSCampaignDto> Handle(SMSCampaignPauseCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var campaign = await _uow.Set<SMSCampaign>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (campaign == null)
                throw new DomainException($"SMS campaign with id [{request.Id}] NOT FOUND.");

            if (campaign.Status != SMSCampaignStatus.Sending && campaign.Status != SMSCampaignStatus.Scheduled)
                throw new DomainException($"Cannot pause campaign with status [{campaign.Status}]");

            campaign.Status = SMSCampaignStatus.Paused;
            campaign.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(campaign);
            await _uow.Commit(ct);

            _logger.LogInformation("SMS campaign paused: {CampaignId}", campaign.Id);

            return MapToDto(campaign);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private SMSCampaignDto MapToDto(SMSCampaign campaign)
    {
        return new SMSCampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Message = campaign.Message,
            Status = campaign.Status.ToString(),
            ScheduledDate = campaign.ScheduledDate,
            SentDate = campaign.SentDate,
            RecipientCount = campaign.RecipientCount,
            SentCount = campaign.SentCount,
            DeliveredCount = campaign.DeliveredCount,
            FailedCount = campaign.FailedCount,
            CharacterCount = campaign.CharacterCount,
            MessageParts = campaign.MessageParts,
            FromNumber = campaign.FromNumber,
            CampaignId = campaign.CampaignId,
            CreatedAt = campaign.CreatedAt,
            UpdatedAt = campaign.UpdatedAt
        };
    }
}

public class SMSCampaignResumeHandler : IRequestHandler<SMSCampaignResumeCmd, SMSCampaignDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public SMSCampaignResumeHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<SMSCampaignDto> Handle(SMSCampaignResumeCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var campaign = await _uow.Set<SMSCampaign>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (campaign == null)
                throw new DomainException($"SMS campaign with id [{request.Id}] NOT FOUND.");

            if (campaign.Status != SMSCampaignStatus.Paused)
                throw new DomainException($"Cannot resume campaign with status [{campaign.Status}]");

            campaign.Status = SMSCampaignStatus.Sending;
            campaign.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(campaign);
            await _uow.Commit(ct);

            _logger.LogInformation("SMS campaign resumed: {CampaignId}", campaign.Id);

            return MapToDto(campaign);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private SMSCampaignDto MapToDto(SMSCampaign campaign)
    {
        return new SMSCampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Message = campaign.Message,
            Status = campaign.Status.ToString(),
            ScheduledDate = campaign.ScheduledDate,
            SentDate = campaign.SentDate,
            RecipientCount = campaign.RecipientCount,
            SentCount = campaign.SentCount,
            DeliveredCount = campaign.DeliveredCount,
            FailedCount = campaign.FailedCount,
            CharacterCount = campaign.CharacterCount,
            MessageParts = campaign.MessageParts,
            FromNumber = campaign.FromNumber,
            CampaignId = campaign.CampaignId,
            CreatedAt = campaign.CreatedAt,
            UpdatedAt = campaign.UpdatedAt
        };
    }
}

public class SMSCampaignCancelHandler : IRequestHandler<SMSCampaignCancelCmd, SMSCampaignDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public SMSCampaignCancelHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<SMSCampaignDto> Handle(SMSCampaignCancelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var campaign = await _uow.Set<SMSCampaign>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (campaign == null)
                throw new DomainException($"SMS campaign with id [{request.Id}] NOT FOUND.");

            if (campaign.Status == SMSCampaignStatus.Sent)
                throw new DomainException("Cannot cancel a sent campaign");

            if (campaign.Status == SMSCampaignStatus.Cancelled)
                throw new DomainException("Campaign is already cancelled");

            campaign.Status = SMSCampaignStatus.Cancelled;
            campaign.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(campaign);
            await _uow.Commit(ct);

            _logger.LogInformation("SMS campaign cancelled: {CampaignId}", campaign.Id);

            return MapToDto(campaign);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private SMSCampaignDto MapToDto(SMSCampaign campaign)
    {
        return new SMSCampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Message = campaign.Message,
            Status = campaign.Status.ToString(),
            ScheduledDate = campaign.ScheduledDate,
            SentDate = campaign.SentDate,
            RecipientCount = campaign.RecipientCount,
            SentCount = campaign.SentCount,
            DeliveredCount = campaign.DeliveredCount,
            FailedCount = campaign.FailedCount,
            CharacterCount = campaign.CharacterCount,
            MessageParts = campaign.MessageParts,
            FromNumber = campaign.FromNumber,
            CampaignId = campaign.CampaignId,
            CreatedAt = campaign.CreatedAt,
            UpdatedAt = campaign.UpdatedAt
        };
    }
}