using Cor.Inventory.Models.DTOs;
using Cor.Inventory.Models.Entities;
using Cor.Inventory.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cor.Inventory.Services;

public class ReorderService : IReorderService
{
    private readonly InventoryDbContext _context;
    private readonly ILogger<ReorderService> _logger;

    public ReorderService(InventoryDbContext context, ILogger<ReorderService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // ============= Rules =============

    public async Task<List<ReorderRuleDto>> GetRulesAsync(CancellationToken ct = default)
    {
        var rules = await _context.ReorderRules.AsNoTracking()
            .OrderBy(r => r.ProductName)
            .ToListAsync(ct);

        return rules.Select(MapRule).ToList();
    }

    public async Task<ReorderRuleDto> CreateRuleAsync(CreateReorderRuleDto dto, CancellationToken ct = default)
    {
        var rule = new ReorderRule
        {
            Id = Guid.NewGuid(),
            ProductId = dto.ProductId,
            ProductName = dto.ProductName,
            WarehouseId = dto.WarehouseId,
            MinLevel = dto.MinLevel,
            MaxLevel = dto.MaxLevel,
            ReorderQuantity = dto.ReorderQuantity,
            IsActive = dto.IsActive,
            DateAdd = DateTime.UtcNow
        };
        rule.UpdateRowVersion();

        await _context.ReorderRules.AddAsync(rule, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Created reorder rule for product {ProductId}", rule.ProductId);
        return MapRule(rule);
    }

    public async Task<ReorderRuleDto> UpdateRuleAsync(UpdateReorderRuleDto dto, CancellationToken ct = default)
    {
        var rule = await _context.ReorderRules.FirstOrDefaultAsync(r => r.Id == dto.Id, ct);
        if (rule == null)
            throw new KeyNotFoundException($"Reorder rule with ID '{dto.Id}' not found");

        if (!string.IsNullOrEmpty(dto.RowVersion) && dto.RowVersion != rule.RowVersion)
            throw new DbUpdateConcurrencyException("The reorder rule was modified by another user");

        if (dto.ProductName != null) rule.ProductName = dto.ProductName;
        if (dto.WarehouseId.HasValue) rule.WarehouseId = dto.WarehouseId;
        if (dto.MinLevel.HasValue) rule.MinLevel = dto.MinLevel.Value;
        if (dto.MaxLevel.HasValue) rule.MaxLevel = dto.MaxLevel.Value;
        if (dto.ReorderQuantity.HasValue) rule.ReorderQuantity = dto.ReorderQuantity.Value;
        if (dto.IsActive.HasValue) rule.IsActive = dto.IsActive.Value;

        rule.DateMod = DateTime.UtcNow;
        rule.UpdateRowVersion();

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Updated reorder rule {RuleId}", rule.Id);
        return MapRule(rule);
    }

    // ============= Alerts =============

    public async Task<List<ReorderAlertDto>> GetAlertsAsync(CancellationToken ct = default)
    {
        var alerts = await _context.StockLevels.AsNoTracking()
            .Where(s => s.QuantityOnHand <= s.ReorderLevel)
            .OrderBy(s => s.ProductName)
            .Select(s => new ReorderAlertDto
            {
                ProductId = s.ProductId,
                ProductName = s.ProductName,
                WarehouseId = s.WarehouseId,
                QuantityOnHand = s.QuantityOnHand,
                ReorderLevel = s.ReorderLevel,
                ReorderQuantity = s.ReorderQuantity
            })
            .ToListAsync(ct);

        return alerts;
    }

    // ============= Requests =============

    public async Task<List<ReorderRequestDto>> GetRequestsAsync(string? status, CancellationToken ct = default)
    {
        var query = _context.ReorderRequests.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(r => r.Status == status);

        var requests = await query
            .OrderByDescending(r => r.DateAdd)
            .ToListAsync(ct);

        return requests.Select(MapRequest).ToList();
    }

    public async Task<ReorderRequestDto> CreateRequestAsync(CreateReorderRequestDto dto, CancellationToken ct = default)
    {
        if (dto.Quantity <= 0)
            throw new InvalidOperationException("Quantity must be greater than zero.");

        var request = new ReorderRequest
        {
            Id = Guid.NewGuid(),
            ProductId = dto.ProductId,
            ProductName = dto.ProductName,
            WarehouseId = dto.WarehouseId,
            Quantity = dto.Quantity,
            Status = "Pending",
            Reason = dto.Reason,
            DateAdd = DateTime.UtcNow
        };
        request.UpdateRowVersion();

        await _context.ReorderRequests.AddAsync(request, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Created reorder request for product {ProductId}", request.ProductId);
        return MapRequest(request);
    }

    public async Task<ReorderRequestDto> ApproveRequestAsync(Guid id, ReorderDecisionDto dto, CancellationToken ct = default)
        => await DecideAsync(id, "Approved", dto, ct);

    public async Task<ReorderRequestDto> RejectRequestAsync(Guid id, ReorderDecisionDto dto, CancellationToken ct = default)
        => await DecideAsync(id, "Rejected", dto, ct);

    public async Task<ReorderRequestDto> ConvertRequestAsync(Guid id, ReorderDecisionDto dto, CancellationToken ct = default)
    {
        var request = await _context.ReorderRequests.FirstOrDefaultAsync(r => r.Id == id, ct);
        if (request == null)
            throw new KeyNotFoundException($"Reorder request with ID '{id}' not found");

        if (request.Status != "Approved")
            throw new InvalidOperationException("Only approved reorder requests can be converted.");

        return await DecideAsync(id, "Converted", dto, ct, request);
    }

    private async Task<ReorderRequestDto> DecideAsync(Guid id, string status, ReorderDecisionDto dto, CancellationToken ct, ReorderRequest? loaded = null)
    {
        var request = loaded ?? await _context.ReorderRequests.FirstOrDefaultAsync(r => r.Id == id, ct);
        if (request == null)
            throw new KeyNotFoundException($"Reorder request with ID '{id}' not found");

        request.Status = status;
        request.DecidedByUserId = dto.DecidedByUserId;
        request.DecidedByName = dto.DecidedByName;
        request.DecisionNote = dto.DecisionNote;
        request.DecisionDate = DateTime.UtcNow;
        request.DateMod = DateTime.UtcNow;
        request.UpdateRowVersion();

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Reorder request {RequestId} set to {Status}", request.Id, status);
        return MapRequest(request);
    }

    // ============= Helpers =============

    private static ReorderRuleDto MapRule(ReorderRule r) => new()
    {
        Id = r.Id,
        ProductId = r.ProductId,
        ProductName = r.ProductName,
        WarehouseId = r.WarehouseId,
        MinLevel = r.MinLevel,
        MaxLevel = r.MaxLevel,
        ReorderQuantity = r.ReorderQuantity,
        IsActive = r.IsActive,
        DateAdd = r.DateAdd,
        DateMod = r.DateMod
    };

    private static ReorderRequestDto MapRequest(ReorderRequest r) => new()
    {
        Id = r.Id,
        ProductId = r.ProductId,
        ProductName = r.ProductName,
        WarehouseId = r.WarehouseId,
        Quantity = r.Quantity,
        Status = r.Status,
        Reason = r.Reason,
        DecidedByUserId = r.DecidedByUserId,
        DecidedByName = r.DecidedByName,
        DecisionNote = r.DecisionNote,
        DecisionDate = r.DecisionDate,
        DateAdd = r.DateAdd,
        DateMod = r.DateMod
    };
}
