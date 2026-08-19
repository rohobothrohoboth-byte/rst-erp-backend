using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Seeder.Permissions;

public static class FinancePermissionsSeeder
{
    public static IEnumerable<PerAccessSeedDto> GetPermissions()
    {
        return new List<PerAccessSeedDto>
        {
            // ============================================
            // DASHBOARD
            // ============================================
            new() { MenuKey = "fnm.db", Key = "fnm.db.view", Desc = "View Finance Dashboard" },
            new() { MenuKey = "fnm.db", Key = "fnm.db.export", Desc = "Export Dashboard Data" },

            // ============================================
            // GENERAL LEDGER
            // ============================================
            new() { MenuKey = "fnm.gl", Key = "fnm.gl.view", Desc = "Access General Ledger" },
            new() { MenuKey = "fnm.gl", Key = "fnm.gl.manage", Desc = "Manage General Ledger" },

            new() { MenuKey = "fnm.gl.coa", Key = "fnm.gl.coa.view", Desc = "View Chart of Accounts" },
            new() { MenuKey = "fnm.gl.coa", Key = "fnm.gl.coa.add", Desc = "Add Account" },
            new() { MenuKey = "fnm.gl.coa", Key = "fnm.gl.coa.mod", Desc = "Edit Account" },
            new() { MenuKey = "fnm.gl.coa", Key = "fnm.gl.coa.del", Desc = "Delete Account" },
            new() { MenuKey = "fnm.gl.coa", Key = "fnm.gl.coa.import", Desc = "Import COA" },
            new() { MenuKey = "fnm.gl.coa", Key = "fnm.gl.coa.export", Desc = "Export COA" },
            new() { MenuKey = "fnm.gl.coa", Key = "fnm.gl.coa.activate", Desc = "Activate Account" },
            new() { MenuKey = "fnm.gl.coa", Key = "fnm.gl.coa.deactivate", Desc = "Deactivate Account" },

            new() { MenuKey = "fnm.gl.journal", Key = "fnm.gl.journal.view", Desc = "View Journal Entries" },
            new() { MenuKey = "fnm.gl.journal", Key = "fnm.gl.journal.add", Desc = "Create Journal Entry" },
            new() { MenuKey = "fnm.gl.journal", Key = "fnm.gl.journal.mod", Desc = "Edit Journal Entry" },
            new() { MenuKey = "fnm.gl.journal", Key = "fnm.gl.journal.del", Desc = "Delete Journal Entry" },
            new() { MenuKey = "fnm.gl.journal", Key = "fnm.gl.journal.approve", Desc = "Approve Journal Entry" },
            new() { MenuKey = "fnm.gl.journal", Key = "fnm.gl.journal.reverse", Desc = "Reverse Journal Entry" },
            new() { MenuKey = "fnm.gl.journal", Key = "fnm.gl.journal.export", Desc = "Export Journal Entries" },
            new() { MenuKey = "fnm.gl.journal", Key = "fnm.gl.journal.template", Desc = "Journal Templates" },

            new() { MenuKey = "fnm.gl.audit", Key = "fnm.gl.audit.view", Desc = "View Audit Trail" },
            new() { MenuKey = "fnm.gl.audit", Key = "fnm.gl.audit.export", Desc = "Export Audit Trail" },
            new() { MenuKey = "fnm.gl.audit", Key = "fnm.gl.audit.filter", Desc = "Filter Audit Logs" },

            new() { MenuKey = "fnm.gl.budget", Key = "fnm.gl.budget.view", Desc = "View Budget" },
            new() { MenuKey = "fnm.gl.budget", Key = "fnm.gl.budget.add", Desc = "Create Budget" },
            new() { MenuKey = "fnm.gl.budget", Key = "fnm.gl.budget.mod", Desc = "Edit Budget" },
            new() { MenuKey = "fnm.gl.budget", Key = "fnm.gl.budget.del", Desc = "Delete Budget" },
            new() { MenuKey = "fnm.gl.budget", Key = "fnm.gl.budget.copy", Desc = "Copy Budget" },
            new() { MenuKey = "fnm.gl.budget", Key = "fnm.gl.budget.approve", Desc = "Approve Budget" },
            new() { MenuKey = "fnm.gl.budget", Key = "fnm.gl.budget.variance", Desc = "View Variance" },

            new() { MenuKey = "fnm.gl.closing", Key = "fnm.gl.closing.view", Desc = "View Period Closing" },
            new() { MenuKey = "fnm.gl.closing", Key = "fnm.gl.closing.process", Desc = "Process Period Closing" },
            new() { MenuKey = "fnm.gl.closing", Key = "fnm.gl.closing.reopen", Desc = "Reopen Period" },
            new() { MenuKey = "fnm.gl.closing", Key = "fnm.gl.closing.report", Desc = "Closing Report" },

            // ============================================
            // VOUCHER MANAGEMENT ✅ NEW
            // ============================================
            new() { MenuKey = "fnm.gl.voucher", Key = "fnm.gl.voucher.view", Desc = "View Vouchers" },
            new() { MenuKey = "fnm.gl.voucher", Key = "fnm.gl.voucher.add", Desc = "Create Voucher" },
            new() { MenuKey = "fnm.gl.voucher", Key = "fnm.gl.voucher.mod", Desc = "Edit Voucher" },
            new() { MenuKey = "fnm.gl.voucher", Key = "fnm.gl.voucher.del", Desc = "Delete Voucher" },
            new() { MenuKey = "fnm.gl.voucher", Key = "fnm.gl.voucher.approve", Desc = "Approve Voucher" },
            new() { MenuKey = "fnm.gl.voucher", Key = "fnm.gl.voucher.reject", Desc = "Reject Voucher" },
            new() { MenuKey = "fnm.gl.voucher", Key = "fnm.gl.voucher.post", Desc = "Post Voucher to GL" },
            new() { MenuKey = "fnm.gl.voucher", Key = "fnm.gl.voucher.unpost", Desc = "Unpost Voucher" },
            new() { MenuKey = "fnm.gl.voucher", Key = "fnm.gl.voucher.export", Desc = "Export Vouchers" },
            new() { MenuKey = "fnm.gl.voucher", Key = "fnm.gl.voucher.void", Desc = "Void Voucher" },

            // ============================================
            // ACCOUNTS PAYABLE
            // ============================================
            new() { MenuKey = "fnm.ap", Key = "fnm.ap.view", Desc = "Access Accounts Payable" },
            new() { MenuKey = "fnm.ap", Key = "fnm.ap.manage", Desc = "Manage AP" },

            new() { MenuKey = "fnm.ap.vendor", Key = "fnm.ap.vendor.view", Desc = "View Vendors" },
            new() { MenuKey = "fnm.ap.vendor", Key = "fnm.ap.vendor.add", Desc = "Add Vendor" },
            new() { MenuKey = "fnm.ap.vendor", Key = "fnm.ap.vendor.mod", Desc = "Edit Vendor" },
            new() { MenuKey = "fnm.ap.vendor", Key = "fnm.ap.vendor.del", Desc = "Delete Vendor" },
            new() { MenuKey = "fnm.ap.vendor", Key = "fnm.ap.vendor.export", Desc = "Export Vendors" },
            new() { MenuKey = "fnm.ap.vendor", Key = "fnm.ap.vendor.import", Desc = "Import Vendors" },
            new() { MenuKey = "fnm.ap.vendor", Key = "fnm.ap.vendor.approve", Desc = "Approve Vendor" },

            new() { MenuKey = "fnm.ap.invoice", Key = "fnm.ap.invoice.view", Desc = "View Invoices" },
            new() { MenuKey = "fnm.ap.invoice", Key = "fnm.ap.invoice.add", Desc = "Create Invoice" },
            new() { MenuKey = "fnm.ap.invoice", Key = "fnm.ap.invoice.mod", Desc = "Edit Invoice" },
            new() { MenuKey = "fnm.ap.invoice", Key = "fnm.ap.invoice.del", Desc = "Delete Invoice" },
            new() { MenuKey = "fnm.ap.invoice", Key = "fnm.ap.invoice.approve", Desc = "Approve Invoice" },
            new() { MenuKey = "fnm.ap.invoice", Key = "fnm.ap.invoice.reject", Desc = "Reject Invoice" },
            new() { MenuKey = "fnm.ap.invoice", Key = "fnm.ap.invoice.export", Desc = "Export Invoices" },
            new() { MenuKey = "fnm.ap.invoice", Key = "fnm.ap.invoice.match", Desc = "Match with PO/GRN" },

            new() { MenuKey = "fnm.ap.payment", Key = "fnm.ap.payment.view", Desc = "View Payments" },
            new() { MenuKey = "fnm.ap.payment", Key = "fnm.ap.payment.add", Desc = "Process Payment" },
            new() { MenuKey = "fnm.ap.payment", Key = "fnm.ap.payment.mod", Desc = "Edit Payment" },
            new() { MenuKey = "fnm.ap.payment", Key = "fnm.ap.payment.del", Desc = "Delete Payment" },
            new() { MenuKey = "fnm.ap.payment", Key = "fnm.ap.payment.approve", Desc = "Approve Payment" },
            new() { MenuKey = "fnm.ap.payment", Key = "fnm.ap.payment.bulk", Desc = "Bulk Payment" },
            new() { MenuKey = "fnm.ap.payment", Key = "fnm.ap.payment.export", Desc = "Export Payments" },

            new() { MenuKey = "fnm.ap.approval", Key = "fnm.ap.approval.view", Desc = "View Approvals" },
            new() { MenuKey = "fnm.ap.approval", Key = "fnm.ap.approval.process", Desc = "Approve/Reject Invoice" },
            new() { MenuKey = "fnm.ap.approval", Key = "fnm.ap.approval.workflow", Desc = "Configure Workflow" },

            new() { MenuKey = "fnm.ap.report", Key = "fnm.ap.report.view", Desc = "View AP Reports" },
            new() { MenuKey = "fnm.ap.report", Key = "fnm.ap.report.export", Desc = "Export AP Reports" },
            new() { MenuKey = "fnm.ap.report", Key = "fnm.ap.report.aging", Desc = "Aging Report" },

            // ============================================
            // ACCOUNTS RECEIVABLE
            // ============================================
            new() { MenuKey = "fnm.ar", Key = "fnm.ar.view", Desc = "Access Accounts Receivable" },
            new() { MenuKey = "fnm.ar", Key = "fnm.ar.manage", Desc = "Manage AR" },

            new() { MenuKey = "fnm.ar.customer", Key = "fnm.ar.customer.view", Desc = "View Customers" },
            new() { MenuKey = "fnm.ar.customer", Key = "fnm.ar.customer.add", Desc = "Add Customer" },
            new() { MenuKey = "fnm.ar.customer", Key = "fnm.ar.customer.mod", Desc = "Edit Customer" },
            new() { MenuKey = "fnm.ar.customer", Key = "fnm.ar.customer.del", Desc = "Delete Customer" },
            new() { MenuKey = "fnm.ar.customer", Key = "fnm.ar.customer.export", Desc = "Export Customers" },
            new() { MenuKey = "fnm.ar.customer", Key = "fnm.ar.customer.credit", Desc = "Set Credit Limit" },

            new() { MenuKey = "fnm.ar.invoice", Key = "fnm.ar.invoice.view", Desc = "View Invoices" },
            new() { MenuKey = "fnm.ar.invoice", Key = "fnm.ar.invoice.add", Desc = "Create Invoice" },
            new() { MenuKey = "fnm.ar.invoice", Key = "fnm.ar.invoice.mod", Desc = "Edit Invoice" },
            new() { MenuKey = "fnm.ar.invoice", Key = "fnm.ar.invoice.del", Desc = "Delete Invoice" },
            new() { MenuKey = "fnm.ar.invoice", Key = "fnm.ar.invoice.approve", Desc = "Approve Invoice" },
            new() { MenuKey = "fnm.ar.invoice", Key = "fnm.ar.invoice.send", Desc = "Send Invoice" },
            new() { MenuKey = "fnm.ar.invoice", Key = "fnm.ar.invoice.export", Desc = "Export Invoices" },
            new() { MenuKey = "fnm.ar.invoice", Key = "fnm.ar.invoice.print", Desc = "Print Invoice" },

            new() { MenuKey = "fnm.ar.receipt", Key = "fnm.ar.receipt.view", Desc = "View Receipts" },
            new() { MenuKey = "fnm.ar.receipt", Key = "fnm.ar.receipt.add", Desc = "Record Receipt" },
            new() { MenuKey = "fnm.ar.receipt", Key = "fnm.ar.receipt.mod", Desc = "Edit Receipt" },
            new() { MenuKey = "fnm.ar.receipt", Key = "fnm.ar.receipt.del", Desc = "Delete Receipt" },
            new() { MenuKey = "fnm.ar.receipt", Key = "fnm.ar.receipt.export", Desc = "Export Receipts" },

            new() { MenuKey = "fnm.ar.collection", Key = "fnm.ar.collection.view", Desc = "View Collections" },
            new() { MenuKey = "fnm.ar.collection", Key = "fnm.ar.collection.followup", Desc = "Follow-up Collections" },
            new() { MenuKey = "fnm.ar.collection", Key = "fnm.ar.collection.reminder", Desc = "Send Reminder" },
            new() { MenuKey = "fnm.ar.collection", Key = "fnm.ar.collection.report", Desc = "Collection Report" },

            new() { MenuKey = "fnm.ar.report", Key = "fnm.ar.report.view", Desc = "View AR Reports" },
            new() { MenuKey = "fnm.ar.report", Key = "fnm.ar.report.export", Desc = "Export AR Reports" },
            new() { MenuKey = "fnm.ar.report", Key = "fnm.ar.report.aging", Desc = "Aging Report" },

            // ============================================
            // CASH & BANK
            // ============================================
            new() { MenuKey = "fnm.cash", Key = "fnm.cash.view", Desc = "Access Cash Management" },
            new() { MenuKey = "fnm.cash", Key = "fnm.cash.manage", Desc = "Manage Cash" },

            new() { MenuKey = "fnm.cash.bank", Key = "fnm.cash.bank.view", Desc = "View Bank Accounts" },
            new() { MenuKey = "fnm.cash.bank", Key = "fnm.cash.bank.add", Desc = "Add Bank Account" },
            new() { MenuKey = "fnm.cash.bank", Key = "fnm.cash.bank.mod", Desc = "Edit Bank Account" },
            new() { MenuKey = "fnm.cash.bank", Key = "fnm.cash.bank.del", Desc = "Delete Bank Account" },
            new() { MenuKey = "fnm.cash.bank", Key = "fnm.cash.bank.activate", Desc = "Activate Account" },

            new() { MenuKey = "fnm.cash.transaction", Key = "fnm.cash.transaction.view", Desc = "View Transactions" },
            new() { MenuKey = "fnm.cash.transaction", Key = "fnm.cash.transaction.add", Desc = "Add Transaction" },
            new() { MenuKey = "fnm.cash.transaction", Key = "fnm.cash.transaction.mod", Desc = "Edit Transaction" },
            new() { MenuKey = "fnm.cash.transaction", Key = "fnm.cash.transaction.del", Desc = "Delete Transaction" },
            new() { MenuKey = "fnm.cash.transaction", Key = "fnm.cash.transaction.export", Desc = "Export Transactions" },

            new() { MenuKey = "fnm.cash.reconciliation", Key = "fnm.cash.reconciliation.view", Desc = "View Reconciliation" },
            new() { MenuKey = "fnm.cash.reconciliation", Key = "fnm.cash.reconciliation.process", Desc = "Process Reconciliation" },
            new() { MenuKey = "fnm.cash.reconciliation", Key = "fnm.cash.reconciliation.import", Desc = "Import Bank Statement" },
            new() { MenuKey = "fnm.cash.reconciliation", Key = "fnm.cash.reconciliation.report", Desc = "Reconciliation Report" },

            new() { MenuKey = "fnm.cash.petty", Key = "fnm.cash.petty.view", Desc = "View Petty Cash" },
            new() { MenuKey = "fnm.cash.petty", Key = "fnm.cash.petty.add", Desc = "Add Petty Cash Transaction" },
            new() { MenuKey = "fnm.cash.petty", Key = "fnm.cash.petty.reconcile", Desc = "Reconcile Petty Cash" },
            new() { MenuKey = "fnm.cash.petty", Key = "fnm.cash.petty.fund", Desc = "Manage Petty Cash Fund" },

            // ============================================
            // COST ACCOUNTING (CO) ✅ NEW
            // ============================================
            new() { MenuKey = "fnm.co", Key = "fnm.co.view", Desc = "Access Cost Accounting" },
            new() { MenuKey = "fnm.co", Key = "fnm.co.manage", Desc = "Manage Cost Accounting" },

            // Cost Centers
            new() { MenuKey = "fnm.co.costcenters", Key = "fnm.co.costcenters.view", Desc = "View Cost Centers" },
            new() { MenuKey = "fnm.co.costcenters", Key = "fnm.co.costcenters.add", Desc = "Add Cost Center" },
            new() { MenuKey = "fnm.co.costcenters", Key = "fnm.co.costcenters.mod", Desc = "Edit Cost Center" },
            new() { MenuKey = "fnm.co.costcenters", Key = "fnm.co.costcenters.del", Desc = "Delete Cost Center" },
            new() { MenuKey = "fnm.co.costcenters", Key = "fnm.co.costcenters.activate", Desc = "Activate Cost Center" },
            new() { MenuKey = "fnm.co.costcenters", Key = "fnm.co.costcenters.deactivate", Desc = "Deactivate Cost Center" },
            new() { MenuKey = "fnm.co.costcenters", Key = "fnm.co.costcenters.export", Desc = "Export Cost Centers" },
            new() { MenuKey = "fnm.co.costcenters", Key = "fnm.co.costcenters.import", Desc = "Import Cost Centers" },
            new() { MenuKey = "fnm.co.costcenters", Key = "fnm.co.costcenters.budget", Desc = "Manage Cost Center Budget" },
            new() { MenuKey = "fnm.co.costcenters", Key = "fnm.co.costcenters.report", Desc = "Cost Center Reports" },

            // Profit Centers
            new() { MenuKey = "fnm.co.profitcenters", Key = "fnm.co.profitcenters.view", Desc = "View Profit Centers" },
            new() { MenuKey = "fnm.co.profitcenters", Key = "fnm.co.profitcenters.add", Desc = "Add Profit Center" },
            new() { MenuKey = "fnm.co.profitcenters", Key = "fnm.co.profitcenters.mod", Desc = "Edit Profit Center" },
            new() { MenuKey = "fnm.co.profitcenters", Key = "fnm.co.profitcenters.del", Desc = "Delete Profit Center" },
            new() { MenuKey = "fnm.co.profitcenters", Key = "fnm.co.profitcenters.activate", Desc = "Activate Profit Center" },
            new() { MenuKey = "fnm.co.profitcenters", Key = "fnm.co.profitcenters.deactivate", Desc = "Deactivate Profit Center" },
            new() { MenuKey = "fnm.co.profitcenters", Key = "fnm.co.profitcenters.export", Desc = "Export Profit Centers" },
            new() { MenuKey = "fnm.co.profitcenters", Key = "fnm.co.profitcenters.import", Desc = "Import Profit Centers" },
            new() { MenuKey = "fnm.co.profitcenters", Key = "fnm.co.profitcenters.report", Desc = "Profit Center Reports" },

            // Internal Orders
            new() { MenuKey = "fnm.co.orders", Key = "fnm.co.orders.view", Desc = "View Internal Orders" },
            new() { MenuKey = "fnm.co.orders", Key = "fnm.co.orders.add", Desc = "Create Internal Order" },
            new() { MenuKey = "fnm.co.orders", Key = "fnm.co.orders.mod", Desc = "Edit Internal Order" },
            new() { MenuKey = "fnm.co.orders", Key = "fnm.co.orders.del", Desc = "Delete Internal Order" },
            new() { MenuKey = "fnm.co.orders", Key = "fnm.co.orders.activate", Desc = "Activate Internal Order" },
            new() { MenuKey = "fnm.co.orders", Key = "fnm.co.orders.close", Desc = "Close Internal Order" },
            new() { MenuKey = "fnm.co.orders", Key = "fnm.co.orders.budget", Desc = "Manage Order Budget" },
            new() { MenuKey = "fnm.co.orders", Key = "fnm.co.orders.report", Desc = "Internal Order Reports" },
            new() { MenuKey = "fnm.co.orders", Key = "fnm.co.orders.export", Desc = "Export Internal Orders" },

            // CO Reports
            new() { MenuKey = "fnm.co.report", Key = "fnm.co.report.view", Desc = "View CO Reports" },
            new() { MenuKey = "fnm.co.report", Key = "fnm.co.report.export", Desc = "Export CO Reports" },
            new() { MenuKey = "fnm.co.report", Key = "fnm.co.report.analysis", Desc = "Cost Analysis Report" },
            new() { MenuKey = "fnm.co.report", Key = "fnm.co.report.variance", Desc = "Variance Analysis" },

            // ============================================
            // CONSOLIDATION ✅ NEW
            // ============================================
            new() { MenuKey = "fnm.cons", Key = "fnm.cons.view", Desc = "Access Consolidation" },
            new() { MenuKey = "fnm.cons", Key = "fnm.cons.manage", Desc = "Manage Consolidation" },

            // Entities
            new() { MenuKey = "fnm.cons.entities", Key = "fnm.cons.entities.view", Desc = "View Entities" },
            new() { MenuKey = "fnm.cons.entities", Key = "fnm.cons.entities.add", Desc = "Add Entity" },
            new() { MenuKey = "fnm.cons.entities", Key = "fnm.cons.entities.mod", Desc = "Edit Entity" },
            new() { MenuKey = "fnm.cons.entities", Key = "fnm.cons.entities.del", Desc = "Delete Entity" },
            new() { MenuKey = "fnm.cons.entities", Key = "fnm.cons.entities.activate", Desc = "Activate Entity" },
            new() { MenuKey = "fnm.cons.entities", Key = "fnm.cons.entities.export", Desc = "Export Entities" },

            // Consolidation Groups
            new() { MenuKey = "fnm.cons.groups", Key = "fnm.cons.groups.view", Desc = "View Consolidation Groups" },
            new() { MenuKey = "fnm.cons.groups", Key = "fnm.cons.groups.add", Desc = "Create Consolidation Group" },
            new() { MenuKey = "fnm.cons.groups", Key = "fnm.cons.groups.mod", Desc = "Edit Consolidation Group" },
            new() { MenuKey = "fnm.cons.groups", Key = "fnm.cons.groups.del", Desc = "Delete Consolidation Group" },
            new() { MenuKey = "fnm.cons.groups", Key = "fnm.cons.groups.run", Desc = "Run Consolidation" },
            new() { MenuKey = "fnm.cons.groups", Key = "fnm.cons.groups.approve", Desc = "Approve Consolidation" },
            new() { MenuKey = "fnm.cons.groups", Key = "fnm.cons.groups.export", Desc = "Export Consolidation" },

            // Elimination Entries
            new() { MenuKey = "fnm.cons.eliminations", Key = "fnm.cons.eliminations.view", Desc = "View Elimination Entries" },
            new() { MenuKey = "fnm.cons.eliminations", Key = "fnm.cons.eliminations.add", Desc = "Add Elimination Entry" },
            new() { MenuKey = "fnm.cons.eliminations", Key = "fnm.cons.eliminations.mod", Desc = "Edit Elimination Entry" },
            new() { MenuKey = "fnm.cons.eliminations", Key = "fnm.cons.eliminations.del", Desc = "Delete Elimination Entry" },
            new() { MenuKey = "fnm.cons.eliminations", Key = "fnm.cons.eliminations.post", Desc = "Post Elimination Entry" },
            new() { MenuKey = "fnm.cons.eliminations", Key = "fnm.cons.eliminations.report", Desc = "Elimination Report" },

            // Consolidation Reports
            new() { MenuKey = "fnm.cons.report", Key = "fnm.cons.report.view", Desc = "View Consolidation Reports" },
            new() { MenuKey = "fnm.cons.report", Key = "fnm.cons.report.export", Desc = "Export Consolidation Reports" },
            new() { MenuKey = "fnm.cons.report", Key = "fnm.cons.report.financial", Desc = "Consolidated Financial Statements" },
            new() { MenuKey = "fnm.cons.report", Key = "fnm.cons.report.analysis", Desc = "Consolidation Analysis" },

            // ============================================
            // COMPLIANCE ✅ NEW
            // ============================================
            new() { MenuKey = "fnm.compliance", Key = "fnm.compliance.view", Desc = "Access Compliance Management" },
            new() { MenuKey = "fnm.compliance", Key = "fnm.compliance.manage", Desc = "Manage Compliance" },

            // Internal Controls
            new() { MenuKey = "fnm.compliance.controls", Key = "fnm.compliance.controls.view", Desc = "View Internal Controls" },
            new() { MenuKey = "fnm.compliance.controls", Key = "fnm.compliance.controls.add", Desc = "Create Internal Control" },
            new() { MenuKey = "fnm.compliance.controls", Key = "fnm.compliance.controls.mod", Desc = "Edit Internal Control" },
            new() { MenuKey = "fnm.compliance.controls", Key = "fnm.compliance.controls.del", Desc = "Delete Internal Control" },
            new() { MenuKey = "fnm.compliance.controls", Key = "fnm.compliance.controls.activate", Desc = "Activate Control" },
            new() { MenuKey = "fnm.compliance.controls", Key = "fnm.compliance.controls.deactivate", Desc = "Deactivate Control" },
            new() { MenuKey = "fnm.compliance.controls", Key = "fnm.compliance.controls.test", Desc = "Test Control" },
            new() { MenuKey = "fnm.compliance.controls", Key = "fnm.compliance.controls.export", Desc = "Export Controls" },
            new() { MenuKey = "fnm.compliance.controls", Key = "fnm.compliance.controls.report", Desc = "Control Reports" },

            // Compliance Requirements
            new() { MenuKey = "fnm.compliance.requirements", Key = "fnm.compliance.requirements.view", Desc = "View Compliance Requirements" },
            new() { MenuKey = "fnm.compliance.requirements", Key = "fnm.compliance.requirements.add", Desc = "Add Requirement" },
            new() { MenuKey = "fnm.compliance.requirements", Key = "fnm.compliance.requirements.mod", Desc = "Edit Requirement" },
            new() { MenuKey = "fnm.compliance.requirements", Key = "fnm.compliance.requirements.del", Desc = "Delete Requirement" },
            new() { MenuKey = "fnm.compliance.requirements", Key = "fnm.compliance.requirements.assess", Desc = "Assess Compliance" },
            new() { MenuKey = "fnm.compliance.requirements", Key = "fnm.compliance.requirements.export", Desc = "Export Requirements" },
            new() { MenuKey = "fnm.compliance.requirements", Key = "fnm.compliance.requirements.report", Desc = "Compliance Reports" },

            // Audit Logs
            new() { MenuKey = "fnm.compliance", Key = "fnm.compliance.audit.view", Desc = "View Audit Logs" },
            new() { MenuKey = "fnm.compliance", Key = "fnm.compliance.audit.export", Desc = "Export Audit Logs" },
            new() { MenuKey = "fnm.compliance", Key = "fnm.compliance.audit.filter", Desc = "Filter Audit Logs" },
            new() { MenuKey = "fnm.compliance", Key = "fnm.compliance.audit.analysis", Desc = "Audit Analysis" },

            // Compliance Reports
            new() { MenuKey = "fnm.compliance.report", Key = "fnm.compliance.report.view", Desc = "View Compliance Reports" },
            new() { MenuKey = "fnm.compliance.report", Key = "fnm.compliance.report.export", Desc = "Export Compliance Reports" },
            new() { MenuKey = "fnm.compliance.report", Key = "fnm.compliance.report.summary", Desc = "Compliance Summary" },

            // ============================================
            // VENDOR PORTAL ✅ NEW
            // ============================================
            new() { MenuKey = "fnm.portal", Key = "fnm.portal.view", Desc = "Access Vendor Portal" },
            new() { MenuKey = "fnm.portal", Key = "fnm.portal.manage", Desc = "Manage Vendor Portal" },

            // Vendors
            new() { MenuKey = "fnm.portal.vendors", Key = "fnm.portal.vendors.view", Desc = "View Portal Vendors" },
            new() { MenuKey = "fnm.portal.vendors", Key = "fnm.portal.vendors.add", Desc = "Add Portal Vendor" },
            new() { MenuKey = "fnm.portal.vendors", Key = "fnm.portal.vendors.mod", Desc = "Edit Portal Vendor" },
            new() { MenuKey = "fnm.portal.vendors", Key = "fnm.portal.vendors.del", Desc = "Delete Portal Vendor" },
            new() { MenuKey = "fnm.portal.vendors", Key = "fnm.portal.vendors.activate", Desc = "Activate Vendor" },
            new() { MenuKey = "fnm.portal.vendors", Key = "fnm.portal.vendors.deactivate", Desc = "Deactivate Vendor" },
            new() { MenuKey = "fnm.portal.vendors", Key = "fnm.portal.vendors.export", Desc = "Export Portal Vendors" },

            // Invoice Submission
            new() { MenuKey = "fnm.portal.invoices", Key = "fnm.portal.invoices.view", Desc = "View Portal Invoices" },
            new() { MenuKey = "fnm.portal.invoices", Key = "fnm.portal.invoices.submit", Desc = "Submit Invoice" },
            new() { MenuKey = "fnm.portal.invoices", Key = "fnm.portal.invoices.mod", Desc = "Edit Submitted Invoice" },
            new() { MenuKey = "fnm.portal.invoices", Key = "fnm.portal.invoices.del", Desc = "Delete Submitted Invoice" },
            new() { MenuKey = "fnm.portal.invoices", Key = "fnm.portal.invoices.approve", Desc = "Approve Portal Invoice" },
            new() { MenuKey = "fnm.portal.invoices", Key = "fnm.portal.invoices.reject", Desc = "Reject Portal Invoice" },
            new() { MenuKey = "fnm.portal.invoices", Key = "fnm.portal.invoices.export", Desc = "Export Portal Invoices" },

            // Payment Tracking
            new() { MenuKey = "fnm.portal.payments", Key = "fnm.portal.payments.view", Desc = "View Payment Tracking" },
            new() { MenuKey = "fnm.portal.payments", Key = "fnm.portal.payments.export", Desc = "Export Payment Tracking" },
            new() { MenuKey = "fnm.portal.payments", Key = "fnm.portal.payments.update", Desc = "Update Payment Status" },

            // Notifications
            new() { MenuKey = "fnm.portal.notifications", Key = "fnm.portal.notifications.view", Desc = "View Notifications" },
            new() { MenuKey = "fnm.portal.notifications", Key = "fnm.portal.notifications.send", Desc = "Send Notification" },
            new() { MenuKey = "fnm.portal.notifications", Key = "fnm.portal.notifications.markread", Desc = "Mark Notifications as Read" },

            // ============================================
            // IFRS REPORTS ✅ NEW
            // ============================================
            new() { MenuKey = "fnm.ifrs", Key = "fnm.ifrs.view", Desc = "Access IFRS Reports" },
            new() { MenuKey = "fnm.ifrs", Key = "fnm.ifrs.manage", Desc = "Manage IFRS Reports" },

            // IFRS 9
            new() { MenuKey = "fnm.ifrs.ifrs9", Key = "fnm.ifrs.ifrs9.view", Desc = "View IFRS 9 Report" },
            new() { MenuKey = "fnm.ifrs.ifrs9", Key = "fnm.ifrs.ifrs9.generate", Desc = "Generate IFRS 9 Report" },
            new() { MenuKey = "fnm.ifrs.ifrs9", Key = "fnm.ifrs.ifrs9.export", Desc = "Export IFRS 9 Report" },
            new() { MenuKey = "fnm.ifrs.ifrs9", Key = "fnm.ifrs.ifrs9.analyze", Desc = "Analyze IFRS 9 Metrics" },
            new() { MenuKey = "fnm.ifrs.ifrs9", Key = "fnm.ifrs.ifrs9.schedule", Desc = "Schedule IFRS 9 Report" },

            // IFRS 15
            new() { MenuKey = "fnm.ifrs.ifrs15", Key = "fnm.ifrs.ifrs15.view", Desc = "View IFRS 15 Report" },
            new() { MenuKey = "fnm.ifrs.ifrs15", Key = "fnm.ifrs.ifrs15.generate", Desc = "Generate IFRS 15 Report" },
            new() { MenuKey = "fnm.ifrs.ifrs15", Key = "fnm.ifrs.ifrs15.export", Desc = "Export IFRS 15 Report" },
            new() { MenuKey = "fnm.ifrs.ifrs15", Key = "fnm.ifrs.ifrs15.analyze", Desc = "Analyze IFRS 15 Metrics" },
            new() { MenuKey = "fnm.ifrs.ifrs15", Key = "fnm.ifrs.ifrs15.schedule", Desc = "Schedule IFRS 15 Report" },

            // IFRS 16
            new() { MenuKey = "fnm.ifrs.ifrs16", Key = "fnm.ifrs.ifrs16.view", Desc = "View IFRS 16 Report" },
            new() { MenuKey = "fnm.ifrs.ifrs16", Key = "fnm.ifrs.ifrs16.generate", Desc = "Generate IFRS 16 Report" },
            new() { MenuKey = "fnm.ifrs.ifrs16", Key = "fnm.ifrs.ifrs16.export", Desc = "Export IFRS 16 Report" },
            new() { MenuKey = "fnm.ifrs.ifrs16", Key = "fnm.ifrs.ifrs16.analyze", Desc = "Analyze IFRS 16 Metrics" },
            new() { MenuKey = "fnm.ifrs.ifrs16", Key = "fnm.ifrs.ifrs16.schedule", Desc = "Schedule IFRS 16 Report" },

            // IFRS 7
            new() { MenuKey = "fnm.ifrs.ifrs7", Key = "fnm.ifrs.ifrs7.view", Desc = "View IFRS 7 Report" },
            new() { MenuKey = "fnm.ifrs.ifrs7", Key = "fnm.ifrs.ifrs7.generate", Desc = "Generate IFRS 7 Report" },
            new() { MenuKey = "fnm.ifrs.ifrs7", Key = "fnm.ifrs.ifrs7.export", Desc = "Export IFRS 7 Report" },
            new() { MenuKey = "fnm.ifrs.ifrs7", Key = "fnm.ifrs.ifrs7.analyze", Desc = "Analyze IFRS 7 Metrics" },
            new() { MenuKey = "fnm.ifrs.ifrs7", Key = "fnm.ifrs.ifrs7.schedule", Desc = "Schedule IFRS 7 Report" },

            // IFRS 8
            new() { MenuKey = "fnm.ifrs.ifrs8", Key = "fnm.ifrs.ifrs8.view", Desc = "View IFRS 8 Report" },
            new() { MenuKey = "fnm.ifrs.ifrs8", Key = "fnm.ifrs.ifrs8.generate", Desc = "Generate IFRS 8 Report" },
            new() { MenuKey = "fnm.ifrs.ifrs8", Key = "fnm.ifrs.ifrs8.export", Desc = "Export IFRS 8 Report" },
            new() { MenuKey = "fnm.ifrs.ifrs8", Key = "fnm.ifrs.ifrs8.analyze", Desc = "Analyze IFRS 8 Metrics" },
            new() { MenuKey = "fnm.ifrs.ifrs8", Key = "fnm.ifrs.ifrs8.schedule", Desc = "Schedule IFRS 8 Report" },

            // IFRS Dashboard
            new() { MenuKey = "fnm.ifrs.dashboard", Key = "fnm.ifrs.dashboard.view", Desc = "View IFRS Dashboard" },
            new() { MenuKey = "fnm.ifrs.dashboard", Key = "fnm.ifrs.dashboard.export", Desc = "Export IFRS Dashboard" },
            new() { MenuKey = "fnm.ifrs.dashboard", Key = "fnm.ifrs.dashboard.comparison", Desc = "Compare IFRS Standards" },

            // ============================================
            // PAYROLL
            // ============================================
            // Payroll Main
            new() { MenuKey = "fnm.payroll", Key = "fnm.payroll.view", Desc = "Access Payroll Module" },
            new() { MenuKey = "fnm.payroll", Key = "fnm.payroll.manage", Desc = "Manage Payroll" },

            // Payroll Dashboard
            new() { MenuKey = "fnm.payroll.dashboard", Key = "fnm.payroll.dashboard.view", Desc = "View Payroll Dashboard" },
            new() { MenuKey = "fnm.payroll.dashboard", Key = "fnm.payroll.dashboard.export", Desc = "Export Dashboard Data" },

            // Run Payroll
            new() { MenuKey = "fnm.payroll.run", Key = "fnm.payroll.run.view", Desc = "View Payroll Runs" },
            new() { MenuKey = "fnm.payroll.run", Key = "fnm.payroll.run.create", Desc = "Create Payroll Run" },
            new() { MenuKey = "fnm.payroll.run", Key = "fnm.payroll.run.process", Desc = "Process Payroll" },
            new() { MenuKey = "fnm.payroll.run", Key = "fnm.payroll.run.approve", Desc = "Approve Payroll" },
            new() { MenuKey = "fnm.payroll.run", Key = "fnm.payroll.run.reject", Desc = "Reject Payroll" },
            new() { MenuKey = "fnm.payroll.run", Key = "fnm.payroll.run.cancel", Desc = "Cancel Payroll" },
            new() { MenuKey = "fnm.payroll.run", Key = "fnm.payroll.run.export", Desc = "Export Payroll Data" },

            // Payroll History
            new() { MenuKey = "fnm.payroll.history", Key = "fnm.payroll.history.view", Desc = "View Payroll History" },
            new() { MenuKey = "fnm.payroll.history", Key = "fnm.payroll.history.export", Desc = "Export Payroll History" },
            new() { MenuKey = "fnm.payroll.history", Key = "fnm.payroll.history.detail", Desc = "View Payroll Details" },

            // Payroll Calendar
            new() { MenuKey = "fnm.payroll.calendar", Key = "fnm.payroll.calendar.view", Desc = "View Payroll Calendar" },
            new() { MenuKey = "fnm.payroll.calendar", Key = "fnm.payroll.calendar.manage", Desc = "Manage Payroll Calendar" },
            new() { MenuKey = "fnm.payroll.calendar", Key = "fnm.payroll.calendar.export", Desc = "Export Calendar" },

            // Employee Salaries
            new() { MenuKey = "fnm.payroll.salaries", Key = "fnm.payroll.salaries.view", Desc = "View Employee Salaries" },
            new() { MenuKey = "fnm.payroll.salaries", Key = "fnm.payroll.salaries.add", Desc = "Add Salary" },
            new() { MenuKey = "fnm.payroll.salaries", Key = "fnm.payroll.salaries.mod", Desc = "Edit Salary" },
            new() { MenuKey = "fnm.payroll.salaries", Key = "fnm.payroll.salaries.del", Desc = "Delete Salary" },
            new() { MenuKey = "fnm.payroll.salaries", Key = "fnm.payroll.salaries.import", Desc = "Import Salaries" },
            new() { MenuKey = "fnm.payroll.salaries", Key = "fnm.payroll.salaries.export", Desc = "Export Salaries" },
            new() { MenuKey = "fnm.payroll.salaries", Key = "fnm.payroll.salaries.bulk", Desc = "Bulk Update Salaries" },

            // Salary Structure
            new() { MenuKey = "fnm.payroll.structure", Key = "fnm.payroll.structure.view", Desc = "View Salary Structures" },
            new() { MenuKey = "fnm.payroll.structure", Key = "fnm.payroll.structure.add", Desc = "Add Salary Structure" },
            new() { MenuKey = "fnm.payroll.structure", Key = "fnm.payroll.structure.mod", Desc = "Edit Salary Structure" },
            new() { MenuKey = "fnm.payroll.structure", Key = "fnm.payroll.structure.del", Desc = "Delete Salary Structure" },
            new() { MenuKey = "fnm.payroll.structure", Key = "fnm.payroll.structure.activate", Desc = "Activate Structure" },
            new() { MenuKey = "fnm.payroll.structure", Key = "fnm.payroll.structure.deactivate", Desc = "Deactivate Structure" },
            new() { MenuKey = "fnm.payroll.structure", Key = "fnm.payroll.structure.export", Desc = "Export Structures" },
            new() { MenuKey = "fnm.payroll.structure", Key = "fnm.payroll.structure.clone", Desc = "Clone Structure" },

            // Payslip History
            new() { MenuKey = "fnm.payroll.payslips", Key = "fnm.payroll.payslips.view", Desc = "View Payslips" },
            new() { MenuKey = "fnm.payroll.payslips", Key = "fnm.payroll.payslips.generate", Desc = "Generate Payslip" },
            new() { MenuKey = "fnm.payroll.payslips", Key = "fnm.payroll.payslips.download", Desc = "Download Payslip" },
            new() { MenuKey = "fnm.payroll.payslips", Key = "fnm.payroll.payslips.send", Desc = "Send Payslip via Email" },
            new() { MenuKey = "fnm.payroll.payslips", Key = "fnm.payroll.payslips.bulk", Desc = "Bulk Generate Payslips" },
            new() { MenuKey = "fnm.payroll.payslips", Key = "fnm.payroll.payslips.print", Desc = "Print Payslip" },
            new() { MenuKey = "fnm.payroll.payslips", Key = "fnm.payroll.payslips.archive", Desc = "Archive Payslips" },

            // Tax Configurations
            new() { MenuKey = "fnm.payroll.tax", Key = "fnm.payroll.tax.view", Desc = "View Tax Configurations" },
            new() { MenuKey = "fnm.payroll.tax", Key = "fnm.payroll.tax.add", Desc = "Add Tax Bracket" },
            new() { MenuKey = "fnm.payroll.tax", Key = "fnm.payroll.tax.mod", Desc = "Edit Tax Bracket" },
            new() { MenuKey = "fnm.payroll.tax", Key = "fnm.payroll.tax.del", Desc = "Delete Tax Bracket" },
            new() { MenuKey = "fnm.payroll.tax", Key = "fnm.payroll.tax.activate", Desc = "Activate Tax Bracket" },
            new() { MenuKey = "fnm.payroll.tax", Key = "fnm.payroll.tax.deactivate", Desc = "Deactivate Tax Bracket" },
            new() { MenuKey = "fnm.payroll.tax", Key = "fnm.payroll.tax.import", Desc = "Import Tax Rates" },
            new() { MenuKey = "fnm.payroll.tax", Key = "fnm.payroll.tax.export", Desc = "Export Tax Configurations" },
            new() { MenuKey = "fnm.payroll.tax", Key = "fnm.payroll.tax.calculate", Desc = "Calculate Tax" },

            // Payroll Reports
            new() { MenuKey = "fnm.payroll.reports", Key = "fnm.payroll.reports.view", Desc = "View Payroll Reports" },
            new() { MenuKey = "fnm.payroll.reports", Key = "fnm.payroll.reports.export", Desc = "Export Payroll Reports" },
            new() { MenuKey = "fnm.payroll.reports", Key = "fnm.payroll.reports.summary", Desc = "Payroll Summary Report" },
            new() { MenuKey = "fnm.payroll.reports", Key = "fnm.payroll.reports.department", Desc = "Department Report" },
            new() { MenuKey = "fnm.payroll.reports", Key = "fnm.payroll.reports.attendance", Desc = "Attendance Report" },
            new() { MenuKey = "fnm.payroll.reports", Key = "fnm.payroll.reports.tax", Desc = "Tax Report" },
            new() { MenuKey = "fnm.payroll.reports", Key = "fnm.payroll.reports.overtime", Desc = "Overtime Report" },

            // Payroll Settings
            new() { MenuKey = "fnm.payroll.settings", Key = "fnm.payroll.settings.view", Desc = "View Payroll Settings" },
            new() { MenuKey = "fnm.payroll.settings", Key = "fnm.payroll.settings.mod", Desc = "Modify Payroll Settings" },
            new() { MenuKey = "fnm.payroll.settings", Key = "fnm.payroll.settings.policies", Desc = "Configure Policies" },
            new() { MenuKey = "fnm.payroll.settings", Key = "fnm.payroll.settings.approval", Desc = "Configure Approval Workflow" },
            new() { MenuKey = "fnm.payroll.settings", Key = "fnm.payroll.settings.integration", Desc = "Configure Integrations" },

            // ============================================
            // FIXED ASSETS
            // ============================================
            new() { MenuKey = "fnm.assets", Key = "fnm.assets.view", Desc = "Access Asset Management" },
            new() { MenuKey = "fnm.assets", Key = "fnm.assets.manage", Desc = "Manage Assets" },

            new() { MenuKey = "fnm.assets.register", Key = "fnm.assets.register.view", Desc = "View Asset Register" },
            new() { MenuKey = "fnm.assets.register", Key = "fnm.assets.register.add", Desc = "Add Asset" },
            new() { MenuKey = "fnm.assets.register", Key = "fnm.assets.register.mod", Desc = "Edit Asset" },
            new() { MenuKey = "fnm.assets.register", Key = "fnm.assets.register.del", Desc = "Delete Asset" },
            new() { MenuKey = "fnm.assets.register", Key = "fnm.assets.register.export", Desc = "Export Assets" },
            new() { MenuKey = "fnm.assets.register", Key = "fnm.assets.register.import", Desc = "Import Assets" },

            new() { MenuKey = "fnm.assets.depreciation", Key = "fnm.assets.depreciation.view", Desc = "View Depreciation" },
            new() { MenuKey = "fnm.assets.depreciation", Key = "fnm.assets.depreciation.run", Desc = "Run Depreciation" },
            new() { MenuKey = "fnm.assets.depreciation", Key = "fnm.assets.depreciation.forecast", Desc = "Depreciation Forecast" },
            new() { MenuKey = "fnm.assets.depreciation", Key = "fnm.assets.depreciation.report", Desc = "Depreciation Report" },

            new() { MenuKey = "fnm.assets.disposal", Key = "fnm.assets.disposal.view", Desc = "View Disposals" },
            new() { MenuKey = "fnm.assets.disposal", Key = "fnm.assets.disposal.process", Desc = "Process Disposal" },
            new() { MenuKey = "fnm.assets.disposal", Key = "fnm.assets.disposal.approve", Desc = "Approve Disposal" },
            new() { MenuKey = "fnm.assets.disposal", Key = "fnm.assets.disposal.gainloss", Desc = "Calculate Gain/Loss" },

            // ============================================
            // TAX MANAGEMENT
            // ============================================
            new() { MenuKey = "fnm.tax", Key = "fnm.tax.view", Desc = "Access Tax Management" },
            new() { MenuKey = "fnm.tax", Key = "fnm.tax.manage", Desc = "Manage Tax" },

            new() { MenuKey = "fnm.tax.vat", Key = "fnm.tax.vat.view", Desc = "View VAT" },
            new() { MenuKey = "fnm.tax.vat", Key = "fnm.tax.vat.add", Desc = "Add VAT Transaction" },
            new() { MenuKey = "fnm.tax.vat", Key = "fnm.tax.vat.report", Desc = "Generate VAT Report" },
            new() { MenuKey = "fnm.tax.vat", Key = "fnm.tax.vat.return", Desc = "File VAT Return" },
            new() { MenuKey = "fnm.tax.vat", Key = "fnm.tax.vat.config", Desc = "Configure VAT Rates" },

            new() { MenuKey = "fnm.tax.withholding", Key = "fnm.tax.withholding.view", Desc = "View Withholding Tax" },
            new() { MenuKey = "fnm.tax.withholding", Key = "fnm.tax.withholding.calculate", Desc = "Calculate Withholding Tax" },
            new() { MenuKey = "fnm.tax.withholding", Key = "fnm.tax.withholding.report", Desc = "Withholding Tax Report" },
            new() { MenuKey = "fnm.tax.withholding", Key = "fnm.tax.withholding.certificate", Desc = "Generate Certificate" },

            new() { MenuKey = "fnm.tax.report", Key = "fnm.tax.report.view", Desc = "View Tax Reports" },
            new() { MenuKey = "fnm.tax.report", Key = "fnm.tax.report.export", Desc = "Export Tax Reports" },
            new() { MenuKey = "fnm.tax.report", Key = "fnm.tax.report.summary", Desc = "Tax Summary Report" },

            // ============================================
            // FINANCIAL REPORTS
            // ============================================
            new() { MenuKey = "fnm.reports", Key = "fnm.reports.view", Desc = "Access Financial Reports" },
            new() { MenuKey = "fnm.reports", Key = "fnm.reports.export", Desc = "Export Reports" },
            new() { MenuKey = "fnm.reports", Key = "fnm.reports.schedule", Desc = "Schedule Reports" },

            new() { MenuKey = "fnm.reports.balance", Key = "fnm.reports.balance.view", Desc = "View Balance Sheet" },
            new() { MenuKey = "fnm.reports.balance", Key = "fnm.reports.balance.export", Desc = "Export Balance Sheet" },
            new() { MenuKey = "fnm.reports.balance", Key = "fnm.reports.balance.compare", Desc = "Compare Balance Sheets" },

            new() { MenuKey = "fnm.reports.income", Key = "fnm.reports.income.view", Desc = "View Income Statement" },
            new() { MenuKey = "fnm.reports.income", Key = "fnm.reports.income.export", Desc = "Export Income Statement" },
            new() { MenuKey = "fnm.reports.income", Key = "fnm.reports.income.compare", Desc = "Compare Income Statements" },

            new() { MenuKey = "fnm.reports.cashflow", Key = "fnm.reports.cashflow.view", Desc = "View Cash Flow" },
            new() { MenuKey = "fnm.reports.cashflow", Key = "fnm.reports.cashflow.export", Desc = "Export Cash Flow" },

            new() { MenuKey = "fnm.reports.trial", Key = "fnm.reports.trial.view", Desc = "View Trial Balance" },
            new() { MenuKey = "fnm.reports.trial", Key = "fnm.reports.trial.export", Desc = "Export Trial Balance" },

            new() { MenuKey = "fnm.reports.ledger", Key = "fnm.reports.ledger.view", Desc = "View General Ledger" },
            new() { MenuKey = "fnm.reports.ledger", Key = "fnm.reports.ledger.export", Desc = "Export General Ledger" },

            // Baseline permissions (menu that previously had no actions)
            new() { MenuKey = "fnm.audit.logs", Key = "fnm.audit.logs.view", Desc = "View Audit Logs" },
            new() { MenuKey = "fnm.audit.logs", Key = "fnm.audit.logs.export", Desc = "Export Audit Logs" },
        };
    }
}