// Services/AggregateService.cs
using Cor.Finance.Models.Entities.Aggregates;
using Cor.Finance.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Cor.Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Services;

public class AggregateService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AggregateService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(15);

    public AggregateService(IServiceProvider serviceProvider, ILogger<AggregateService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🔄 Aggregate Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ComputeAggregatesAsync(stoppingToken);
                await Task.Delay(_interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error computing aggregates");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        _logger.LogInformation("🔄 Aggregate Service stopped");
    }

    private async Task ComputeAggregatesAsync(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
        var invoiceAggRepo = scope.ServiceProvider.GetRequiredService<IAggregateRepository<InvoiceAggregate>>();
        var paymentAggRepo = scope.ServiceProvider.GetRequiredService<IAggregateRepository<PaymentAggregate>>();

        var today = DateTime.UtcNow.Date;
        var period = today.ToString("yyyy-MM");
        var aggregateType = "Monthly";

        _logger.LogInformation("📊 Computing aggregates for {Period}", period);

        // Compute invoice aggregates
        await ComputeInvoiceAggregatesAsync(context, invoiceAggRepo, period, aggregateType, ct);

        // Compute payment aggregates
        await ComputePaymentAggregatesAsync(context, paymentAggRepo, period, aggregateType, ct);

        _logger.LogInformation("✅ Aggregates computed for {Period}", period);
    }

    private async Task ComputeInvoiceAggregatesAsync(
        FinanceDbContext context,
        IAggregateRepository<InvoiceAggregate> repo,
        string period,
        string aggregateType,
        CancellationToken ct)
    {
        var (startDate, endDate) = GetPeriodDates(period);

        var existing = await repo.GetByPeriodAsync(period, aggregateType, ct);
        if (existing != null)
        {
            await repo.DeleteAsync(existing, ct);
        }

        // ✅ Use ToListAsync and compute in memory to avoid EF translation issues
        var invoices = await context.Invoices
            .Where(i => !i.IsDeleted && i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
            .ToListAsync(ct);

        if (!invoices.Any())
        {
            _logger.LogWarning("⚠️ No invoices found for period {Period}", period);
            return;
        }

        var aggregate = new InvoiceAggregate
        {
            Id = Guid.NewGuid(),
            AggregateDate = DateTime.UtcNow,
            Period = period,
            AggregateType = aggregateType,
            TotalInvoices = invoices.Count,
            TotalAmount = invoices.Sum(x => x.TotalAmount),
            TotalPaid = invoices.Sum(x => x.PaidAmount),
            TotalBalance = invoices.Sum(x => x.TotalAmount - x.PaidAmount),
            PaidCount = invoices.Count(x => x.Status == "Paid"),
            OverdueCount = invoices.Count(x => x.Status == "Overdue"),
            DraftCount = invoices.Count(x => x.Status == "Draft"),
            AverageInvoiceAmount = invoices.Average(x => x.TotalAmount),
            SalesCount = invoices.Count(x => x.InvoiceType == "Sales"),
            SalesAmount = invoices.Where(x => x.InvoiceType == "Sales").Sum(x => x.TotalAmount),
            PurchaseCount = invoices.Count(x => x.InvoiceType == "Purchase"),
            PurchaseAmount = invoices.Where(x => x.InvoiceType == "Purchase").Sum(x => x.TotalAmount),
           AveragePaymentDays = (decimal)invoices.Average(x =>
               x.DueDate.HasValue ? (x.DueDate.Value - x.InvoiceDate).Days : 0),
            DateAdd = DateTime.UtcNow
        };

        await repo.AddAsync(aggregate, ct);

        _logger.LogInformation("✅ Invoice aggregates saved: {Period} - {Count} invoices",
            period, aggregate.TotalInvoices);
    }

    private async Task ComputePaymentAggregatesAsync(
        FinanceDbContext context,
        IAggregateRepository<PaymentAggregate> repo,
        string period,
        string aggregateType,
        CancellationToken ct)
    {
        var (startDate, endDate) = GetPeriodDates(period);

        var existing = await repo.GetByPeriodAsync(period, aggregateType, ct);
        if (existing != null)
        {
            await repo.DeleteAsync(existing, ct);
        }

        // ✅ Use ToListAsync and compute in memory
        var payments = await context.Payments
            .Where(p => !p.IsDeleted && p.PaymentDate >= startDate && p.PaymentDate <= endDate)
            .ToListAsync(ct);

        if (!payments.Any())
        {
            _logger.LogWarning("⚠️ No payments found for period {Period}", period);
            return;
        }

        var aggregate = new PaymentAggregate
        {
            Id = Guid.NewGuid(),
            AggregateDate = DateTime.UtcNow,
            Period = period,
            AggregateType = aggregateType,
            TotalPayments = payments.Count,
            TotalAmount = payments.Sum(x => x.Amount),
            VendorPayments = payments.Where(x => x.PaymentType == "Purchase").Sum(x => x.Amount),
            CustomerPayments = payments.Where(x => x.PaymentType == "Sales").Sum(x => x.Amount),
            ProcessedCount = payments.Count(x => x.Status == "Completed"),
            PendingCount = payments.Count(x => x.Status == "Pending"),
            DateAdd = DateTime.UtcNow
        };

        await repo.AddAsync(aggregate, ct);

        _logger.LogInformation("✅ Payment aggregates saved: {Period} - {Count} payments",
            period, aggregate.TotalPayments);
    }

    private (DateTime start, DateTime end) GetPeriodDates(string period)
    {
        var parts = period.Split('-');
        var year = int.Parse(parts[0]);
        var month = int.Parse(parts[1]);

        var start = new DateTime(year, month, 1);
        var end = start.AddMonths(1).AddDays(-1);

        return (start, end);
    }
}