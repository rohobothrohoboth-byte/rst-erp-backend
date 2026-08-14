// Handlers/BudgetHandlers.cs - COMPLETE FIXED
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Cor.ProjectManagement.Models.Entities;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Persistence;
using Cor.ProjectManagement.Commands.BudgetCommands;
using Cor.ProjectManagement.Queries.BudgetQueries;

namespace Cor.ProjectManagement.Handlers
{
    // ============ COMMAND HANDLERS ============

    public class CreateBudgetCommandHandler : IRequestHandler<CreateBudgetCommand, ProjectBudgetDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateBudgetCommandHandler> _logger;

        public CreateBudgetCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<CreateBudgetCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectBudgetDto> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == request.ProjectId && !p.IsDeleted, cancellationToken);

                if (project == null)
                    throw new Exception($"Project with ID {request.ProjectId} not found");

                var budget = new ProjectBudget
                {
                    Id = Guid.NewGuid(),
                    ProjectId = request.ProjectId,
                    Category = request.Category,
                    CategoryName = request.CategoryName ?? request.Category.ToString(),
                    PlannedAmount = request.PlannedAmount,
                    ActualAmount = 0,
                    CommittedAmount = 0,
                    RemainingAmount = request.PlannedAmount,
                    PlannedDate = request.PlannedDate,
                    Description = request.Description,
                    VendorId = request.VendorId,
                    VendorName = request.VendorName ?? string.Empty,
                    IsApproved = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy ?? "System"
                };

                await _context.ProjectBudgets.AddAsync(budget, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                await UpdateProjectBudgetTotal(request.ProjectId, cancellationToken);

                _logger.LogInformation("Budget created successfully with ID: {BudgetId}", budget.Id);
                return _mapper.Map<ProjectBudgetDto>(budget);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating budget");
                throw;
            }
        }

        private async Task UpdateProjectBudgetTotal(Guid projectId, CancellationToken cancellationToken)
        {
            var totalBudget = await _context.ProjectBudgets
                .Where(b => b.ProjectId == projectId && !b.IsDeleted)
                .SumAsync(b => b.PlannedAmount, cancellationToken);

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted, cancellationToken);

            if (project != null)
            {
                project.Budget = totalBudget;
                project.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }

    public class UpdateBudgetCommandHandler : IRequestHandler<UpdateBudgetCommand, ProjectBudgetDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateBudgetCommandHandler> _logger;

        public UpdateBudgetCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<UpdateBudgetCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectBudgetDto> Handle(UpdateBudgetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var budget = await _context.ProjectBudgets
                    .FirstOrDefaultAsync(b => b.Id == request.Id && !b.IsDeleted, cancellationToken);

                if (budget == null)
                    throw new Exception($"Budget with ID {request.Id} not found");

                if (budget.IsApproved && (request.PlannedAmount.HasValue || request.CommittedAmount.HasValue))
                    throw new Exception("Cannot modify approved budget amounts");

                if (request.PlannedAmount.HasValue)
                {
                    budget.PlannedAmount = request.PlannedAmount.Value;
                    budget.RemainingAmount = budget.PlannedAmount - budget.ActualAmount - budget.CommittedAmount;
                }

                if (request.ActualAmount.HasValue)
                {
                    budget.ActualAmount = request.ActualAmount.Value;
                    budget.RemainingAmount = budget.PlannedAmount - budget.ActualAmount - budget.CommittedAmount;
                }

                if (request.CommittedAmount.HasValue)
                {
                    budget.CommittedAmount = request.CommittedAmount.Value;
                    budget.RemainingAmount = budget.PlannedAmount - budget.ActualAmount - budget.CommittedAmount;
                }

                if (!string.IsNullOrEmpty(request.Description))
                    budget.Description = request.Description;

                budget.UpdatedAt = DateTime.UtcNow;
                budget.UpdatedBy = request.UpdatedBy ?? "System";
                budget.Version += 1;

                await _context.SaveChangesAsync(cancellationToken);

                await UpdateProjectBudgetTotal(budget.ProjectId, cancellationToken);

                _logger.LogInformation("Budget updated successfully with ID: {BudgetId}", budget.Id);
                return _mapper.Map<ProjectBudgetDto>(budget);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating budget with ID: {BudgetId}", request.Id);
                throw;
            }
        }

        private async Task UpdateProjectBudgetTotal(Guid projectId, CancellationToken cancellationToken)
        {
            var totalBudget = await _context.ProjectBudgets
                .Where(b => b.ProjectId == projectId && !b.IsDeleted)
                .SumAsync(b => b.PlannedAmount, cancellationToken);

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted, cancellationToken);

            if (project != null)
            {
                project.Budget = totalBudget;
                project.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }

    public class DeleteBudgetCommandHandler : IRequestHandler<DeleteBudgetCommand, bool>
    {
        private readonly ProjectDbContext _context;
        private readonly ILogger<DeleteBudgetCommandHandler> _logger;

        public DeleteBudgetCommandHandler(
            ProjectDbContext context,
            ILogger<DeleteBudgetCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteBudgetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var budget = await _context.ProjectBudgets
                    .FirstOrDefaultAsync(b => b.Id == request.Id && !b.IsDeleted, cancellationToken);

                if (budget == null)
                    throw new Exception($"Budget with ID {request.Id} not found");

                if (budget.IsApproved)
                    throw new Exception("Cannot delete approved budget. Please reject first.");

                var projectId = budget.ProjectId;

                budget.IsDeleted = true;
                budget.DeletedAt = DateTime.UtcNow;
                budget.DeletedBy = request.DeletedBy ?? "System";

                await _context.SaveChangesAsync(cancellationToken);

                await UpdateProjectBudgetTotal(projectId, cancellationToken);

                _logger.LogInformation("Budget deleted successfully with ID: {BudgetId}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting budget with ID: {BudgetId}", request.Id);
                throw;
            }
        }

        private async Task UpdateProjectBudgetTotal(Guid projectId, CancellationToken cancellationToken)
        {
            var totalBudget = await _context.ProjectBudgets
                .Where(b => b.ProjectId == projectId && !b.IsDeleted)
                .SumAsync(b => b.PlannedAmount, cancellationToken);

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted, cancellationToken);

            if (project != null)
            {
                project.Budget = totalBudget;
                project.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }

    public class ApproveBudgetCommandHandler : IRequestHandler<ApproveBudgetCommand, ProjectBudgetDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ApproveBudgetCommandHandler> _logger;

        public ApproveBudgetCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<ApproveBudgetCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectBudgetDto> Handle(ApproveBudgetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var budget = await _context.ProjectBudgets
                    .FirstOrDefaultAsync(b => b.Id == request.Id && !b.IsDeleted, cancellationToken);

                if (budget == null)
                    throw new Exception($"Budget with ID {request.Id} not found");

                if (budget.IsApproved)
                    throw new Exception("Budget is already approved");

                budget.IsApproved = true;
                budget.ApprovedAt = DateTime.UtcNow;
                budget.ApprovedById = Guid.TryParse(request.ApprovedBy, out var id) ? id : null;
                budget.ApprovedByName = request.ApprovedBy ?? "System";
                budget.UpdatedAt = DateTime.UtcNow;
                budget.UpdatedBy = request.ApprovedBy ?? "System";
                budget.Version += 1;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Budget approved successfully with ID: {BudgetId}", budget.Id);
                return _mapper.Map<ProjectBudgetDto>(budget);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving budget with ID: {BudgetId}", request.Id);
                throw;
            }
        }
    }

    // ============ QUERY HANDLERS ============

    public class GetBudgetByIdQueryHandler : IRequestHandler<GetBudgetByIdQuery, ProjectBudgetDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetBudgetByIdQueryHandler> _logger;

        public GetBudgetByIdQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetBudgetByIdQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectBudgetDto> Handle(GetBudgetByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var budget = await _context.ProjectBudgets
                    .AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Id == request.Id && !b.IsDeleted, cancellationToken);

                if (budget == null)
                    throw new Exception($"Budget with ID {request.Id} not found");

                return _mapper.Map<ProjectBudgetDto>(budget);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting budget with ID: {BudgetId}", request.Id);
                throw;
            }
        }
    }

    public class GetBudgetsByProjectQueryHandler : IRequestHandler<GetBudgetsByProjectQuery, List<ProjectBudgetDto>>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetBudgetsByProjectQueryHandler> _logger;

        public GetBudgetsByProjectQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetBudgetsByProjectQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ProjectBudgetDto>> Handle(GetBudgetsByProjectQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.ProjectBudgets
                    .AsNoTracking()
                    .Where(b => b.ProjectId == request.ProjectId && !b.IsDeleted);

                if (request.Category.HasValue)
                    query = query.Where(b => b.Category == request.Category.Value);

                if (request.IsApproved.HasValue)
                    query = query.Where(b => b.IsApproved == request.IsApproved.Value);

                var items = await query
                    .OrderBy(b => b.Category)
                    .Select(b => _mapper.Map<ProjectBudgetDto>(b))
                    .ToListAsync(cancellationToken);

                return items;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting budgets for project ID: {ProjectId}", request.ProjectId);
                throw;
            }
        }
    }

    public class GetBudgetSummaryQueryHandler : IRequestHandler<GetBudgetSummaryQuery, BudgetSummaryDto>
    {
        private readonly ProjectDbContext _context;
        private readonly ILogger<GetBudgetSummaryQueryHandler> _logger;

        public GetBudgetSummaryQueryHandler(
            ProjectDbContext context,
            ILogger<GetBudgetSummaryQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BudgetSummaryDto> Handle(GetBudgetSummaryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == request.ProjectId && !p.IsDeleted, cancellationToken);

                if (project == null)
                    throw new Exception($"Project with ID {request.ProjectId} not found");

                var budgets = await _context.ProjectBudgets
                    .AsNoTracking()
                    .Where(b => b.ProjectId == request.ProjectId && !b.IsDeleted)
                    .ToListAsync(cancellationToken);

                var summary = new BudgetSummaryDto
                {
                    ProjectId = project.Id,
                    ProjectName = project.Name,
                    TotalBudget = budgets.Sum(b => b.PlannedAmount),
                    TotalActual = budgets.Sum(b => b.ActualAmount),
                    TotalCommitted = budgets.Sum(b => b.CommittedAmount),
                    TotalRemaining = budgets.Sum(b => b.RemainingAmount),
                    ApprovedBudget = budgets.Where(b => b.IsApproved).Sum(b => b.PlannedAmount),
                    PendingApproval = budgets.Where(b => !b.IsApproved).Sum(b => b.PlannedAmount)
                };

                // ✅ FIX: Convert to double properly
                summary.UtilizationPercentage = summary.TotalBudget > 0
                    ? (double)((summary.TotalActual / summary.TotalBudget) * 100)
                    : 0;

                // ✅ Use BudgetCategory enum as key
                foreach (var budget in budgets)
                {
                    summary.BudgetByCategory[budget.Category] = budget.PlannedAmount;
                    summary.ActualByCategory[budget.Category] = budget.ActualAmount;
                }

                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting budget summary for project ID: {ProjectId}", request.ProjectId);
                throw;
            }
        }
    }

    public class GetBudgetUtilizationQueryHandler : IRequestHandler<GetBudgetUtilizationQuery, BudgetUtilizationDto>
    {
        private readonly ProjectDbContext _context;
        private readonly ILogger<GetBudgetUtilizationQueryHandler> _logger;

        public GetBudgetUtilizationQueryHandler(
            ProjectDbContext context,
            ILogger<GetBudgetUtilizationQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BudgetUtilizationDto> Handle(GetBudgetUtilizationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == request.ProjectId && !p.IsDeleted, cancellationToken);

                if (project == null)
                    throw new Exception($"Project with ID {request.ProjectId} not found");

                var budgets = await _context.ProjectBudgets
                    .AsNoTracking()
                    .Where(b => b.ProjectId == request.ProjectId && !b.IsDeleted)
                    .ToListAsync(cancellationToken);

                // Calculate monthly utilization
                var monthlyData = new List<MonthlyBudgetData>();
                var currentDate = request.FromDate;

                while (currentDate <= request.ToDate)
                {
                    var monthEnd = currentDate.AddMonths(1);
                    var monthBudgets = budgets.Where(b =>
                        b.PlannedDate.HasValue &&
                        b.PlannedDate.Value >= currentDate &&
                        b.PlannedDate.Value < monthEnd);

                    var plannedSum = monthBudgets.Sum(b => b.PlannedAmount);
                    var actualSum = monthBudgets.Sum(b => b.ActualAmount);

                    monthlyData.Add(new MonthlyBudgetData
                    {
                        Month = currentDate.ToString("yyyy-MM"),
                        Planned = plannedSum,
                        Actual = actualSum,
                        Utilization = plannedSum > 0
                            ? (double)((actualSum / plannedSum) * 100)
                            : 0
                    });

                    currentDate = monthEnd;
                }

                var totalPlanned = budgets.Sum(b => b.PlannedAmount);
                var totalActual = budgets.Sum(b => b.ActualAmount);

                return new BudgetUtilizationDto
                {
                    ProjectId = project.Id,
                    ProjectName = project.Name,
                    TotalBudget = totalPlanned,
                    TotalUtilized = totalActual,
                    TotalCommitted = budgets.Sum(b => b.CommittedAmount),
                    OverallUtilization = totalPlanned > 0
                        ? (double)((totalActual / totalPlanned) * 100)
                        : 0,
                    MonthlyData = monthlyData,
                    CategoryUtilization = budgets
                        .GroupBy(b => b.Category)
                        .Select(g => {
                            var planned = g.Sum(b => b.PlannedAmount);
                            var actual = g.Sum(b => b.ActualAmount);
                            return new CategoryUtilizationData
                            {
                                Category = g.Key.ToString(),
                                Planned = planned,
                                Actual = actual,
                                Utilization = planned > 0
                                    ? (double)((actual / planned) * 100)
                                    : 0
                            };
                        })
                        .ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting budget utilization for project ID: {ProjectId}", request.ProjectId);
                throw;
            }
        }
    }
}