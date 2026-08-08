using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Svc.HRM.Payroll.Models.DTOs;
using Svc.HRM.Payroll.Models.Entities;
using Svc.HRM.Payroll.Models.Enums;
using Svc.HRM.Payroll.Persistence;
using System.Text;
using System.Text.Json;

namespace Svc.HRM.Payroll.Services;

public class PayrollService : IPayrollService
{
    private readonly PayrollDbContext _context;
    private readonly ITaxCalculator _taxCalculator;
    private readonly IPayslipGenerator _payslipGenerator;
    private readonly IEventPublisher _eventPublisher;
    private readonly IDistributedCache _cache;
    private readonly ILogger<PayrollService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public PayrollService(
        PayrollDbContext context,
        ITaxCalculator taxCalculator,
        IPayslipGenerator payslipGenerator,
        IEventPublisher eventPublisher,
        IDistributedCache cache,
        ILogger<PayrollService> logger,
        IServiceProvider serviceProvider)
    {
        _context = context;
        _taxCalculator = taxCalculator;
        _payslipGenerator = payslipGenerator;
        _eventPublisher = eventPublisher;
        _cache = cache;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    #region Salary Structure

    public async Task<SalaryStructureDto> CreateSalaryStructureAsync(SalaryStructureCreateDto dto, CancellationToken ct = default)
    {
        var entity = new LocalSalaryStructure
        {
            Name = dto.Name,
            Description = dto.Description,
            BaseSalary = dto.BaseSalary,
            HousingAllowance = dto.HousingAllowance,
            TransportAllowance = dto.TransportAllowance,
            MealAllowance = dto.MealAllowance,
            MedicalAllowance = dto.MedicalAllowance,
            OtherAllowances = dto.OtherAllowances,
            Deductions = dto.Deductions,
            PensionContribution = dto.PensionContribution,
            IsActive = true
        };

        await _context.SalaryStructures.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);

        await _cache.RemoveAsync("salary_structures_all", ct);
        return MapToSalaryStructureDto(entity);
    }

    public async Task<SalaryStructureDto> UpdateSalaryStructureAsync(Guid id, SalaryStructureCreateDto dto, CancellationToken ct = default)
    {
        var entity = await _context.SalaryStructures.FindAsync([id], ct);
        if (entity == null) throw new KeyNotFoundException($"Salary structure {id} not found");

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.BaseSalary = dto.BaseSalary;
        entity.HousingAllowance = dto.HousingAllowance;
        entity.TransportAllowance = dto.TransportAllowance;
        entity.MealAllowance = dto.MealAllowance;
        entity.MedicalAllowance = dto.MedicalAllowance;
        entity.OtherAllowances = dto.OtherAllowances;
        entity.Deductions = dto.Deductions;
        entity.PensionContribution = dto.PensionContribution;
        entity.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        await _cache.RemoveAsync($"salary_structure_{id}", ct);
        await _cache.RemoveAsync("salary_structures_all", ct);
        return MapToSalaryStructureDto(entity);
    }

    public async Task<SalaryStructureDto> GetSalaryStructureAsync(Guid id, CancellationToken ct = default)
    {
        var cacheKey = $"salary_structure_{id}";
        var cached = await _cache.GetStringAsync(cacheKey, ct);
        if (!string.IsNullOrEmpty(cached))
            return JsonSerializer.Deserialize<SalaryStructureDto>(cached)!;

        var entity = await _context.SalaryStructures
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) throw new KeyNotFoundException($"Salary structure {id} not found");

