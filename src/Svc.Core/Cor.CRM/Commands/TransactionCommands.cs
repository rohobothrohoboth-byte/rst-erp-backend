// Cor.CRM/Commands/TransactionCommands.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Helpers;
using MediatR;
using Task = System.Threading.Tasks.Task;

namespace Cor.CRM.Commands;

// ============================================================
// COMMANDS
// ============================================================

public class TransactionAddCmd : IRequest<RealEstateTransactionDto>
{
    public CreateTransactionDto Dto { get; set; } = default!;
}

public class TransactionUpdateCmd : IRequest<RealEstateTransactionDto>
{
    public Guid Id { get; set; }
    public UpdateTransactionDto Dto { get; set; } = default!;
}

public class TransactionDelCmd : IRequest
{
    public Guid Id { get; set; }
}

public class TransactionAcceptCmd : IRequest<RealEstateTransactionDto>
{
    public Guid Id { get; set; }
}

public class TransactionCloseCmd : IRequest<RealEstateTransactionDto>
{
    public Guid Id { get; set; }
}

public class TransactionToClosingCmd : IRequest<RealEstateTransactionDto>
{
    public Guid Id { get; set; }
}

// ============================================================
// HELPERS
// ============================================================

public static class TransactionStatusHelper
{
    /// <summary>
    /// Validates if a transaction can transition from one status to another
    /// </summary>
    public static bool CanTransitionTo(TransactionStatus current, TransactionStatus newStatus)
    {
        // Allow same status (no change)
        if (current == newStatus)
            return true;

        // Define allowed transitions
        var allowedTransitions = new Dictionary<TransactionStatus, List<TransactionStatus>>
        {
            // From Negotiation (1)
            { TransactionStatus.Negotiation, new List<TransactionStatus>
                { TransactionStatus.Accepted, TransactionStatus.Cancelled } },

            // From Accepted (2)
            { TransactionStatus.Accepted, new List<TransactionStatus>
                { TransactionStatus.PendingInspection, TransactionStatus.PendingFinancing,
                  TransactionStatus.PendingAppraisal, TransactionStatus.Closing,
                  TransactionStatus.Cancelled } },

            // From PendingInspection (3)
            { TransactionStatus.PendingInspection, new List<TransactionStatus>
                { TransactionStatus.Accepted, TransactionStatus.PendingFinancing,
                  TransactionStatus.Closing, TransactionStatus.Cancelled } },

            // From PendingFinancing (4)
            { TransactionStatus.PendingFinancing, new List<TransactionStatus>
                { TransactionStatus.Accepted, TransactionStatus.PendingAppraisal,
                  TransactionStatus.Closing, TransactionStatus.Cancelled } },

            // From PendingAppraisal (5)
            { TransactionStatus.PendingAppraisal, new List<TransactionStatus>
                { TransactionStatus.Accepted, TransactionStatus.Closing,
                  TransactionStatus.Cancelled } },

            // From Closing (6)
            { TransactionStatus.Closing, new List<TransactionStatus>
                { TransactionStatus.Completed, TransactionStatus.Cancelled } },

            // From Completed (7) - no further transitions allowed
            { TransactionStatus.Completed, new List<TransactionStatus>() },

            // From Cancelled (8) - no further transitions allowed
            { TransactionStatus.Cancelled, new List<TransactionStatus>() }
        };

        return allowedTransitions.TryGetValue(current, out var allowed) &&
               allowed.Contains(newStatus);
    }

