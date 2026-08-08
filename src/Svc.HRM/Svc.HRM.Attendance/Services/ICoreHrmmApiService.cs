using Svc.HRM.Attendance.Models.DTOs;
using Svc.HRM.Attendance.Models.Entities.Local;
namespace Svc.HRM.Attendance.Services;

public interface ICoreHrmmApiService
{


    Task<List<PositionDto>> GetAllPositionsAsync(CancellationToken ct = default);
    Task<List<JobGradeDto>> GetAllJobGradesAsync(CancellationToken ct = default);

}