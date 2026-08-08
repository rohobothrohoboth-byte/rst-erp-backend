using Leave.Domain.Entities.Local;
using Leave.Domain.DTOs;
namespace Leave.App.Services;

public interface ICoreHrmmApiService
{
    Task<List<LocalPosition>> GetAllPositionsAsync(CancellationToken ct = default);
    Task<List<LocalJobGrade>> GetAllJobGradesAsync(CancellationToken ct = default);
    Task<List<LocalJgStep>> GetAllJgStepsAsync(CancellationToken ct = default);
    Task<List<PositionReqDto>> GetAllPositionRequirementsAsync(CancellationToken ct = default);
}