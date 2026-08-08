// Commands/RequisitionCommands.cs
using MediatR;
using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Models.Entities;
using Cor.Procurement.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;

namespace Cor.Procurement.Commands;

// ============================================================
// COMMANDS
// ============================================================

public class CreateRequisitionCommand : IRequest<RequisitionDto>
{
    public CreateRequisitionDto CreateDto { get; set; } = new();
}

public class UpdateRequisitionCommand : IRequest<RequisitionDto>
{
    public UpdateRequisitionDto UpdateDto { get; set; } = new();
}

public class DeleteRequisitionCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class SubmitRequisitionCommand : IRequest<RequisitionDto>
{
    public Guid Id { get; set; }
}

public class ApproveRequisitionCommand : IRequest<RequisitionDto>
{
    public RequisitionApprovalActionDto ActionDto { get; set; } = new();
}

// ============================================================
// HANDLERS
// ============================================================

// ==================== CREATE REQUISITION ====================

public class CreateRequisitionCommandHandler : IRequestHandler<CreateRequisitionCommand, RequisitionDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<CreateRequisitionCommandHandler> _logger;
    private readonly ICacheService _cache;

    public CreateRequisitionCommandHandler(
        ProcurementDbContext context,
        ILogger<CreateRequisitionCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<RequisitionDto> Handle(CreateRequisitionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating new requisition");

            if (request.CreateDto == null)
                throw new ArgumentException("Requisition data is required");

            if (request.CreateDto.Lines == null || !request.CreateDto.Lines.Any())
                throw new ArgumentException("At least one line item is required");

            var requisitionNumber = GenerateRequisitionNumber();

            var requisition = new Requisition
            {
                Id = Guid.NewGuid(),
                RequisitionNumber = requisitionNumber,
                Title = request.CreateDto.Title,
                Description = request.CreateDto.Description,
                DepartmentId = request.CreateDto.DepartmentId,
                DepartmentName = request.CreateDto.DepartmentName,
                RequesterId = request.CreateDto.RequesterId,
                RequesterName = request.CreateDto.RequesterName,
                RequiredDate = request.CreateDto.RequiredDate,
                Priority = request.CreateDto.Priority,
                Status = "Draft",
                BudgetCode = request.CreateDto.BudgetCode,
                SubmittedDate = DateTime.UtcNow,
                TotalAmount = 0,
                DateAdd = DateTime.UtcNow,
                IsDeleted = false,
                CreatedByUserId = request.CreateDto.CreatedByUserId,
                CreatedByUserName = request.CreateDto.CreatedByUserName,
                PeriodId = request.CreateDto.PeriodId
            };

            decimal totalAmount = 0;
            foreach (var lineDto in request.CreateDto.Lines)
            {
                var lineTotal = lineDto.Quantity * lineDto.UnitPrice;
                totalAmount += lineTotal;

                var line = new RequisitionLine
                {
                    Id = Guid.NewGuid(),
                    RequisitionId = requisition.Id,
                    Description = lineDto.Description,
                    Quantity = lineDto.Quantity,
                    UnitPrice = lineDto.UnitPrice,
                    TotalAmount = lineTotal,
                    UnitOfMeasure = lineDto.UnitOfMeasure ?? "Each",
                    Notes = lineDto.Notes,
                    PeriodId = request.CreateDto.PeriodId,
                    DateAdd = DateTime.UtcNow,
                    IsDeleted = false
                };

                requisition.Lines.Add(line);
            }

            requisition.TotalAmount = totalAmount;

            await _context.Requisitions.AddAsync(requisition, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync("requisitions_all", cancellationToken);

            _logger.LogInformation("Requisition created successfully: {RequisitionNumber}", requisitionNumber);

            return MapToDto(requisition);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating requisition");
            throw;
        }
    }

    private string GenerateRequisitionNumber()
    {
        var today = DateTime.UtcNow;
        return $"REQ-{today:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
    }

    private RequisitionDto MapToDto(Requisition requisition)
    {
        return new RequisitionDto
        {
            Id = requisition.Id,
            RequisitionNumber = requisition.RequisitionNumber,
            Title = requisition.Title,
            Description = requisition.Description,
            DepartmentId = requisition.DepartmentId,
            DepartmentName = requisition.DepartmentName,
            RequesterId = requisition.RequesterId,
            RequesterName = requisition.RequesterName,
            RequiredDate = requisition.RequiredDate,
            SubmittedDate = requisition.SubmittedDate,
            Priority = requisition.Priority,
            Status = requisition.Status,
            TotalAmount = requisition.TotalAmount,
            BudgetCode = requisition.BudgetCode,
            PurchaseOrderId = requisition.PurchaseOrderId,
            PurchaseOrderNumber = requisition.PurchaseOrderNumber,
            RejectionReason = requisition.RejectionReason,
            DateAdd = requisition.DateAdd,
            DateMod = requisition.DateMod,
            RowVersion = requisition.RowVersion,
            Lines = requisition.Lines.Select(l => new RequisitionLineDto
            {
                Id = l.Id,
                Description = l.Description,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice,
                TotalAmount = l.TotalAmount,
                UnitOfMeasure = l.UnitOfMeasure,
                Notes = l.Notes
            }).ToList()
        };
    }
}

