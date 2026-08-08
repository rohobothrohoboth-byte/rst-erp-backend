// Cor.CRM/Commands/CommissionCommands.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;
using Cor.CRM.Models.Entities.Local;

namespace Cor.CRM.Commands;

public class CommissionAddCmd : IRequest<CommissionDto>
{
    public CreateCommissionDto Dto { get; set; } = default!;
}

public class CommissionUpdateCmd : IRequest<CommissionDto>
{
    public Guid Id { get; set; }
    public UpdateCommissionDto Dto { get; set; } = default!;
}

public class CommissionDeleteCmd : IRequest
{
    public Guid Id { get; set; }
}

public class CommissionPayCmd : IRequest<CommissionDto>
{
    public Guid Id { get; set; }
    public DateTime? PaymentDate { get; set; }
}

// ✅ Add missing CommissionApproveCmd
public class CommissionApproveCmd : IRequest<CommissionDto>
{
    public Guid Id { get; set; }
}

public class CommissionAddHandler : IRequestHandler<CommissionAddCmd, CommissionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public CommissionAddHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<CommissionDto> Handle(CommissionAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            // Validate transaction exists
            var transaction = await _uow.Set<RealEstateTransaction>()
                .FirstOrDefaultAsync(x => x.Id == request.Dto.TransactionId && !x.IsDeleted, ct);

            if (transaction == null)
                throw new DomainException($"Transaction with id [{request.Dto.TransactionId}] NOT FOUND.");

            // Validate agent exists
            var agent = await _uow.Set<LocalEmployee>()
                .FirstOrDefaultAsync(x => x.AppUserId == request.Dto.AgentId && !x.IsDeleted, ct);

            if (agent == null)
                throw new DomainException($"Agent with id [{request.Dto.AgentId}] NOT FOUND.");

            // ✅ Use CommissionStatus enum - default to Pending
            var status = request.Dto.Status.HasValue
                ? (CommissionStatus)request.Dto.Status.Value
                : CommissionStatus.Pending;

            // Create commission
            var commission = new Commission
            {
                Id = Guid.CreateVersion7(),
                TransactionId = request.Dto.TransactionId,
                AgentId = request.Dto.AgentId,
                Amount = request.Dto.Amount,
                Percentage = request.Dto.Percentage,
                Status = status,
                IsBuyerAgent = request.Dto.IsBuyerAgent,
                IsSellerAgent = request.Dto.IsSellerAgent,
                Notes = request.Dto.Notes,
                CreatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                CreatedByUserId = request.Dto.CreatedByUserId,
                CreatedByUserName = request.Dto.CreatedByUserName,
                IsDeleted = false
            };

            await _uow.Add(commission, ct);
            await _uow.Commit(ct);

            _logger.LogInformation("Commission created for agent: {AgentId} - Amount: {Amount}",
                commission.AgentId, commission.Amount);

            return MapToDto(commission, transaction, agent);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private CommissionDto MapToDto(Commission commission, RealEstateTransaction transaction, LocalEmployee agent)
    {
        return new CommissionDto
        {
            Id = commission.Id,
            TransactionId = commission.TransactionId,
            TransactionNumber = transaction?.TransactionNumber,
            AgentId = commission.AgentId,
            AgentName = agent != null ? $"{agent.FirstName} {agent.LastName}" : null,
            Amount = commission.Amount,
            Percentage = commission.Percentage, // ✅ No ?? operator needed
            Status = commission.Status.ToString(),
            StatusValue = (int)commission.Status,
            IsBuyerAgent = commission.IsBuyerAgent,
            IsSellerAgent = commission.IsSellerAgent,
            Notes = commission.Notes,
            PaymentDate = commission.PaymentDate,
            CreatedAt = commission.CreatedAt,
            UpdatedAt = commission.UpdatedAt,
            CreatedByUserId = commission.CreatedByUserId,
            CreatedByUserName = commission.CreatedByUserName
        };
    }
}

