using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Seeder.Permissions;

public static class InventoryPermissionsSeeder
{
    public static IEnumerable<PerAccessSeedDto> GetPermissions()
    {
        return new List<PerAccessSeedDto>
        {
            // Dashboard
            new() { MenuKey = "inv.db", Key = "inv.db.view", Desc = "View Inventory Dashboard" },
            new() { MenuKey = "inv.db", Key = "inv.db.export", Desc = "Export Dashboard Data" },

            // Product Management
            new() { MenuKey = "inv.products", Key = "inv.products.view", Desc = "Access Product Management" },
            new() { MenuKey = "inv.products", Key = "inv.products.manage", Desc = "Manage Products" },

            new() { MenuKey = "inv.products.list", Key = "inv.products.list.view", Desc = "View Products" },
            new() { MenuKey = "inv.products.list", Key = "inv.products.list.add", Desc = "Add Product" },
            new() { MenuKey = "inv.products.list", Key = "inv.products.list.mod", Desc = "Edit Product" },
            new() { MenuKey = "inv.products.list", Key = "inv.products.list.del", Desc = "Delete Product" },
            new() { MenuKey = "inv.products.list", Key = "inv.products.list.export", Desc = "Export Products" },
            new() { MenuKey = "inv.products.list", Key = "inv.products.list.import", Desc = "Import Products" },
            new() { MenuKey = "inv.products.list", Key = "inv.products.list.copy", Desc = "Copy Product" },
            new() { MenuKey = "inv.products.list", Key = "inv.products.list.activate", Desc = "Activate Product" },
            new() { MenuKey = "inv.products.list", Key = "inv.products.list.deactivate", Desc = "Deactivate Product" },
            new() { MenuKey = "inv.products.list", Key = "inv.products.list.price", Desc = "Update Pricing" },
            new() { MenuKey = "inv.products.list", Key = "inv.products.list.image", Desc = "Manage Product Images" },

            new() { MenuKey = "inv.products.category", Key = "inv.products.category.view", Desc = "View Categories" },
            new() { MenuKey = "inv.products.category", Key = "inv.products.category.add", Desc = "Add Category" },
            new() { MenuKey = "inv.products.category", Key = "inv.products.category.mod", Desc = "Edit Category" },
            new() { MenuKey = "inv.products.category", Key = "inv.products.category.del", Desc = "Delete Category" },
            new() { MenuKey = "inv.products.category", Key = "inv.products.category.export", Desc = "Export Categories" },
            new() { MenuKey = "inv.products.category", Key = "inv.products.category.sub", Desc = "Manage Subcategories" },

            new() { MenuKey = "inv.products.unit", Key = "inv.products.unit.view", Desc = "View Units" },
            new() { MenuKey = "inv.products.unit", Key = "inv.products.unit.add", Desc = "Add Unit" },
            new() { MenuKey = "inv.products.unit", Key = "inv.products.unit.mod", Desc = "Edit Unit" },
            new() { MenuKey = "inv.products.unit", Key = "inv.products.unit.del", Desc = "Delete Unit" },
            new() { MenuKey = "inv.products.unit", Key = "inv.products.unit.conversion", Desc = "Set Unit Conversion" },

            new() { MenuKey = "inv.products.barcode", Key = "inv.products.barcode.view", Desc = "View Barcodes" },
            new() { MenuKey = "inv.products.barcode", Key = "inv.products.barcode.generate", Desc = "Generate Barcode" },
            new() { MenuKey = "inv.products.barcode", Key = "inv.products.barcode.print", Desc = "Print Barcode" },
            new() { MenuKey = "inv.products.barcode", Key = "inv.products.barcode.bulk", Desc = "Bulk Barcode Generation" },
            new() { MenuKey = "inv.products.barcode", Key = "inv.products.barcode.scan", Desc = "Scan Barcode" },

            // Stock Management
            new() { MenuKey = "inv.stock", Key = "inv.stock.view", Desc = "Access Stock Management" },
            new() { MenuKey = "inv.stock", Key = "inv.stock.manage", Desc = "Manage Stock" },

            new() { MenuKey = "inv.stock.inbound", Key = "inv.stock.inbound.view", Desc = "View Stock In" },
            new() { MenuKey = "inv.stock.inbound", Key = "inv.stock.inbound.add", Desc = "Create Stock In" },
            new() { MenuKey = "inv.stock.inbound", Key = "inv.stock.inbound.mod", Desc = "Edit Stock In" },
            new() { MenuKey = "inv.stock.inbound", Key = "inv.stock.inbound.del", Desc = "Delete Stock In" },
            new() { MenuKey = "inv.stock.inbound", Key = "inv.stock.inbound.approve", Desc = "Approve Stock In" },
            new() { MenuKey = "inv.stock.inbound", Key = "inv.stock.inbound.reject", Desc = "Reject Stock In" },
            new() { MenuKey = "inv.stock.inbound", Key = "inv.stock.inbound.export", Desc = "Export Stock In" },
            new() { MenuKey = "inv.stock.inbound", Key = "inv.stock.inbound.bulk", Desc = "Bulk Stock In" },

            new() { MenuKey = "inv.stock.outbound", Key = "inv.stock.outbound.view", Desc = "View Stock Out" },
            new() { MenuKey = "inv.stock.outbound", Key = "inv.stock.outbound.add", Desc = "Create Stock Out" },
            new() { MenuKey = "inv.stock.outbound", Key = "inv.stock.outbound.mod", Desc = "Edit Stock Out" },
            new() { MenuKey = "inv.stock.outbound", Key = "inv.stock.outbound.del", Desc = "Delete Stock Out" },
            new() { MenuKey = "inv.stock.outbound", Key = "inv.stock.outbound.approve", Desc = "Approve Stock Out" },
            new() { MenuKey = "inv.stock.outbound", Key = "inv.stock.outbound.export", Desc = "Export Stock Out" },
            new() { MenuKey = "inv.stock.outbound", Key = "inv.stock.outbound.bulk", Desc = "Bulk Stock Out" },

            new() { MenuKey = "inv.stock.transfer", Key = "inv.stock.transfer.view", Desc = "View Stock Transfers" },
            new() { MenuKey = "inv.stock.transfer", Key = "inv.stock.transfer.add", Desc = "Create Stock Transfer" },
            new() { MenuKey = "inv.stock.transfer", Key = "inv.stock.transfer.mod", Desc = "Edit Stock Transfer" },
            new() { MenuKey = "inv.stock.transfer", Key = "inv.stock.transfer.del", Desc = "Delete Stock Transfer" },
            new() { MenuKey = "inv.stock.transfer", Key = "inv.stock.transfer.approve", Desc = "Approve Transfer" },
            new() { MenuKey = "inv.stock.transfer", Key = "inv.stock.transfer.receive", Desc = "Receive Transfer" },
            new() { MenuKey = "inv.stock.transfer", Key = "inv.stock.transfer.export", Desc = "Export Transfers" },

            new() { MenuKey = "inv.stock.adjustment", Key = "inv.stock.adjustment.view", Desc = "View Adjustments" },
            new() { MenuKey = "inv.stock.adjustment", Key = "inv.stock.adjustment.add", Desc = "Create Adjustment" },
            new() { MenuKey = "inv.stock.adjustment", Key = "inv.stock.adjustment.mod", Desc = "Edit Adjustment" },
            new() { MenuKey = "inv.stock.adjustment", Key = "inv.stock.adjustment.del", Desc = "Delete Adjustment" },
            new() { MenuKey = "inv.stock.adjustment", Key = "inv.stock.adjustment.approve", Desc = "Approve Adjustment" },
            new() { MenuKey = "inv.stock.adjustment", Key = "inv.stock.adjustment.reason", Desc = "Manage Adjustment Reasons" },

            new() { MenuKey = "inv.stock.count", Key = "inv.stock.count.view", Desc = "View Stock Counts" },
            new() { MenuKey = "inv.stock.count", Key = "inv.stock.count.create", Desc = "Create Stock Count" },
            new() { MenuKey = "inv.stock.count", Key = "inv.stock.count.perform", Desc = "Perform Stock Count" },
            new() { MenuKey = "inv.stock.count", Key = "inv.stock.count.approve", Desc = "Approve Stock Count" },
            new() { MenuKey = "inv.stock.count", Key = "inv.stock.count.reconcile", Desc = "Reconcile Stock Count" },
            new() { MenuKey = "inv.stock.count", Key = "inv.stock.count.schedule", Desc = "Schedule Stock Count" },
            new() { MenuKey = "inv.stock.count", Key = "inv.stock.count.export", Desc = "Export Stock Count" },
            new() { MenuKey = "inv.stock.count", Key = "inv.stock.count.variance", Desc = "View Variance Report" },

            // Warehouse Management
            new() { MenuKey = "inv.warehouse", Key = "inv.warehouse.view", Desc = "Access Warehouse Management" },
            new() { MenuKey = "inv.warehouse", Key = "inv.warehouse.manage", Desc = "Manage Warehouses" },

            new() { MenuKey = "inv.warehouse.list", Key = "inv.warehouse.list.view", Desc = "View Warehouses" },
            new() { MenuKey = "inv.warehouse.list", Key = "inv.warehouse.list.add", Desc = "Add Warehouse" },
            new() { MenuKey = "inv.warehouse.list", Key = "inv.warehouse.list.mod", Desc = "Edit Warehouse" },
            new() { MenuKey = "inv.warehouse.list", Key = "inv.warehouse.list.del", Desc = "Delete Warehouse" },
            new() { MenuKey = "inv.warehouse.list", Key = "inv.warehouse.list.activate", Desc = "Activate Warehouse" },
            new() { MenuKey = "inv.warehouse.list", Key = "inv.warehouse.list.deactivate", Desc = "Deactivate Warehouse" },

            new() { MenuKey = "inv.warehouse.zone", Key = "inv.warehouse.zone.view", Desc = "View Zones" },
            new() { MenuKey = "inv.warehouse.zone", Key = "inv.warehouse.zone.add", Desc = "Add Zone" },
            new() { MenuKey = "inv.warehouse.zone", Key = "inv.warehouse.zone.mod", Desc = "Edit Zone" },
            new() { MenuKey = "inv.warehouse.zone", Key = "inv.warehouse.zone.del", Desc = "Delete Zone" },
            new() { MenuKey = "inv.warehouse.zone", Key = "inv.warehouse.zone.bin", Desc = "Manage Bins/Racks" },

            new() { MenuKey = "inv.warehouse.layout", Key = "inv.warehouse.layout.view", Desc = "View Layout" },
            new() { MenuKey = "inv.warehouse.layout", Key = "inv.warehouse.layout.mod", Desc = "Edit Layout" },
            new() { MenuKey = "inv.warehouse.layout", Key = "inv.warehouse.layout.design", Desc = "Design Layout" },
            new() { MenuKey = "inv.warehouse.layout", Key = "inv.warehouse.layout.print", Desc = "Print Layout" },

            // Inventory Valuation
            new() { MenuKey = "inv.valuation", Key = "inv.valuation.view", Desc = "Access Inventory Valuation" },
            new() { MenuKey = "inv.valuation", Key = "inv.valuation.manage", Desc = "Manage Valuation" },

            new() { MenuKey = "inv.valuation.method", Key = "inv.valuation.method.view", Desc = "View Valuation Methods" },
            new() { MenuKey = "inv.valuation.method", Key = "inv.valuation.method.mod", Desc = "Edit Valuation Methods" },
            new() { MenuKey = "inv.valuation.method", Key = "inv.valuation.method.fifo", Desc = "Configure FIFO" },
            new() { MenuKey = "inv.valuation.method", Key = "inv.valuation.method.lifo", Desc = "Configure LIFO" },
            new() { MenuKey = "inv.valuation.method", Key = "inv.valuation.method.avg", Desc = "Configure Weighted Average" },

            new() { MenuKey = "inv.valuation.report", Key = "inv.valuation.report.view", Desc = "View Valuation Reports" },
            new() { MenuKey = "inv.valuation.report", Key = "inv.valuation.report.export", Desc = "Export Valuation Reports" },
            new() { MenuKey = "inv.valuation.report", Key = "inv.valuation.report.summary", Desc = "Valuation Summary" },
            new() { MenuKey = "inv.valuation.report", Key = "inv.valuation.report.detail", Desc = "Valuation Details" },

            // Reorder Management
            new() { MenuKey = "inv.reorder", Key = "inv.reorder.view", Desc = "Access Reorder Management" },
            new() { MenuKey = "inv.reorder", Key = "inv.reorder.manage", Desc = "Manage Reorder" },

            new() { MenuKey = "inv.reorder.level", Key = "inv.reorder.level.view", Desc = "View Reorder Levels" },
            new() { MenuKey = "inv.reorder.level", Key = "inv.reorder.level.mod", Desc = "Set Reorder Levels" },
            new() { MenuKey = "inv.reorder.level", Key = "inv.reorder.level.bulk", Desc = "Bulk Update Levels" },
            new() { MenuKey = "inv.reorder.level", Key = "inv.reorder.level.alert", Desc = "Configure Alerts" },

            new() { MenuKey = "inv.reorder.request", Key = "inv.reorder.request.view", Desc = "View Reorder Requests" },
            new() { MenuKey = "inv.reorder.request", Key = "inv.reorder.request.create", Desc = "Create Reorder Request" },
            new() { MenuKey = "inv.reorder.request", Key = "inv.reorder.request.approve", Desc = "Approve Reorder Request" },
            new() { MenuKey = "inv.reorder.request", Key = "inv.reorder.request.reject", Desc = "Reject Reorder Request" },
            new() { MenuKey = "inv.reorder.request", Key = "inv.reorder.request.convert", Desc = "Convert to PO" },
            new() { MenuKey = "inv.reorder.request", Key = "inv.reorder.request.export", Desc = "Export Requests" },

            // Analytics
            new() { MenuKey = "inv.analytics", Key = "inv.analytics.view", Desc = "Access Inventory Analytics" },
            new() { MenuKey = "inv.analytics", Key = "inv.analytics.manage", Desc = "Manage Analytics" },

            new() { MenuKey = "inv.analytics.stock", Key = "inv.analytics.stock.view", Desc = "View Stock Reports" },
            new() { MenuKey = "inv.analytics.stock", Key = "inv.analytics.stock.export", Desc = "Export Stock Reports" },
            new() { MenuKey = "inv.analytics.stock", Key = "inv.analytics.stock.summary", Desc = "Stock Summary" },
            new() { MenuKey = "inv.analytics.stock", Key = "inv.analytics.stock.value", Desc = "Stock Value Report" },

            new() { MenuKey = "inv.analytics.movement", Key = "inv.analytics.movement.view", Desc = "View Movement Reports" },
            new() { MenuKey = "inv.analytics.movement", Key = "inv.analytics.movement.export", Desc = "Export Movement Reports" },
            new() { MenuKey = "inv.analytics.movement", Key = "inv.analytics.movement.slow", Desc = "Slow Moving Report" },
            new() { MenuKey = "inv.analytics.movement", Key = "inv.analytics.movement.fast", Desc = "Fast Moving Report" },

            new() { MenuKey = "inv.analytics.forecast", Key = "inv.analytics.forecast.view", Desc = "View Demand Forecast" },
            new() { MenuKey = "inv.analytics.forecast", Key = "inv.analytics.forecast.generate", Desc = "Generate Forecast" },
            new() { MenuKey = "inv.analytics.forecast", Key = "inv.analytics.forecast.export", Desc = "Export Forecast" },
            new() { MenuKey = "inv.analytics.forecast", Key = "inv.analytics.forecast.seasonal", Desc = "Seasonal Analysis" },
        };
    }
}