using MediatR;
using Cor.PlanDev.Models.DTOs;
using Cor.PlanDev.Models.Entities;
using Cor.PlanDev.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;
using Cor.PlanDev.Constants;
using Cor.PlanDev.Queries;

namespace Cor.PlanDev.Commands;

public class CreateMilestoneCommandHandler
    : IRequestHandler<CreateMilestoneCommand, MilestoneDto>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<CreateMilestoneCommandHandler> _logger;
    private readonly ICacheService _cache;
    private readonly IMediator _mediator;

    public CreateMilestoneCommandHandler(
        PlanDevDbContext context,
        ILogger<CreateMilestoneCommandHandler> logger,
        ICacheService cache,
        IMediator mediator)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
        _mediator = mediator;
    }

    public async Task<MilestoneDto> Handle(CreateMilestoneCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating new milestone for project: {ProjectId}", request.CreateDto.ProjectId);

            // Get max order
            var maxOrder = await _context.Milestones
                .Where(m => m.ProjectId == request.CreateDto.ProjectId)
                .MaxAsync(m => (int?)m.Order) ?? 0;

            var milestone = new Milestone
            {
                Id = Guid.NewGuid(),
                ProjectId = request.CreateDto.ProjectId,
                Name = request.CreateDto.Name,
                Description = request.CreateDto.Description,
                TargetDate = request.CreateDto.TargetDate,
                Status = "Pending",
                Order = maxOrder + 1,
                CompletionPercentage = 0,
                MilestoneType = request.CreateDto.MilestoneType,
                Deliverable = request.CreateDto.Deliverable,
                IsCritical = request.CreateDto.IsCritical,
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            };

            milestone.UpdateRowVersion();

            await _context.Milestones.AddAsync(milestone, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync(string.Format(CacheKeys.MilestonesByProject, request.CreateDto.ProjectId), cancellationToken);

            _logger.LogInformation("✅ Milestone created successfully: {MilestoneName}", milestone.Name);

            // Use MediatR to get the created milestone
            return await _mediator.Send(new GetMilestoneByIdQuery { Id = milestone.Id }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating milestone");
            throw;
        }
    }
}

public class AchieveMilestoneCommandHandler
    : IRequestHandler<AchieveMilestoneCommand, MilestoneDto>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<AchieveMilestoneCommandHandler> _logger;
    private readonly ICacheService _cache;
    private readonly IMediator _mediator;

    public AchieveMilestoneCommandHandler(
        PlanDevDbContext context,
        ILogger<AchieveMilestoneCommandHandler> logger,
        ICacheService cache,
        IMediator mediator)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
        _mediator = mediator;
    }

    public async Task<MilestoneDto> Handle(AchieveMilestoneCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Achieving milestone: {MilestoneId}", request.Id);

            var milestone = await _context.Milestones
                .FirstOrDefaultAsync(m => m.Id == request.Id && !m.IsDeleted, cancellationToken);

            if (milestone == null)
                throw new KeyNotFoundException($"Milestone with ID '{request.Id}' not found");

            if (milestone.Status == "Achieved")
            {
                _logger.LogInformation("Milestone already achieved: {MilestoneName}", milestone.Name);
                return await _mediator.Send(new GetMilestoneByIdQuery { Id = milestone.Id }, cancellationToken);
            }

            milestone.Status = "Achieved";
            milestone.AchievedDate = DateTime.UtcNow;
            milestone.CompletionPercentage = 100;
            milestone.DateMod = DateTime.UtcNow;
            milestone.UpdateRowVersion();

            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync(string.Format(CacheKeys.MilestonesByProject, milestone.ProjectId), cancellationToken);
            await _cache.RemoveAsync($"plandev:milestone:{milestone.Id}", cancellationToken);

            _logger.LogInformation("✅ Milestone achieved successfully: {MilestoneName}", milestone.Name);

            return await _mediator.Send(new GetMilestoneByIdQuery { Id = milestone.Id }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error achieving milestone");
            throw;
        }
    }
}
 public class BulkCreateMilestonesCommandHandler
     : IRequestHandler<BulkCreateMilestonesCommand, List<MilestoneDto>>
 {
     private readonly PlanDevDbContext _context;
     private readonly ILogger<BulkCreateMilestonesCommandHandler> _logger;
     private readonly ICacheService _cache;
     private readonly IMediator _mediator;

     public BulkCreateMilestonesCommandHandler(
         PlanDevDbContext context,
         ILogger<BulkCreateMilestonesCommandHandler> logger,
         ICacheService cache,
         IMediator mediator)
     {
         _context = context;
         _logger = logger;
         _cache = cache;
         _mediator = mediator;
     }

     public async Task<List<MilestoneDto>> Handle(BulkCreateMilestonesCommand request, CancellationToken cancellationToken)
     {
         try
         {
             _logger.LogInformation("Bulk creating {Count} milestones for project: {ProjectId}",
                 request.Milestones.Count,
                 request.Milestones.FirstOrDefault()?.ProjectId);

             var projectId = request.Milestones.FirstOrDefault()?.ProjectId;
             if (projectId == null)
                 throw new ArgumentException("No milestones provided");

             // Get max order for the project
             var maxOrder = await _context.Milestones
                 .Where(m => m.ProjectId == projectId)
                 .MaxAsync(m => (int?)m.Order) ?? 0;

             var milestones = new List<Milestone>();
             var order = maxOrder;

             foreach (var dto in request.Milestones)
             {
                 order++;
                 var milestone = new Milestone
                 {
                     Id = Guid.NewGuid(),
                     ProjectId = dto.ProjectId,
                     Name = dto.Name,
                     Description = dto.Description,
                     TargetDate = dto.TargetDate,
                     Status = "Pending",
                     Order = order,
                     CompletionPercentage = 0,
                     MilestoneType = dto.MilestoneType,
                     Deliverable = dto.Deliverable,
                     IsCritical = dto.IsCritical,
                     DateAdd = DateTime.UtcNow,
                     IsDeleted = false
                 };

                 milestone.UpdateRowVersion();
                 milestones.Add(milestone);
             }

             await _context.Milestones.AddRangeAsync(milestones, cancellationToken);
             await _context.SaveChangesAsync(cancellationToken);

             // Clear cache
             await _cache.RemoveAsync(string.Format(CacheKeys.MilestonesByProject, projectId), cancellationToken);

             _logger.LogInformation("✅ {Count} milestones created successfully", milestones.Count);

             // Return the created milestones
             var result = new List<MilestoneDto>();
             foreach (var milestone in milestones)
             {
                 var dto = await _mediator.Send(new GetMilestoneByIdQuery { Id = milestone.Id }, cancellationToken);
                 result.Add(dto);
             }

             return result;
         }
         catch (Exception ex)
         {
             _logger.LogError(ex, "Error bulk creating milestones");
             throw;
         }
     }
 }