using Cor.CRM.Models.Entities.Local;
using Cor.CRM.Models.DTOs;
namespace Cor.CRM.Services;

public interface ICoreHrmmApiService
{


    Task<List<PositionDto>> GetAllPositionsAsync(CancellationToken ct = default);
    Task<List<JobGradeDto>> GetAllJobGradesAsync(CancellationToken ct = default);

}