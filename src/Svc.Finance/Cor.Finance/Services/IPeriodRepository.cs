// Repositories/Interfaces/IPeriodRepository.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using  Cor.Finance.Persistence;
using  Cor.Finance.Models.DTOs;
using  Cor.Finance.Models.Enums;
using  Cor.Finance.Models.Entities;
using  Cor.Finance.Services;

 namespace Cor.Finance.Services;


    public interface IPeriodRepository
    {
        Task<FinancialPeriod?> GetByIdAsync(Guid id);
        Task<(IEnumerable<FinancialPeriod> periods, int total)> GetFilteredAsync(PeriodFilterDto filter);
        Task<FinancialPeriod> CreateAsync(FinancialPeriod period);
        Task<FinancialPeriod> UpdateAsync(FinancialPeriod period);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> HasOverlappingPeriodAsync(DateTime startDate, DateTime endDate, Guid? excludeId = null);
        Task<int> GetUnpostedEntriesCountAsync(Guid periodId);
        Task<int> GetTotalEntriesCountAsync(Guid periodId);
        Task<bool> CanClosePeriodAsync(Guid periodId);
    }
