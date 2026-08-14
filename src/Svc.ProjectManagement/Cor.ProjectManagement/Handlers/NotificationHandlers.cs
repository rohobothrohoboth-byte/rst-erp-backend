// Handlers/NotificationHandlers.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Cor.ProjectManagement.Models.Entities;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Persistence;
using Cor.ProjectManagement.Commands.NotificationCommands;
using Cor.ProjectManagement.Queries.NotificationQueries;

namespace Cor.ProjectManagement.Handlers
{
    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, ProjectNotificationDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateNotificationCommandHandler> _logger;

        public CreateNotificationCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<CreateNotificationCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectNotificationDto> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var notification = new ProjectNotification
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    UserName = request.UserName,
                    Type = request.Type,
                    Title = request.Title,
                    Message = request.Message,
                    ProjectId = request.ProjectId,
                    ProjectName = request.ProjectId.HasValue ?
                        await _context.Projects
                            .Where(p => p.Id == request.ProjectId)
                            .Select(p => p.Name)
                            .FirstOrDefaultAsync(cancellationToken) ?? string.Empty
                        : string.Empty,
                    EntityId = request.EntityId,
                    EntityType = request.EntityType,
                    Priority = request.Priority,
                    IsRead = false,
                    IsDelivered = false,
                    ActionUrl = request.ActionUrl,
                    ExpiresAt = DateTime.UtcNow.AddDays(30),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy ?? "System"
                };

                await _context.ProjectNotifications.AddAsync(notification, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Notification created successfully for user {UserId} with ID: {NotificationId}",
                    request.UserId, notification.Id);
                return _mapper.Map<ProjectNotificationDto>(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification");
                throw;
            }
        }
    }

    public class MarkNotificationReadCommandHandler : IRequestHandler<MarkNotificationReadCommand, bool>
    {
        private readonly ProjectDbContext _context;
        private readonly ILogger<MarkNotificationReadCommandHandler> _logger;

        public MarkNotificationReadCommandHandler(
            ProjectDbContext context,
            ILogger<MarkNotificationReadCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.MarkAllAsRead)
                {
                    // Mark all notifications as read for the user
                    await _context.ProjectNotifications
                        .Where(n => n.UserId == request.UserId && !n.IsRead && !n.IsDeleted)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(n => n.IsRead, true)
                            .SetProperty(n => n.ReadAt, DateTime.UtcNow),
                            cancellationToken);

                    _logger.LogInformation("All notifications marked as read for user {UserId}", request.UserId);
                }
                else if (request.NotificationIds.Any())
                {
                    // Mark specific notifications as read
                    await _context.ProjectNotifications
                        .Where(n => request.NotificationIds.Contains(n.Id) &&
                                   n.UserId == request.UserId &&
                                   !n.IsRead &&
                                   !n.IsDeleted)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(n => n.IsRead, true)
                            .SetProperty(n => n.ReadAt, DateTime.UtcNow),
                            cancellationToken);

                    _logger.LogInformation("{Count} notifications marked as read for user {UserId}",
                        request.NotificationIds.Count, request.UserId);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notifications as read for user {UserId}", request.UserId);
                throw;
            }
        }
    }

    public class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand, bool>
    {
        private readonly ProjectDbContext _context;
        private readonly ILogger<DeleteNotificationCommandHandler> _logger;

        public DeleteNotificationCommandHandler(
            ProjectDbContext context,
            ILogger<DeleteNotificationCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var notification = await _context.ProjectNotifications
                    .FirstOrDefaultAsync(n => n.Id == request.Id &&
                                             n.UserId == request.UserId &&
                                             !n.IsDeleted, cancellationToken);

                if (notification == null)
                    throw new Exception($"Notification with ID {request.Id} not found for user {request.UserId}");

                notification.IsDeleted = true;
                notification.DeletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Notification {NotificationId} deleted for user {UserId}",
                    request.Id, request.UserId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification {NotificationId} for user {UserId}",
                    request.Id, request.UserId);
                throw;
            }
        }
    }

    public class DeleteAllNotificationsCommandHandler : IRequestHandler<DeleteAllNotificationsCommand, bool>
    {
        private readonly ProjectDbContext _context;
        private readonly ILogger<DeleteAllNotificationsCommandHandler> _logger;

        public DeleteAllNotificationsCommandHandler(
            ProjectDbContext context,
            ILogger<DeleteAllNotificationsCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteAllNotificationsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _context.ProjectNotifications
                    .Where(n => n.UserId == request.UserId && !n.IsDeleted)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(n => n.IsDeleted, true)
                        .SetProperty(n => n.DeletedAt, DateTime.UtcNow),
                        cancellationToken);

                _logger.LogInformation("All notifications deleted for user {UserId}", request.UserId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting all notifications for user {UserId}", request.UserId);
                throw;
            }
        }
    }

    // Query Handlers
    public class GetNotificationByIdQueryHandler : IRequestHandler<GetNotificationByIdQuery, ProjectNotificationDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetNotificationByIdQueryHandler> _logger;

        public GetNotificationByIdQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetNotificationByIdQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectNotificationDto> Handle(GetNotificationByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var notification = await _context.ProjectNotifications
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.Id == request.Id &&
                                             n.UserId == request.UserId &&
                                             !n.IsDeleted, cancellationToken);

                if (notification == null)
                    throw new Exception($"Notification with ID {request.Id} not found");

                return _mapper.Map<ProjectNotificationDto>(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification {NotificationId} for user {UserId}",
                    request.Id, request.UserId);
                throw;
            }
        }
    }

    public class GetUserNotificationsQueryHandler : IRequestHandler<GetUserNotificationsQuery, PaginatedResponse<ProjectNotificationDto>>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserNotificationsQueryHandler> _logger;

        public GetUserNotificationsQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetUserNotificationsQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponse<ProjectNotificationDto>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.ProjectNotifications
                    .AsNoTracking()
                    .Where(n => n.UserId == request.UserId && !n.IsDeleted);

                if (request.IsRead.HasValue)
                    query = query.Where(n => n.IsRead == request.IsRead.Value);

                if (request.Type.HasValue)
                    query = query.Where(n => n.Type == request.Type.Value);

                if (request.Priority.HasValue)
                    query = query.Where(n => n.Priority == request.Priority.Value);

                var totalCount = await query.CountAsync(cancellationToken);

                var items = await query
                    .OrderByDescending(n => n.CreatedAt)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(n => _mapper.Map<ProjectNotificationDto>(n))
                    .ToListAsync(cancellationToken);

                return new PaginatedResponse<ProjectNotificationDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications for user {UserId}", request.UserId);
                throw;
            }
        }
    }

    public class GetUnreadCountQueryHandler : IRequestHandler<GetUnreadCountQuery, int>
    {
        private readonly ProjectDbContext _context;
        private readonly ILogger<GetUnreadCountQueryHandler> _logger;

        public GetUnreadCountQueryHandler(
            ProjectDbContext context,
            ILogger<GetUnreadCountQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> Handle(GetUnreadCountQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var count = await _context.ProjectNotifications
                    .CountAsync(n => n.UserId == request.UserId && !n.IsRead && !n.IsDeleted, cancellationToken);

                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread count for user {UserId}", request.UserId);
                throw;
            }
        }
    }
}