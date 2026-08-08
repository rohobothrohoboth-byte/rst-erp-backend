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

public class CreateBudgetCommandHandler
    : IRequestHandler<CreateBudgetCommand, BudgetDto>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<CreateBudgetCommandHandler> _logger;
    private readonly ICacheService _cache;
    private readonly IMediator _mediator;

    public CreateBudgetCommandHandler(
        PlanDevDbContext context,
        ILogger<CreateBudgetCommandHandler> logger,
        ICacheService cache,
        IMediator mediator)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
        _mediator = mediator;
    }

    public async Task<BudgetDto> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating new budget for project: {ProjectId}", request.CreateDto.ProjectId);

            var budget = new Budget
            {
                Id = Guid.NewGuid(),
                ProjectId = request.CreateDto.ProjectId,
                Category = request.CreateDto.Category,
                Description = request.CreateDto.Description,
                PlannedAmount = request.CreateDto.PlannedAmount,
                ActualAmount = 0,
                PlannedQuantity = request.CreateDto.PlannedQuantity,
                ActualQuantity = 0,
                Unit = request.CreateDto.Unit,
                Status = "Draft",
                BudgetType = request.CreateDto.BudgetType,
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            };

            budget.UpdateRowVersion();

            await _context.Budgets.AddAsync(budget, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync(string.Format(CacheKeys.BudgetsByProject, request.CreateDto.ProjectId), cancellationToken);

            _logger.LogInformation("✅ Budget created successfully: {Category} - {Amount}",
                budget.Category, budget.PlannedAmount);

            // Use MediatR to get the created budget
            return await _mediator.Send(new GetBudgetByIdQuery { Id = budget.Id }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating budget");
            throw;
        }
    }
}

public class UpdateBudgetCommandHandler
    : IRequestHandler<UpdateBudgetCommand, BudgetDto>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<UpdateBudgetCommandHandler> _logger;
    private readonly ICacheService _cache;
    private readonly IMediator _mediator;

    public UpdateBudgetCommandHandler(
        PlanDevDbContext context,
        ILogger<UpdateBudgetCommandHandler> logger,
        ICacheService cache,
        IMediator mediator)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
        _mediator = mediator;
    }

    public async Task<BudgetDto> Handle(UpdateBudgetCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating budget: {BudgetId}", request.UpdateDto.Id);

            var budget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.Id == request.UpdateDto.Id && !b.IsDeleted, cancellationToken);

            if (budget == null)
                throw new KeyNotFoundException($"Budget with ID '{request.UpdateDto.Id}' not found");

            if (!string.IsNullOrEmpty(request.UpdateDto.Category))
                budget.Category = request.UpdateDto.Category;

            if (!string.IsNullOrEmpty(request.UpdateDto.Description))
                budget.Description = request.UpdateDto.Description;

            if (request.UpdateDto.PlannedAmount.HasValue)
                budget.PlannedAmount = request.UpdateDto.PlannedAmount.Value;

            if (request.UpdateDto.ActualAmount.HasValue)
                budget.ActualAmount = request.UpdateDto.ActualAmount.Value;

            if (request.UpdateDto.PlannedQuantity.HasValue)
                budget.PlannedQuantity = request.UpdateDto.PlannedQuantity.Value;

            if (request.UpdateDto.ActualQuantity.HasValue)
                budget.ActualQuantity = request.UpdateDto.ActualQuantity.Value;

            if (!string.IsNullOrEmpty(request.UpdateDto.Unit))
                budget.Unit = request.UpdateDto.Unit;

            if (!string.IsNullOrEmpty(request.UpdateDto.Status))
                budget.Status = request.UpdateDto.Status;

            budget.DateMod = DateTime.UtcNow;
            budget.UpdateRowVersion();

            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync(string.Format(CacheKeys.BudgetsByProject, budget.ProjectId), cancellationToken);
            await _cache.RemoveAsync(string.Format(CacheKeys.BudgetById, budget.Id), cancellationToken);

            _logger.LogInformation("✅ Budget updated successfully: {Category}", budget.Category);

            // Use MediatR to get the updated budget
            return await _mediator.Send(new GetBudgetByIdQuery { Id = budget.Id }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating budget");
            throw;
        }
    }
}

public class DeleteBudgetCommandHandler
    : IRequestHandler<DeleteBudgetCommand, bool>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<DeleteBudgetCommandHandler> _logger;
    private readonly ICacheService _cache;

    public DeleteBudgetCommandHandler(
        PlanDevDbContext context,
        ILogger<DeleteBudgetCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<bool> Handle(DeleteBudgetCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deleting budget: {BudgetId}", request.Id);

            var budget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.Id == request.Id && !b.IsDeleted, cancellationToken);

            if (budget == null)
                return false;

            var projectId = budget.ProjectId;

            budget.IsDeleted = true;
            budget.DateMod = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync(string.Format(CacheKeys.BudgetsByProject, projectId), cancellationToken);
            await _cache.RemoveAsync(string.Format(CacheKeys.BudgetById, request.Id), cancellationToken);

            _logger.LogInformation("✅ Budget deleted successfully: {Category}", budget.Category);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting budget");
            throw;
        }
    }
}