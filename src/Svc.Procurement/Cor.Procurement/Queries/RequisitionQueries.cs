// Queries/RequisitionQueries.cs
using MediatR;
using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Cor.Procurement.Models.Entities;

namespace Cor.Procurement.Queries;

// ============================================================
// QUERIES
// ============================================================

public class GetAllRequisitionsQuery : IRequest<List<RequisitionDto>>
{
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public Guid? DepartmentId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? SearchTerm { get; set; }
}

public class GetPagedRequisitionsQuery : IRequest<PagedResult<RequisitionDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public Guid? DepartmentId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; } = "SubmittedDate";
    public string? SortOrder { get; set; } = "DESC";
}

public class GetRequisitionByIdQuery : IRequest<RequisitionDto?>
{
    public Guid Id { get; set; }
}

public class GetRequisitionsByStatusQuery : IRequest<List<RequisitionDto>>
{
    public string Status { get; set; } = string.Empty;
}

public class GetRequisitionsForApprovalQuery : IRequest<List<RequisitionDto>>
{
    public Guid ApproverId { get; set; }
}

public class GetRequisitionSummaryQuery : IRequest<RequisitionSummaryDto>
{
    public Guid? DepartmentId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

// ============================================================
// DTOs
// ============================================================

public class RequisitionSummaryDto
{
    public int TotalRequisitions { get; set; }
    public int DraftCount { get; set; }
    public int SubmittedCount { get; set; }
    public int UnderReviewCount { get; set; }
    public int ApprovedCount { get; set; }
    public int RejectedCount { get; set; }
    public int PurchasedCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AverageAmount { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}



// ============================================================
// HANDLERS
// ============================================================

// ==================== GET ALL REQUISITIONS ====================

public class GetAllRequisitionsHandler : IRequestHandler<GetAllRequisitionsQuery, List<RequisitionDto>>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetAllRequisitionsHandler> _logger;

    public GetAllRequisitionsHandler(ProcurementDbContext context, ILogger<GetAllRequisitionsHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<RequisitionDto>> Handle(GetAllRequisitionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.Requisitions
                .Include(r => r.Lines)
                .Where(r => !r.IsDeleted)
                .AsQueryable();

            // ✅ FIX: Handle multiple statuses with comma separation
            if (!string.IsNullOrEmpty(request.Status))
            {
                var statuses = request.Status.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToList();

                if (statuses.Count == 1)
                {
                    query = query.Where(r => r.Status == statuses[0]);
                }
                else if (statuses.Count > 1)
                {
                    query = query.Where(r => statuses.Contains(r.Status));
                }
            }

            if (!string.IsNullOrEmpty(request.Priority))
                query = query.Where(r => r.Priority == request.Priority);

            if (request.DepartmentId.HasValue)
                query = query.Where(r => r.DepartmentId == request.DepartmentId.Value);

            if (request.FromDate.HasValue)
                query = query.Where(r => r.RequiredDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(r => r.RequiredDate <= request.ToDate.Value);

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var search = request.SearchTerm.ToLower();
                query = query.Where(r =>
                    r.RequisitionNumber.ToLower().Contains(search) ||
                    r.Title.ToLower().Contains(search) ||
                    (r.Description != null && r.Description.ToLower().Contains(search))
                );
            }

            var requisitions = await query
                .OrderByDescending(r => r.SubmittedDate)
                .ToListAsync(cancellationToken);

            _logger.LogInformation($"Retrieved {requisitions.Count} requisitions");

            return requisitions.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all requisitions");
            throw;
        }
    }

