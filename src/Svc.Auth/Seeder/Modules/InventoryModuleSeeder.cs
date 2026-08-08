using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Seeder.Modules;

public static class InventoryModuleSeeder
{
    public static IEnumerable<PerMenuSeedDto> GetMenus()
    {
        return new List<PerMenuSeedDto>
        {
            // ===== DASHBOARD =====
            new() { ModKey = "mod.inv", Key = "inv.db", Label = "Dashboard", Path = "/inventory", Icon = "LayoutDashboard", ParKey = "", IsChild = false, Order = 1 },

            // ===== PRODUCT MANAGEMENT =====
            new() { ModKey = "mod.inv", Key = "inv.products", Label = "Product Management", Path = "", Icon = "Package", ParKey = "", IsChild = false, Order = 2 },
            new() { ModKey = "mod.inv", Key = "inv.products.list", Label = "Product List", Path = "/inventory/products", Icon = "List", ParKey = "inv.products", IsChild = true, Order = 1 },
            new() { ModKey = "mod.inv", Key = "inv.products.category", Label = "Categories", Path = "/inventory/categories", Icon = "Folder", ParKey = "inv.products", IsChild = true, Order = 2 },
            new() { ModKey = "mod.inv", Key = "inv.products.unit", Label = "Units of Measure", Path = "/inventory/units", Icon = "Ruler", ParKey = "inv.products", IsChild = true, Order = 3 },
            new() { ModKey = "mod.inv", Key = "inv.products.barcode", Label = "Barcode Management", Path = "/inventory/barcodes", Icon = "QrCode", ParKey = "inv.products", IsChild = true, Order = 4 },

            // ===== STOCK MANAGEMENT =====
            new() { ModKey = "mod.inv", Key = "inv.stock", Label = "Stock Management", Path = "", Icon = "Package", ParKey = "", IsChild = false, Order = 3 },
            new() { ModKey = "mod.inv", Key = "inv.stock.inbound", Label = "Stock In", Path = "/inventory/stock-in", Icon = "ArrowDown", ParKey = "inv.stock", IsChild = true, Order = 1 },
            new() { ModKey = "mod.inv", Key = "inv.stock.outbound", Label = "Stock Out", Path = "/inventory/stock-out", Icon = "ArrowUp", ParKey = "inv.stock", IsChild = true, Order = 2 },
            new() { ModKey = "mod.inv", Key = "inv.stock.transfer", Label = "Stock Transfer", Path = "/inventory/stock-transfer", Icon = "Move", ParKey = "inv.stock", IsChild = true, Order = 3 },
            new() { ModKey = "mod.inv", Key = "inv.stock.adjustment", Label = "Stock Adjustment", Path = "/inventory/stock-adjustment", Icon = "Sliders", ParKey = "inv.stock", IsChild = true, Order = 4 },
            new() { ModKey = "mod.inv", Key = "inv.stock.count", Label = "Stock Count", Path = "/inventory/stock-count", Icon = "CheckSquare", ParKey = "inv.stock", IsChild = true, Order = 5 },

            // ===== WAREHOUSE MANAGEMENT =====
            new() { ModKey = "mod.inv", Key = "inv.warehouse", Label = "Warehouse Management", Path = "", Icon = "Warehouse", ParKey = "", IsChild = false, Order = 4 },
            new() { ModKey = "mod.inv", Key = "inv.warehouse.list", Label = "Warehouses", Path = "/inventory/warehouses", Icon = "Building", ParKey = "inv.warehouse", IsChild = true, Order = 1 },
            new() { ModKey = "mod.inv", Key = "inv.warehouse.zone", Label = "Zones & Bins", Path = "/inventory/warehouse-zones", Icon = "Map", ParKey = "inv.warehouse", IsChild = true, Order = 2 },
            new() { ModKey = "mod.inv", Key = "inv.warehouse.layout", Label = "Warehouse Layout", Path = "/inventory/warehouse-layout", Icon = "Layout", ParKey = "inv.warehouse", IsChild = true, Order = 3 },

            // ===== INVENTORY VALUATION =====
            new() { ModKey = "mod.inv", Key = "inv.valuation", Label = "Inventory Valuation", Path = "", Icon = "BarChart4", ParKey = "", IsChild = false, Order = 5 },
            new() { ModKey = "mod.inv", Key = "inv.valuation.method", Label = "Valuation Methods", Path = "/inventory/valuation-methods", Icon = "Settings", ParKey = "inv.valuation", IsChild = true, Order = 1 },
            new() { ModKey = "mod.inv", Key = "inv.valuation.report", Label = "Valuation Report", Path = "/inventory/valuation-report", Icon = "FileText", ParKey = "inv.valuation", IsChild = true, Order = 2 },

            // ===== REORDER MANAGEMENT =====
            new() { ModKey = "mod.inv", Key = "inv.reorder", Label = "Reorder Management", Path = "", Icon = "RefreshCw", ParKey = "", IsChild = false, Order = 6 },
            new() { ModKey = "mod.inv", Key = "inv.reorder.level", Label = "Reorder Levels", Path = "/inventory/reorder-levels", Icon = "AlertTriangle", ParKey = "inv.reorder", IsChild = true, Order = 1 },
            new() { ModKey = "mod.inv", Key = "inv.reorder.request", Label = "Reorder Requests", Path = "/inventory/reorder-requests", Icon = "ShoppingCart", ParKey = "inv.reorder", IsChild = true, Order = 2 },

            // ===== ANALYTICS =====
            new() { ModKey = "mod.inv", Key = "inv.analytics", Label = "Analytics", Path = "", Icon = "BarChart4", ParKey = "", IsChild = false, Order = 7 },
            new() { ModKey = "mod.inv", Key = "inv.analytics.stock", Label = "Stock Reports", Path = "/inventory/stock-reports", Icon = "FileText", ParKey = "inv.analytics", IsChild = true, Order = 1 },
            new() { ModKey = "mod.inv", Key = "inv.analytics.movement", Label = "Movement Reports", Path = "/inventory/movement-reports", Icon = "Activity", ParKey = "inv.analytics", IsChild = true, Order = 2 },
            new() { ModKey = "mod.inv", Key = "inv.analytics.forecast", Label = "Demand Forecast", Path = "/inventory/forecast", Icon = "TrendingUp", ParKey = "inv.analytics", IsChild = true, Order = 3 },
        };
    }
}