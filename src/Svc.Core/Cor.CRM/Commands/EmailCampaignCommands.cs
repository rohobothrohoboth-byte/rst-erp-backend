// Cor.CRM/Commands/EmailCampaignCommands.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Cor.CRM.Commands;

public class EmailCampaignAddCmd : IRequest<EmailCampaignDto>
{
    public CreateEmailCampaignDto Dto { get; set; } = default!;
}

public class EmailCampaignUpdateCmd : IRequest<EmailCampaignDto>
{
    public Guid Id { get; set; }
    public UpdateEmailCampaignDto Dto { get; set; } = default!;
}

public class EmailCampaignDeleteCmd : IRequest
{
    public Guid Id { get; set; }
}

public class EmailCampaignSendCmd : IRequest<EmailCampaignDto>
{
    public Guid Id { get; set; }
}

public class EmailCampaignDuplicateCmd : IRequest<EmailCampaignDto>
{
    public Guid Id { get; set; }
}

public class EmailCampaignPauseCmd : IRequest<EmailCampaignDto>
{
    public Guid Id { get; set; }
}

public class EmailCampaignResumeCmd : IRequest<EmailCampaignDto>
{
    public Guid Id { get; set; }
}

public class EmailCampaignCancelCmd : IRequest<EmailCampaignDto>
{
    public Guid Id { get; set; }
}

public class EmailCampaignAddHandler : IRequestHandler<EmailCampaignAddCmd, EmailCampaignDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public EmailCampaignAddHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<EmailCampaignDto> Handle(EmailCampaignAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var campaign = new EmailCampaign
            {
                Id = Guid.CreateVersion7(),
                Name = request.Dto.Name,
                Subject = request.Dto.Subject,
                Content = request.Dto.Content,
                HtmlContent = request.Dto.HtmlContent,
                Status = string.IsNullOrEmpty(request.Dto.Status)
                    ? EmailCampaignStatus.Draft
                    : Enum.Parse<EmailCampaignStatus>(request.Dto.Status),
                ScheduledDate = request.Dto.ScheduledDate.HasValue
                    ? DateTimeHelper.EnsureUtc(request.Dto.ScheduledDate.Value)
                    : null,
                TemplateId = request.Dto.TemplateId,
                CampaignId = request.Dto.CampaignId,
                CreatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                IsDeleted = false
            };

            await _uow.Add(campaign, ct);
            await _uow.Commit(ct);

            _logger.LogInformation("Email campaign created: {CampaignId}", campaign.Id);

            return MapToDto(campaign);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private EmailCampaignDto MapToDto(EmailCampaign campaign)
    {
        return new EmailCampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Subject = campaign.Subject,
            Content = campaign.Content,
            HtmlContent = campaign.HtmlContent,
            Status = campaign.Status.ToString(),
            ScheduledDate = campaign.ScheduledDate,
            SentDate = campaign.SentDate,
            RecipientCount = campaign.RecipientCount,
            SentCount = campaign.SentCount,
            DeliveredCount = campaign.DeliveredCount,
            OpenCount = campaign.OpenCount,
            ClickCount = campaign.ClickCount,
            BounceCount = campaign.BounceCount,
            UnsubscribeCount = campaign.UnsubscribeCount,
            OpenRate = campaign.OpenRate,
            ClickRate = campaign.ClickRate,
            TemplateId = campaign.TemplateId,
            CampaignId = campaign.CampaignId,
            CreatedAt = campaign.CreatedAt,
            UpdatedAt = campaign.UpdatedAt
        };
    }
}

