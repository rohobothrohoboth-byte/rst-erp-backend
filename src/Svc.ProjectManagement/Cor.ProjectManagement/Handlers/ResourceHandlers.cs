// Handlers/ResourceHandlers.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Cor.ProjectManagement.Models.Entities;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Persistence;
using Cor.ProjectManagement.Commands.ResourceCommands;
using Cor.ProjectManagement.Queries.ResourceQueries;

namespace Cor.ProjectManagement.Handlers
{
    // ============ COMMAND HANDLERS ============

    public class AllocateResourceCommandHandler : IRequestHandler<AllocateResourceCommand, ProjectResourceDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AllocateResourceCommandHandler> _logger;

        public AllocateResourceCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<AllocateResourceCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectResourceDto> Handle(AllocateResourceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate project exists
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == request.ProjectId && !p.IsDeleted, cancellationToken);

                if (project == null)
                    throw new Exception($"Project with ID {request.ProjectId} not found");

                // Check for overlapping allocations
                var overlapping = await _context.ProjectResources
                    .AnyAsync(r => r.ProjectId == request.ProjectId &&
                                   r.ResourceId == request.ResourceId &&
                                   r.StartDate < request.EndDate &&
                                   r.EndDate > request.StartDate &&
                                   r.Status != ResourceAllocationStatus.Released &&
                                   r.Status != ResourceAllocationStatus.Completed &&
                                   !r.IsDeleted, cancellationToken);

                if (overlapping)
                    throw new Exception("Resource is already allocated to this project during the specified period");

                var resourceAllocation = new ProjectResource
                {
                    Id = Guid.NewGuid(),
                    ProjectId = request.ProjectId,
                    ResourceId = request.ResourceId,
                    ResourceName = request.ResourceName ?? string.Empty,
                    Type = request.Type,
                    Quantity = request.Quantity,
                    CostPerUnit = request.CostPerUnit,
                    TotalCost = request.Quantity * request.CostPerUnit,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Status = ResourceAllocationStatus.Allocated,
                    Notes = request.Notes ?? string.Empty,
                    UnitOfMeasure = request.UnitOfMeasure ?? string.Empty,
                    Skills = request.Skills ?? string.Empty,
                    Department = request.Department ?? string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy ?? "System"
                };

                await _context.ProjectResources.AddAsync(resourceAllocation, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // Update project actual cost
                await UpdateProjectCost(request.ProjectId, cancellationToken);

                _logger.LogInformation("Resource allocated successfully with ID: {ResourceId}", resourceAllocation.Id);
                return _mapper.Map<ProjectResourceDto>(resourceAllocation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error allocating resource");
                throw;
            }
        }

        private async Task UpdateProjectCost(Guid projectId, CancellationToken cancellationToken)
        {
            var totalCost = await _context.ProjectResources
                .Where(r => r.ProjectId == projectId && !r.IsDeleted && r.Status != ResourceAllocationStatus.Released)
                .SumAsync(r => r.TotalCost, cancellationToken);

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted, cancellationToken);

            if (project != null)
            {
                project.ActualCost = totalCost;
                project.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }

    public class UpdateResourceAllocationCommandHandler : IRequestHandler<UpdateResourceAllocationCommand, ProjectResourceDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateResourceAllocationCommandHandler> _logger;

        public UpdateResourceAllocationCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<UpdateResourceAllocationCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectResourceDto> Handle(UpdateResourceAllocationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var resource = await _context.ProjectResources
                    .FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, cancellationToken);

                if (resource == null)
                    throw new Exception($"Resource allocation with ID {request.Id} not found");

                if (resource.Status == ResourceAllocationStatus.Released ||
                    resource.Status == ResourceAllocationStatus.Completed)
                    throw new Exception("Cannot update a released or completed resource allocation");

                // Update only provided fields
                if (request.Quantity.HasValue)
                    resource.Quantity = request.Quantity.Value;

                if (request.CostPerUnit.HasValue)
                    resource.CostPerUnit = request.CostPerUnit.Value;

                // Recalculate total cost
                resource.TotalCost = resource.Quantity * resource.CostPerUnit;

                if (request.StartDate.HasValue)
                    resource.StartDate = request.StartDate.Value;

                if (request.EndDate.HasValue)
                    resource.EndDate = request.EndDate.Value;

                if (request.Status.HasValue)
                    resource.Status = request.Status.Value;

                if (!string.IsNullOrEmpty(request.Notes))
                    resource.Notes = request.Notes;

                resource.UpdatedAt = DateTime.UtcNow;
                resource.UpdatedBy = request.UpdatedBy ?? "System";
                resource.Version += 1;

                await _context.SaveChangesAsync(cancellationToken);

                // Update project cost
                await UpdateProjectCost(resource.ProjectId, cancellationToken);