// ==================== UPDATE REQUISITION ====================

public class UpdateRequisitionCommandHandler : IRequestHandler<UpdateRequisitionCommand, RequisitionDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<UpdateRequisitionCommandHandler> _logger;
    private readonly ICacheService _cache;

    public UpdateRequisitionCommandHandler(
        ProcurementDbContext context,
        ILogger<UpdateRequisitionCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<RequisitionDto> Handle(UpdateRequisitionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating requisition: {RequisitionId}", request.UpdateDto.Id);

            var requisition = await _context.Requisitions
                .Include(r => r.Lines)
                .FirstOrDefaultAsync(r => r.Id == request.UpdateDto.Id && !r.IsDeleted, cancellationToken);

            if (requisition == null)
                throw new KeyNotFoundException($"Requisition with ID '{request.UpdateDto.Id}' not found");

            if (requisition.Status != "Draft" && requisition.Status != "Rejected")
                throw new InvalidOperationException($"Cannot update requisition with status '{requisition.Status}'");

            requisition.Title = request.UpdateDto.Title;
            requisition.Description = request.UpdateDto.Description;
            requisition.DepartmentId = request.UpdateDto.DepartmentId;
            requisition.DepartmentName = request.UpdateDto.DepartmentName;
            requisition.RequiredDate = request.UpdateDto.RequiredDate;
            requisition.Priority = request.UpdateDto.Priority;
            requisition.BudgetCode = request.UpdateDto.BudgetCode;
            requisition.DateMod = DateTime.UtcNow;

            if (request.UpdateDto.Lines != null && request.UpdateDto.Lines.Any())
            {
                _context.RequisitionLines.RemoveRange(requisition.Lines);
                requisition.Lines.Clear();

                decimal totalAmount = 0;
                foreach (var lineDto in request.UpdateDto.Lines)
                {
                    var lineTotal = lineDto.Quantity * lineDto.UnitPrice;
                    totalAmount += lineTotal;

                    var line = new RequisitionLine
                    {
                        Id = lineDto.Id ?? Guid.NewGuid(),
                        RequisitionId = requisition.Id,
                        Description = lineDto.Description,
                        Quantity = lineDto.Quantity,
                        UnitPrice = lineDto.UnitPrice,
                        TotalAmount = lineTotal,
                        UnitOfMeasure = lineDto.UnitOfMeasure ?? "Each",
                        Notes = lineDto.Notes,
                        PeriodId = requisition.PeriodId,
                        DateAdd = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    requisition.Lines.Add(line);
                }

                requisition.TotalAmount = totalAmount;
            }

            requisition.RowVersion = Guid.NewGuid().ToString();

            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync($"requisition_{requisition.Id}", cancellationToken);
            await _cache.RemoveAsync("requisitions_all", cancellationToken);

            _logger.LogInformation("Requisition updated successfully: {RequisitionNumber}", requisition.RequisitionNumber);

            return MapToDto(requisition);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency conflict while updating requisition {RequisitionId}", request.UpdateDto.Id);

            foreach (var entry in ex.Entries)
            {
                await entry.ReloadAsync(cancellationToken);
            }

            return await Handle(request, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating requisition");
            throw;
        }
    }

    private RequisitionDto MapToDto(Requisition requisition)
    {
        return new RequisitionDto
        {
            Id = requisition.Id,
            RequisitionNumber = requisition.RequisitionNumber,
            Title = requisition.Title,
            Description = requisition.Description,
            DepartmentId = requisition.DepartmentId,
            DepartmentName = requisition.DepartmentName,
            RequesterId = requisition.RequesterId,
            RequesterName = requisition.RequesterName,
            RequiredDate = requisition.RequiredDate,
            SubmittedDate = requisition.SubmittedDate,
            Priority = requisition.Priority,
            Status = requisition.Status,
            TotalAmount = requisition.TotalAmount,
            BudgetCode = requisition.BudgetCode,
            PurchaseOrderId = requisition.PurchaseOrderId,
            PurchaseOrderNumber = requisition.PurchaseOrderNumber,
            RejectionReason = requisition.RejectionReason,
            DateAdd = requisition.DateAdd,
            DateMod = requisition.DateMod,
            RowVersion = requisition.RowVersion,
            Lines = requisition.Lines.Select(l => new RequisitionLineDto
            {
                Id = l.Id,
                Description = l.Description,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice,
                TotalAmount = l.TotalAmount,
                UnitOfMeasure = l.UnitOfMeasure,
                Notes = l.Notes
            }).ToList()
        };
    }
}

// ==================== DELETE REQUISITION ====================

public class DeleteRequisitionCommandHandler : IRequestHandler<DeleteRequisitionCommand, bool>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<DeleteRequisitionCommandHandler> _logger;
    private readonly ICacheService _cache;

    public DeleteRequisitionCommandHandler(
        ProcurementDbContext context,
        ILogger<DeleteRequisitionCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<bool> Handle(DeleteRequisitionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deleting requisition: {RequisitionId}", request.Id);

            var requisition = await _context.Requisitions
                .FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, cancellationToken);

            if (requisition == null)
                return false;

            if (requisition.Status != "Draft" && requisition.Status != "Rejected")
                throw new InvalidOperationException($"Cannot delete requisition with status '{requisition.Status}'");

            requisition.IsDeleted = true;
            requisition.DateMod = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync($"requisition_{request.Id}", cancellationToken);
            await _cache.RemoveAsync("requisitions_all", cancellationToken);

            _logger.LogInformation("Requisition deleted successfully: {RequisitionNumber}", requisition.RequisitionNumber);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting requisition");
            throw;
        }
    }
}