public class EmailCampaignUpdateHandler : IRequestHandler<EmailCampaignUpdateCmd, EmailCampaignDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public EmailCampaignUpdateHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<EmailCampaignDto> Handle(EmailCampaignUpdateCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var campaign = await _uow.Set<EmailCampaign>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (campaign == null)
                throw new DomainException($"Email campaign with id [{request.Id}] NOT FOUND.");

            if (campaign.Status == EmailCampaignStatus.Sent || campaign.Status == EmailCampaignStatus.Sending)
                throw new DomainException($"Cannot update campaign with status [{campaign.Status}]");

            if (!string.IsNullOrEmpty(request.Dto.Name))
                campaign.Name = request.Dto.Name;

            if (!string.IsNullOrEmpty(request.Dto.Subject))
                campaign.Subject = request.Dto.Subject;

            if (!string.IsNullOrEmpty(request.Dto.Content))
                campaign.Content = request.Dto.Content;

            if (request.Dto.HtmlContent != null)
                campaign.HtmlContent = request.Dto.HtmlContent;

            if (!string.IsNullOrEmpty(request.Dto.Status))
                campaign.Status = Enum.Parse<EmailCampaignStatus>(request.Dto.Status);

            if (request.Dto.ScheduledDate.HasValue)
                campaign.ScheduledDate = DateTimeHelper.EnsureUtc(request.Dto.ScheduledDate.Value);

            if (request.Dto.TemplateId.HasValue)
                campaign.TemplateId = request.Dto.TemplateId;

            campaign.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(campaign);
            await _uow.Commit(ct);

            _logger.LogInformation("Email campaign updated: {CampaignId}", campaign.Id);

            return MapToDto(campaign);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private EmailCampaignDto MapToDto(EmailCampaign campaign)
    {
        return new EmailCampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Subject = campaign.Subject,
            Content = campaign.Content,
            HtmlContent = campaign.HtmlContent,
            Status = campaign.Status.ToString(),
            ScheduledDate = campaign.ScheduledDate,
            SentDate = campaign.SentDate,
            RecipientCount = campaign.RecipientCount,
            SentCount = campaign.SentCount,
            DeliveredCount = campaign.DeliveredCount,
            OpenCount = campaign.OpenCount,
            ClickCount = campaign.ClickCount,
            BounceCount = campaign.BounceCount,
            UnsubscribeCount = campaign.UnsubscribeCount,
            OpenRate = campaign.OpenRate,
            ClickRate = campaign.ClickRate,
            TemplateId = campaign.TemplateId,
            CampaignId = campaign.CampaignId,
            CreatedAt = campaign.CreatedAt,
            UpdatedAt = campaign.UpdatedAt
        };
    }
}

public class EmailCampaignDeleteHandler : IRequestHandler<EmailCampaignDeleteCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public EmailCampaignDeleteHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task Handle(EmailCampaignDeleteCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var campaign = await _uow.Set<EmailCampaign>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (campaign == null)
                throw new DomainException($"Email campaign with id [{request.Id}] NOT FOUND.");

            if (campaign.Status == EmailCampaignStatus.Sent || campaign.Status == EmailCampaignStatus.Sending)
                throw new DomainException($"Cannot delete campaign with status [{campaign.Status}]");

            campaign.IsDeleted = true;
            campaign.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(campaign);
            await _uow.Commit(ct);

            _logger.LogInformation("Email campaign deleted: {CampaignId}", campaign.Id);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class EmailCampaignSendHandler : IRequestHandler<EmailCampaignSendCmd, EmailCampaignDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public EmailCampaignSendHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<EmailCampaignDto> Handle(EmailCampaignSendCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var campaign = await _uow.Set<EmailCampaign>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (campaign == null)
                throw new DomainException($"Email campaign with id [{request.Id}] NOT FOUND.");

            if (campaign.Status == EmailCampaignStatus.Sent)
                throw new DomainException("Campaign is already sent");

            if (campaign.Status == EmailCampaignStatus.Sending)
                throw new DomainException("Campaign is already sending");

            campaign.Status = EmailCampaignStatus.Sending;
            campaign.SentDate = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
            campaign.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(campaign);
            await _uow.Commit(ct);

            _logger.LogInformation("Email campaign sent: {CampaignId}", campaign.Id);

            return MapToDto(campaign);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private EmailCampaignDto MapToDto(EmailCampaign campaign)
    {
        return new EmailCampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Subject = campaign.Subject,
            Content = campaign.Content,
            HtmlContent = campaign.HtmlContent,
            Status = campaign.Status.ToString(),
            ScheduledDate = campaign.ScheduledDate,
            SentDate = campaign.SentDate,
            RecipientCount = campaign.RecipientCount,
            SentCount = campaign.SentCount,
            DeliveredCount = campaign.DeliveredCount,
            OpenCount = campaign.OpenCount,
            ClickCount = campaign.ClickCount,
            BounceCount = campaign.BounceCount,
            UnsubscribeCount = campaign.UnsubscribeCount,
            OpenRate = campaign.OpenRate,
            ClickRate = campaign.ClickRate,
            TemplateId = campaign.TemplateId,
            CampaignId = campaign.CampaignId,
            CreatedAt = campaign.CreatedAt,
            UpdatedAt = campaign.UpdatedAt
        };
    }
}

