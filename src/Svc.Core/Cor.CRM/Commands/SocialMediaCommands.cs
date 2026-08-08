// Cor.CRM/Commands/SocialMediaCommands.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Cor.CRM.Commands;

public class SocialMediaAddCmd : IRequest<SocialMediaPostDto>
{
    public CreateSocialMediaPostDto Dto { get; set; } = default!;
}

public class SocialMediaUpdateCmd : IRequest<SocialMediaPostDto>
{
    public Guid Id { get; set; }
    public UpdateSocialMediaPostDto Dto { get; set; } = default!;
}

public class SocialMediaDeleteCmd : IRequest
{
    public Guid Id { get; set; }
}

public class SocialMediaPublishCmd : IRequest<SocialMediaPostDto>
{
    public Guid Id { get; set; }
}

public class SocialMediaDuplicateCmd : IRequest<SocialMediaPostDto>
{
    public Guid Id { get; set; }
}

public class SocialMediaAddHandler : IRequestHandler<SocialMediaAddCmd, SocialMediaPostDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public SocialMediaAddHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<SocialMediaPostDto> Handle(SocialMediaAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var post = new SocialMediaPost
            {
                Id = Guid.CreateVersion7(),
                Content = request.Dto.Content,
                Platform = Enum.Parse<SocialMediaPlatform>(request.Dto.Platform),
                ImageUrl = request.Dto.ImageUrl,
                VideoUrl = request.Dto.VideoUrl,
                LinkUrl = request.Dto.LinkUrl,
                Location = request.Dto.Location,
                Hashtags = request.Dto.Hashtags,
                Status = string.IsNullOrEmpty(request.Dto.Status)
                    ? SocialMediaStatus.Draft
                    : Enum.Parse<SocialMediaStatus>(request.Dto.Status),
                ScheduledDate = request.Dto.ScheduledDate.HasValue
                    ? DateTimeHelper.EnsureUtc(request.Dto.ScheduledDate.Value)
                    : null,
                CampaignId = request.Dto.CampaignId,
                CreatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                IsDeleted = false
            };

            await _uow.Add(post, ct);
            await _uow.Commit(ct);

            _logger.LogInformation("Social media post created: {PostId}", post.Id);

            return MapToDto(post);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private SocialMediaPostDto MapToDto(SocialMediaPost post)
    {
        return new SocialMediaPostDto
        {
            Id = post.Id,
            Content = post.Content,
            Platform = post.Platform.ToString(),
            ImageUrl = post.ImageUrl,
            VideoUrl = post.VideoUrl,
            LinkUrl = post.LinkUrl,
            Location = post.Location,
            Hashtags = post.Hashtags,
            Status = post.Status.ToString(),
            ScheduledDate = post.ScheduledDate,
            PublishedDate = post.PublishedDate,
            EngagementCount = post.EngagementCount,
            ReachCount = post.ReachCount,
            LikeCount = post.LikeCount,
            ShareCount = post.ShareCount,
            CommentCount = post.CommentCount,
            PostId = post.PostId,
            CampaignId = post.CampaignId,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }
}

public class SocialMediaUpdateHandler : IRequestHandler<SocialMediaUpdateCmd, SocialMediaPostDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public SocialMediaUpdateHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<SocialMediaPostDto> Handle(SocialMediaUpdateCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var post = await _uow.Set<SocialMediaPost>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (post == null)
                throw new DomainException($"Social media post with id [{request.Id}] NOT FOUND.");

            if (post.Status == SocialMediaStatus.Published)
                throw new DomainException("Cannot update a published post");

            if (!string.IsNullOrEmpty(request.Dto.Content))
                post.Content = request.Dto.Content;

            if (!string.IsNullOrEmpty(request.Dto.Platform))
                post.Platform = Enum.Parse<SocialMediaPlatform>(request.Dto.Platform);

            if (request.Dto.ImageUrl != null)
                post.ImageUrl = request.Dto.ImageUrl;

            if (request.Dto.VideoUrl != null)
                post.VideoUrl = request.Dto.VideoUrl;

            if (request.Dto.LinkUrl != null)
                post.LinkUrl = request.Dto.LinkUrl;

            if (request.Dto.Location != null)
                post.Location = request.Dto.Location;

            if (request.Dto.Hashtags != null)
                post.Hashtags = request.Dto.Hashtags;

            if (!string.IsNullOrEmpty(request.Dto.Status))
                post.Status = Enum.Parse<SocialMediaStatus>(request.Dto.Status);

            if (request.Dto.ScheduledDate.HasValue)
                post.ScheduledDate = DateTimeHelper.EnsureUtc(request.Dto.ScheduledDate.Value);

            post.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(post);
            await _uow.Commit(ct);

            _logger.LogInformation("Social media post updated: {PostId}", post.Id);

            return MapToDto(post);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private SocialMediaPostDto MapToDto(SocialMediaPost post)
    {
        return new SocialMediaPostDto
        {
            Id = post.Id,
            Content = post.Content,
            Platform = post.Platform.ToString(),
            ImageUrl = post.ImageUrl,
            VideoUrl = post.VideoUrl,
            LinkUrl = post.LinkUrl,
            Location = post.Location,
            Hashtags = post.Hashtags,
            Status = post.Status.ToString(),
            ScheduledDate = post.ScheduledDate,
            PublishedDate = post.PublishedDate,
            EngagementCount = post.EngagementCount,
            ReachCount = post.ReachCount,
            LikeCount = post.LikeCount,
            ShareCount = post.ShareCount,
            CommentCount = post.CommentCount,
            PostId = post.PostId,
            CampaignId = post.CampaignId,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }
}

public class SocialMediaDeleteHandler : IRequestHandler<SocialMediaDeleteCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public SocialMediaDeleteHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task Handle(SocialMediaDeleteCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var post = await _uow.Set<SocialMediaPost>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (post == null)
                throw new DomainException($"Social media post with id [{request.Id}] NOT FOUND.");

            if (post.Status == SocialMediaStatus.Published)
                throw new DomainException("Cannot delete a published post");

            post.IsDeleted = true;
            post.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(post);
            await _uow.Commit(ct);

            _logger.LogInformation("Social media post deleted: {PostId}", post.Id);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class SocialMediaPublishHandler : IRequestHandler<SocialMediaPublishCmd, SocialMediaPostDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public SocialMediaPublishHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<SocialMediaPostDto> Handle(SocialMediaPublishCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var post = await _uow.Set<SocialMediaPost>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (post == null)
                throw new DomainException($"Social media post with id [{request.Id}] NOT FOUND.");

            if (post.Status == SocialMediaStatus.Published)
                throw new DomainException("Post is already published");

            post.Status = SocialMediaStatus.Published;
            post.PublishedDate = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
            post.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(post);
            await _uow.Commit(ct);

            _logger.LogInformation("Social media post published: {PostId}", post.Id);

            return MapToDto(post);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private SocialMediaPostDto MapToDto(SocialMediaPost post)
    {
        return new SocialMediaPostDto
        {
            Id = post.Id,
            Content = post.Content,
            Platform = post.Platform.ToString(),
            ImageUrl = post.ImageUrl,
            VideoUrl = post.VideoUrl,
            LinkUrl = post.LinkUrl,
            Location = post.Location,
            Hashtags = post.Hashtags,
            Status = post.Status.ToString(),
            ScheduledDate = post.ScheduledDate,
            PublishedDate = post.PublishedDate,
            EngagementCount = post.EngagementCount,
            ReachCount = post.ReachCount,
            LikeCount = post.LikeCount,
            ShareCount = post.ShareCount,
            CommentCount = post.CommentCount,
            PostId = post.PostId,
            CampaignId = post.CampaignId,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }
}

public class SocialMediaDuplicateHandler : IRequestHandler<SocialMediaDuplicateCmd, SocialMediaPostDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public SocialMediaDuplicateHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<SocialMediaPostDto> Handle(SocialMediaDuplicateCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var original = await _uow.Set<SocialMediaPost>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (original == null)
                throw new DomainException($"Social media post with id [{request.Id}] NOT FOUND.");

            var duplicate = new SocialMediaPost
            {
                Id = Guid.CreateVersion7(),
                Content = $"{original.Content} (Copy)",
                Platform = original.Platform,
                ImageUrl = original.ImageUrl,
                VideoUrl = original.VideoUrl,
                LinkUrl = original.LinkUrl,
                Location = original.Location,
                Hashtags = original.Hashtags,
                Status = SocialMediaStatus.Draft,
                CampaignId = original.CampaignId,
                CreatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                IsDeleted = false
            };

            await _uow.Add(duplicate, ct);
            await _uow.Commit(ct);

            _logger.LogInformation("Social media post duplicated: {OriginalId} -> {DuplicateId}", original.Id, duplicate.Id);

            return MapToDto(duplicate);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private SocialMediaPostDto MapToDto(SocialMediaPost post)
    {
        return new SocialMediaPostDto
        {
            Id = post.Id,
            Content = post.Content,
            Platform = post.Platform.ToString(),
            ImageUrl = post.ImageUrl,
            VideoUrl = post.VideoUrl,
            LinkUrl = post.LinkUrl,
            Location = post.Location,
            Hashtags = post.Hashtags,
            Status = post.Status.ToString(),
            ScheduledDate = post.ScheduledDate,
            PublishedDate = post.PublishedDate,
            EngagementCount = post.EngagementCount,
            ReachCount = post.ReachCount,
            LikeCount = post.LikeCount,
            ShareCount = post.ShareCount,
            CommentCount = post.CommentCount,
            PostId = post.PostId,
            CampaignId = post.CampaignId,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }
}