                _logger.LogInformation("Resource allocation updated successfully with ID: {ResourceId}", resource.Id);
                return _mapper.Map<ProjectResourceDto>(resource);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating resource allocation with ID: {ResourceId}", request.Id);
                throw;
            }
        }

        private async Task UpdateProjectCost(Guid projectId, CancellationToken cancellationToken)
        {
            var totalCost = await _context.ProjectResources
                .Where(r => r.ProjectId == projectId && !r.IsDeleted && r.Status != ResourceAllocationStatus.Released)
                .SumAsync(r => r.TotalCost, cancellationToken);

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted, cancellationToken);

            if (project != null)
            {
                project.ActualCost = totalCost;
                project.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }

    public class ReleaseResourceCommandHandler : IRequestHandler<ReleaseResourceCommand, bool>
    {
        private readonly ProjectDbContext _context;
        private readonly ILogger<ReleaseResourceCommandHandler> _logger;

        public ReleaseResourceCommandHandler(
            ProjectDbContext context,
            ILogger<ReleaseResourceCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(ReleaseResourceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var resource = await _context.ProjectResources
                    .FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, cancellationToken);

                if (resource == null)
                    throw new Exception($"Resource allocation with ID {request.Id} not found");

                if (resource.Status == ResourceAllocationStatus.Released)
                    throw new Exception("Resource is already released");

                resource.Status = ResourceAllocationStatus.Released;
                resource.EndDate = DateTime.UtcNow;
                resource.Notes = !string.IsNullOrEmpty(resource.Notes)
                    ? $"{resource.Notes}\nReleased: {request.Notes ?? "No reason provided"}"
                    : $"Released: {request.Notes ?? "No reason provided"}";
                resource.UpdatedAt = DateTime.UtcNow;
                resource.UpdatedBy = request.ReleasedBy ?? "System";
                resource.Version += 1;

                await _context.SaveChangesAsync(cancellationToken);

                // Update project cost
                await UpdateProjectCost(resource.ProjectId, cancellationToken);

                _logger.LogInformation("Resource released successfully with ID: {ResourceId}", resource.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error releasing resource with ID: {ResourceId}", request.Id);
                throw;
            }
        }

        private async Task UpdateProjectCost(Guid projectId, CancellationToken cancellationToken)
        {
            var totalCost = await _context.ProjectResources
                .Where(r => r.ProjectId == projectId && !r.IsDeleted && r.Status != ResourceAllocationStatus.Released)
                .SumAsync(r => r.TotalCost, cancellationToken);

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted, cancellationToken);

            if (project != null)
            {
                project.ActualCost = totalCost;
                project.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }

    // ============ QUERY HANDLERS ============

    public class GetResourceAllocationsQueryHandler : IRequestHandler<GetResourceAllocationsQuery, PaginatedResponse<ProjectResourceDto>>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetResourceAllocationsQueryHandler> _logger;

        public GetResourceAllocationsQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetResourceAllocationsQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponse<ProjectResourceDto>> Handle(GetResourceAllocationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.ProjectResources
                    .AsNoTracking()
                    .Where(r => !r.IsDeleted);

                if (request.ProjectId.HasValue)
                    query = query.Where(r => r.ProjectId == request.ProjectId.Value);

                if (request.ResourceId.HasValue)
                    query = query.Where(r => r.ResourceId == request.ResourceId.Value);

                if (request.Type.HasValue)
                    query = query.Where(r => r.Type == request.Type.Value);

                if (request.Status.HasValue)
                    query = query.Where(r => r.Status == request.Status.Value);

                if (request.StartDateFrom.HasValue)
                    query = query.Where(r => r.StartDate >= request.StartDateFrom.Value);

                if (request.StartDateTo.HasValue)
                    query = query.Where(r => r.StartDate <= request.StartDateTo.Value);

                var totalCount = await query.CountAsync(cancellationToken);

                var items = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .OrderBy(r => r.StartDate)
                    .Select(r => _mapper.Map<ProjectResourceDto>(r))
                    .ToListAsync(cancellationToken);

                return new PaginatedResponse<ProjectResourceDto>
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
                _logger.LogError(ex, "Error getting resource allocations");
                throw;
            }
        }
    }

    public class GetResourcesByProjectQueryHandler : IRequestHandler<GetResourcesByProjectQuery, List<ProjectResourceDto>>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetResourcesByProjectQueryHandler> _logger;

        public GetResourcesByProjectQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetResourcesByProjectQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ProjectResourceDto>> Handle(GetResourcesByProjectQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.ProjectResources
                    .AsNoTracking()
                    .Where(r => r.ProjectId == request.ProjectId && !r.IsDeleted);

                if (request.Status.HasValue)
                    query = query.Where(r => r.Status == request.Status.Value);

                var items = await query
                    .OrderBy(r => r.StartDate)
                    .Select(r => _mapper.Map<ProjectResourceDto>(r))
                    .ToListAsync(cancellationToken);

                return items;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting resources for project ID: {ProjectId}", request.ProjectId);
                throw;
            }
        }
    }

    public class GetAvailableResourcesQueryHandler : IRequestHandler<GetAvailableResourcesQuery, List<ResourceAvailabilityDto>>
    {
        private readonly ProjectDbContext _context;
        private readonly ILogger<GetAvailableResourcesQueryHandler> _logger;

        public GetAvailableResourcesQueryHandler(
            ProjectDbContext context,
            ILogger<GetAvailableResourcesQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<ResourceAvailabilityDto>> Handle(GetAvailableResourcesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Get all resources that are not allocated during the specified period
                var allocatedResourceIds = await _context.ProjectResources
                    .Where(r => !r.IsDeleted &&
                               r.Status != ResourceAllocationStatus.Released &&
                               r.Status != ResourceAllocationStatus.Completed &&
                               r.StartDate < request.EndDate &&
                               (!r.EndDate.HasValue || r.EndDate > request.StartDate))
                    .Select(r => r.ResourceId)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                // Query for available resources
                var availableResources = new List<ResourceAvailabilityDto>();

                // In a real implementation, this would query a resource master table
                // For now, we'll return a list of available resources
                // This is a placeholder - you'd typically integrate with HRM/Inventory modules

                _logger.LogInformation("Retrieved available resources for period {StartDate} - {EndDate}",
                    request.StartDate, request.EndDate);

                return availableResources;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available resources");
                throw;
            }
        }
    }
}