using MediatR;
using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;

namespace Cor.Procurement.Queries;

public class GetDashboardDataQueryHandler
    : IRequestHandler<GetDashboardDataQuery, DashboardDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetDashboardDataQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetDashboardDataQueryHandler(
        ProcurementDbContext context,
        ILogger<GetDashboardDataQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<DashboardDto> Handle(GetDashboardDataQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Check cache first
            var cacheKey = "dashboard_data";
            var cached = await _cache.GetAsync<DashboardDto>(cacheKey, cancellationToken);
            if (cached != null)
            {
                _logger.LogInformation("📦 Cache HIT: Dashboard data");
                return cached;
            }

            _logger.LogInformation("📦 Cache MISS: Fetching dashboard data");

            var dashboard = new DashboardDto();
            var now = DateTime.UtcNow;
            var oneMonthAgo = now.AddMonths(-1);
            var threeMonthsAgo = now.AddMonths(-3);

            // ============================================================
            // 1. STATS
            // ============================================================

            // Get all invoices (for spend calculations)
            var invoices = await _context.Invoices
                .Where(i => !i.IsDeleted)
                .ToListAsync(cancellationToken);

            // Get all vendors
            var vendors = await _context.Vendors
                .Where(v => !v.IsDeleted && v.IsActive)
                .ToListAsync(cancellationToken);

            // Get purchase orders
            var pos = await _context.PurchaseOrders
                .Where(p => !p.IsDeleted)
                .ToListAsync(cancellationToken);

            // Get requisitions
            var requisitions = await _context.Requisitions
                .Where(r => !r.IsDeleted)
                .ToListAsync(cancellationToken);

            // Get contracts
            var contracts = await _context.VendorContracts
                .Where(c => !c.IsDeleted)
                .ToListAsync(cancellationToken);

            // Calculate totals
            var totalSpent = invoices.Sum(i => i.TotalAmount);
            var monthlySpend = invoices
                .Where(i => i.InvoiceDate >= oneMonthAgo)
                .Sum(i => i.TotalAmount);

            var previousMonthSpend = invoices
                .Where(i => i.InvoiceDate >= threeMonthsAgo && i.InvoiceDate < oneMonthAgo)
                .Sum(i => i.TotalAmount);

            var monthlyChange = previousMonthSpend > 0
                ? ((monthlySpend - previousMonthSpend) / previousMonthSpend) * 100
                : 0;

            dashboard.Stats = new DashboardStats
            {
                MonthlySpend = monthlySpend,
                CostSavings = Math.Round(monthlySpend * 0.15m, 2), // Estimated 15% savings
                ActiveVendors = vendors.Count,
                OpenPOs = pos.Count(p => p.Status == "Active" || p.Status == "Sent"),
                TotalRequisitions = requisitions.Count,
                PendingInvoices = invoices.Count(i => i.Status == "Sent" || i.Status == "Verified"),
                ActiveContracts = contracts.Count(c => c.Status == "Active"),
                TotalSpent = totalSpent,
                MonthlyChange = monthlyChange,
                SavingsChange = 8.2m,
                VendorsChange = 5.3m,
                OrdersChange = -3.2m
            };

            // ============================================================
            // 2. VENDOR PERFORMANCE
            // ============================================================

            // Get PO counts per vendor
            var vendorOrderCounts = pos
                .Where(p => p.VendorId.HasValue)
                .GroupBy(p => p.VendorId!.Value)
                .Select(g => new { VendorId = g.Key, Count = g.Count() });

            // Get spend per vendor from invoices
            var vendorSpend = invoices
                .Where(i => i.VendorId != Guid.Empty)
                .GroupBy(i => i.VendorId)
                .Select(g => new { VendorId = g.Key, Spend = g.Sum(i => i.TotalAmount) });

            var vendorData = vendors
                .Select(v =>
                {
                    var orderCount = vendorOrderCounts.FirstOrDefault(o => o.VendorId == v.Id)?.Count ?? 0;
                    var spend = vendorSpend.FirstOrDefault(s => s.VendorId == v.Id)?.Spend ?? 0;
                    var rating = v.Rating ?? 0;

                    string performance = rating >= 4.5m ? "Excellent"
                        : rating >= 4.0m ? "Good"
                        : rating >= 3.0m ? "Average"
                        : "Needs Improvement";

                    return new DashboardVendorDto
                    {
                        Id = v.Id,
                        Name = v.Name,
                        Code = v.Code,
                        Rating = rating,
                        Performance = performance,
                        Orders = orderCount,
                        Spend = spend
                    };
                })
                .OrderByDescending(v => v.Rating)
                .Take(request.TopVendorsCount)
                .ToList();

            dashboard.Vendors = vendorData;

            // ============================================================
            // 3. ACTIVE PURCHASE ORDERS
            // ============================================================

            var activeOrders = await _context.PurchaseOrders
                .Where(p => !p.IsDeleted && (p.Status == "Active" || p.Status == "Sent"))
                .OrderByDescending(p => p.OrderDate)
                .Take(5)
                .Select(p => new DashboardPurchaseOrderDto
                {
                    Id = p.Id,
                    PurchaseOrderNumber = p.PurchaseOrderNumber,
                    VendorName = p.VendorName,
                    TotalAmount = p.TotalAmount,
                    Status = p.Status,
                    OrderDate = p.OrderDate,
                    ExpectedDeliveryDate = p.ExpectedDeliveryDate
                })
                .ToListAsync(cancellationToken);

            dashboard.ActiveOrders = activeOrders;

            // ============================================================
            // 4. SPEND BY CATEGORY (Using Vendor Types)
            // ============================================================

            var spendByCategory = invoices
                .Where(i => i.VendorId != Guid.Empty)
                .Join(
                    vendors,
                    i => i.VendorId,
                    v => v.Id,
                    (i, v) => new { Invoice = i, Vendor = v }
                )
                .GroupBy(x => x.Vendor.VendorType ?? "Uncategorized")
                .Select(g => new
                {
                    Category = g.Key,
                    Amount = g.Sum(x => x.Invoice.TotalAmount)
                })
                .ToList();

            var totalSpendCategory = spendByCategory.Sum(x => x.Amount);
            var categoryColors = new[] {
                "from-purple-500 to-purple-600",
                "from-blue-500 to-blue-600",
                "from-emerald-500 to-emerald-600",
                "from-amber-500 to-amber-600",
                "from-rose-500 to-rose-600",
                "from-cyan-500 to-cyan-600"
            };

            var spendCategories = spendByCategory
                .OrderByDescending(x => x.Amount)
                .Select((x, index) => new DashboardSpendCategoryDto
                {
                    Name = x.Category,
                    Amount = x.Amount,
                    Percentage = totalSpendCategory > 0
                        ? (int)Math.Round((x.Amount / totalSpendCategory) * 100)
                        : 0,
                    Color = categoryColors[index % categoryColors.Length]
                })
                .Take(6)
                .ToList();

            dashboard.SpendByCategory = spendCategories;

            // ============================================================
            // 5. RECENT ACTIVITIES
            // ============================================================

            var recentActivities = new List<DashboardActivityDto>();

            // Recent Requisitions
            var recentRequisitions = await _context.Requisitions
                .Where(r => !r.IsDeleted)
                .OrderByDescending(r => r.DateAdd)
                .Take(3)
                .Select(r => new DashboardActivityDto
                {
                    Type = "Requisition",
                    Title = r.RequisitionNumber,
                    Description = r.Title,
                    Status = r.Status,
                    Date = r.DateAdd,
                    ReferenceId = r.Id.ToString()
                })
                .ToListAsync(cancellationToken);

            // Recent Invoices
            var recentInvoices = await _context.Invoices
                .Where(i => !i.IsDeleted)
                .OrderByDescending(i => i.DateAdd)
                .Take(3)
                .Select(i => new DashboardActivityDto
                {
                    Type = "Invoice",
                    Title = i.InvoiceNumber,
                    Description = $"Amount: {i.TotalAmount:C}",
                    Status = i.Status,
                    Date = i.DateAdd,
                    ReferenceId = i.Id.ToString()
                })
                .ToListAsync(cancellationToken);

            // Recent GRNs
            var recentGrns = await _context.GoodsReceiptNotes
                .Where(g => !g.IsDeleted)
                .OrderByDescending(g => g.DateAdd)
                .Take(2)
                .Select(g => new DashboardActivityDto
                {
                    Type = "GRN",
                    Title = g.GrnNumber,
                    Description = $"Received: {g.TotalReceived} items",
                    Status = g.Status,
                    Date = g.DateAdd,
                    ReferenceId = g.Id.ToString()
                })
                .ToListAsync(cancellationToken);

            recentActivities.AddRange(recentRequisitions);
            recentActivities.AddRange(recentInvoices);
            recentActivities.AddRange(recentGrns);

            dashboard.RecentActivities = recentActivities
                .OrderByDescending(a => a.Date)
                .Take(request.RecentActivitiesCount)
                .ToList();

            // ============================================================
            // 6. LAST UPDATED
            // ============================================================

            dashboard.LastUpdated = DateTime.UtcNow;

            // Cache for 5 minutes
            await _cache.SetAsync(cacheKey, dashboard, TimeSpan.FromMinutes(5), cancellationToken);

            _logger.LogInformation("✅ Dashboard data fetched and cached");

            return dashboard;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching dashboard data");
            throw;
        }
    }
}