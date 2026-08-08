using MediatR;
using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Models.Entities;
using Cor.Procurement.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;
using System.Text.Json;

namespace Cor.Procurement.Queries;

public class GetAllInspectionsQueryHandler
    : IRequestHandler<GetAllInspectionsQuery, List<InspectionDto>>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetAllInspectionsQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetAllInspectionsQueryHandler(
        ProcurementDbContext context,
        ILogger<GetAllInspectionsQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<InspectionDto>> Handle(GetAllInspectionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.Inspections
                .Include(i => i.Items)
                .Include(i => i.GoodsReceiptNote)
                .Where(i => !i.IsDeleted);

            if (!string.IsNullOrEmpty(request.Status))
                query = query.Where(i => i.Status == request.Status);

            if (!string.IsNullOrEmpty(request.InspectorId))
                query = query.Where(i => i.InspectorId == request.InspectorId);

            if (request.GrnId.HasValue)
                query = query.Where(i => i.GoodsReceiptNoteId == request.GrnId.Value);

            if (request.FromDate.HasValue)
                query = query.Where(i => i.InspectionDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(i => i.InspectionDate <= request.ToDate.Value);

            var inspections = await query
                .OrderByDescending(i => i.InspectionDate)
                .Select(i => MapToDto(i))
                .ToListAsync(cancellationToken);

            return inspections;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching inspections");
            throw;
        }
    }

    private InspectionDto MapToDto(Inspection inspection)
    {
        return new InspectionDto
        {
            Id = inspection.Id,
            InspectionNumber = inspection.InspectionNumber,
            GoodsReceiptNoteId = inspection.GoodsReceiptNoteId,
            GrnNumber = inspection.GoodsReceiptNote != null ? inspection.GoodsReceiptNote.GrnNumber : null,
            InspectionDate = inspection.InspectionDate,
            InspectorId = inspection.InspectorId,
            InspectorName = inspection.InspectorName,
            Status = inspection.Status,
            Department = inspection.Department,
            Remarks = inspection.Remarks,
            QualityScore = inspection.QualityScore,
            CompletedDate = inspection.CompletedDate,
            TotalItems = inspection.TotalItems,
            ItemsPassed = inspection.ItemsPassed,
            ItemsFailed = inspection.ItemsFailed,
            Items = inspection.Items.Select(i => new InspectionItemDto
            {
                Id = i.Id,
                PurchaseOrderItemId = i.PurchaseOrderItemId,
                Description = i.Description,
                QuantityReceived = i.QuantityReceived,
                QuantityAccepted = i.QuantityAccepted,
                QuantityRejected = i.QuantityRejected,
                Condition = i.Condition,
                RejectionReason = i.RejectionReason,
                UnitPrice = i.UnitPrice,
                InspectedBy = i.InspectedBy,
                Status = i.Status
            }).ToList(),
            DateAdd = inspection.DateAdd,
            DateMod = inspection.DateMod
        };
    }
}