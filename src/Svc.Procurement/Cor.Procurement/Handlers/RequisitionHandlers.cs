// Handlers/RequisitionHandlers.cs
using MediatR;
using Cor.Procurement.Commands;
using Cor.Procurement.Queries;
using Cor.Procurement.Models.Entities;
using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cor.Procurement.Handlers;

public class CreateRequisitionHandler : IRequestHandler<CreateRequisitionCommand, RequisitionDto>
{
    private readonly ProcurementDbContext _context;

    public CreateRequisitionHandler(ProcurementDbContext context)
    {
        _context = context;
    }

    public async Task<RequisitionDto> Handle(CreateRequisitionCommand request, CancellationToken ct)
    {
        // Generate requisition number
        var number = await GenerateRequisitionNumber(ct);

        var requisition = new Requisition
        {
            Id = Guid.NewGuid(),
            RequisitionNumber = number,
            Title = request.CreateDto.Title,
            Description = request.CreateDto.Description,
            DepartmentId = request.CreateDto.DepartmentId,
            DepartmentName = request.CreateDto.DepartmentName,
            RequesterId = request.CreateDto.RequesterId,
            RequesterName = request.CreateDto.RequesterName,
            RequiredDate = request.CreateDto.RequiredDate,
            Priority = request.CreateDto.Priority,
            BudgetCode = request.CreateDto.BudgetCode,
            Status = "Draft",
            SubmittedDate = DateTime.UtcNow,
            DateAdd = DateTime.UtcNow,
            IsDeleted = false,
            RowVersion = Guid.NewGuid().ToString("N")
        };

        // Calculate total amount
        decimal totalAmount = 0;
        foreach (var lineDto in request.CreateDto.Lines)
        {
            var line = new RequisitionLine
            {
                Id = Guid.NewGuid(),
                RequisitionId = requisition.Id,
                Description = lineDto.Description,
                Quantity = lineDto.Quantity,
                UnitPrice = lineDto.UnitPrice,
                TotalAmount = lineDto.Quantity * lineDto.UnitPrice,
                UnitOfMeasure = lineDto.UnitOfMeasure,
                Notes = lineDto.Notes,
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            };
            totalAmount += line.TotalAmount;
            requisition.Lines.Add(line);
        }
        requisition.TotalAmount = totalAmount;

        _context.Requisitions.Add(requisition);
        await _context.SaveChangesAsync(ct);

        return MapToDto(requisition);
    }

    private async Task<string> GenerateRequisitionNumber(CancellationToken ct)
    {
        var last = await _context.Requisitions
            .OrderByDescending(r => r.RequisitionNumber)
            .FirstOrDefaultAsync(ct);

        if (last == null)
            return "REQ-0001";

        var parts = last.RequisitionNumber.Split('-');
        if (parts.Length == 2 && int.TryParse(parts[1], out int number))
        {
            return $"REQ-{(number + 1):D4}";
        }
        return "REQ-0001";
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
            Lines = requisition.Lines.Where(l => !l.IsDeleted).Select(l => new RequisitionLineDto
            {
                Id = l.Id,
                Description = l.Description,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice,
                TotalAmount = l.TotalAmount,
                UnitOfMeasure = l.UnitOfMeasure,
                Notes = l.Notes
            }).ToList(),
            DateAdd = requisition.DateAdd,
            DateMod = requisition.DateMod,
            RowVersion = requisition.RowVersion
        };
    }
}