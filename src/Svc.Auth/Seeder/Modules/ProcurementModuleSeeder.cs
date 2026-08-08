using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Seeder.Modules;

public static class ProcurementModuleSeeder
{
    public static IEnumerable<PerMenuSeedDto> GetMenus()
    {
        return new List<PerMenuSeedDto>
        {
            // ===== DASHBOARD =====
            new() { ModKey = "mod.pro", Key = "pro.db", Label = "Dashboard", Path = "/procurement", Icon = "LayoutDashboard", ParKey = "", IsChild = false, Order = 1 },

            // ===== REQUISITIONS =====
            new() { ModKey = "mod.pro", Key = "pro.req", Label = "Requisitions", Path = "", Icon = "FileText", ParKey = "", IsChild = false, Order = 2 },
            new() { ModKey = "mod.pro", Key = "pro.req.create", Label = "Create Requisition", Path = "/procurement/requisitions/create", Icon = "Plus", ParKey = "pro.req", IsChild = true, Order = 1 },
            new() { ModKey = "mod.pro", Key = "pro.req.list", Label = "Requisition List", Path = "/procurement/requisitions", Icon = "List", ParKey = "pro.req", IsChild = true, Order = 2 },
            new() { ModKey = "mod.pro", Key = "pro.req.approval", Label = "Requisition Approval", Path = "/procurement/requisitions/approval", Icon = "CheckCircle", ParKey = "pro.req", IsChild = true, Order = 3 },

            // ===== VENDOR MANAGEMENT =====
            new() { ModKey = "mod.pro", Key = "pro.vendor", Label = "Vendor Management", Path = "", Icon = "Users", ParKey = "", IsChild = false, Order = 3 },
            new() { ModKey = "mod.pro", Key = "pro.vendor.list", Label = "Vendors", Path = "/procurement/vendors", Icon = "Truck", ParKey = "pro.vendor", IsChild = true, Order = 1 },
            new() { ModKey = "mod.pro", Key = "pro.vendor.evaluation", Label = "Vendor Evaluation", Path = "/procurement/vendor-evaluation", Icon = "Star", ParKey = "pro.vendor", IsChild = true, Order = 2 },
            new() { ModKey = "mod.pro", Key = "pro.vendor.contract", Label = "Vendor Contracts", Path = "/procurement/vendor-contracts", Icon = "FileText", ParKey = "pro.vendor", IsChild = true, Order = 3 },

            // ===== PURCHASE ORDERS =====
            new() { ModKey = "mod.pro", Key = "pro.po", Label = "Purchase Orders", Path = "", Icon = "ClipboardCheck", ParKey = "", IsChild = false, Order = 4 },
            new() { ModKey = "mod.pro", Key = "pro.po.create", Label = "Create PO", Path = "/procurement/po/create", Icon = "Plus", ParKey = "pro.po", IsChild = true, Order = 1 },
            new() { ModKey = "mod.pro", Key = "pro.po.list", Label = "PO List", Path = "/procurement/po", Icon = "List", ParKey = "pro.po", IsChild = true, Order = 2 },
            new() { ModKey = "mod.pro", Key = "pro.po.approval", Label = "PO Approval", Path = "/procurement/po/approval", Icon = "CheckCircle", ParKey = "pro.po", IsChild = true, Order = 3 },
            new() { ModKey = "mod.pro", Key = "pro.po.tracking", Label = "PO Tracking", Path = "/procurement/po/tracking", Icon = "MapPin", ParKey = "pro.po", IsChild = true, Order = 4 },

            // ===== GOODS RECEIPT =====
            new() { ModKey = "mod.pro", Key = "pro.receipt", Label = "Goods Receipt", Path = "", Icon = "CheckCircle2", ParKey = "", IsChild = false, Order = 5 },
            new() { ModKey = "mod.pro", Key = "pro.receipt.create", Label = "Create GRN", Path = "/procurement/receipt/create", Icon = "Plus", ParKey = "pro.receipt", IsChild = true, Order = 1 },
            new() { ModKey = "mod.pro", Key = "pro.receipt.list", Label = "GRN List", Path = "/procurement/receipt", Icon = "List", ParKey = "pro.receipt", IsChild = true, Order = 2 },
            new() { ModKey = "mod.pro", Key = "pro.receipt.inspection", Label = "Quality Inspection", Path = "/procurement/inspection", Icon = "ClipboardCheck", ParKey = "pro.receipt", IsChild = true, Order = 3 },

            // ===== INVOICE MANAGEMENT =====
            new() { ModKey = "mod.pro", Key = "pro.invoice", Label = "Invoice Management", Path = "", Icon = "FileCheck", ParKey = "", IsChild = false, Order = 6 },
            new() { ModKey = "mod.pro", Key = "pro.invoice.verify", Label = "Invoice Verification", Path = "/procurement/invoice/verify", Icon = "CheckCircle", ParKey = "pro.invoice", IsChild = true, Order = 1 },
            new() { ModKey = "mod.pro", Key = "pro.invoice.list", Label = "Invoice List", Path = "/procurement/invoice", Icon = "List", ParKey = "pro.invoice", IsChild = true, Order = 2 },
            new() { ModKey = "mod.pro", Key = "pro.invoice.payment", Label = "Payment Processing", Path = "/procurement/invoice/payment", Icon = "DollarSign", ParKey = "pro.invoice", IsChild = true, Order = 3 },

            // ===== ANALYTICS =====
            new() { ModKey = "mod.pro", Key = "pro.analytics", Label = "Analytics", Path = "", Icon = "BarChart4", ParKey = "", IsChild = false, Order = 7 },
            new() { ModKey = "mod.pro", Key = "pro.analytics.spend", Label = "Spend Analysis", Path = "/procurement/analytics/spend", Icon = "PieChart", ParKey = "pro.analytics", IsChild = true, Order = 1 },
            new() { ModKey = "mod.pro", Key = "pro.analytics.vendor", Label = "Vendor Performance", Path = "/procurement/analytics/vendor", Icon = "TrendingUp", ParKey = "pro.analytics", IsChild = true, Order = 2 },
            new() { ModKey = "mod.pro", Key = "pro.analytics.report", Label = "Procurement Reports", Path = "/procurement/reports", Icon = "FileText", ParKey = "pro.analytics", IsChild = true, Order = 3 },
        };
    }
}