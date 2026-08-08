using Cor.Finance.Models.Entities.Local;
using Cor.Finance.Models.DTOs;
namespace Cor.Finance.Services;

public interface ICoreHrmmApiService
{


    Task<List<PositionDto>> GetAllPositionsAsync(CancellationToken ct = default);
    Task<List<JobGradeDto>> GetAllJobGradesAsync(CancellationToken ct = default);

}