// ==================== SUBMIT REQUISITION ====================

public class SubmitRequisitionCommandHandler : IRequestHandler<SubmitRequisitionCommand, RequisitionDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<SubmitRequisitionCommandHandler> _logger;
    private readonly ICacheService _cache;

    public SubmitRequisitionCommandHandler(
        ProcurementDbContext context,
        ILogger<SubmitRequisitionCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<RequisitionDto> Handle(SubmitRequisitionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Submitting requisition: {RequisitionId}", request.Id);

            var requisition = await _context.Requisitions
                .Include(r => r.Lines)
                .FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, cancellationToken);

            if (requisition == null)
                throw new KeyNotFoundException($"Requisition with ID '{request.Id}' not found");

            if (requisition.Status != "Draft")
                throw new InvalidOperationException($"Cannot submit requisition with status '{requisition.Status}'");

            requisition.Status = "Submitted";
            requisition.SubmittedDate = DateTime.UtcNow;
            requisition.DateMod = DateTime.UtcNow;

            // ✅ DO NOT update RowVersion to avoid concurrency issues
            // requisition.RowVersion = Guid.NewGuid().ToString();

            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync($"requisition_{requisition.Id}", cancellationToken);
            await _cache.RemoveAsync("requisitions_all", cancellationToken);

            _logger.LogInformation("Requisition submitted successfully: {RequisitionNumber}", requisition.RequisitionNumber);

            return MapToDto(requisition);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency conflict while submitting requisition {RequisitionId}", request.Id);

            foreach (var entry in ex.Entries)
            {
                await entry.ReloadAsync(cancellationToken);
            }

            return await Handle(request, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting requisition");
            throw;
        }
    }

    private RequisitionDto MapToDto(Requisition requisition)
    {
        return new RequisitionDto
        {
            Id = requisition.Id,
            RequisitionNumber = requisition.RequisitionNumber,
            Title = requisition.Title,
            Description = requisition.Description,
            DepartmentId = requisition.DepartmentId,
            DepartmentName = requisition.DepartmentName,
            RequesterId = requisition.RequesterId,
            RequesterName = requisition.RequesterName,
            RequiredDate = requisition.RequiredDate,
            SubmittedDate = requisition.SubmittedDate,
            Priority = requisition.Priority,
            Status = requisition.Status,
            TotalAmount = requisition.TotalAmount,
            BudgetCode = requisition.BudgetCode,
            PurchaseOrderId = requisition.PurchaseOrderId,
            PurchaseOrderNumber = requisition.PurchaseOrderNumber,
            RejectionReason = requisition.RejectionReason,
            DateAdd = requisition.DateAdd,
            DateMod = requisition.DateMod,
            RowVersion = requisition.RowVersion,
            Lines = requisition.Lines.Select(l => new RequisitionLineDto
            {
                Id = l.Id,
                Description = l.Description,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice,
                TotalAmount = l.TotalAmount,
                UnitOfMeasure = l.UnitOfMeasure,
                Notes = l.Notes
            }).ToList()
        };
    }
}

