using MediatR;
using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Models.Entities;
using Cor.Procurement.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;
using System.Text.Json;

namespace Cor.Procurement.Commands;

public class CompleteInspectionCommandHandler
    : IRequestHandler<CompleteInspectionCommand, InspectionDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<CompleteInspectionCommandHandler> _logger;
    private readonly ICacheService _cache;

    public CompleteInspectionCommandHandler(
        ProcurementDbContext context,
        ILogger<CompleteInspectionCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

   public async Task<InspectionDto> Handle(CompleteInspectionCommand request, CancellationToken cancellationToken)
   {
       try
       {
           _logger.LogInformation("Completing inspection for GRN: {GrnId}", request.CompleteDto.GrnId);

           // Get or create inspection
           var inspection = await _context.Inspections
               .Include(i => i.Items)
               .FirstOrDefaultAsync(i => i.GoodsReceiptNoteId == request.CompleteDto.GrnId && !i.IsDeleted, cancellationToken);

           if (inspection == null)
           {
               // Create new inspection
               inspection = new Inspection
               {
                   Id = Guid.NewGuid(),
                   GoodsReceiptNoteId = request.CompleteDto.GrnId,
                   InspectionNumber = GenerateInspectionNumber(),
                   // ✅ Ensure UTC
                   InspectionDate = request.CompleteDto.InspectionDate.Kind == DateTimeKind.Utc
                       ? request.CompleteDto.InspectionDate
                       : DateTime.SpecifyKind(request.CompleteDto.InspectionDate, DateTimeKind.Utc),
                   Status = "InProgress",
                   Department = request.CompleteDto.Team.Department,
                   DateAdd = DateTime.UtcNow,
                   IsDeleted = false
               };

               await _context.Inspections.AddAsync(inspection, cancellationToken);
           }

           // Update inspection details
           // ✅ Ensure UTC
           inspection.InspectionDate = request.CompleteDto.InspectionDate.Kind == DateTimeKind.Utc
               ? request.CompleteDto.InspectionDate
               : DateTime.SpecifyKind(request.CompleteDto.InspectionDate, DateTimeKind.Utc);
           inspection.Remarks = request.CompleteDto.Remarks;
           inspection.QualityScore = request.CompleteDto.QualityScore;
           inspection.DateMod = DateTime.UtcNow;

           // Update team members
           var teamJson = JsonSerializer.Serialize(request.CompleteDto.Team);
           inspection.TeamMembersJson = teamJson;

           // Get the team leader
           var teamLeader = request.CompleteDto.Team.Members.FirstOrDefault(m => m.Role == "Team Leader");
           if (teamLeader != null)
           {
               inspection.InspectorId = teamLeader.Id;
               inspection.InspectorName = teamLeader.Name;
           }

           // Update items
           foreach (var itemDto in request.CompleteDto.Items)
           {
               var existingItem = inspection.Items.FirstOrDefault(i => i.PurchaseOrderItemId == itemDto.PurchaseOrderItemId);

               if (existingItem != null)
               {
                   // Update existing item
                   existingItem.QuantityAccepted = itemDto.QuantityAccepted;
                   existingItem.QuantityRejected = itemDto.QuantityRejected;
                   existingItem.Condition = itemDto.Condition;
                   existingItem.RejectionReason = itemDto.RejectionReason;
                   existingItem.InspectedBy = itemDto.InspectedBy;
                   existingItem.Status = itemDto.QuantityRejected > 0 ? "Failed" : "Passed";
                   existingItem.DateMod = DateTime.UtcNow;
               }
               else
               {
                   // Add new item
                   var newItem = new InspectionItem
                   {
                       Id = Guid.NewGuid(),
                       InspectionId = inspection.Id,
                       PurchaseOrderItemId = itemDto.PurchaseOrderItemId,
                       QuantityAccepted = itemDto.QuantityAccepted,
                       QuantityRejected = itemDto.QuantityRejected,
                       Condition = itemDto.Condition,
                       RejectionReason = itemDto.RejectionReason,
                       InspectedBy = itemDto.InspectedBy,
                       Status = itemDto.QuantityRejected > 0 ? "Failed" : "Passed",
                       DateAdd = DateTime.UtcNow,
                       IsDeleted = false
                   };
                   inspection.Items.Add(newItem);
               }
           }

           // Calculate totals
           inspection.TotalItems = inspection.Items.Count;
           inspection.ItemsPassed = inspection.Items.Count(i => i.Status == "Passed");
           inspection.ItemsFailed = inspection.Items.Count(i => i.Status == "Failed");

           // Set status based on quality score
           inspection.Status = request.CompleteDto.QualityScore >= 60 ? "Completed" : "Failed";
           inspection.CompletedDate = DateTime.UtcNow;

           // Update GRN status
           var grn = await _context.GoodsReceiptNotes
               .FirstOrDefaultAsync(g => g.Id == request.CompleteDto.GrnId, cancellationToken);

           if (grn != null)
           {
               grn.Status = inspection.Status == "Completed" ? "Completed" : "Cancelled";
               grn.CompletedDate = DateTime.UtcNow;
               grn.DateMod = DateTime.UtcNow;
           }

           await _context.SaveChangesAsync(cancellationToken);

           // Invalidate cache
           await _cache.RemoveAsync($"inspections_grn_{request.CompleteDto.GrnId}", cancellationToken);
           await _cache.RemoveAsync("inspections_all", cancellationToken);
           await _cache.RemoveAsync($"grn_{request.CompleteDto.GrnId}", cancellationToken);

           _logger.LogInformation("Inspection completed successfully: {InspectionNumber}", inspection.InspectionNumber);

           return MapToDto(inspection);
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "Error completing inspection");
           throw;
       }
   }

    private string GenerateInspectionNumber()
    {
        var today = DateTime.UtcNow;
        return $"INS-{today:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
    }

    private InspectionDto MapToDto(Inspection inspection)
    {
        return new InspectionDto
        {
            Id = inspection.Id,
            InspectionNumber = inspection.InspectionNumber,
            GoodsReceiptNoteId = inspection.GoodsReceiptNoteId,
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