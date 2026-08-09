// Cor.CRM/Commands/ContractCommands.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;
using ContractStatus = Cor.CRM.Models.Entities.ContractStatus;
namespace Cor.CRM.Commands;

// ============================================================
// COMMANDS
// ============================================================

public class ContractAddCmd : IRequest<ContractDto>
{
    public CreateContractDto Dto { get; set; } = default!;
}

public class ContractUpdateCmd : IRequest<ContractDto>
{
    public Guid Id { get; set; }
    public UpdateContractDto Dto { get; set; } = default!;
}

public class ContractDelCmd : IRequest
{
    public Guid Id { get; set; }
}

public class ContractSignCmd : IRequest<ContractDto>
{
    public Guid Id { get; set; }
}

public class ContractActivateCmd : IRequest<ContractDto>
{
    public Guid Id { get; set; }
}

public class ContractTerminateCmd : IRequest<ContractDto>
{
    public Guid Id { get; set; }
}

// ============================================================
// CONTRACT ADD HANDLER
// ============================================================

public class ContractAddHandler : IRequestHandler<ContractAddCmd, ContractDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public ContractAddHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<ContractDto> Handle(ContractAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            // Generate contract number
            var contractNumber = await GenerateContractNumber(ct);

            var contract = new Contract
            {
                Id = Guid.CreateVersion7(),
                ContractNumber = contractNumber,
                CustomerId = request.Dto.CustomerId,
                OpportunityId = request.Dto.OpportunityId,
                QuoteId = request.Dto.QuoteId,
                Title = request.Dto.Title,
                Description = request.Dto.Description,
                TotalValue = request.Dto.TotalValue,
                Status = Enum.TryParse<ContractStatus>(request.Dto.Status, true, out var status)
                    ? status
                    : ContractStatus.Draft,
                StartDate = DateTimeHelper.EnsureUtc(request.Dto.StartDate),
                EndDate = request.Dto.EndDate.HasValue
                    ? DateTimeHelper.EnsureUtc(request.Dto.EndDate.Value)
                    : null,
                TermsAndConditions = request.Dto.TermsAndConditions,
                Notes = request.Dto.Notes,
                CreatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                IsDeleted = false
            };

            // Add contract lines if provided
            var contractLines = new List<ContractLine>();
            int sortOrder = 0;

            if (request.Dto.ContractLines != null && request.Dto.ContractLines.Any())
            {
                foreach (var lineDto in request.Dto.ContractLines)
                {
                    var contractLine = new ContractLine
                    {
                        Id = Guid.CreateVersion7(),
                        ContractId = contract.Id,
                        ProductId = lineDto.ProductId,
                        Description = lineDto.Description,
                        Quantity = lineDto.Quantity,
                        UnitPrice = lineDto.UnitPrice,
                        TotalPrice = lineDto.Quantity * lineDto.UnitPrice,
                        SortOrder = sortOrder++,
                        Notes = lineDto.Notes,
                        CreatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                        UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                        IsDeleted = false
                    };
                    contractLines.Add(contractLine);
                    await _uow.Add(contractLine, ct);
                }
            }

            await _uow.Add(contract, ct);
            await _uow.Commit(ct);

            _logger.LogInformation("Contract created: {ContractNumber} - {Title}",
                contract.ContractNumber, contract.Title);

            return MapToDto(contract, contractLines);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private async Task<string> GenerateContractNumber(CancellationToken ct)
    {
        var count = await _uow.Set<Contract>().CountAsync(ct) + 1;
        return $"CT-{DateTime.UtcNow:yyyyMMdd}-{count:D4}";
    }

    public static ContractDto MapToDto(Contract contract, List<ContractLine> contractLines)
    {
        var dto = new ContractDto
        {
            Id = contract.Id,
            ContractNumber = contract.ContractNumber,
            Title = contract.Title,
            Description = contract.Description,
            CustomerId = contract.CustomerId,
            CustomerName = contract.Customer?.Name,
            OpportunityId = contract.OpportunityId,
            OpportunityName = contract.Opportunity?.Name,
            QuoteId = contract.QuoteId,
            QuoteNumber = contract.Quote?.QuoteNumber,
            TotalValue = contract.TotalValue,
            Status = contract.Status.ToString(),
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
            SignedDate = contract.SignedDate,
            TermsAndConditions = contract.TermsAndConditions,
            Notes = contract.Notes,
            CreatedAt = contract.CreatedAt,
            UpdatedAt = contract.UpdatedAt,
            ContractLines = contractLines.Where(l => !l.IsDeleted).Select(l => new ContractLineDto
            {
                Id = l.Id,
                ContractId = l.ContractId,
                ProductId = l.ProductId,
                ProductName = l.Product?.Name,
                Description = l.Description,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice,
                TotalPrice = l.TotalPrice,
                SortOrder = l.SortOrder,
                Notes = l.Notes
            }).ToList()
        };

        return dto;
    }
}

