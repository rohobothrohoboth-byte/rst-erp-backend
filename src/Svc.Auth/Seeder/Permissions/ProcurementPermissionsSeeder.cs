using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Seeder.Permissions;

public static class ProcurementPermissionsSeeder
{
    public static IEnumerable<PerAccessSeedDto> GetPermissions()
    {
        return new List<PerAccessSeedDto>
        {
            // Dashboard
            new() { MenuKey = "pro.db", Key = "pro.db.view", Desc = "View Procurement Dashboard" },
            new() { MenuKey = "pro.db", Key = "pro.db.export", Desc = "Export Dashboard Data" },

            // Requisitions
            new() { MenuKey = "pro.req", Key = "pro.req.view", Desc = "Access Requisitions" },
            new() { MenuKey = "pro.req", Key = "pro.req.manage", Desc = "Manage Requisitions" },

            new() { MenuKey = "pro.req.create", Key = "pro.req.create.add", Desc = "Create Requisition" },
            new() { MenuKey = "pro.req.create", Key = "pro.req.create.template", Desc = "Use Templates" },

            new() { MenuKey = "pro.req.list", Key = "pro.req.list.view", Desc = "View Requisitions" },
            new() { MenuKey = "pro.req.list", Key = "pro.req.list.mod", Desc = "Edit Requisition" },
            new() { MenuKey = "pro.req.list", Key = "pro.req.list.del", Desc = "Delete Requisition" },
            new() { MenuKey = "pro.req.list", Key = "pro.req.list.copy", Desc = "Copy Requisition" },
            new() { MenuKey = "pro.req.list", Key = "pro.req.list.export", Desc = "Export Requisitions" },

            new() { MenuKey = "pro.req.approval", Key = "pro.req.approval.view", Desc = "View Requisition Approvals" },
            new() { MenuKey = "pro.req.approval", Key = "pro.req.approval.approve", Desc = "Approve Requisition" },
            new() { MenuKey = "pro.req.approval", Key = "pro.req.approval.reject", Desc = "Reject Requisition" },
            new() { MenuKey = "pro.req.approval", Key = "pro.req.approval.return", Desc = "Return for Revision" },
            new() { MenuKey = "pro.req.approval", Key = "pro.req.approval.bulk", Desc = "Bulk Approval" },
            new() { MenuKey = "pro.req.approval", Key = "pro.req.approval.workflow", Desc = "Configure Workflow" },

            // Vendor Management
            new() { MenuKey = "pro.vendor", Key = "pro.vendor.view", Desc = "Access Vendor Management" },
            new() { MenuKey = "pro.vendor", Key = "pro.vendor.manage", Desc = "Manage Vendors" },

            new() { MenuKey = "pro.vendor.list", Key = "pro.vendor.list.view", Desc = "View Vendors" },
            new() { MenuKey = "pro.vendor.list", Key = "pro.vendor.list.add", Desc = "Add Vendor" },
            new() { MenuKey = "pro.vendor.list", Key = "pro.vendor.list.mod", Desc = "Edit Vendor" },
            new() { MenuKey = "pro.vendor.list", Key = "pro.vendor.list.del", Desc = "Delete Vendor" },
            new() { MenuKey = "pro.vendor.list", Key = "pro.vendor.list.export", Desc = "Export Vendors" },
            new() { MenuKey = "pro.vendor.list", Key = "pro.vendor.list.import", Desc = "Import Vendors" },
            new() { MenuKey = "pro.vendor.list", Key = "pro.vendor.list.approve", Desc = "Approve Vendor" },
            new() { MenuKey = "pro.vendor.list", Key = "pro.vendor.list.suspend", Desc = "Suspend Vendor" },
            new() { MenuKey = "pro.vendor.list", Key = "pro.vendor.list.category", Desc = "Manage Categories" },
            new() { MenuKey = "pro.vendor.list", Key = "pro.vendor.list.rating", Desc = "Rate Vendor" },

            new() { MenuKey = "pro.vendor.evaluation", Key = "pro.vendor.evaluation.view", Desc = "View Evaluations" },
            new() { MenuKey = "pro.vendor.evaluation", Key = "pro.vendor.evaluation.add", Desc = "Create Evaluation" },
            new() { MenuKey = "pro.vendor.evaluation", Key = "pro.vendor.evaluation.mod", Desc = "Edit Evaluation" },
            new() { MenuKey = "pro.vendor.evaluation", Key = "pro.vendor.evaluation.del", Desc = "Delete Evaluation" },
            new() { MenuKey = "pro.vendor.evaluation", Key = "pro.vendor.evaluation.scorecard", Desc = "Manage Scorecards" },
            new() { MenuKey = "pro.vendor.evaluation", Key = "pro.vendor.evaluation.report", Desc = "Evaluation Report" },

            new() { MenuKey = "pro.vendor.contract", Key = "pro.vendor.contract.view", Desc = "View Contracts" },
            new() { MenuKey = "pro.vendor.contract", Key = "pro.vendor.contract.add", Desc = "Create Contract" },
            new() { MenuKey = "pro.vendor.contract", Key = "pro.vendor.contract.mod", Desc = "Edit Contract" },
            new() { MenuKey = "pro.vendor.contract", Key = "pro.vendor.contract.del", Desc = "Delete Contract" },
            new() { MenuKey = "pro.vendor.contract", Key = "pro.vendor.contract.renew", Desc = "Renew Contract" },
            new() { MenuKey = "pro.vendor.contract", Key = "pro.vendor.contract.terminate", Desc = "Terminate Contract" },
            new() { MenuKey = "pro.vendor.contract", Key = "pro.vendor.contract.template", Desc = "Contract Templates" },

            // Purchase Orders
            new() { MenuKey = "pro.po", Key = "pro.po.view", Desc = "Access Purchase Orders" },
            new() { MenuKey = "pro.po", Key = "pro.po.manage", Desc = "Manage Purchase Orders" },

            new() { MenuKey = "pro.po.create", Key = "pro.po.create.add", Desc = "Create PO" },
            new() { MenuKey = "pro.po.create", Key = "pro.po.create.from_req", Desc = "Create from Requisition" },

            new() { MenuKey = "pro.po.list", Key = "pro.po.list.view", Desc = "View POs" },
            new() { MenuKey = "pro.po.list", Key = "pro.po.list.mod", Desc = "Edit PO" },
            new() { MenuKey = "pro.po.list", Key = "pro.po.list.del", Desc = "Delete PO" },
            new() { MenuKey = "pro.po.list", Key = "pro.po.list.send", Desc = "Send PO to Vendor" },
            new() { MenuKey = "pro.po.list", Key = "pro.po.list.copy", Desc = "Copy PO" },
            new() { MenuKey = "pro.po.list", Key = "pro.po.list.export", Desc = "Export POs" },
            new() { MenuKey = "pro.po.list", Key = "pro.po.list.print", Desc = "Print PO" },

            new() { MenuKey = "pro.po.approval", Key = "pro.po.approval.view", Desc = "View PO Approvals" },
            new() { MenuKey = "pro.po.approval", Key = "pro.po.approval.approve", Desc = "Approve PO" },
            new() { MenuKey = "pro.po.approval", Key = "pro.po.approval.reject", Desc = "Reject PO" },
            new() { MenuKey = "pro.po.approval", Key = "pro.po.approval.bulk", Desc = "Bulk Approval" },

            new() { MenuKey = "pro.po.tracking", Key = "pro.po.tracking.view", Desc = "Track PO Status" },
            new() { MenuKey = "pro.po.tracking", Key = "pro.po.tracking.update", Desc = "Update PO Status" },
            new() { MenuKey = "pro.po.tracking", Key = "pro.po.tracking.report", Desc = "PO Tracking Report" },

            // Goods Receipt
            new() { MenuKey = "pro.receipt", Key = "pro.receipt.view", Desc = "Access Goods Receipt" },
            new() { MenuKey = "pro.receipt", Key = "pro.receipt.manage", Desc = "Manage Goods Receipt" },

            new() { MenuKey = "pro.receipt.create", Key = "pro.receipt.create.add", Desc = "Create GRN" },
            new() { MenuKey = "pro.receipt.create", Key = "pro.receipt.create.from_po", Desc = "Create from PO" },

            new() { MenuKey = "pro.receipt.list", Key = "pro.receipt.list.view", Desc = "View GRNs" },
            new() { MenuKey = "pro.receipt.list", Key = "pro.receipt.list.mod", Desc = "Edit GRN" },
            new() { MenuKey = "pro.receipt.list", Key = "pro.receipt.list.del", Desc = "Delete GRN" },
            new() { MenuKey = "pro.receipt.list", Key = "pro.receipt.list.export", Desc = "Export GRNs" },

            new() { MenuKey = "pro.receipt.inspection", Key = "pro.receipt.inspection.view", Desc = "View Inspections" },
            new() { MenuKey = "pro.receipt.inspection", Key = "pro.receipt.inspection.perform", Desc = "Perform Inspection" },
            new() { MenuKey = "pro.receipt.inspection", Key = "pro.receipt.inspection.approve", Desc = "Approve Receipt" },
            new() { MenuKey = "pro.receipt.inspection", Key = "pro.receipt.inspection.reject", Desc = "Reject Receipt" },
            new() { MenuKey = "pro.receipt.inspection", Key = "pro.receipt.inspection.qc", Desc = "Quality Control Checklist" },

            // Invoice Management
            new() { MenuKey = "pro.invoice", Key = "pro.invoice.view", Desc = "Access Invoice Management" },
            new() { MenuKey = "pro.invoice", Key = "pro.invoice.manage", Desc = "Manage Invoices" },

            new() { MenuKey = "pro.invoice.verify", Key = "pro.invoice.verify.process", Desc = "Verify Invoice" },
            new() { MenuKey = "pro.invoice.verify", Key = "pro.invoice.verify.match", Desc = "Match with PO/GRN" },
            new() { MenuKey = "pro.invoice.verify", Key = "pro.invoice.verify.discrepancy", Desc = "Handle Discrepancies" },

            new() { MenuKey = "pro.invoice.list", Key = "pro.invoice.list.view", Desc = "View Invoices" },
            new() { MenuKey = "pro.invoice.list", Key = "pro.invoice.list.mod", Desc = "Edit Invoice" },
            new() { MenuKey = "pro.invoice.list", Key = "pro.invoice.list.del", Desc = "Delete Invoice" },
            new() { MenuKey = "pro.invoice.list", Key = "pro.invoice.list.export", Desc = "Export Invoices" },

            new() { MenuKey = "pro.invoice.payment", Key = "pro.invoice.payment.process", Desc = "Process Payment" },
            new() { MenuKey = "pro.invoice.payment", Key = "pro.invoice.payment.approve", Desc = "Approve Payment" },
            new() { MenuKey = "pro.invoice.payment", Key = "pro.invoice.payment.schedule", Desc = "Schedule Payment" },
            new() { MenuKey = "pro.invoice.payment", Key = "pro.invoice.payment.history", Desc = "Payment History" },

            // Analytics
            new() { MenuKey = "pro.analytics", Key = "pro.analytics.view", Desc = "Access Procurement Analytics" },
            new() { MenuKey = "pro.analytics", Key = "pro.analytics.manage", Desc = "Manage Analytics" },

            new() { MenuKey = "pro.analytics.spend", Key = "pro.analytics.spend.view", Desc = "View Spend Analysis" },
            new() { MenuKey = "pro.analytics.spend", Key = "pro.analytics.spend.export", Desc = "Export Spend Analysis" },
            new() { MenuKey = "pro.analytics.spend", Key = "pro.analytics.spend.bycategory", Desc = "Spend by Category" },
            new() { MenuKey = "pro.analytics.spend", Key = "pro.analytics.spend.byvendor", Desc = "Spend by Vendor" },

            new() { MenuKey = "pro.analytics.vendor", Key = "pro.analytics.vendor.view", Desc = "View Vendor Performance" },
            new() { MenuKey = "pro.analytics.vendor", Key = "pro.analytics.vendor.export", Desc = "Export Vendor Performance" },
            new() { MenuKey = "pro.analytics.vendor", Key = "pro.analytics.vendor.on_time", Desc = "On-Time Delivery" },
            new() { MenuKey = "pro.analytics.vendor", Key = "pro.analytics.vendor.quality", Desc = "Quality Rating" },
            new() { MenuKey = "pro.analytics.vendor", Key = "pro.analytics.vendor.price", Desc = "Price Comparison" },

            new() { MenuKey = "pro.analytics.report", Key = "pro.analytics.report.view", Desc = "View Procurement Reports" },
            new() { MenuKey = "pro.analytics.report", Key = "pro.analytics.report.export", Desc = "Export Procurement Reports" },
            new() { MenuKey = "pro.analytics.report", Key = "pro.analytics.report.summary", Desc = "Procurement Summary" },
            new() { MenuKey = "pro.analytics.report", Key = "pro.analytics.report.savings", Desc = "Cost Savings Report" },
        };
    }
}