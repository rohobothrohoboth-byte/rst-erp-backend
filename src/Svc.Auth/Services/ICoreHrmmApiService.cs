using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Services;

public interface ICoreHrmmApiService
{
    Task<List<PositionDto>> GetAllPositionsAsync(CancellationToken ct = default);
    Task<List<JobGradeDto>> GetAllJobGradesAsync(CancellationToken ct = default);
    Task<PositionDto?> GetPositionAsync(Guid id, CancellationToken ct = default);
    Task<JobGradeDto?> GetJobGradeAsync(Guid id, CancellationToken ct = default);
}