// ============================================================
// CONTRACT UPDATE HANDLER
// ============================================================

public class ContractUpdateHandler : IRequestHandler<ContractUpdateCmd, ContractDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public ContractUpdateHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<ContractDto> Handle(ContractUpdateCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var contract = await _uow.Set<Contract>()
                .Include(x => x.ContractLines)
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (contract == null)
                throw new DomainException($"Contract with id [{request.Id}] NOT FOUND.");

            // Update basic fields
            if (!string.IsNullOrEmpty(request.Dto.Title))
                contract.Title = request.Dto.Title;

            if (request.Dto.Description != null)
                contract.Description = request.Dto.Description;

            if (request.Dto.TotalValue.HasValue)
                contract.TotalValue = request.Dto.TotalValue.Value;

            if (!string.IsNullOrEmpty(request.Dto.Status))
            {
                contract.Status = Enum.TryParse<ContractStatus>(request.Dto.Status, true, out var status)
                    ? status
                    : contract.Status;
            }

            if (request.Dto.StartDate.HasValue)
                contract.StartDate = DateTimeHelper.EnsureUtc(request.Dto.StartDate.Value);

            if (request.Dto.EndDate.HasValue)
                contract.EndDate = DateTimeHelper.EnsureUtc(request.Dto.EndDate.Value);

            if (request.Dto.SignedDate.HasValue)
                contract.SignedDate = DateTimeHelper.EnsureUtc(request.Dto.SignedDate.Value);

            if (request.Dto.TermsAndConditions != null)
                contract.TermsAndConditions = request.Dto.TermsAndConditions;

            if (request.Dto.Notes != null)
                contract.Notes = request.Dto.Notes;

            // Update lines if provided
            if (request.Dto.ContractLines != null && request.Dto.ContractLines.Any())
            {
                // Soft delete existing lines
                var existingLines = await _uow.Set<ContractLine>()
                    .Where(x => x.ContractId == contract.Id && !x.IsDeleted)
                    .ToListAsync(ct);

                foreach (var line in existingLines)
                {
                    line.IsDeleted = true;
                    line.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
                    await _uow.Update(line);
                }

                // Add new lines
                int sortOrder = 0;
                var newContractLines = new List<ContractLine>();

                foreach (var lineDto in request.Dto.ContractLines)
                {
                    var contractLine = new ContractLine
                    {
                        Id = Guid.CreateVersion7(),
                        ContractId = contract.Id,
                        ProductId = lineDto.ProductId,
                        Description = lineDto.Description ?? string.Empty,
                        Quantity = lineDto.Quantity,
                        UnitPrice = lineDto.UnitPrice,
                        TotalPrice = lineDto.Quantity * lineDto.UnitPrice,
                        SortOrder = sortOrder++,
                        Notes = lineDto.Notes,
                        CreatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                        UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                        IsDeleted = false
                    };
                    newContractLines.Add(contractLine);
                    await _uow.Add(contractLine, ct);
                }
            }

            contract.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
            await _uow.Update(contract);
            await _uow.Commit(ct);

            _logger.LogInformation("Contract updated: {ContractNumber}", contract.ContractNumber);

            var activeLines = contract.ContractLines.Where(l => !l.IsDeleted).ToList();
            return ContractAddHandler.MapToDto(contract, activeLines);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// CONTRACT DELETE HANDLER
// ============================================================

public class ContractDelHandler : IRequestHandler<ContractDelCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public ContractDelHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task Handle(ContractDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var contract = await _uow.Set<Contract>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (contract == null)
                throw new DomainException($"Contract with id [{request.Id}] NOT FOUND.");

            // Only Draft or Pending contracts can be deleted
            if (contract.Status != ContractStatus.Draft && contract.Status != ContractStatus.Pending)
                throw new DomainException($"Cannot delete contract with status [{contract.Status}]");

            contract.IsDeleted = true;
            contract.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(contract);
            await _uow.Commit(ct);

            _logger.LogInformation("Contract deleted: {ContractNumber}", contract.ContractNumber);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// CONTRACT SIGN HANDLER
// ============================================================

public class ContractSignHandler : IRequestHandler<ContractSignCmd, ContractDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public ContractSignHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<ContractDto> Handle(ContractSignCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var contract = await _uow.Set<Contract>()
                .Include(x => x.ContractLines)
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (contract == null)
                throw new DomainException($"Contract with id [{request.Id}] NOT FOUND.");

            if (contract.Status != ContractStatus.Pending && contract.Status != ContractStatus.Active)
                throw new DomainException($"Cannot sign contract with status [{contract.Status}]");

            contract.Status = ContractStatus.Signed;
            contract.SignedDate = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
            contract.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(contract);
            await _uow.Commit(ct);

            _logger.LogInformation("Contract signed: {ContractNumber}", contract.ContractNumber);

            var activeLines = contract.ContractLines.Where(l => !l.IsDeleted).ToList();
            return ContractAddHandler.MapToDto(contract, activeLines);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// CONTRACT ACTIVATE HANDLER
// ============================================================

public class ContractActivateHandler : IRequestHandler<ContractActivateCmd, ContractDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public ContractActivateHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<ContractDto> Handle(ContractActivateCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var contract = await _uow.Set<Contract>()
                .Include(x => x.ContractLines)
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (contract == null)
                throw new DomainException($"Contract with id [{request.Id}] NOT FOUND.");

            if (contract.Status != ContractStatus.Draft && contract.Status != ContractStatus.Pending)
                throw new DomainException($"Cannot activate contract with status [{contract.Status}]");

            contract.Status = ContractStatus.Active;
            contract.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(contract);
            await _uow.Commit(ct);

            _logger.LogInformation("Contract activated: {ContractNumber}", contract.ContractNumber);

            var activeLines = contract.ContractLines.Where(l => !l.IsDeleted).ToList();
            return ContractAddHandler.MapToDto(contract, activeLines);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// CONTRACT TERMINATE HANDLER
// ============================================================

public class ContractTerminateHandler : IRequestHandler<ContractTerminateCmd, ContractDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public ContractTerminateHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<ContractDto> Handle(ContractTerminateCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var contract = await _uow.Set<Contract>()
                .Include(x => x.ContractLines)
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (contract == null)
                throw new DomainException($"Contract with id [{request.Id}] NOT FOUND.");

            if (contract.Status == ContractStatus.Terminated || contract.Status == ContractStatus.Expired)
                throw new DomainException($"Cannot terminate contract with status [{contract.Status}]");

            contract.Status = ContractStatus.Terminated;
            contract.EndDate = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
            contract.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(contract);
            await _uow.Commit(ct);

            _logger.LogInformation("Contract terminated: {ContractNumber}", contract.ContractNumber);

            var activeLines = contract.ContractLines.Where(l => !l.IsDeleted).ToList();
            return ContractAddHandler.MapToDto(contract, activeLines);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}