    /// <summary>
    /// Gets the next allowed statuses for a given status
    /// </summary>
    public static List<TransactionStatus> GetNextStatuses(TransactionStatus current)
    {
        var allowedTransitions = new Dictionary<TransactionStatus, List<TransactionStatus>>
        {
            { TransactionStatus.Negotiation, new List<TransactionStatus>
                { TransactionStatus.Accepted, TransactionStatus.Cancelled } },
            { TransactionStatus.Accepted, new List<TransactionStatus>
                { TransactionStatus.PendingInspection, TransactionStatus.PendingFinancing,
                  TransactionStatus.PendingAppraisal, TransactionStatus.Closing,
                  TransactionStatus.Cancelled } },
            { TransactionStatus.PendingInspection, new List<TransactionStatus>
                { TransactionStatus.Accepted, TransactionStatus.PendingFinancing,
                  TransactionStatus.Closing, TransactionStatus.Cancelled } },
            { TransactionStatus.PendingFinancing, new List<TransactionStatus>
                { TransactionStatus.Accepted, TransactionStatus.PendingAppraisal,
                  TransactionStatus.Closing, TransactionStatus.Cancelled } },
            { TransactionStatus.PendingAppraisal, new List<TransactionStatus>
                { TransactionStatus.Accepted, TransactionStatus.Closing,
                  TransactionStatus.Cancelled } },
            { TransactionStatus.Closing, new List<TransactionStatus>
                { TransactionStatus.Completed, TransactionStatus.Cancelled } },
            { TransactionStatus.Completed, new List<TransactionStatus>() },
            { TransactionStatus.Cancelled, new List<TransactionStatus>() }
        };

        return allowedTransitions.TryGetValue(current, out var allowed) ? allowed : new List<TransactionStatus>();
    }

    /// <summary>
    /// Gets the display name for a status
    /// </summary>
    public static string GetStatusDisplayName(TransactionStatus status)
    {
        return status switch
        {
            TransactionStatus.Negotiation => "Negotiation",
            TransactionStatus.Accepted => "Accepted",
            TransactionStatus.PendingInspection => "Pending Inspection",
            TransactionStatus.PendingFinancing => "Pending Financing",
            TransactionStatus.PendingAppraisal => "Pending Appraisal",
            TransactionStatus.Closing => "Closing",
            TransactionStatus.Completed => "Completed",
            TransactionStatus.Cancelled => "Cancelled",
            _ => status.ToString()
        };
    }
}

// ============================================================
// HANDLERS
// ============================================================

public class TransactionAddHandler : IRequestHandler<TransactionAddCmd, RealEstateTransactionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public TransactionAddHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<RealEstateTransactionDto> Handle(TransactionAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            // Generate transaction number
            var transactionNumber = await GenerateTransactionNumber(ct);

            var transaction = new RealEstateTransaction
            {
                Id = Guid.CreateVersion7(),
                TransactionNumber = transactionNumber,
                PropertyId = request.Dto.PropertyId,
                BuyerId = request.Dto.BuyerId,
                SellerId = request.Dto.SellerId,
                BuyerAgentId = request.Dto.BuyerAgentId,
                SellerAgentId = request.Dto.SellerAgentId,
                SalePrice = request.Dto.SalePrice,
                DepositAmount = request.Dto.DepositAmount,
                CommissionAmount = request.Dto.CommissionAmount,
                Status = request.Dto.Status.HasValue
                    ? (TransactionStatus)request.Dto.Status.Value
                    : TransactionStatus.Negotiation,
                OfferDate = request.Dto.OfferDate.HasValue
                    ? DateTimeHelper.EnsureUtc(request.Dto.OfferDate.Value)
                    : null,
                AcceptanceDate = request.Dto.AcceptanceDate.HasValue
                    ? DateTimeHelper.EnsureUtc(request.Dto.AcceptanceDate.Value)
                    : null,
                ClosingDate = request.Dto.ClosingDate.HasValue
                    ? DateTimeHelper.EnsureUtc(request.Dto.ClosingDate.Value)
                    : null,
                PossessionDate = request.Dto.PossessionDate.HasValue
                    ? DateTimeHelper.EnsureUtc(request.Dto.PossessionDate.Value)
                    : null,
                Notes = request.Dto.Notes,
                CreatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                IsDeleted = false
            };

            await _uow.Add(transaction, ct);
            await _uow.Commit(ct);

            _logger.LogInformation("Transaction created: {TransactionNumber}", transaction.TransactionNumber);

            return MapToDto(transaction);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private async Task<string> GenerateTransactionNumber(CancellationToken ct)
    {
        var count = await _uow.Set<RealEstateTransaction>().CountAsync(ct) + 1;
        return $"TR-{DateTime.UtcNow:yyyyMMdd}-{count:D4}";
    }

    private RealEstateTransactionDto MapToDto(RealEstateTransaction transaction)
    {
        return new RealEstateTransactionDto
        {
            Id = transaction.Id,
            TransactionNumber = transaction.TransactionNumber,
            PropertyId = transaction.PropertyId,
            BuyerId = transaction.BuyerId,
            SellerId = transaction.SellerId,
            BuyerAgentId = transaction.BuyerAgentId,
            SellerAgentId = transaction.SellerAgentId,
            SalePrice = transaction.SalePrice,
            DepositAmount = transaction.DepositAmount,
            CommissionAmount = transaction.CommissionAmount,
            Status = transaction.Status.ToString(),
            OfferDate = transaction.OfferDate,
            AcceptanceDate = transaction.AcceptanceDate,
            ClosingDate = transaction.ClosingDate,
            PossessionDate = transaction.PossessionDate,
            Notes = transaction.Notes,
            CreatedAt = transaction.CreatedAt,
            UpdatedAt = transaction.UpdatedAt
        };
    }
}

