// Services/Interfaces/IPeriodClosingService.cs
using System;
using System.Threading.Tasks;
using  Cor.Finance.Persistence;
using  Cor.Finance.Models.DTOs;
using  Cor.Finance.Models.Enums;
using  Cor.Finance.Models.Entities;
using  Cor.Finance.Services;

 namespace Cor.Finance.Services;


    public interface IPeriodClosingService
    {
        Task<PeriodResponseDto> CreatePeriodAsync(CreatePeriodDto dto, Guid userId);
        Task<PeriodResponseDto> UpdatePeriodAsync(Guid id, UpdatePeriodDto dto, Guid userId);
        Task<PeriodResponseDto> ClosePeriodAsync(Guid id, ClosePeriodDto dto, Guid userId);
        Task<PeriodResponseDto> OpenPeriodAsync(Guid id, Guid userId);
        Task DeletePeriodAsync(Guid id, Guid userId);
        Task<PeriodResponseDto> GetPeriodByIdAsync(Guid id);
        Task<(IEnumerable<PeriodResponseDto> periods, int total, int page, int totalPages)>
            GetPeriodsAsync(PeriodFilterDto filter);
        Task<PeriodStatsDto> GetPeriodStatsAsync(Guid periodId);
        Task<(bool canClose, string reason)> ValidateClosingAsync(Guid periodId);
        Task<(IEnumerable<AuditLog> logs, int total)> GetAuditTrailAsync(Guid periodId, int page, int limit);
        Task<object> ExportPeriodDataAsync(Guid periodId, Guid userId);
    }
