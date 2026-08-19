using Cor.Inventory.Models.DTOs;
using Cor.Inventory.Models.Entities;
using Cor.Inventory.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cor.Inventory.Services;

public class MaterialService : IMaterialService
{
    private readonly InventoryDbContext _context;
    private readonly ILogger<MaterialService> _logger;

    public MaterialService(InventoryDbContext context, ILogger<MaterialService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // ============================================================
    // REQUESTS
    // ============================================================

    public async Task<MaterialRequestDto> CreateRequest(Guid employeeId, string? employeeName, CreateMaterialRequestDto dto, CancellationToken ct = default)
    {
        var request = new MaterialRequest
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            EmployeeName = employeeName,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            Reason = dto.Reason,
            Status = "Pending",
            DateAdd = DateTime.UtcNow
        };
        request.UpdateRowVersion();

        await _context.MaterialRequests.AddAsync(request, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Created material request {RequestId} for employee {EmployeeId}", request.Id, employeeId);
        return MapToDto(request);
    }

    public async Task<List<MaterialRequestDto>> GetMyRequests(Guid employeeId, CancellationToken ct = default)
    {
        return await _context.MaterialRequests
            .Where(r => r.EmployeeId == employeeId)
            .OrderByDescending(r => r.DateAdd)
            .Select(r => MapToDto(r))
            .ToListAsync(ct);
    }

    public async Task<List<MaterialRequestDto>> GetAllRequests(CancellationToken ct = default)
    {
        return await _context.MaterialRequests
            .OrderByDescending(r => r.DateAdd)
            .Select(r => MapToDto(r))
            .ToListAsync(ct);
    }

    public async Task<MaterialRequestDto> ApproveRequest(Guid id, Guid? decidedByUserId, string? decidedByName, MaterialRequestDecisionDto dto, CancellationToken ct = default)
    {
        var request = await GetRequestForDecision(id, ct);

        request.Status = "Approved";
        ApplyDecision(request, decidedByUserId, decidedByName, dto);

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Approved material request {RequestId}", id);
        return MapToDto(request);
    }

    public async Task<MaterialRequestDto> RejectRequest(Guid id, Guid? decidedByUserId, string? decidedByName, MaterialRequestDecisionDto dto, CancellationToken ct = default)
    {
        var request = await GetRequestForDecision(id, ct);

        request.Status = "Rejected";
        ApplyDecision(request, decidedByUserId, decidedByName, dto);

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Rejected material request {RequestId}", id);
        return MapToDto(request);
    }

    public async Task<MaterialAssignmentDto> IssueRequest(Guid id, Guid? decidedByUserId, string? decidedByName, MaterialRequestDecisionDto dto, CancellationToken ct = default)
    {
        var request = await GetRequestForDecision(id, ct);

        request.Status = "Issued";
        ApplyDecision(request, decidedByUserId, decidedByName, dto);

        var assignment = new MaterialAssignment
        {
            Id = Guid.NewGuid(),
            EmployeeId = request.EmployeeId,
            EmployeeName = request.EmployeeName,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            IssuedDate = DateTime.UtcNow,
            Status = "Issued",
            RequestId = request.Id,
            Note = dto.Note,
            DateAdd = DateTime.UtcNow
        };
        assignment.UpdateRowVersion();

        await _context.MaterialAssignments.AddAsync(assignment, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Issued material request {RequestId} as assignment {AssignmentId}", id, assignment.Id);
        return MapToDto(assignment);
    }

    // ============================================================
    // ASSIGNMENTS
    // ============================================================

    public async Task<List<MaterialAssignmentDto>> GetMyAssignments(Guid employeeId, CancellationToken ct = default)
    {
        return await _context.MaterialAssignments
            .Where(a => a.EmployeeId == employeeId)
            .OrderByDescending(a => a.IssuedDate)
            .Select(a => MapToDto(a))
            .ToListAsync(ct);
    }

    public async Task<List<MaterialAssignmentDto>> GetAllAssignments(CancellationToken ct = default)
    {
        return await _context.MaterialAssignments
            .OrderByDescending(a => a.IssuedDate)
            .Select(a => MapToDto(a))
            .ToListAsync(ct);
    }

    public async Task<MaterialAssignmentDto> CreateAssignment(CreateMaterialAssignmentDto dto, CancellationToken ct = default)
    {
        var assignment = new MaterialAssignment
        {
            Id = Guid.NewGuid(),
            EmployeeId = dto.EmployeeId,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            IssuedDate = DateTime.UtcNow,
            Status = "Issued",
            Note = dto.Note,
            DateAdd = DateTime.UtcNow
        };
        assignment.UpdateRowVersion();

        await _context.MaterialAssignments.AddAsync(assignment, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Created material assignment {AssignmentId} for employee {EmployeeId}", assignment.Id, dto.EmployeeId);
        return MapToDto(assignment);
    }

    public async Task<MaterialAssignmentDto> ReturnAssignment(Guid id, CancellationToken ct = default)
    {
        var assignment = await _context.MaterialAssignments.FirstOrDefaultAsync(a => a.Id == id, ct);
        if (assignment == null)
            throw new KeyNotFoundException($"Material assignment with ID '{id}' not found");

        assignment.Status = "Returned";
        assignment.ReturnedDate = DateTime.UtcNow;
        assignment.DateMod = DateTime.UtcNow;
        assignment.UpdateRowVersion();

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Returned material assignment {AssignmentId}", id);
        return MapToDto(assignment);
    }

    // ============================================================
    // HELPERS
    // ============================================================

    private async Task<MaterialRequest> GetRequestForDecision(Guid id, CancellationToken ct)
    {
        var request = await _context.MaterialRequests.FirstOrDefaultAsync(r => r.Id == id, ct);
        if (request == null)
            throw new KeyNotFoundException($"Material request with ID '{id}' not found");
        return request;
    }

    private static void ApplyDecision(MaterialRequest request, Guid? decidedByUserId, string? decidedByName, MaterialRequestDecisionDto dto)
    {
        request.DecidedByUserId = decidedByUserId;
        request.DecidedByName = decidedByName;
        request.DecisionNote = dto.Note;
        request.DecisionDate = DateTime.UtcNow;
        request.DateMod = DateTime.UtcNow;
        request.UpdateRowVersion();
    }

    private static MaterialRequestDto MapToDto(MaterialRequest r) => new()
    {
        Id = r.Id,
        EmployeeId = r.EmployeeId,
        EmployeeName = r.EmployeeName,
        ProductId = r.ProductId,
        Quantity = r.Quantity,
        Reason = r.Reason,
        Status = r.Status,
        DecidedByUserId = r.DecidedByUserId,
        DecidedByName = r.DecidedByName,
        DecisionNote = r.DecisionNote,
        DecisionDate = r.DecisionDate,
        DateAdd = r.DateAdd,
        DateMod = r.DateMod
    };

    private static MaterialAssignmentDto MapToDto(MaterialAssignment a) => new()
    {
        Id = a.Id,
        EmployeeId = a.EmployeeId,
        EmployeeName = a.EmployeeName,
        ProductId = a.ProductId,
        Quantity = a.Quantity,
        IssuedDate = a.IssuedDate,
        Status = a.Status,
        ReturnedDate = a.ReturnedDate,
        RequestId = a.RequestId,
        Note = a.Note,
        DateAdd = a.DateAdd,
        DateMod = a.DateMod
    };
}