public class TransactionUpdateHandler : IRequestHandler<TransactionUpdateCmd, RealEstateTransactionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public TransactionUpdateHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<RealEstateTransactionDto> Handle(TransactionUpdateCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var transaction = await _uow.Set<RealEstateTransaction>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (transaction == null)
                throw new DomainException($"Transaction with id [{request.Id}] NOT FOUND.");

            // ✅ Validate status transition if status is being updated
            if (request.Dto.Status.HasValue)
            {
                var newStatus = (TransactionStatus)request.Dto.Status.Value;
                var currentStatus = transaction.Status;

                if (!TransactionStatusHelper.CanTransitionTo(currentStatus, newStatus))
                {
                    throw new DomainException($"Cannot transition transaction from [{currentStatus}] to [{newStatus}]");
                }

                transaction.Status = newStatus;

                // Update dates based on status
                if (newStatus == TransactionStatus.Accepted && !transaction.AcceptanceDate.HasValue)
                {
                    transaction.AcceptanceDate = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
                }

                if (newStatus == TransactionStatus.Completed && !transaction.ClosingDate.HasValue)
                {
                    transaction.ClosingDate = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
                }
            }

            // Update other fields
            if (request.Dto.SalePrice.HasValue)
                transaction.SalePrice = request.Dto.SalePrice.Value;

            if (request.Dto.DepositAmount.HasValue)
                transaction.DepositAmount = request.Dto.DepositAmount;

            if (request.Dto.CommissionAmount.HasValue)
                transaction.CommissionAmount = request.Dto.CommissionAmount;

            if (request.Dto.OfferDate.HasValue)
                transaction.OfferDate = DateTimeHelper.EnsureUtc(request.Dto.OfferDate.Value);

            if (request.Dto.AcceptanceDate.HasValue)
                transaction.AcceptanceDate = DateTimeHelper.EnsureUtc(request.Dto.AcceptanceDate.Value);

            if (request.Dto.ClosingDate.HasValue)
                transaction.ClosingDate = DateTimeHelper.EnsureUtc(request.Dto.ClosingDate.Value);

            if (request.Dto.PossessionDate.HasValue)
                transaction.PossessionDate = DateTimeHelper.EnsureUtc(request.Dto.PossessionDate.Value);

            if (request.Dto.Notes != null)
                transaction.Notes = request.Dto.Notes;

            transaction.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(transaction);
            await _uow.Commit(ct);

            _logger.LogInformation("Transaction updated: {TransactionNumber}", transaction.TransactionNumber);

            return MapToDto(transaction);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private RealEstateTransactionDto MapToDto(RealEstateTransaction transaction)
    {
        return new RealEstateTransactionDto
        {
            Id = transaction.Id,
            TransactionNumber = transaction.TransactionNumber,
            PropertyId = transaction.PropertyId,
            BuyerId = transaction.BuyerId,
            SellerId = transaction.SellerId,
            BuyerAgentId = transaction.BuyerAgentId,
            SellerAgentId = transaction.SellerAgentId,
            SalePrice = transaction.SalePrice,
            DepositAmount = transaction.DepositAmount,
            CommissionAmount = transaction.CommissionAmount,
            Status = transaction.Status.ToString(),
            OfferDate = transaction.OfferDate,
            AcceptanceDate = transaction.AcceptanceDate,
            ClosingDate = transaction.ClosingDate,
            PossessionDate = transaction.PossessionDate,
            Notes = transaction.Notes,
            CreatedAt = transaction.CreatedAt,
            UpdatedAt = transaction.UpdatedAt
        };
    }
}

