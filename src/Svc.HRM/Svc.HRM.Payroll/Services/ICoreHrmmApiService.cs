using Svc.HRM.Payroll.Models.Entities.Local;
using Svc.HRM.Payroll.Models.DTOs;
using Microsoft.Extensions.Logging;
namespace Svc.HRM.Payroll.Services;

public interface ICoreHrmmApiService
{
     Task<List<PositionDto>> GetAllPositionsAsync(CancellationToken ct = default);
         Task<List<JobGradeDto>> GetAllJobGradesAsync(CancellationToken ct = default);
}