public class CommissionUpdateHandler : IRequestHandler<CommissionUpdateCmd, CommissionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public CommissionUpdateHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<CommissionDto> Handle(CommissionUpdateCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var commission = await _uow.Set<Commission>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (commission == null)
                throw new DomainException($"Commission with id [{request.Id}] NOT FOUND.");

            if (request.Dto.Amount.HasValue)
                commission.Amount = request.Dto.Amount.Value;

            if (request.Dto.Percentage.HasValue)
                commission.Percentage = request.Dto.Percentage.Value;

            if (request.Dto.Status.HasValue)
                commission.Status = (CommissionStatus)request.Dto.Status.Value;

            if (request.Dto.Notes != null)
                commission.Notes = request.Dto.Notes;

            if (request.Dto.PaymentDate.HasValue)
                commission.PaymentDate = DateTimeHelper.EnsureUtc(request.Dto.PaymentDate.Value);

            if (request.Dto.IsBuyerAgent.HasValue)
                commission.IsBuyerAgent = request.Dto.IsBuyerAgent.Value;

            if (request.Dto.IsSellerAgent.HasValue)
                commission.IsSellerAgent = request.Dto.IsSellerAgent.Value;

            commission.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(commission);
            await _uow.Commit(ct);

            _logger.LogInformation("Commission updated: {CommissionId}", commission.Id);

            var transaction = await _uow.Set<RealEstateTransaction>()
                .FirstOrDefaultAsync(x => x.Id == commission.TransactionId && !x.IsDeleted, ct);

            var agent = await _uow.Set<LocalEmployee>()
                .FirstOrDefaultAsync(x => x.AppUserId == commission.AgentId && !x.IsDeleted, ct);

            return MapToDto(commission, transaction, agent);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private CommissionDto MapToDto(Commission commission, RealEstateTransaction? transaction, LocalEmployee? agent)
    {
        return new CommissionDto
        {
            Id = commission.Id,
            TransactionId = commission.TransactionId,
            TransactionNumber = transaction?.TransactionNumber,
            AgentId = commission.AgentId,
            AgentName = agent != null ? $"{agent.FirstName} {agent.LastName}" : null,
            Amount = commission.Amount,
            Percentage = commission.Percentage, // ✅ No ?? operator needed
            Status = commission.Status.ToString(),
            StatusValue = (int)commission.Status,
            IsBuyerAgent = commission.IsBuyerAgent,
            IsSellerAgent = commission.IsSellerAgent,
            Notes = commission.Notes,
            PaymentDate = commission.PaymentDate,
            CreatedAt = commission.CreatedAt,
            UpdatedAt = commission.UpdatedAt,
            CreatedByUserId = commission.CreatedByUserId,
            CreatedByUserName = commission.CreatedByUserName
        };
    }
}

public class CommissionApproveHandler : IRequestHandler<CommissionApproveCmd, CommissionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public CommissionApproveHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<CommissionDto> Handle(CommissionApproveCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var commission = await _uow.Set<Commission>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (commission == null)
                throw new DomainException($"Commission with id [{request.Id}] NOT FOUND.");

            if (commission.Status != CommissionStatus.Pending)
                throw new DomainException($"Cannot approve commission with status [{commission.Status}]. Only Pending commissions can be approved.");

            commission.Status = CommissionStatus.Approved;
            commission.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(commission);
            await _uow.Commit(ct);

            _logger.LogInformation("Commission approved: {CommissionId}", commission.Id);

            var transaction = await _uow.Set<RealEstateTransaction>()
                .FirstOrDefaultAsync(x => x.Id == commission.TransactionId && !x.IsDeleted, ct);

            var agent = await _uow.Set<LocalEmployee>()
                .FirstOrDefaultAsync(x => x.AppUserId == commission.AgentId && !x.IsDeleted, ct);

            return MapToDto(commission, transaction, agent);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private CommissionDto MapToDto(Commission commission, RealEstateTransaction? transaction, LocalEmployee? agent)
    {
        return new CommissionDto
        {
            Id = commission.Id,
            TransactionId = commission.TransactionId,
            TransactionNumber = transaction?.TransactionNumber,
            AgentId = commission.AgentId,
            AgentName = agent != null ? $"{agent.FirstName} {agent.LastName}" : null,
            Amount = commission.Amount,
            Percentage = commission.Percentage,
            Status = commission.Status.ToString(),
            StatusValue = (int)commission.Status,
            IsBuyerAgent = commission.IsBuyerAgent,
            IsSellerAgent = commission.IsSellerAgent,
            Notes = commission.Notes,
            PaymentDate = commission.PaymentDate,
            CreatedAt = commission.CreatedAt,
            UpdatedAt = commission.UpdatedAt,
            CreatedByUserId = commission.CreatedByUserId,
            CreatedByUserName = commission.CreatedByUserName
        };
    }
}

