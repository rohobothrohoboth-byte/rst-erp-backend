using Svc.HRM.Reports.Models.DTOs;

namespace Svc.HRM.Reports.Services;

public interface IHrReportService
{
    Task<HrReportEnvelope> GetEmployeeReportAsync(CancellationToken ct = default);
    Task<HrReportEnvelope> GetAttendanceReportAsync(int? year, int? month, DateTime? date, CancellationToken ct = default);
    Task<HrReportEnvelope> GetLeaveReportAsync(CancellationToken ct = default);
    Task<HrReportEnvelope> GetPayrollReportAsync(CancellationToken ct = default);
    Task<HrReportEnvelope> GetRecruitmentReportAsync(CancellationToken ct = default);
    Task<HrReportsSummaryDto> GetSummaryAsync(CancellationToken ct = default);
}
