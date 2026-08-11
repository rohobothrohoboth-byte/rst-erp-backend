using Cor.Inventory.Models.DTOs;

namespace Cor.Inventory.Services;

public interface IMaterialService
{
    // Requests
    Task<MaterialRequestDto> CreateRequest(Guid employeeId, string? employeeName, CreateMaterialRequestDto dto, CancellationToken ct = default);
    Task<List<MaterialRequestDto>> GetMyRequests(Guid employeeId, CancellationToken ct = default);
    Task<List<MaterialRequestDto>> GetAllRequests(CancellationToken ct = default);
    Task<MaterialRequestDto> ApproveRequest(Guid id, Guid? decidedByUserId, string? decidedByName, MaterialRequestDecisionDto dto, CancellationToken ct = default);
    Task<MaterialRequestDto> RejectRequest(Guid id, Guid? decidedByUserId, string? decidedByName, MaterialRequestDecisionDto dto, CancellationToken ct = default);
    Task<MaterialAssignmentDto> IssueRequest(Guid id, Guid? decidedByUserId, string? decidedByName, MaterialRequestDecisionDto dto, CancellationToken ct = default);

    // Assignments
    Task<List<MaterialAssignmentDto>> GetMyAssignments(Guid employeeId, CancellationToken ct = default);
    Task<List<MaterialAssignmentDto>> GetAllAssignments(CancellationToken ct = default);
    Task<MaterialAssignmentDto> CreateAssignment(CreateMaterialAssignmentDto dto, CancellationToken ct = default);
    Task<MaterialAssignmentDto> ReturnAssignment(Guid id, CancellationToken ct = default);
}