public class CommissionPayHandler : IRequestHandler<CommissionPayCmd, CommissionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public CommissionPayHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<CommissionDto> Handle(CommissionPayCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var commission = await _uow.Set<Commission>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (commission == null)
                throw new DomainException($"Commission with id [{request.Id}] NOT FOUND.");

            if (commission.Status != CommissionStatus.Approved)
                throw new DomainException($"Cannot pay commission with status [{commission.Status}]. Only Approved commissions can be paid.");

            if (commission.Status == CommissionStatus.Paid)
                throw new DomainException($"Commission is already paid");

            commission.Status = CommissionStatus.Paid;
            commission.PaymentDate = request.PaymentDate.HasValue
                ? DateTimeHelper.EnsureUtc(request.PaymentDate.Value)
                : DateTimeHelper.EnsureUtc(DateTime.UtcNow);
            commission.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(commission);
            await _uow.Commit(ct);

            _logger.LogInformation("Commission paid: {CommissionId}", commission.Id);

            var transaction = await _uow.Set<RealEstateTransaction>()
                .FirstOrDefaultAsync(x => x.Id == commission.TransactionId && !x.IsDeleted, ct);

            var agent = await _uow.Set<LocalEmployee>()
                .FirstOrDefaultAsync(x => x.AppUserId == commission.AgentId && !x.IsDeleted, ct);

            return MapToDto(commission, transaction, agent);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private CommissionDto MapToDto(Commission commission, RealEstateTransaction? transaction, LocalEmployee? agent)
    {
        return new CommissionDto
        {
            Id = commission.Id,
            TransactionId = commission.TransactionId,
            TransactionNumber = transaction?.TransactionNumber,
            AgentId = commission.AgentId,
            AgentName = agent != null ? $"{agent.FirstName} {agent.LastName}" : null,
            Amount = commission.Amount,
            Percentage = commission.Percentage,
            Status = commission.Status.ToString(),
            StatusValue = (int)commission.Status,
            IsBuyerAgent = commission.IsBuyerAgent,
            IsSellerAgent = commission.IsSellerAgent,
            Notes = commission.Notes,
            PaymentDate = commission.PaymentDate,
            CreatedAt = commission.CreatedAt,
            UpdatedAt = commission.UpdatedAt,
            CreatedByUserId = commission.CreatedByUserId,
            CreatedByUserName = commission.CreatedByUserName
        };
    }
}

public class CommissionDeleteHandler : IRequestHandler<CommissionDeleteCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public CommissionDeleteHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task Handle(CommissionDeleteCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var commission = await _uow.Set<Commission>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (commission == null)
                throw new DomainException($"Commission with id [{request.Id}] NOT FOUND.");

            if (commission.Status == CommissionStatus.Paid)
                throw new DomainException($"Cannot delete a paid commission");

            commission.IsDeleted = true;
            commission.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(commission);
            await _uow.Commit(ct);

            _logger.LogInformation("Commission deleted: {CommissionId}", commission.Id);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}