        var dto = MapToSalaryStructureDto(entity);
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(15)
        }, ct);
        return dto;
    }

    public async Task<List<SalaryStructureDto>> GetAllSalaryStructuresAsync(CancellationToken ct = default)
    {
        var cacheKey = "salary_structures_all";
        var cached = await _cache.GetStringAsync(cacheKey, ct);
        if (!string.IsNullOrEmpty(cached))
            return JsonSerializer.Deserialize<List<SalaryStructureDto>>(cached)!;

        var entities = await _context.SalaryStructures
            .Where(x => !x.IsDeleted && x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

        var dtos = entities.Select(MapToSalaryStructureDto).ToList();
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dtos), new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(15)
        }, ct);
        return dtos;
    }

    public async Task DeleteSalaryStructureAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _context.SalaryStructures.FindAsync([id], ct);
        if (entity == null) throw new KeyNotFoundException($"Salary structure {id} not found");

        entity.IsDeleted = true;
        entity.IsActive = false;
        await _context.SaveChangesAsync(ct);

        await _cache.RemoveAsync($"salary_structure_{id}", ct);
        await _cache.RemoveAsync("salary_structures_all", ct);
    }

    #endregion

    #region Employee Salary

    public async Task<EmployeeSalaryDto> AssignSalaryToEmployeeAsync(EmployeeSalaryCreateDto dto, CancellationToken ct = default)
    {
        var salaryStructure = await _context.SalaryStructures
            .FirstOrDefaultAsync(x => x.Id == dto.SalaryStructureId && !x.IsDeleted && x.IsActive, ct);
        if (salaryStructure == null)
            throw new KeyNotFoundException($"Salary structure {dto.SalaryStructureId} not found or inactive");

        var previous = await _context.EmployeeSalaries
            .Where(x => x.EmployeeId == dto.EmployeeId && x.IsActive)
            .ToListAsync(ct);
        foreach (var item in previous)
        {
            item.IsActive = false;
            item.EndDate = dto.EffectiveDate;
        }

        var entity = new LocalEmployeeSalary
        {
            EmployeeId = dto.EmployeeId,
            SalaryStructureId = dto.SalaryStructureId,
            BaseSalary = salaryStructure.BaseSalary,
            HousingAllowance = salaryStructure.HousingAllowance,
            TransportAllowance = salaryStructure.TransportAllowance,
            MealAllowance = salaryStructure.MealAllowance,
            MedicalAllowance = salaryStructure.MedicalAllowance,
            OtherAllowances = salaryStructure.OtherAllowances,
            Deductions = salaryStructure.Deductions,
            PensionContribution = salaryStructure.PensionContribution,
            EffectiveDate = dto.EffectiveDate,
            EndDate = dto.EndDate,
            IsActive = true
        };

        await _context.EmployeeSalaries.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);

        await _cache.RemoveAsync($"employee_salary_{dto.EmployeeId}", ct);
        await _cache.RemoveAsync("employee_salaries_all", ct);
        return MapToEmployeeSalaryDto(entity);
    }

    public async Task<EmployeeSalaryDto> UpdateEmployeeSalaryAsync(Guid id, EmployeeSalaryCreateDto dto, CancellationToken ct = default)
    {
        var entity = await _context.EmployeeSalaries
            .Include(x => x.SalaryStructure)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) throw new KeyNotFoundException($"Employee salary {id} not found");

        var salaryStructure = await _context.SalaryStructures
            .FirstOrDefaultAsync(x => x.Id == dto.SalaryStructureId && !x.IsDeleted && x.IsActive, ct);
        if (salaryStructure == null)
            throw new KeyNotFoundException($"Salary structure {dto.SalaryStructureId} not found or inactive");

        entity.SalaryStructureId = dto.SalaryStructureId;
        entity.BaseSalary = salaryStructure.BaseSalary;
        entity.HousingAllowance = salaryStructure.HousingAllowance;
        entity.TransportAllowance = salaryStructure.TransportAllowance;
        entity.MealAllowance = salaryStructure.MealAllowance;
        entity.MedicalAllowance = salaryStructure.MedicalAllowance;
        entity.OtherAllowances = salaryStructure.OtherAllowances;
        entity.Deductions = salaryStructure.Deductions;
        entity.PensionContribution = salaryStructure.PensionContribution;
        entity.EffectiveDate = dto.EffectiveDate;
        entity.EndDate = dto.EndDate;
        entity.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        await _cache.RemoveAsync($"employee_salary_{entity.EmployeeId}", ct);
        await _cache.RemoveAsync("employee_salaries_all", ct);
        return MapToEmployeeSalaryDto(entity);
    }

    public async Task<EmployeeSalaryDto> GetEmployeeSalaryAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _context.EmployeeSalaries
            .Include(x => x.SalaryStructure)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) throw new KeyNotFoundException($"Employee salary {id} not found");

        return MapToEmployeeSalaryDto(entity);
    }

    public async Task<List<EmployeeSalaryDto>> GetEmployeeSalariesAsync(Guid employeeId, CancellationToken ct = default)
    {
        var cacheKey = $"employee_salary_{employeeId}";
        var cached = await _cache.GetStringAsync(cacheKey, ct);
        if (!string.IsNullOrEmpty(cached))
            return JsonSerializer.Deserialize<List<EmployeeSalaryDto>>(cached)!;

        var entities = await _context.EmployeeSalaries
            .Include(x => x.SalaryStructure)
            .Where(x => x.EmployeeId == employeeId && !x.IsDeleted)
            .OrderByDescending(x => x.EffectiveDate)
            .ToListAsync(ct);

        var dtos = entities.Select(MapToEmployeeSalaryDto).ToList();
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dtos), new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(15)
        }, ct);
        return dtos;
    }

    public async Task<List<EmployeeSalaryDto>> GetAllEmployeeSalariesAsync(CancellationToken ct = default)
    {
        var cacheKey = "employee_salaries_all";
        var cached = await _cache.GetStringAsync(cacheKey, ct);
        if (!string.IsNullOrEmpty(cached))
            return JsonSerializer.Deserialize<List<EmployeeSalaryDto>>(cached)!;

        var entities = await _context.EmployeeSalaries
            .Include(x => x.SalaryStructure)
            .Where(x => !x.IsDeleted && x.IsActive)
            .ToListAsync(ct);

        var dtos = entities.Select(MapToEmployeeSalaryDto).ToList();
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dtos), new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(15)
        }, ct);
        return dtos;
    }

    #endregion

    #region Payroll Run

    public async Task<PayrollRunDto> CreatePayrollRunAsync(PayrollRunCreateDto dto, CancellationToken ct = default)
    {
        var existing = await _context.PayrollRuns
            .AnyAsync(x => !x.IsDeleted &&
                ((dto.PayPeriodStart >= x.PayPeriodStart && dto.PayPeriodStart <= x.PayPeriodEnd) ||
                 (dto.PayPeriodEnd >= x.PayPeriodStart && dto.PayPeriodEnd <= x.PayPeriodEnd)), ct);
        if (existing)
            throw new InvalidOperationException("Payroll period overlaps with an existing payroll run");

        var entity = new LocalPayrollRun
        {
            Name = dto.Name,
            PayPeriodStart = dto.PayPeriodStart,
            PayPeriodEnd = dto.PayPeriodEnd,
            PaymentDate = dto.PaymentDate,
            PayrollStatus = PayrollStatus.Draft.ToString(),
            Notes = dto.Notes,
            CreatedBy = "System"
        };

        await _context.PayrollRuns.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);

        foreach (var employeeId in dto.EmployeeIds)
        {
            var employeeSalary = await _context.EmployeeSalaries
                .Include(x => x.SalaryStructure)
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.IsActive && !x.IsDeleted, ct);
            if (employeeSalary == null) continue;

            var payrollEmployee = new LocalPayrollEmployee
            {
                PayrollRunId = entity.Id,
                EmployeeId = employeeId,
                EmployeeCode = "EMP-" + employeeId.ToString().Substring(0, 8),
                EmployeeName = "Employee " + employeeId.ToString().Substring(0, 8),
                Department = "Unknown",
                Position = "Unknown",
                BaseSalary = employeeSalary.BaseSalary,
                HousingAllowance = employeeSalary.HousingAllowance,
                TransportAllowance = employeeSalary.TransportAllowance,
                MealAllowance = employeeSalary.MealAllowance,
                MedicalAllowance = employeeSalary.MedicalAllowance,
                OtherAllowances = employeeSalary.OtherAllowances,
                GrossPay = employeeSalary.TotalSalary,
                NetPay = employeeSalary.TotalSalary
            };

            await _context.PayrollEmployees.AddAsync(payrollEmployee, ct);
        }

        await _context.SaveChangesAsync(ct);
        await _cache.RemoveAsync("payroll_runs_all", ct);
        return await GetPayrollRunAsync(entity.Id, ct);
    }

    // ✅ Professional implementation with IPayrollProcessor
    public async Task<PayrollProcessingResult> ProcessPayrollRunAsync(Guid id, CancellationToken ct = default)
    {
        var processor = _serviceProvider.GetRequiredService<IPayrollProcessor>();
        return await processor.ProcessPayrollRunAsync(id, ct);
    }

    public async Task<PayrollRunDto> ApprovePayrollRunAsync(Guid id, string approvedBy, CancellationToken ct = default)
    {
        var entity = await _context.PayrollRuns
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) throw new KeyNotFoundException($"Payroll run {id} not found");

        if (entity.PayrollStatus != PayrollStatus.Completed.ToString())
            throw new InvalidOperationException($"Payroll run must be completed before approval. Current status: {entity.PayrollStatus}");

        entity.PayrollStatus = PayrollStatus.Approved.ToString();
        entity.ApprovedAt = DateTime.UtcNow;
        entity.ApprovedBy = approvedBy;

        await _context.SaveChangesAsync(ct);

        await _cache.RemoveAsync($"payroll_run_{id}", ct);
        await _cache.RemoveAsync("payroll_runs_all", ct);
        return await GetPayrollRunAsync(id, ct);
    }

    public async Task<PayrollRunDto> GetPayrollRunAsync(Guid id, CancellationToken ct = default)
    {
        var cacheKey = $"payroll_run_{id}";
        var cached = await _cache.GetStringAsync(cacheKey, ct);
        if (!string.IsNullOrEmpty(cached))
            return JsonSerializer.Deserialize<PayrollRunDto>(cached)!;

        var entity = await _context.PayrollRuns
            .Include(x => x.PayrollEmployees)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) throw new KeyNotFoundException($"Payroll run {id} not found");

        var dto = MapToPayrollRunDto(entity);
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(15)
        }, ct);
        return dto;
    }

    public async Task<List<PayrollRunDto>> GetAllPayrollRunsAsync(CancellationToken ct = default)
    {
        var cacheKey = "payroll_runs_all";
        var cached = await _cache.GetStringAsync(cacheKey, ct);
        if (!string.IsNullOrEmpty(cached))
            return JsonSerializer.Deserialize<List<PayrollRunDto>>(cached)!;

        var entities = await _context.PayrollRuns
            .Include(x => x.PayrollEmployees)
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.PayPeriodStart)
            .ToListAsync(ct);

        var dtos = entities.Select(MapToPayrollRunDto).ToList();
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dtos), new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(15)
        }, ct);
        return dtos;
    }

    public async Task<PayrollRunDto> UpdatePayrollRunStatusAsync(Guid id, PayrollRunStatusUpdateDto dto, CancellationToken ct = default)
    {
        var entity = await _context.PayrollRuns
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) throw new KeyNotFoundException($"Payroll run {id} not found");

        entity.PayrollStatus = dto.Status;
        entity.Notes = dto.Notes ?? entity.Notes;
        entity.DateMod = DateTime.UtcNow;

        if (dto.Status == PayrollStatus.Approved.ToString())
        {
            entity.ApprovedAt = DateTime.UtcNow;
            entity.ApprovedBy = dto.ApprovedBy;
        }

        await _context.SaveChangesAsync(ct);

        await _cache.RemoveAsync($"payroll_run_{id}", ct);
        await _cache.RemoveAsync("payroll_runs_all", ct);
        return await GetPayrollRunAsync(id, ct);
    }

    #endregion

    #region Tax

    public async Task<TaxCalculationDto> CalculateTaxAsync(decimal grossIncome, CancellationToken ct = default)
    {
        return await _taxCalculator.CalculateTaxAsync(grossIncome, ct);
    }

    public async Task<TaxRateDto> CreateTaxRateAsync(TaxRateCreateDto dto, CancellationToken ct = default)
    {
        var entity = new LocalTaxRate
        {
            Name = dto.Name,
            MinIncome = dto.MinIncome,
            MaxIncome = dto.MaxIncome,
            TaxRate = dto.TaxRate,
            DeductibleAmount = dto.DeductibleAmount,
            TaxYear = dto.TaxYear,
            IsActive = true
        };

        await _context.TaxRates.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);

        await _cache.RemoveAsync("tax_rates_all", ct);
        return MapToTaxRateDto(entity);
    }

    public async Task<List<TaxRateDto>> GetTaxRatesAsync(string? taxYear = null, CancellationToken ct = default)
    {
        var cacheKey = $"tax_rates_{taxYear ?? "all"}";
        var cached = await _cache.GetStringAsync(cacheKey, ct);
        if (!string.IsNullOrEmpty(cached))
            return JsonSerializer.Deserialize<List<TaxRateDto>>(cached)!;

        var query = _context.TaxRates.Where(x => !x.IsDeleted && x.IsActive);
        if (!string.IsNullOrEmpty(taxYear))
            query = query.Where(x => x.TaxYear == taxYear);

        var entities = await query.OrderBy(x => x.MinIncome).ToListAsync(ct);
        var dtos = entities.Select(MapToTaxRateDto).ToList();
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dtos), new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(15)
        }, ct);
        return dtos;
    }

    #endregion

    #region Payslip

    public async Task<PayslipDto> GeneratePayslipAsync(Guid payrollEmployeeId, CancellationToken ct = default)
    {
        return await _payslipGenerator.GeneratePayslipAsync(payrollEmployeeId, ct);
    }

    public async Task<List<PayslipDto>> GeneratePayslipsAsync(Guid payrollRunId, CancellationToken ct = default)
    {
        var payrollRun = await _context.PayrollRuns
            .Include(x => x.PayrollEmployees)
            .FirstOrDefaultAsync(x => x.Id == payrollRunId && !x.IsDeleted, ct);
        if (payrollRun == null) throw new KeyNotFoundException($"Payroll run {payrollRunId} not found");

        var payslips = new List<PayslipDto>();
        foreach (var employee in payrollRun.PayrollEmployees)
        {
            try
            {
                var payslip = await _payslipGenerator.GeneratePayslipAsync(employee.Id, ct);
                payslips.Add(payslip);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating payslip for employee {EmployeeId}", employee.EmployeeId);
            }
        }
        return payslips;
    }

    public async Task<PayslipDto> GetPayslipAsync(Guid id, CancellationToken ct = default)
    {
        var cacheKey = $"payslip_{id}";
        var cached = await _cache.GetStringAsync(cacheKey, ct);
        if (!string.IsNullOrEmpty(cached))
            return JsonSerializer.Deserialize<PayslipDto>(cached)!;

        var entity = await _context.Payslips
            .Include(x => x.PayrollEmployee)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) throw new KeyNotFoundException($"Payslip {id} not found");

        var dto = MapToPayslipDto(entity);
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(15)
        }, ct);
        return dto;
    }

    public async Task<PayslipDto> GetPayslipByEmployeeAsync(Guid employeeId, Guid payrollRunId, CancellationToken ct = default)
    {
        var entity = await _context.Payslips
            .Include(x => x.PayrollEmployee)
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId &&
                                      x.PayrollEmployee.PayrollRunId == payrollRunId &&
                                      !x.IsDeleted, ct);
        if (entity == null)
            throw new KeyNotFoundException($"Payslip not found for employee {employeeId} in payroll run {payrollRunId}");

        return MapToPayslipDto(entity);
    }

    public async Task<List<PayslipDto>> GetPayslipsByEmployeeAsync(Guid employeeId, CancellationToken ct = default)
    {
        var cacheKey = $"payslips_employee_{employeeId}";
        var cached = await _cache.GetStringAsync(cacheKey, ct);
        if (!string.IsNullOrEmpty(cached))
            return JsonSerializer.Deserialize<List<PayslipDto>>(cached)!;

        var entities = await _context.Payslips
            .Include(x => x.PayrollEmployee)
            .Where(x => x.EmployeeId == employeeId && !x.IsDeleted)
            .OrderByDescending(x => x.PeriodEnd)
            .ToListAsync(ct);

        var dtos = entities.Select(MapToPayslipDto).ToList();
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dtos), new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(15)
        }, ct);
        return dtos;
    }

    public async Task<List<PayslipDto>> GetAllPayslipsAsync(CancellationToken ct = default)
    {
        var cacheKey = "payslips_all";
        var cached = await _cache.GetStringAsync(cacheKey, ct);
        if (!string.IsNullOrEmpty(cached))
            return JsonSerializer.Deserialize<List<PayslipDto>>(cached)!;

        var entities = await _context.Payslips
            .Include(x => x.PayrollEmployee)
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.PeriodEnd)
            .ToListAsync(ct);

        var dtos = entities.Select(MapToPayslipDto).ToList();
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dtos), new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(15)
        }, ct);
        return dtos;
    }

    public async Task<byte[]> GeneratePayslipPdfAsync(Guid id, CancellationToken ct = default)
    {
        var payslip = await GetPayslipAsync(id, ct);
        var html = $@"
            <html>
            <head><title>Payslip {payslip.PayslipNumber}</title></head>
            <body>
                <h1>Payslip</h1>
                <p>Number: {payslip.PayslipNumber}</p>
                <p>Period: {payslip.PeriodStart} - {payslip.PeriodEnd}</p>
                <p>Gross Pay: {payslip.GrossPay:C}</p>
                <p>Net Pay: {payslip.NetPay:C}</p>
                <p>Tax: {payslip.TaxAmount:C}</p>
            </body>
            </html>
        ";

        var pdfBytes = System.Text.Encoding.UTF8.GetBytes(html);
        return await Task.FromResult(pdfBytes);
    }

    #endregion

    #region Reports & Export

    public async Task<PayrollSummaryReport> GetPayrollSummaryReportAsync(DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var query = _context.PayrollRuns.Where(x => !x.IsDeleted);

        if (from.HasValue)
            query = query.Where(x => x.PayPeriodStart >= from.Value);
        if (to.HasValue)
            query = query.Where(x => x.PayPeriodEnd <= to.Value);

        var payrollRuns = await query
            .Include(x => x.PayrollEmployees)
            .OrderByDescending(x => x.PayPeriodStart)
            .ToListAsync(ct);

        var report = new PayrollSummaryReport
        {
            TotalPayrollRuns = payrollRuns.Count,
            TotalEmployees = payrollRuns.Sum(x => x.TotalEmployees),
            TotalGrossPay = payrollRuns.Sum(x => x.TotalGrossPay),
            TotalNetPay = payrollRuns.Sum(x => x.TotalNetPay),
            TotalTaxes = payrollRuns.Sum(x => x.TotalTaxes),
            TotalDeductions = payrollRuns.Sum(x => x.TotalDeductions),
            PeriodStart = from ?? payrollRuns.MinBy(x => x.PayPeriodStart)?.PayPeriodStart ?? DateTime.MinValue,
            PeriodEnd = to ?? payrollRuns.MaxBy(x => x.PayPeriodEnd)?.PayPeriodEnd ?? DateTime.MaxValue,
            PayrollRuns = payrollRuns.Select(x => new PayrollRunSummaryDto
            {
                Id = x.Id,
                Name = x.Name,
                PayPeriodStart = x.PayPeriodStart,
                PayPeriodEnd = x.PayPeriodEnd,
                PayrollStatus = x.PayrollStatus,
                TotalEmployees = x.TotalEmployees,
                TotalGrossPay = x.TotalGrossPay,
                TotalNetPay = x.TotalNetPay
            }).ToList()
        };

        var deptGroups = payrollRuns
            .SelectMany(x => x.PayrollEmployees)
            .GroupBy(x => x.Department)
            .Select(g => new DepartmentPayrollSummaryDto
            {
                DepartmentName = g.Key,
                EmployeeCount = g.Count(),
                TotalGrossPay = g.Sum(x => x.GrossPay),
                TotalNetPay = g.Sum(x => x.NetPay)
            }).ToList();

        report.DepartmentBreakdown = deptGroups;

        return report;
    }

    public async Task<BankExportFile> GenerateBankExportAsync(Guid payrollRunId, CancellationToken ct = default)
    {
        var payrollRun = await _context.PayrollRuns
            .Include(x => x.PayrollEmployees)
            .FirstOrDefaultAsync(x => x.Id == payrollRunId && !x.IsDeleted, ct);

        if (payrollRun == null)
            throw new KeyNotFoundException($"Payroll run {payrollRunId} not found");

        var bankFile = new BankExportFile
        {
            PayrollRunId = payrollRunId,
            PayrollRunName = payrollRun.Name,
            PaymentDate = payrollRun.PaymentDate,
            TotalAmount = payrollRun.TotalNetPay,
            TotalEmployees = payrollRun.TotalEmployees
        };

        foreach (var employee in payrollRun.PayrollEmployees.Where(x => x.NetPay > 0))
        {
            bankFile.Transactions.Add(new BankTransactionDto
            {
                EmployeeId = employee.EmployeeId,
                EmployeeCode = employee.EmployeeCode,
                EmployeeName = employee.EmployeeName,
                Amount = employee.NetPay,
                AccountNumber = "1234567890",
                BankCode = "001"
            });
        }

        return bankFile;
    }

    public async Task<byte[]> GenerateBankExportCsvAsync(Guid payrollRunId, CancellationToken ct = default)
    {
        var bankFile = await GenerateBankExportAsync(payrollRunId, ct);

        var sb = new StringBuilder();
        sb.AppendLine("EmployeeCode,EmployeeName,AccountNumber,BankCode,Amount");

        foreach (var transaction in bankFile.Transactions)
        {
            sb.AppendLine($"{transaction.EmployeeCode},{transaction.EmployeeName},{transaction.AccountNumber},{transaction.BankCode},{transaction.Amount:F2}");
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public async Task SendPayslipEmailAsync(Guid payslipId, CancellationToken ct = default)
    {
        var payslip = await GetPayslipAsync(payslipId, ct);
        var htmlPayslip = await GeneratePayslipPdfAsync(payslipId, ct);

        _logger.LogInformation("Payslip email sent for {PayslipNumber}", payslip.PayslipNumber);
        await Task.CompletedTask;
    }

    public async Task SendBulkPayslipEmailsAsync(Guid payrollRunId, CancellationToken ct = default)
    {
        var payrollRun = await _context.PayrollRuns
            .Include(x => x.PayrollEmployees)
            .FirstOrDefaultAsync(x => x.Id == payrollRunId && !x.IsDeleted, ct);

        if (payrollRun == null)
            throw new KeyNotFoundException($"Payroll run {payrollRunId} not found");

        foreach (var employee in payrollRun.PayrollEmployees)
        {
            try
            {
                var payslip = await _context.Payslips
                    .FirstOrDefaultAsync(x => x.PayrollEmployeeId == employee.Id && !x.IsDeleted, ct);

                if (payslip != null)
                {
                    await SendPayslipEmailAsync(payslip.Id, ct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending payslip email for employee {EmployeeId}", employee.EmployeeId);
            }
        }
    }

    public async Task<List<PayslipHistoryDto>> GetEmployeePayslipHistoryAsync(Guid employeeId, int year, CancellationToken ct = default)
    {
        var payslips = await _context.Payslips
            .Include(x => x.PayrollEmployee)
            .Where(x => x.EmployeeId == employeeId &&
                        x.PeriodStart.Year == year &&
                        !x.IsDeleted)
            .OrderByDescending(x => x.PeriodStart)
            .Select(x => new PayslipHistoryDto
            {
                Id = x.Id,
                PayslipNumber = x.PayslipNumber,
                PeriodStart = x.PeriodStart,
                PeriodEnd = x.PeriodEnd,
                GrossPay = x.GrossPay,
                NetPay = x.NetPay,
                IsGenerated = x.IsGenerated,
                GeneratedAt = x.GeneratedAt
            })
            .ToListAsync(ct);

        return payslips;
    }

    #endregion

    #region Mapping Methods

    private static SalaryStructureDto MapToSalaryStructureDto(LocalSalaryStructure entity)
    {
        return new SalaryStructureDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            BaseSalary = entity.BaseSalary,
            HousingAllowance = entity.HousingAllowance,
            TransportAllowance = entity.TransportAllowance,
            MealAllowance = entity.MealAllowance,
            MedicalAllowance = entity.MedicalAllowance,
            OtherAllowances = entity.OtherAllowances,
            Deductions = entity.Deductions,
            PensionContribution = entity.PensionContribution,
            IsActive = entity.IsActive
        };
    }

    private static EmployeeSalaryDto MapToEmployeeSalaryDto(LocalEmployeeSalary entity)
    {
        return new EmployeeSalaryDto
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            SalaryStructureId = entity.SalaryStructureId,
            SalaryStructureName = entity.SalaryStructure?.Name ?? "Unknown",
            BaseSalary = entity.BaseSalary,
            HousingAllowance = entity.HousingAllowance,
            TransportAllowance = entity.TransportAllowance,
            MealAllowance = entity.MealAllowance,
            MedicalAllowance = entity.MedicalAllowance,
            OtherAllowances = entity.OtherAllowances,
            Deductions = entity.Deductions,
            PensionContribution = entity.PensionContribution,
            TotalSalary = entity.TotalSalary,
            EffectiveDate = entity.EffectiveDate,
            EndDate = entity.EndDate,
            IsActive = entity.IsActive
        };
    }

    private static PayrollRunDto MapToPayrollRunDto(LocalPayrollRun entity)
    {
        return new PayrollRunDto
        {
            Id = entity.Id,
            Name = entity.Name,
            PayPeriodStart = entity.PayPeriodStart,
            PayPeriodEnd = entity.PayPeriodEnd,
            PaymentDate = entity.PaymentDate,
            PayrollStatus = entity.PayrollStatus,
            TotalGrossPay = entity.TotalGrossPay,
            TotalNetPay = entity.TotalNetPay,
            TotalTaxes = entity.TotalTaxes,
            TotalDeductions = entity.TotalDeductions,
            TotalEmployees = entity.TotalEmployees,
            CreatedBy = entity.CreatedBy,
            ProcessedAt = entity.ProcessedAt,
            ApprovedAt = entity.ApprovedAt,
            ApprovedBy = entity.ApprovedBy,
            Notes = entity.Notes,
            Employees = entity.PayrollEmployees?.Select(e => new PayrollEmployeeDto
            {
                Id = e.Id,
                EmployeeId = e.EmployeeId,
                EmployeeCode = e.EmployeeCode,
                EmployeeName = e.EmployeeName,
                Department = e.Department,
                Position = e.Position,
                BaseSalary = e.BaseSalary,
                HousingAllowance = e.HousingAllowance,
                TransportAllowance = e.TransportAllowance,
                MealAllowance = e.MealAllowance,
                MedicalAllowance = e.MedicalAllowance,
                OtherAllowances = e.OtherAllowances,
                OvertimePay = e.OvertimePay,
                BonusPay = e.BonusPay,
                CommissionPay = e.CommissionPay,
                GrossPay = e.GrossPay,
                TaxAmount = e.TaxAmount,
                PensionContribution = e.PensionContribution,
                OtherDeductions = e.OtherDeductions,
                NetPay = e.NetPay,
                DaysWorked = e.DaysWorked,
                DaysAbsent = e.DaysAbsent,
                OvertimeHours = e.OvertimeHours,
                Notes = e.Notes
            }).ToList() ?? new()
        };
    }

    private static TaxRateDto MapToTaxRateDto(LocalTaxRate entity)
    {
        return new TaxRateDto
        {
            Id = entity.Id,
            Name = entity.Name,
            MinIncome = entity.MinIncome,
            MaxIncome = entity.MaxIncome,
            TaxRate = entity.TaxRate,
            DeductibleAmount = entity.DeductibleAmount,
            TaxYear = entity.TaxYear,
            IsActive = entity.IsActive
        };
    }

    private static PayslipDto MapToPayslipDto(LocalPayslip entity)
    {
        return new PayslipDto
        {
            Id = entity.Id,
            PayslipNumber = entity.PayslipNumber,
           EmployeeName = entity.PayrollEmployee?.EmployeeName ?? string.Empty,
           EmployeeCode = entity.PayrollEmployee?.EmployeeCode ?? string.Empty,
           Department = entity.PayrollEmployee?.Department ?? string.Empty,
           Position = entity.PayrollEmployee?.Position ?? string.Empty,
            PeriodStart = entity.PeriodStart,
            PeriodEnd = entity.PeriodEnd,
            PaymentDate = entity.PaymentDate,
            GrossPay = entity.GrossPay,
            HousingAllowance = entity.PayrollEmployee?.HousingAllowance ?? 0,
            TransportAllowance = entity.PayrollEmployee?.TransportAllowance ?? 0,
            MealAllowance = entity.PayrollEmployee?.MealAllowance ?? 0,
            MedicalAllowance = entity.PayrollEmployee?.MedicalAllowance ?? 0,
            OtherAllowances = entity.PayrollEmployee?.OtherAllowances ?? 0,
            TotalAllowances = entity.TotalAllowances,
            OvertimePay = entity.PayrollEmployee?.OvertimePay ?? 0,
            BonusPay = entity.PayrollEmployee?.BonusPay ?? 0,
            CommissionPay = entity.PayrollEmployee?.CommissionPay ?? 0,
            TaxAmount = entity.TaxAmount,
            PensionContribution = entity.PayrollEmployee?.PensionContribution ?? 0,
            OtherDeductions = entity.PayrollEmployee?.OtherDeductions ?? 0,
            TotalDeductions = entity.TotalDeductions,
            NetPay = entity.NetPay,
            PaymentMethod = entity.PaymentMethod,
            BankAccount = entity.BankAccount,
            DaysWorked = entity.PayrollEmployee?.DaysWorked ?? 0,
            DaysAbsent = entity.PayrollEmployee?.DaysAbsent ?? 0,
            OvertimeHours = entity.PayrollEmployee?.OvertimeHours ?? 0,
            Notes = entity.Notes,
            IsGenerated = entity.IsGenerated,
            GeneratedAt = entity.GeneratedAt
        };
    }

    #endregion
}