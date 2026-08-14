// Handlers/CommentHandlers.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Cor.ProjectManagement.Models.Entities;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Persistence;
using Cor.ProjectManagement.Commands.CommentCommands;
using Cor.ProjectManagement.Queries.CommentQueries;

namespace Cor.ProjectManagement.Handlers
{
    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, ProjectCommentDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateCommentCommandHandler> _logger;

        public CreateCommentCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<CreateCommentCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectCommentDto> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate project exists
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == request.ProjectId && !p.IsDeleted, cancellationToken);

                if (project == null)
                    throw new Exception($"Project with ID {request.ProjectId} not found");

                // Validate parent comment if provided
                if (request.ParentCommentId.HasValue)
                {
                    var parent = await _context.ProjectComments
                        .FirstOrDefaultAsync(c => c.Id == request.ParentCommentId.Value && !c.IsDeleted, cancellationToken);

                    if (parent == null)
                        throw new Exception($"Parent comment with ID {request.ParentCommentId} not found");
                }

                var comment = new ProjectComment
                {
                    Id = Guid.NewGuid(),
                    Content = request.Content,
                    ProjectId = request.ProjectId,
                    TaskId = request.TaskId,
                    MilestoneId = request.MilestoneId,
                    IssueId = request.IssueId,
                    ParentCommentId = request.ParentCommentId,
                    AuthorId = Guid.TryParse(request.CreatedBy, out var id) ? id : Guid.Empty,
                    AuthorName = request.AuthorName ?? request.CreatedBy ?? "System",
                    IsPinned = false,
                    IsResolved = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy ?? "System"
                };

                await _context.ProjectComments.AddAsync(comment, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Comment created successfully with ID: {CommentId}", comment.Id);
                return _mapper.Map<ProjectCommentDto>(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating comment");
                throw;
            }
        }
    }

    public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, ProjectCommentDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateCommentCommandHandler> _logger;

        public UpdateCommentCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<UpdateCommentCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectCommentDto> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var comment = await _context.ProjectComments
                    .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

                if (comment == null)
                    throw new Exception($"Comment with ID {request.Id} not found");

                if (!string.IsNullOrEmpty(request.Content))
                {
                    comment.Content = request.Content;
                    comment.EditedAt = DateTime.UtcNow;
                    comment.EditedById = Guid.TryParse(request.UpdatedBy, out var id) ? id : null;
                    comment.EditedByName = request.UpdatedBy ?? "System";
                }

                if (request.IsPinned.HasValue)
                    comment.IsPinned = request.IsPinned.Value;

                if (request.IsResolved.HasValue)
                    comment.IsResolved = request.IsResolved.Value;

                comment.UpdatedAt = DateTime.UtcNow;
                comment.UpdatedBy = request.UpdatedBy ?? "System";
                comment.Version += 1;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Comment updated successfully with ID: {CommentId}", comment.Id);
                return _mapper.Map<ProjectCommentDto>(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating comment with ID: {CommentId}", request.Id);
                throw;
            }
        }
    }

    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, bool>
    {
        private readonly ProjectDbContext _context;
        private readonly ILogger<DeleteCommentCommandHandler> _logger;

        public DeleteCommentCommandHandler(
            ProjectDbContext context,
            ILogger<DeleteCommentCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var comment = await _context.ProjectComments
                    .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

                if (comment == null)
                    throw new Exception($"Comment with ID {request.Id} not found");

                // Check for replies
                var hasReplies = await _context.ProjectComments
                    .AnyAsync(c => c.ParentCommentId == request.Id && !c.IsDeleted, cancellationToken);

                if (hasReplies)
                    throw new Exception("Cannot delete comment with replies. Delete replies first.");

                comment.IsDeleted = true;
                comment.DeletedAt = DateTime.UtcNow;
                comment.DeletedBy = request.DeletedBy ?? "System";

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Comment deleted successfully with ID: {CommentId}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment with ID: {CommentId}", request.Id);
                throw;
            }
        }
    }

    public class PinCommentCommandHandler : IRequestHandler<PinCommentCommand, ProjectCommentDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<PinCommentCommandHandler> _logger;

        public PinCommentCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<PinCommentCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectCommentDto> Handle(PinCommentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var comment = await _context.ProjectComments
                    .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

                if (comment == null)
                    throw new Exception($"Comment with ID {request.Id} not found");

                comment.IsPinned = request.IsPinned;
                comment.UpdatedAt = DateTime.UtcNow;
                comment.UpdatedBy = request.UpdatedBy ?? "System";
                comment.Version += 1;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Comment pin status updated to {IsPinned} for ID: {CommentId}",
                    request.IsPinned, comment.Id);
                return _mapper.Map<ProjectCommentDto>(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error pinning comment with ID: {CommentId}", request.Id);
                throw;
            }
        }
    }

    public class ResolveCommentCommandHandler : IRequestHandler<ResolveCommentCommand, ProjectCommentDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ResolveCommentCommandHandler> _logger;

        public ResolveCommentCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<ResolveCommentCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectCommentDto> Handle(ResolveCommentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var comment = await _context.ProjectComments
                    .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

                if (comment == null)
                    throw new Exception($"Comment with ID {request.Id} not found");

                comment.IsResolved = request.IsResolved;
                comment.UpdatedAt = DateTime.UtcNow;
                comment.UpdatedBy = request.UpdatedBy ?? "System";
                comment.Version += 1;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Comment resolution status updated to {IsResolved} for ID: {CommentId}",
                    request.IsResolved, comment.Id);
                return _mapper.Map<ProjectCommentDto>(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving comment with ID: {CommentId}", request.Id);
                throw;
            }
        }
    }

    // Query Handlers
    public class GetCommentByIdQueryHandler : IRequestHandler<GetCommentByIdQuery, ProjectCommentDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCommentByIdQueryHandler> _logger;

        public GetCommentByIdQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetCommentByIdQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectCommentDto> Handle(GetCommentByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var comment = await _context.ProjectComments
                    .AsNoTracking()
                    .Include(c => c.Replies.Where(r => !r.IsDeleted))
                    .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

                if (comment == null)
                    throw new Exception($"Comment with ID {request.Id} not found");

                var commentDto = _mapper.Map<ProjectCommentDto>(comment);
                commentDto.ReplyCount = comment.Replies?.Count ?? 0;
                commentDto.Replies = _mapper.Map<List<ProjectCommentDto>>(comment.Replies ?? new List<ProjectComment>());

                return commentDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comment with ID: {CommentId}", request.Id);
                throw;
            }
        }
    }

    public class GetCommentsByProjectQueryHandler : IRequestHandler<GetCommentsByProjectQuery, List<ProjectCommentDto>>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCommentsByProjectQueryHandler> _logger;

        public GetCommentsByProjectQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetCommentsByProjectQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ProjectCommentDto>> Handle(GetCommentsByProjectQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.ProjectComments
                    .AsNoTracking()
                    .Where(c => c.ProjectId == request.ProjectId && !c.IsDeleted && c.ParentCommentId == null);

                if (request.TaskId.HasValue)
                    query = query.Where(c => c.TaskId == request.TaskId.Value);

                if (request.MilestoneId.HasValue)
                    query = query.Where(c => c.MilestoneId == request.MilestoneId.Value);

                if (request.IssueId.HasValue)
                    query = query.Where(c => c.IssueId == request.IssueId.Value);

                if (request.IsPinned.HasValue)
                    query = query.Where(c => c.IsPinned == request.IsPinned.Value);

                if (request.IsResolved.HasValue)
                    query = query.Where(c => c.IsResolved == request.IsResolved.Value);

                var items = await query
                    .OrderByDescending(c => c.IsPinned)
                    .ThenByDescending(c => c.CreatedAt)
                    .Select(c => new ProjectCommentDto
                    {
                        Id = c.Id,
                        Content = c.Content,
                        ProjectId = c.ProjectId,
                        TaskId = c.TaskId,
                        MilestoneId = c.MilestoneId,
                        IssueId = c.IssueId,
                        AuthorName = c.AuthorName,
                        CreatedAt = c.CreatedAt,
                        EditedAt = c.EditedAt,
                        IsPinned = c.IsPinned,
                        IsResolved = c.IsResolved,
                        ReplyCount = c.Replies.Count(r => !r.IsDeleted)
                    })
                    .ToListAsync(cancellationToken);

                return items;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comments for project ID: {ProjectId}", request.ProjectId);
                throw;
            }
        }
    }

    public class GetCommentThreadQueryHandler : IRequestHandler<GetCommentThreadQuery, List<ProjectCommentDto>>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCommentThreadQueryHandler> _logger;

        public GetCommentThreadQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetCommentThreadQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ProjectCommentDto>> Handle(GetCommentThreadQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var rootComment = await _context.ProjectComments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == request.CommentId && !c.IsDeleted, cancellationToken);

                if (rootComment == null)
                    throw new Exception($"Comment with ID {request.CommentId} not found");

                // Get root comment and all replies
                var thread = new List<ProjectCommentDto>();
                var rootDto = _mapper.Map<ProjectCommentDto>(rootComment);
                thread.Add(rootDto);

                // Get all replies recursively
                var replies = await GetReplies(rootComment.Id, cancellationToken);
                thread.AddRange(replies);

                return thread;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comment thread for ID: {CommentId}", request.CommentId);
                throw;
            }
        }

        private async Task<List<ProjectCommentDto>> GetReplies(Guid parentId, CancellationToken cancellationToken)
        {
            var result = new List<ProjectCommentDto>();

            var replies = await _context.ProjectComments
                .AsNoTracking()
                .Where(c => c.ParentCommentId == parentId && !c.IsDeleted)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync(cancellationToken);

            foreach (var reply in replies)
            {
                var replyDto = _mapper.Map<ProjectCommentDto>(reply);
                result.Add(replyDto);

                // Get nested replies
                var nestedReplies = await GetReplies(reply.Id, cancellationToken);
                result.AddRange(nestedReplies);
            }

            return result;
        }
    }
}