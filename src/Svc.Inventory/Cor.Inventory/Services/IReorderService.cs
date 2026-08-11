using Cor.Inventory.Models.DTOs;

namespace Cor.Inventory.Services;

public interface IReorderService
{
    Task<List<ReorderRuleDto>> GetRulesAsync(CancellationToken ct = default);
    Task<ReorderRuleDto> CreateRuleAsync(CreateReorderRuleDto dto, CancellationToken ct = default);
    Task<ReorderRuleDto> UpdateRuleAsync(UpdateReorderRuleDto dto, CancellationToken ct = default);

    Task<List<ReorderAlertDto>> GetAlertsAsync(CancellationToken ct = default);

    Task<List<ReorderRequestDto>> GetRequestsAsync(string? status, CancellationToken ct = default);
    Task<ReorderRequestDto> CreateRequestAsync(CreateReorderRequestDto dto, CancellationToken ct = default);
    Task<ReorderRequestDto> ApproveRequestAsync(Guid id, ReorderDecisionDto dto, CancellationToken ct = default);
    Task<ReorderRequestDto> RejectRequestAsync(Guid id, ReorderDecisionDto dto, CancellationToken ct = default);
    Task<ReorderRequestDto> ConvertRequestAsync(Guid id, ReorderDecisionDto dto, CancellationToken ct = default);
}