public class EmailCampaignDuplicateHandler : IRequestHandler<EmailCampaignDuplicateCmd, EmailCampaignDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public EmailCampaignDuplicateHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<EmailCampaignDto> Handle(EmailCampaignDuplicateCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var original = await _uow.Set<EmailCampaign>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (original == null)
                throw new DomainException($"Email campaign with id [{request.Id}] NOT FOUND.");

            var duplicate = new EmailCampaign
            {
                Id = Guid.CreateVersion7(),
                Name = $"{original.Name} (Copy)",
                Subject = original.Subject,
                Content = original.Content,
                HtmlContent = original.HtmlContent,
                Status = EmailCampaignStatus.Draft,
                TemplateId = original.TemplateId,
                CampaignId = original.CampaignId,
                CreatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                IsDeleted = false
            };

            await _uow.Add(duplicate, ct);
            await _uow.Commit(ct);

            _logger.LogInformation("Email campaign duplicated: {OriginalId} -> {DuplicateId}", original.Id, duplicate.Id);

            return MapToDto(duplicate);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private EmailCampaignDto MapToDto(EmailCampaign campaign)
    {
        return new EmailCampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Subject = campaign.Subject,
            Content = campaign.Content,
            HtmlContent = campaign.HtmlContent,
            Status = campaign.Status.ToString(),
            ScheduledDate = campaign.ScheduledDate,
            SentDate = campaign.SentDate,
            RecipientCount = campaign.RecipientCount,
            SentCount = campaign.SentCount,
            DeliveredCount = campaign.DeliveredCount,
            OpenCount = campaign.OpenCount,
            ClickCount = campaign.ClickCount,
            BounceCount = campaign.BounceCount,
            UnsubscribeCount = campaign.UnsubscribeCount,
            OpenRate = campaign.OpenRate,
            ClickRate = campaign.ClickRate,
            TemplateId = campaign.TemplateId,
            CampaignId = campaign.CampaignId,
            CreatedAt = campaign.CreatedAt,
            UpdatedAt = campaign.UpdatedAt
        };
    }
}

public class EmailCampaignPauseHandler : IRequestHandler<EmailCampaignPauseCmd, EmailCampaignDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public EmailCampaignPauseHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<EmailCampaignDto> Handle(EmailCampaignPauseCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var campaign = await _uow.Set<EmailCampaign>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (campaign == null)
                throw new DomainException($"Email campaign with id [{request.Id}] NOT FOUND.");

            if (campaign.Status != EmailCampaignStatus.Sending && campaign.Status != EmailCampaignStatus.Scheduled)
                throw new DomainException($"Cannot pause campaign with status [{campaign.Status}]");

            campaign.Status = EmailCampaignStatus.Paused;
            campaign.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(campaign);
            await _uow.Commit(ct);

            _logger.LogInformation("Email campaign paused: {CampaignId}", campaign.Id);

            return MapToDto(campaign);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private EmailCampaignDto MapToDto(EmailCampaign campaign)
    {
        return new EmailCampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Subject = campaign.Subject,
            Content = campaign.Content,
            HtmlContent = campaign.HtmlContent,
            Status = campaign.Status.ToString(),
            ScheduledDate = campaign.ScheduledDate,
            SentDate = campaign.SentDate,
            RecipientCount = campaign.RecipientCount,
            SentCount = campaign.SentCount,
            DeliveredCount = campaign.DeliveredCount,
            OpenCount = campaign.OpenCount,
            ClickCount = campaign.ClickCount,
            BounceCount = campaign.BounceCount,
            UnsubscribeCount = campaign.UnsubscribeCount,
            OpenRate = campaign.OpenRate,
            ClickRate = campaign.ClickRate,
            TemplateId = campaign.TemplateId,
            CampaignId = campaign.CampaignId,
            CreatedAt = campaign.CreatedAt,
            UpdatedAt = campaign.UpdatedAt
        };
    }
}