// ==================== APPROVE/REJECT REQUISITION ====================
// ==================== APPROVE/REJECT REQUISITION ====================

public class ApproveRequisitionCommandHandler : IRequestHandler<ApproveRequisitionCommand, RequisitionDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<ApproveRequisitionCommandHandler> _logger;
    private readonly ICacheService _cache;

    public ApproveRequisitionCommandHandler(
        ProcurementDbContext context,
        ILogger<ApproveRequisitionCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<RequisitionDto> Handle(ApproveRequisitionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing requisition approval: {RequisitionId}", request.ActionDto.RequisitionId);

            // ✅ Use a fresh query to get the latest data
            var requisition = await _context.Requisitions
                .Include(r => r.Lines)
                .Include(r => r.Approvals)
                .AsNoTracking() // ✅ Use AsNoTracking to avoid tracking issues
                .FirstOrDefaultAsync(r => r.Id == request.ActionDto.RequisitionId && !r.IsDeleted, cancellationToken);

            if (requisition == null)
                throw new KeyNotFoundException($"Requisition with ID '{request.ActionDto.RequisitionId}' not found");

            // ✅ Check if already approved
            if (requisition.Status == "Approved")
            {
                _logger.LogInformation("Requisition already approved: {RequisitionNumber}", requisition.RequisitionNumber);
                return MapToDto(requisition);
            }

            // ✅ Check if already rejected
            if (requisition.Status == "Rejected")
            {
                _logger.LogInformation("Requisition already rejected: {RequisitionNumber}", requisition.RequisitionNumber);
                return MapToDto(requisition);
            }

            if (requisition.Status != "Submitted" && requisition.Status != "UnderReview")
                throw new InvalidOperationException($"Cannot approve requisition with status '{requisition.Status}'");

            // ✅ Store the current RowVersion for concurrency check
            var currentRowVersion = requisition.RowVersion;
            var currentStatus = requisition.Status;

            if (request.ActionDto.Action?.ToLower() == "approve")
            {
                requisition.Status = "Approved";
                requisition.ApprovedAt = DateTime.UtcNow;
                requisition.ApprovedBy = request.ActionDto.ApproverId ?? Guid.NewGuid();

                var approval = new RequisitionApproval
                {
                    Id = Guid.NewGuid(),
                    RequisitionId = requisition.Id,
                    ApproverId = request.ActionDto.ApproverId ?? Guid.NewGuid(),
                    ApproverName = request.ActionDto.ApproverName ?? "System",
                    Status = "Approved",
                    Comments = request.ActionDto.Comments,
                    ApprovedAt = DateTime.UtcNow,
                    ApprovalLevel = request.ActionDto.ApprovalLevel ?? 1,
                    DateAdd = DateTime.UtcNow,
                    IsDeleted = false
                };

                requisition.Approvals.Add(approval);

                _logger.LogInformation("Requisition approved successfully: {RequisitionNumber}", requisition.RequisitionNumber);
            }
            else if (request.ActionDto.Action?.ToLower() == "reject")
            {
                requisition.Status = "Rejected";
                requisition.RejectionReason = request.ActionDto.RejectionReason ?? request.ActionDto.Comments;

                var approval = new RequisitionApproval
                {
                    Id = Guid.NewGuid(),
                    RequisitionId = requisition.Id,
                    ApproverId = request.ActionDto.ApproverId ?? Guid.NewGuid(),
                    ApproverName = request.ActionDto.ApproverName ?? "System",
                    Status = "Rejected",
                    Comments = request.ActionDto.Comments,
                    ApprovedAt = DateTime.UtcNow,
                    ApprovalLevel = request.ActionDto.ApprovalLevel ?? 1,
                    DateAdd = DateTime.UtcNow,
                    IsDeleted = false
                };

                requisition.Approvals.Add(approval);

                _logger.LogInformation("Requisition rejected successfully: {RequisitionNumber}", requisition.RequisitionNumber);
            }
            else
            {
                throw new InvalidOperationException($"Invalid action: '{request.ActionDto.Action}'. Must be 'Approve' or 'Reject'.");
            }

            requisition.DateMod = DateTime.UtcNow;
            requisition.UpdatedByUserId = request.ActionDto.ApproverId;
            requisition.UpdatedByUserName = request.ActionDto.ApproverName;

            // ✅ Update RowVersion
            requisition.UpdateRowVersion();

            // ✅ Attach the entity and mark it as modified
            _context.Requisitions.Attach(requisition);
            _context.Entry(requisition).State = EntityState.Modified;

            // ✅ Also mark the approvals as added
            foreach (var approval in requisition.Approvals)
            {
                _context.RequisitionApprovals.Add(approval);
            }

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict while approving requisition {RequisitionId}", request.ActionDto.RequisitionId);

                // ✅ Reload the entity to get the latest database values
                foreach (var entry in ex.Entries)
                {
                    await entry.ReloadAsync(cancellationToken);
                    var entity = entry.Entity as Requisition;
                    if (entity != null)
                    {
                        if (entity.Status == "Approved")
                        {
                            _logger.LogInformation("Requisition was already approved by another user: {RequisitionNumber}", entity.RequisitionNumber);
                            return MapToDto(entity);
                        }
                        else if (entity.Status == "Rejected")
                        {
                            _logger.LogInformation("Requisition was already rejected by another user: {RequisitionNumber}", entity.RequisitionNumber);
                            return MapToDto(entity);
                        }
                        else if (entity.Status != currentStatus)
                        {
                            _logger.LogWarning("Requisition status changed from {OldStatus} to {NewStatus} by another user",
                                currentStatus, entity.Status);
                            throw new InvalidOperationException($"Requisition status has been changed to '{entity.Status}' by another user. Please refresh and try again.");
                        }
                        else if (entity.RowVersion != currentRowVersion)
                        {
                            _logger.LogWarning("RowVersion mismatch. Retrying with updated data...");
                            // ✅ Retry the operation with the refreshed entity
                            entry.CurrentValues.SetValues(entity);
                            return await Handle(request, cancellationToken);
                        }
                    }
                }

                // ✅ If we can't resolve, throw
                throw;
            }

            await _cache.RemoveAsync($"requisition_{requisition.Id}", cancellationToken);
            await _cache.RemoveAsync("requisitions_all", cancellationToken);

            // ✅ Reload to get the latest data
            await _context.Entry(requisition).ReloadAsync(cancellationToken);

            return MapToDto(requisition);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing requisition approval");
            throw;
        }
    }

    private RequisitionDto MapToDto(Requisition requisition)
    {
        return new RequisitionDto
        {
            Id = requisition.Id,
            RequisitionNumber = requisition.RequisitionNumber,
            Title = requisition.Title,
            Description = requisition.Description,
            DepartmentId = requisition.DepartmentId,
            DepartmentName = requisition.DepartmentName,
            RequesterId = requisition.RequesterId,
            RequesterName = requisition.RequesterName,
            RequiredDate = requisition.RequiredDate,
            SubmittedDate = requisition.SubmittedDate,
            Priority = requisition.Priority,
            Status = requisition.Status,
            TotalAmount = requisition.TotalAmount,
            BudgetCode = requisition.BudgetCode,
            PurchaseOrderId = requisition.PurchaseOrderId,
            PurchaseOrderNumber = requisition.PurchaseOrderNumber,
            RejectionReason = requisition.RejectionReason,
            DateAdd = requisition.DateAdd,
            DateMod = requisition.DateMod,
            RowVersion = requisition.RowVersion,
            Lines = requisition.Lines.Select(l => new RequisitionLineDto
            {
                Id = l.Id,
                Description = l.Description,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice,
                TotalAmount = l.TotalAmount,
                UnitOfMeasure = l.UnitOfMeasure,
                Notes = l.Notes
            }).ToList(),
            Approvals = requisition.Approvals.Select(a => new RequisitionApprovalDto
            {
                Id = a.Id,
                ApproverId = a.ApproverId,
                ApproverName = a.ApproverName,
                Status = a.Status,
                Comments = a.Comments,
                ApprovedAt = a.ApprovedAt,
                ApprovalLevel = a.ApprovalLevel
            }).ToList()
        };
    }
}