using Profile.Domain.Entities.Local;

namespace Profile.App.Services;

public interface ICoreHrmmApiService
{
    Task<List<LocalPosition>> GetAllPositionsAsync(CancellationToken ct = default);
    Task<List<LocalJobGrade>> GetAllJobGradesAsync(CancellationToken ct = default);
    Task<List<LocalJgStep>> GetAllJgStepsAsync(CancellationToken ct = default);
}