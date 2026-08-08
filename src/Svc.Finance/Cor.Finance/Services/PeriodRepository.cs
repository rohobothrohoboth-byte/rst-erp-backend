// Repositories/PeriodRepository.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using  Cor.Finance.Persistence;
using  Cor.Finance.Models.DTOs;
using  Cor.Finance.Models.Enums;
using  Cor.Finance.Models.Entities;
using  Cor.Finance.Services;

namespace Cor.Finance.Services;

    public class PeriodRepository : IPeriodRepository
    {
        private readonly FinanceDbContext _context;

        public PeriodRepository(FinanceDbContext context)
        {
            _context = context;
        }

      public async Task<FinancialPeriod?> GetByIdAsync(Guid id)
      {
          return await _context.FinancialPeriods
              .FirstOrDefaultAsync(p => p.Id == id);
      }

        public async Task<(IEnumerable<FinancialPeriod> periods, int total)> GetFilteredAsync(PeriodFilterDto filter)
        {
            var query = _context.FinancialPeriods.AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(p => p.Name.Contains(filter.Search));
            }

            if (filter.IsClosed.HasValue)
            {
                query = query.Where(p => p.IsClosed == filter.IsClosed.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                if (Enum.TryParse<PeriodStatus>(filter.Status, true, out var status))
                {
                    query = query.Where(p => p.Status == status);
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.PeriodType))
            {
                if (Enum.TryParse<PeriodType>(filter.PeriodType, true, out var periodType))
                {
                    query = query.Where(p => p.PeriodType == periodType);
                }
            }

            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                query = query.Where(p => p.StartDate >= filter.StartDate.Value && p.EndDate <= filter.EndDate.Value);
            }

            // Get total count
            var total = await query.CountAsync();

            // Apply sorting
            var sortBy = filter.SortBy ?? "CreatedAt";
            var sortOrder = filter.SortOrder ?? "DESC";

            query = sortOrder.ToUpper() == "DESC"
                ? query.OrderByDescending(p => EF.Property<object>(p, sortBy))
                : query.OrderBy(p => EF.Property<object>(p, sortBy));

            // Apply pagination
            var skip = (filter.Page - 1) * filter.Limit;
            var periods = await query
                .Skip(skip)
                .Take(filter.Limit)
                .ToListAsync();

            return (periods, total);
        }

        public async Task<FinancialPeriod> CreateAsync(FinancialPeriod period)
        {
            period.DateAdd = DateTime.UtcNow;
            period.DateMod = DateTime.UtcNow;
            period.GenerateNameIfNotProvided();
            period.ValidateDates();

            await _context.FinancialPeriods.AddAsync(period);
            await _context.SaveChangesAsync();
            return period;
        }

        public async Task<FinancialPeriod> UpdateAsync(FinancialPeriod period)
        {
            period.DateMod = DateTime.UtcNow;
            _context.FinancialPeriods.Update(period);
            await _context.SaveChangesAsync();
            return period;
        }

       public async Task DeleteAsync(Guid id)
       {
           var period = await GetByIdAsync(id);
           if (period != null)
           {
               _context.FinancialPeriods.Remove(period);
               await _context.SaveChangesAsync();
           }
       }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.FinancialPeriods.AnyAsync(p => p.Id == id);
        }

        public async Task<bool> HasOverlappingPeriodAsync(DateTime startDate, DateTime endDate, Guid? excludeId = null)
        {
            var query = _context.FinancialPeriods
                .Where(p => (p.StartDate <= endDate && p.EndDate >= startDate));

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<int> GetUnpostedEntriesCountAsync(Guid periodId)
        {
            return await _context.JournalEntries
                .CountAsync(e => e.PeriodId == periodId && !e.IsPosted);
        }

        public async Task<int> GetTotalEntriesCountAsync(Guid periodId)
        {
            return await _context.JournalEntries
                .CountAsync(e => e.PeriodId == periodId);
        }

        public async Task<bool> CanClosePeriodAsync(Guid periodId)
        {
            var period = await GetByIdAsync(periodId);
            if (period == null || period.IsClosed)
                return false;

            var unpostedCount = await GetUnpostedEntriesCountAsync(periodId);
            return unpostedCount == 0;
        }
    }