public class EmailCampaignResumeHandler : IRequestHandler<EmailCampaignResumeCmd, EmailCampaignDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public EmailCampaignResumeHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<EmailCampaignDto> Handle(EmailCampaignResumeCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var campaign = await _uow.Set<EmailCampaign>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (campaign == null)
                throw new DomainException($"Email campaign with id [{request.Id}] NOT FOUND.");

            if (campaign.Status != EmailCampaignStatus.Paused)
                throw new DomainException($"Cannot resume campaign with status [{campaign.Status}]");

            campaign.Status = EmailCampaignStatus.Sending;
            campaign.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(campaign);
            await _uow.Commit(ct);

            _logger.LogInformation("Email campaign resumed: {CampaignId}", campaign.Id);

            return MapToDto(campaign);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private EmailCampaignDto MapToDto(EmailCampaign campaign)
    {
        return new EmailCampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Subject = campaign.Subject,
            Content = campaign.Content,
            HtmlContent = campaign.HtmlContent,
            Status = campaign.Status.ToString(),
            ScheduledDate = campaign.ScheduledDate,
            SentDate = campaign.SentDate,
            RecipientCount = campaign.RecipientCount,
            SentCount = campaign.SentCount,
            DeliveredCount = campaign.DeliveredCount,
            OpenCount = campaign.OpenCount,
            ClickCount = campaign.ClickCount,
            BounceCount = campaign.BounceCount,
            UnsubscribeCount = campaign.UnsubscribeCount,
            OpenRate = campaign.OpenRate,
            ClickRate = campaign.ClickRate,
            TemplateId = campaign.TemplateId,
            CampaignId = campaign.CampaignId,
            CreatedAt = campaign.CreatedAt,
            UpdatedAt = campaign.UpdatedAt
        };
    }
}

public class EmailCampaignCancelHandler : IRequestHandler<EmailCampaignCancelCmd, EmailCampaignDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public EmailCampaignCancelHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<EmailCampaignDto> Handle(EmailCampaignCancelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var campaign = await _uow.Set<EmailCampaign>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (campaign == null)
                throw new DomainException($"Email campaign with id [{request.Id}] NOT FOUND.");

            if (campaign.Status == EmailCampaignStatus.Sent)
                throw new DomainException("Cannot cancel a sent campaign");

            if (campaign.Status == EmailCampaignStatus.Cancelled)
                throw new DomainException("Campaign is already cancelled");

            campaign.Status = EmailCampaignStatus.Cancelled;
            campaign.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(campaign);
            await _uow.Commit(ct);

            _logger.LogInformation("Email campaign cancelled: {CampaignId}", campaign.Id);

            return MapToDto(campaign);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private EmailCampaignDto MapToDto(EmailCampaign campaign)
    {
        return new EmailCampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Subject = campaign.Subject,
            Content = campaign.Content,
            HtmlContent = campaign.HtmlContent,
            Status = campaign.Status.ToString(),
            ScheduledDate = campaign.ScheduledDate,
            SentDate = campaign.SentDate,
            RecipientCount = campaign.RecipientCount,
            SentCount = campaign.SentCount,
            DeliveredCount = campaign.DeliveredCount,
            OpenCount = campaign.OpenCount,
            ClickCount = campaign.ClickCount,
            BounceCount = campaign.BounceCount,
            UnsubscribeCount = campaign.UnsubscribeCount,
            OpenRate = campaign.OpenRate,
            ClickRate = campaign.ClickRate,
            TemplateId = campaign.TemplateId,
            CampaignId = campaign.CampaignId,
            CreatedAt = campaign.CreatedAt,
            UpdatedAt = campaign.UpdatedAt
        };
    }
}