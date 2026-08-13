using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Services;

public interface IBudgetReservationService
{
    Task<BudgetAvailabilityDto> CheckAvailabilityAsync(Guid budgetId, decimal amount, CancellationToken ct = default);
    Task<BudgetAvailabilityDto> ReserveAsync(BudgetReserveRequest req, CancellationToken ct = default);
    Task ReleaseAsync(BudgetReleaseRequest req, CancellationToken ct = default);
    Task ConsumeAsync(BudgetConsumeRequest req, CancellationToken ct = default);
}

public class BudgetReservationService : IBudgetReservationService
{
    private readonly FinanceDbContext _db;
    public BudgetReservationService(FinanceDbContext db) { _db = db; }

    private async Task<BudgetAvailabilityDto> BuildAvailability(Budget budget, decimal requested, CancellationToken ct)
    {
        var committed = await _db.BudgetReservations
            .Where(r => r.BudgetId == budget.Id && r.Status == "Active" && !r.IsDeleted)
            .SumAsync(r => (decimal?)r.Amount, ct) ?? 0m;

        var available = budget.TotalAmount - budget.SpentAmount - committed;
        return new BudgetAvailabilityDto
        {
            BudgetId = budget.Id,
            BudgetName = budget.Name,
            Allocated = budget.TotalAmount,
            Spent = budget.SpentAmount,
            Committed = committed,
            Available = available,
            Requested = requested,
            Ok = available >= requested,
            Message = available >= requested
                ? "Budget available."
                : $"Insufficient budget. Available {available:N2}, requested {requested:N2}.",
        };
    }

    public async Task<BudgetAvailabilityDto> CheckAvailabilityAsync(Guid budgetId, decimal amount, CancellationToken ct = default)
    {
        var budget = await _db.Budgets.FirstOrDefaultAsync(b => b.Id == budgetId && !b.IsDeleted, ct)
            ?? throw new InvalidOperationException($"Budget {budgetId} not found.");
        return await BuildAvailability(budget, amount, ct);
    }

    public async Task<BudgetAvailabilityDto> ReserveAsync(BudgetReserveRequest req, CancellationToken ct = default)
    {
        var budget = await _db.Budgets.FirstOrDefaultAsync(b => b.Id == req.BudgetId && !b.IsDeleted, ct)
            ?? throw new InvalidOperationException($"Budget {req.BudgetId} not found.");

        // Idempotent: reuse an existing active reservation for this reference.
        var existing = await _db.BudgetReservations.FirstOrDefaultAsync(
            r => r.BudgetId == req.BudgetId && r.ReferenceId == req.ReferenceId
                 && r.ReferenceType == req.ReferenceType && r.Status == "Active" && !r.IsDeleted, ct);

        // Availability excluding this reference's existing reservation (so re-reserving is a no-op).
        var committedOthers = await _db.BudgetReservations
            .Where(r => r.BudgetId == budget.Id && r.Status == "Active" && !r.IsDeleted
                        && !(r.ReferenceId == req.ReferenceId && r.ReferenceType == req.ReferenceType))
            .SumAsync(r => (decimal?)r.Amount, ct) ?? 0m;
        var available = budget.TotalAmount - budget.SpentAmount - committedOthers;

        if (available < req.Amount)
        {
            return new BudgetAvailabilityDto
            {
                BudgetId = budget.Id,
                BudgetName = budget.Name,
                Allocated = budget.TotalAmount,
                Spent = budget.SpentAmount,
                Committed = committedOthers,
                Available = available,
                Requested = req.Amount,
                Ok = false,
                Message = $"Insufficient budget. Available {available:N2}, requested {req.Amount:N2}.",
            };
        }

        if (existing != null)
        {
            existing.Amount = req.Amount;
            existing.Note = req.Note;
            existing.DateMod = DateTime.UtcNow;
        }
        else
        {
            _db.BudgetReservations.Add(new BudgetReservation
            {
                Id = Guid.NewGuid(),
                BudgetId = req.BudgetId,
                ReferenceId = req.ReferenceId,
                ReferenceType = req.ReferenceType,
                Amount = req.Amount,
                Status = "Active",
                Note = req.Note,
                DateAdd = DateTime.UtcNow,
                IsDeleted = false,
            });
        }
        await _db.SaveChangesAsync(ct);
        return await BuildAvailability(budget, req.Amount, ct);
    }

    public async Task ReleaseAsync(BudgetReleaseRequest req, CancellationToken ct = default)
    {
        var reservations = await _db.BudgetReservations
            .Where(r => r.ReferenceId == req.ReferenceId && r.ReferenceType == req.ReferenceType
                        && r.Status == "Active" && !r.IsDeleted)
            .ToListAsync(ct);
        foreach (var r in reservations)
        {
            r.Status = "Released";
            r.DateMod = DateTime.UtcNow;
        }
        if (reservations.Count > 0) { await _db.SaveChangesAsync(ct); }
    }

    public async Task ConsumeAsync(BudgetConsumeRequest req, CancellationToken ct = default)
    {
        var reservations = await _db.BudgetReservations
            .Where(r => r.ReferenceId == req.ReferenceId && r.ReferenceType == req.ReferenceType
                        && r.Status == "Active" && !r.IsDeleted)
            .ToListAsync(ct);
        if (reservations.Count == 0) { return; }

        foreach (var r in reservations)
        {
            var consume = req.Amount ?? r.Amount;
            var budget = await _db.Budgets.FirstOrDefaultAsync(b => b.Id == r.BudgetId && !b.IsDeleted, ct);
            if (budget != null)
            {
                budget.SpentAmount += consume;      // move committed -> spent
                budget.DateMod = DateTime.UtcNow;
            }
            r.Status = "Consumed";
            r.Amount = consume;
            r.DateMod = DateTime.UtcNow;
        }
        await _db.SaveChangesAsync(ct);
    }
}
