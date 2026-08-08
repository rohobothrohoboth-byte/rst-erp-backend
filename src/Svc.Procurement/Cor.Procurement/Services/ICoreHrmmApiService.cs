using Cor.Procurement.Models.Entities.Local;
using Cor.Procurement.Models.DTOs;

namespace Cor.Procurement.Services;

public interface ICoreHrmmApiService
{


    Task<List<PositionDto>> GetAllPositionsAsync(CancellationToken ct = default);
    Task<List<JobGradeDto>> GetAllJobGradesAsync(CancellationToken ct = default);

}