public class TransactionDelHandler : IRequestHandler<TransactionDelCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public TransactionDelHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task Handle(TransactionDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var transaction = await _uow.Set<RealEstateTransaction>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (transaction == null)
                throw new DomainException($"Transaction with id [{request.Id}] NOT FOUND.");

            // Only allow deletion of Negotiation or Cancelled transactions
            if (transaction.Status != TransactionStatus.Negotiation &&
                transaction.Status != TransactionStatus.Cancelled)
                throw new DomainException($"Cannot delete transaction with status [{transaction.Status}]");

            transaction.IsDeleted = true;
            transaction.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(transaction);
            await _uow.Commit(ct);

            _logger.LogInformation("Transaction deleted: {TransactionNumber}", transaction.TransactionNumber);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class TransactionAcceptHandler : IRequestHandler<TransactionAcceptCmd, RealEstateTransactionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public TransactionAcceptHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<RealEstateTransactionDto> Handle(TransactionAcceptCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var transaction = await _uow.Set<RealEstateTransaction>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (transaction == null)
                throw new DomainException($"Transaction with id [{request.Id}] NOT FOUND.");

            // ✅ Use the helper for validation
            if (!TransactionStatusHelper.CanTransitionTo(transaction.Status, TransactionStatus.Accepted))
            {
                throw new DomainException($"Cannot accept transaction with status [{transaction.Status}]");
            }

            transaction.Status = TransactionStatus.Accepted;
            transaction.AcceptanceDate = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
            transaction.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            // Update property status to Pending
            var property = await _uow.Set<Property>()
                .FirstOrDefaultAsync(x => x.Id == transaction.PropertyId && !x.IsDeleted, ct);

            if (property != null)
            {
                property.Status = PropertyStatus.Pending;
                property.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
                await _uow.Update(property);
            }

            await _uow.Commit(ct);

            _logger.LogInformation("Transaction accepted: {TransactionNumber}", transaction.TransactionNumber);

            return MapToDto(transaction);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private RealEstateTransactionDto MapToDto(RealEstateTransaction transaction)
    {
        return new RealEstateTransactionDto
        {
            Id = transaction.Id,
            TransactionNumber = transaction.TransactionNumber,
            PropertyId = transaction.PropertyId,
            BuyerId = transaction.BuyerId,
            SellerId = transaction.SellerId,
            BuyerAgentId = transaction.BuyerAgentId,
            SellerAgentId = transaction.SellerAgentId,
            SalePrice = transaction.SalePrice,
            DepositAmount = transaction.DepositAmount,
            CommissionAmount = transaction.CommissionAmount,
            Status = transaction.Status.ToString(),
            OfferDate = transaction.OfferDate,
            AcceptanceDate = transaction.AcceptanceDate,
            ClosingDate = transaction.ClosingDate,
            PossessionDate = transaction.PossessionDate,
            Notes = transaction.Notes,
            CreatedAt = transaction.CreatedAt,
            UpdatedAt = transaction.UpdatedAt
        };
    }
}

public class TransactionToClosingHandler : IRequestHandler<TransactionToClosingCmd, RealEstateTransactionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public TransactionToClosingHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<RealEstateTransactionDto> Handle(TransactionToClosingCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var transaction = await _uow.Set<RealEstateTransaction>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (transaction == null)
                throw new DomainException($"Transaction with id [{request.Id}] NOT FOUND.");

            // ✅ Use the helper for validation
            if (!TransactionStatusHelper.CanTransitionTo(transaction.Status, TransactionStatus.Closing))
            {
                throw new DomainException($"Cannot move transaction from [{transaction.Status}] to Closing");
            }

            transaction.Status = TransactionStatus.Closing;
            transaction.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(transaction);
            await _uow.Commit(ct);

            _logger.LogInformation("Transaction moved to Closing: {TransactionNumber}", transaction.TransactionNumber);

            return MapToDto(transaction);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private RealEstateTransactionDto MapToDto(RealEstateTransaction transaction)
    {
        return new RealEstateTransactionDto
        {
            Id = transaction.Id,
            TransactionNumber = transaction.TransactionNumber,
            PropertyId = transaction.PropertyId,
            BuyerId = transaction.BuyerId,
            SellerId = transaction.SellerId,
            BuyerAgentId = transaction.BuyerAgentId,
            SellerAgentId = transaction.SellerAgentId,
            SalePrice = transaction.SalePrice,
            DepositAmount = transaction.DepositAmount,
            CommissionAmount = transaction.CommissionAmount,
            Status = transaction.Status.ToString(),
            OfferDate = transaction.OfferDate,
            AcceptanceDate = transaction.AcceptanceDate,
            ClosingDate = transaction.ClosingDate,
            PossessionDate = transaction.PossessionDate,
            Notes = transaction.Notes,
            CreatedAt = transaction.CreatedAt,
            UpdatedAt = transaction.UpdatedAt
        };
    }
}

public class TransactionCloseHandler : IRequestHandler<TransactionCloseCmd, RealEstateTransactionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public TransactionCloseHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<RealEstateTransactionDto> Handle(TransactionCloseCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var transaction = await _uow.Set<RealEstateTransaction>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (transaction == null)
                throw new DomainException($"Transaction with id [{request.Id}] NOT FOUND.");

            // ✅ CORRECTED: Allow closing from Accepted or Closing status
            if (transaction.Status != TransactionStatus.Closing && transaction.Status != TransactionStatus.Accepted)
            {
                throw new DomainException($"Cannot close transaction with status [{transaction.Status}]. Only Closing or Accepted transactions can be closed.");
            }

            // ✅ If it's Accepted, move to Closing first
            if (transaction.Status == TransactionStatus.Accepted)
            {
                transaction.Status = TransactionStatus.Closing;
                transaction.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
            }

            // ✅ Then close the transaction
            transaction.Status = TransactionStatus.Completed;
            transaction.ClosingDate = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
            transaction.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            // Update property status to Sold
            var property = await _uow.Set<Property>()
                .FirstOrDefaultAsync(x => x.Id == transaction.PropertyId && !x.IsDeleted, ct);

            if (property != null)
            {
                property.Status = PropertyStatus.Sold;
                property.SoldDate = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
                property.SoldPrice = transaction.SalePrice;
                property.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
                await _uow.Update(property);
            }

            await _uow.Commit(ct);

            _logger.LogInformation("Transaction closed: {TransactionNumber}", transaction.TransactionNumber);

            return MapToDto(transaction);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private RealEstateTransactionDto MapToDto(RealEstateTransaction transaction)
    {
        return new RealEstateTransactionDto
        {
            Id = transaction.Id,
            TransactionNumber = transaction.TransactionNumber,
            PropertyId = transaction.PropertyId,
            BuyerId = transaction.BuyerId,
            SellerId = transaction.SellerId,
            BuyerAgentId = transaction.BuyerAgentId,
            SellerAgentId = transaction.SellerAgentId,
            SalePrice = transaction.SalePrice,
            DepositAmount = transaction.DepositAmount,
            CommissionAmount = transaction.CommissionAmount,
            Status = transaction.Status.ToString(),
            OfferDate = transaction.OfferDate,
            AcceptanceDate = transaction.AcceptanceDate,
            ClosingDate = transaction.ClosingDate,
            PossessionDate = transaction.PossessionDate,
            Notes = transaction.Notes,
            CreatedAt = transaction.CreatedAt,
            UpdatedAt = transaction.UpdatedAt
        };
    }
}