    private RequisitionDto MapToDto(Requisition req)
    {
        return new RequisitionDto
        {
            Id = req.Id,
            RequisitionNumber = req.RequisitionNumber,
            Title = req.Title,
            Description = req.Description,
            DepartmentId = req.DepartmentId,
            DepartmentName = req.DepartmentName,
            RequesterId = req.RequesterId,
            RequesterName = req.RequesterName,
            RequiredDate = req.RequiredDate,
            SubmittedDate = req.SubmittedDate,
            Priority = req.Priority,
            Status = req.Status,
            TotalAmount = req.TotalAmount,
            BudgetCode = req.BudgetCode,
            PurchaseOrderId = req.PurchaseOrderId,
            PurchaseOrderNumber = req.PurchaseOrderNumber,
            RejectionReason = req.RejectionReason,
            DateAdd = req.DateAdd,
            DateMod = req.DateMod,
            RowVersion = req.RowVersion,
            Lines = req.Lines.Select(l => new RequisitionLineDto
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

// ==================== GET PAGED REQUISITIONS ====================

public class GetPagedRequisitionsHandler : IRequestHandler<GetPagedRequisitionsQuery, PagedResult<RequisitionDto>>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetPagedRequisitionsHandler> _logger;

    public GetPagedRequisitionsHandler(ProcurementDbContext context, ILogger<GetPagedRequisitionsHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<RequisitionDto>> Handle(GetPagedRequisitionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.Requisitions
                .Include(r => r.Lines)
                .Where(r => !r.IsDeleted)
                .AsQueryable();

            // ✅ FIX: Handle multiple statuses with comma separation
            if (!string.IsNullOrEmpty(request.Status))
            {
                var statuses = request.Status.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToList();

                if (statuses.Count == 1)
                {
                    query = query.Where(r => r.Status == statuses[0]);
                }
                else if (statuses.Count > 1)
                {
                    query = query.Where(r => statuses.Contains(r.Status));
                }
            }

            if (!string.IsNullOrEmpty(request.Priority))
                query = query.Where(r => r.Priority == request.Priority);

            if (request.DepartmentId.HasValue)
                query = query.Where(r => r.DepartmentId == request.DepartmentId.Value);

            if (request.FromDate.HasValue)
                query = query.Where(r => r.RequiredDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(r => r.RequiredDate <= request.ToDate.Value);

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var search = request.SearchTerm.ToLower();
                query = query.Where(r =>
                    r.RequisitionNumber.ToLower().Contains(search) ||
                    r.Title.ToLower().Contains(search) ||
                    (r.Description != null && r.Description.ToLower().Contains(search))
                );
            }

            var totalCount = await query.CountAsync(cancellationToken);

            // Apply sorting
            query = request.SortBy?.ToLower() switch
            {
                "requisitionnumber" => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(r => r.RequisitionNumber)
                    : query.OrderBy(r => r.RequisitionNumber),
                "title" => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(r => r.Title)
                    : query.OrderBy(r => r.Title),
                "status" => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(r => r.Status)
                    : query.OrderBy(r => r.Status),
                "totalamount" => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(r => r.TotalAmount)
                    : query.OrderBy(r => r.TotalAmount),
                "submitteddate" => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(r => r.SubmittedDate)
                    : query.OrderBy(r => r.SubmittedDate),
                _ => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(r => r.SubmittedDate)
                    : query.OrderBy(r => r.SubmittedDate)
            };

            var requisitions = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            _logger.LogInformation($"Retrieved {requisitions.Count} requisitions (Page {request.Page})");

            return new PagedResult<RequisitionDto>
            {
                Data = requisitions.Select(MapToDto).ToList(),
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged requisitions");
            throw;
        }
    }

    private RequisitionDto MapToDto(Requisition req)
    {
        return new RequisitionDto
        {
            Id = req.Id,
            RequisitionNumber = req.RequisitionNumber,
            Title = req.Title,
            Description = req.Description,
            DepartmentId = req.DepartmentId,
            DepartmentName = req.DepartmentName,
            RequesterId = req.RequesterId,
            RequesterName = req.RequesterName,
            RequiredDate = req.RequiredDate,
            SubmittedDate = req.SubmittedDate,
            Priority = req.Priority,
            Status = req.Status,
            TotalAmount = req.TotalAmount,
            BudgetCode = req.BudgetCode,
            PurchaseOrderId = req.PurchaseOrderId,
            PurchaseOrderNumber = req.PurchaseOrderNumber,
            RejectionReason = req.RejectionReason,
            DateAdd = req.DateAdd,
            DateMod = req.DateMod,
            RowVersion = req.RowVersion,
            Lines = req.Lines.Select(l => new RequisitionLineDto
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

// ==================== GET REQUISITION BY ID ====================

public class GetRequisitionByIdHandler : IRequestHandler<GetRequisitionByIdQuery, RequisitionDto?>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetRequisitionByIdHandler> _logger;

    public GetRequisitionByIdHandler(ProcurementDbContext context, ILogger<GetRequisitionByIdHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<RequisitionDto?> Handle(GetRequisitionByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var requisition = await _context.Requisitions
                .Include(r => r.Lines)
                .Include(r => r.Approvals)
                .FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, cancellationToken);

            if (requisition == null)
                return null;

            return MapToDto(requisition);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting requisition by ID: {Id}", request.Id);
            throw;
        }
    }

    private RequisitionDto MapToDto(Requisition req)
    {
        return new RequisitionDto
        {
            Id = req.Id,
            RequisitionNumber = req.RequisitionNumber,
            Title = req.Title,
            Description = req.Description,
            DepartmentId = req.DepartmentId,
            DepartmentName = req.DepartmentName,
            RequesterId = req.RequesterId,
            RequesterName = req.RequesterName,
            RequiredDate = req.RequiredDate,
            SubmittedDate = req.SubmittedDate,
            Priority = req.Priority,
            Status = req.Status,
            TotalAmount = req.TotalAmount,
            BudgetCode = req.BudgetCode,
            PurchaseOrderId = req.PurchaseOrderId,
            PurchaseOrderNumber = req.PurchaseOrderNumber,
            RejectionReason = req.RejectionReason,
            DateAdd = req.DateAdd,
            DateMod = req.DateMod,
            RowVersion = req.RowVersion,
            Lines = req.Lines.Select(l => new RequisitionLineDto
            {
                Id = l.Id,
                Description = l.Description,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice,
                TotalAmount = l.TotalAmount,
                UnitOfMeasure = l.UnitOfMeasure,
                Notes = l.Notes
            }).ToList(),
            Approvals = req.Approvals.Select(a => new RequisitionApprovalDto
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

// ==================== GET REQUISITIONS BY STATUS ====================

public class GetRequisitionsByStatusHandler : IRequestHandler<GetRequisitionsByStatusQuery, List<RequisitionDto>>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetRequisitionsByStatusHandler> _logger;

    public GetRequisitionsByStatusHandler(ProcurementDbContext context, ILogger<GetRequisitionsByStatusHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<RequisitionDto>> Handle(GetRequisitionsByStatusQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var requisitions = await _context.Requisitions
                .Include(r => r.Lines)
                .Where(r => r.Status == request.Status && !r.IsDeleted)
                .OrderByDescending(r => r.SubmittedDate)
                .ToListAsync(cancellationToken);

            return requisitions.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting requisitions by status: {Status}", request.Status);
            throw;
        }
    }

    private RequisitionDto MapToDto(Requisition req)
    {
        return new RequisitionDto
        {
            Id = req.Id,
            RequisitionNumber = req.RequisitionNumber,
            Title = req.Title,
            Description = req.Description,
            DepartmentId = req.DepartmentId,
            DepartmentName = req.DepartmentName,
            RequesterId = req.RequesterId,
            RequesterName = req.RequesterName,
            RequiredDate = req.RequiredDate,
            SubmittedDate = req.SubmittedDate,
            Priority = req.Priority,
            Status = req.Status,
            TotalAmount = req.TotalAmount,
            BudgetCode = req.BudgetCode,
            PurchaseOrderId = req.PurchaseOrderId,
            PurchaseOrderNumber = req.PurchaseOrderNumber,
            RejectionReason = req.RejectionReason,
            DateAdd = req.DateAdd,
            DateMod = req.DateMod,
            RowVersion = req.RowVersion,
            Lines = req.Lines.Select(l => new RequisitionLineDto
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

// ==================== GET REQUISITIONS FOR APPROVAL ====================

public class GetRequisitionsForApprovalHandler : IRequestHandler<GetRequisitionsForApprovalQuery, List<RequisitionDto>>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetRequisitionsForApprovalHandler> _logger;

    public GetRequisitionsForApprovalHandler(ProcurementDbContext context, ILogger<GetRequisitionsForApprovalHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<RequisitionDto>> Handle(GetRequisitionsForApprovalQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var requisitions = await _context.Requisitions
                .Include(r => r.Lines)
                .Include(r => r.Approvals)
                .Where(r => (r.Status == "Submitted" || r.Status == "UnderReview") && !r.IsDeleted)
                .OrderByDescending(r => r.SubmittedDate)
                .ToListAsync(cancellationToken);

            return requisitions.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting requisitions for approval");
            throw;
        }
    }

    private RequisitionDto MapToDto(Requisition req)
    {
        return new RequisitionDto
        {
            Id = req.Id,
            RequisitionNumber = req.RequisitionNumber,
            Title = req.Title,
            Description = req.Description,
            DepartmentId = req.DepartmentId,
            DepartmentName = req.DepartmentName,
            RequesterId = req.RequesterId,
            RequesterName = req.RequesterName,
            RequiredDate = req.RequiredDate,
            SubmittedDate = req.SubmittedDate,
            Priority = req.Priority,
            Status = req.Status,
            TotalAmount = req.TotalAmount,
            BudgetCode = req.BudgetCode,
            PurchaseOrderId = req.PurchaseOrderId,
            PurchaseOrderNumber = req.PurchaseOrderNumber,
            RejectionReason = req.RejectionReason,
            DateAdd = req.DateAdd,
            DateMod = req.DateMod,
            RowVersion = req.RowVersion,
            Lines = req.Lines.Select(l => new RequisitionLineDto
            {
                Id = l.Id,
                Description = l.Description,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice,
                TotalAmount = l.TotalAmount,
                UnitOfMeasure = l.UnitOfMeasure,
                Notes = l.Notes
            }).ToList(),
            Approvals = req.Approvals.Select(a => new RequisitionApprovalDto
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

// ==================== GET REQUISITION SUMMARY ====================

public class GetRequisitionSummaryHandler : IRequestHandler<GetRequisitionSummaryQuery, RequisitionSummaryDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetRequisitionSummaryHandler> _logger;

    public GetRequisitionSummaryHandler(ProcurementDbContext context, ILogger<GetRequisitionSummaryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<RequisitionSummaryDto> Handle(GetRequisitionSummaryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.Requisitions
                .Where(r => !r.IsDeleted)
                .AsQueryable();

            if (request.DepartmentId.HasValue)
                query = query.Where(r => r.DepartmentId == request.DepartmentId.Value);

            if (request.FromDate.HasValue)
                query = query.Where(r => r.SubmittedDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(r => r.SubmittedDate <= request.ToDate.Value);

            var requisitions = await query.ToListAsync(cancellationToken);

            var total = requisitions.Count;
            var totalAmount = requisitions.Sum(r => r.TotalAmount);

            return new RequisitionSummaryDto
            {
                TotalRequisitions = total,
                DraftCount = requisitions.Count(r => r.Status == "Draft"),
                SubmittedCount = requisitions.Count(r => r.Status == "Submitted"),
                UnderReviewCount = requisitions.Count(r => r.Status == "UnderReview"),
                ApprovedCount = requisitions.Count(r => r.Status == "Approved"),
                RejectedCount = requisitions.Count(r => r.Status == "Rejected"),
                PurchasedCount = requisitions.Count(r => r.Status == "Purchased"),
                TotalAmount = totalAmount,
                AverageAmount = total > 0 ? totalAmount / total : 0,
                FromDate = request.FromDate,
                ToDate = request.ToDate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting requisition summary");
            throw;
        }
    }
}