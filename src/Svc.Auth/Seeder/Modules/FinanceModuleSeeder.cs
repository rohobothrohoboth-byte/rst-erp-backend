// Svc.Auth.Seeder.Modules/FinanceModuleSeeder.cs
// Add these after the existing menus

using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Seeder.Modules;

public static class FinanceModuleSeeder
{
    public static IEnumerable<PerMenuSeedDto> GetMenus()
    {
        return new List<PerMenuSeedDto>
        {
            // ============================================
            // DASHBOARD
            // ============================================
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.db",
                Label = "Dashboard",
                Path = "/finance",
                Icon = "LayoutDashboard",
                ParKey = "",
                IsChild = false,
                Order = 1
            },

            // ============================================
            // GENERAL LEDGER
            // ============================================
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.gl",
                Label = "General Ledger",
                Path = "/finance/gl",
                Icon = "FileText",
                ParKey = "",
                IsChild = false,
                Order = 2
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.gl.coa",
                Label = "Chart of Accounts",
                Path = "/finance/gl/chart-of-accounts",
                Icon = "Layers",
                ParKey = "fnm.gl",
                IsChild = true,
                Order = 1
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.gl.journal",
                Label = "Journal Entries",
                Path = "/finance/gl/journal-entries",
                Icon = "FileText",
                ParKey = "fnm.gl",
                IsChild = true,
                Order = 2
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.gl.audit",
                Label = "Audit Trail",
                Path = "/finance/gl/audit-trail",
                Icon = "History",
                ParKey = "fnm.gl",
                IsChild = true,
                Order = 3
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.gl.budget",
                Label = "Budget Management",
                Path = "/finance/budget-management",
                Icon = "PieChart",
                ParKey = "fnm.gl",
                IsChild = true,
                Order = 4
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.gl.closing",
                Label = "Period Closing",
                Path = "/finance/period-closing",
                Icon = "Lock",
                ParKey = "fnm.gl",
                IsChild = true,
                Order = 5
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.gl.voucher",
                Label = "Voucher Management",
                Path = "/finance/voucher-management",
                Icon = "FileCheck",
                ParKey = "fnm.gl",
                IsChild = true,
                Order = 6
            },

            // ============================================
            // ACCOUNTS PAYABLE
            // ============================================
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.ap",
                Label = "Accounts Payable",
                Path = "/finance/accounts-payable",
                Icon = "DollarSign",
                ParKey = "",
                IsChild = false,
                Order = 3
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.ap.vendor",
                Label = "Vendor Management",
                Path = "/finance/vendor-management",
                Icon = "Truck",
                ParKey = "fnm.ap",
                IsChild = true,
                Order = 1
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.ap.invoice",
                Label = "Invoice Entry",
                Path = "/finance/ap/invoices",
                Icon = "FileText",
                ParKey = "fnm.ap",
                IsChild = true,
                Order = 2
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.ap.payment",
                Label = "Payment Processing",
                Path = "/finance/ap/payments",
                Icon = "DollarSign",
                ParKey = "fnm.ap",
                IsChild = true,
                Order = 3
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.ap.approval",
                Label = "Invoice Approval",
                Path = "/finance/invoice-approval-ap",
                Icon = "CheckCircle",
                ParKey = "fnm.ap",
                IsChild = true,
                Order = 4
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.ap.report",
                Label = "AP Reports",
                Path = "/finance/ap-reports",
                Icon = "BarChart",
                ParKey = "fnm.ap",
                IsChild = true,
                Order = 5
            },

            // ============================================
            // ACCOUNTS RECEIVABLE
            // ============================================
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.ar",
                Label = "Accounts Receivable",
                Path = "/finance/accounts",
                Icon = "Wallet",
                ParKey = "",
                IsChild = false,
                Order = 4
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.ar.customer",
                Label = "Customer Management",
                Path = "/finance/customer-management",
                Icon = "Users",
                ParKey = "fnm.ar",
                IsChild = true,
                Order = 1
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.ar.invoice",
                Label = "Invoice Posting",
                Path = "/finance/ar/invoices",
                Icon = "FileCheck",
                ParKey = "fnm.ar",
                IsChild = true,
                Order = 2
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.ar.receipt",
                Label = "Payment Receipt",
                Path = "/finance/ar/receipts",
                Icon = "Wallet",
                ParKey = "fnm.ar",
                IsChild = true,
                Order = 3
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.ar.collection",
                Label = "Collection Follow-up",
                Path = "/finance/collection-followup",
                Icon = "Phone",
                ParKey = "fnm.ar",
                IsChild = true,
                Order = 4
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.ar.report",
                Label = "AR Reports",
                Path = "/finance/ar-reports",
                Icon = "BarChart",
                ParKey = "fnm.ar",
                IsChild = true,
                Order = 5
            },

            // ============================================
            // CASH & BANK
            // ============================================
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.cash",
                Label = "Cash & Bank",
                Path = "/finance/bank-accounts",
                Icon = "CreditCard",
                ParKey = "",
                IsChild = false,
                Order = 5
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.cash.bank",
                Label = "Bank Accounts",
                Path = "/finance/bank-accounts",
                Icon = "Building",
                ParKey = "fnm.cash",
                IsChild = true,
                Order = 1
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.cash.transaction",
                Label = "Transactions",
                Path = "/finance/transactions",
                Icon = "Repeat",
                ParKey = "fnm.cash",
                IsChild = true,
                Order = 2
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.cash.reconciliation",
                Label = "Bank Reconciliation",
                Path = "/finance/bank-reconciliation",
                Icon = "RefreshCw",
                ParKey = "fnm.cash",
                IsChild = true,
                Order = 3
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.cash.petty",
                Label = "Petty Cash",
                Path = "/finance/petty-cash",
                Icon = "Coins",
                ParKey = "fnm.cash",
                IsChild = true,
                Order = 4
            },

            // ============================================
            // COST ACCOUNTING (CO) - FIXED
            // ============================================
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.co",
                Label = "Cost Accounting",
                Path = "/finance/cost-centers",
                Icon = "Target",
                ParKey = "",
                IsChild = false,
                Order = 6
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.co.costcenters",
                Label = "Cost Centers",
                Path = "/finance/cost-centers",
                Icon = "Layers",
                ParKey = "fnm.co",
                IsChild = true,
                Order = 1
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.co.profitcenters",
                Label = "Profit Centers",
                Path = "/finance/profit-centers",
                Icon = "BarChart3",
                ParKey = "fnm.co",
                IsChild = true,
                Order = 2
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.co.orders",
                Label = "Internal Orders",
                Path = "/finance/internal-orders",
                Icon = "GitBranch",
                ParKey = "fnm.co",
                IsChild = true,
                Order = 3
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.co.report",
                Label = "CO Reports",
                Path = "/finance/co-reports",
                Icon = "BarChart",
                ParKey = "fnm.co",
                IsChild = true,
                Order = 4
            },

            // ============================================
            // CONSOLIDATION
            // ============================================
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.cons",
                Label = "Consolidation",
                Path = "/finance/consolidation",
                Icon = "GitMerge",
                ParKey = "",
                IsChild = false,
                Order = 7
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.cons.entities",
                Label = "Entities",
                Path = "/finance/consolidation/entities",
                Icon = "Building2",
                ParKey = "fnm.cons",
                IsChild = true,
                Order = 1
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.cons.groups",
                Label = "Consolidation Groups",
                Path = "/finance/consolidation/groups",
                Icon = "Layers",
                ParKey = "fnm.cons",
                IsChild = true,
                Order = 2
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.cons.eliminations",
                Label = "Elimination Entries",
                Path = "/finance/consolidation/eliminations",
                Icon = "Unlink",
                ParKey = "fnm.cons",
                IsChild = true,
                Order = 3
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.cons.report",
                Label = "Consolidation Reports",
                Path = "/finance/consolidation/reports",
                Icon = "FileText",
                ParKey = "fnm.cons",
                IsChild = true,
                Order = 4
            },

            // ============================================
            // COMPLIANCE MANAGEMENT - FIXED
            // ============================================
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.compliance",
                Label = "Compliance Management",
                Path = "/finance/compliance",
                Icon = "Shield",
                ParKey = "",
                IsChild = false,
                Order = 8
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.compliance.controls",
                Label = "Internal Controls",
                Path = "/finance/compliance/controls",
                Icon = "ListChecks",
                ParKey = "fnm.compliance",
                IsChild = true,
                Order = 1
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.compliance.requirements",
                Label = "Compliance Requirements",
                Path = "/finance/compliance/requirements",
                Icon = "ClipboardCheck",
                ParKey = "fnm.compliance",
                IsChild = true,
                Order = 2
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.compliance.report",
                Label = "Compliance Reports",
                Path = "/finance/compliance/reports",
                Icon = "FileText",
                ParKey = "fnm.compliance",
                IsChild = true,
                Order = 4
            },

            // ============================================
            // AUDIT LOGS - MOVED FROM COMPLIANCE
            // ============================================
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.audit.logs",
                Label = "Audit Logs",
                Path = "/finance/audit-logs",
                Icon = "Activity",
                ParKey = "",
                IsChild = false,
                Order = 9
            },

            // ============================================
            // VENDOR PORTAL
            // ============================================
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.portal",
                Label = "Vendor Portal",
                Path = "/finance/vendor-portal",
                Icon = "Globe",
                ParKey = "",
                IsChild = false,
                Order = 10
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.portal.vendors",
                Label = "Vendors",
                Path = "/finance/portal/vendors",
                Icon = "Users",
                ParKey = "fnm.portal",
                IsChild = true,
                Order = 1
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.portal.invoices",
                Label = "Invoice Submission",
                Path = "/finance/portal/invoices",
                Icon = "Upload",
                ParKey = "fnm.portal",
                IsChild = true,
                Order = 2
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.portal.payments",
                Label = "Payment Tracking",
                Path = "/finance/portal/payments",
                Icon = "CreditCard",
                ParKey = "fnm.portal",
                IsChild = true,
                Order = 3
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.portal.notifications",
                Label = "Notifications",
                Path = "/finance/portal/notifications",
                Icon = "Bell",
                ParKey = "fnm.portal",
                IsChild = true,
                Order = 4
            },

            // ============================================
            // IFRS REPORTS
            // ============================================
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.ifrs",
                Label = "IFRS Reports",
                Path = "/finance/ifrs",
                Icon = "BookOpen",
                ParKey = "",
                IsChild = false,
                Order = 11
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.ifrs.ifrs9",
                Label = "IFRS 9 - Financial Instruments",
                Path = "/finance/ifrs/ifrs9",
                Icon = "FileText",
                ParKey = "fnm.ifrs",
                IsChild = true,
                Order = 1
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.ifrs.ifrs15",
                Label = "IFRS 15 - Revenue Recognition",
                Path = "/finance/ifrs/ifrs15",
                Icon = "FileText",
                ParKey = "fnm.ifrs",
                IsChild = true,
                Order = 2
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.ifrs.ifrs16",
                Label = "IFRS 16 - Leases",
                Path = "/finance/ifrs/ifrs16",
                Icon = "FileText",
                ParKey = "fnm.ifrs",
                IsChild = true,
                Order = 3
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.ifrs.ifrs7",
                Label = "IFRS 7 - Disclosures",
                Path = "/finance/ifrs/ifrs7",
                Icon = "FileText",
                ParKey = "fnm.ifrs",
                IsChild = true,
                Order = 4
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.ifrs.ifrs8",
                Label = "IFRS 8 - Operating Segments",
                Path = "/finance/ifrs/ifrs8",
                Icon = "FileText",
                ParKey = "fnm.ifrs",
                IsChild = true,
                Order = 5
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.ifrs.dashboard",
                Label = "IFRS Dashboard",
                Path = "/finance/ifrs/dashboard",
                Icon = "LayoutDashboard",
                ParKey = "fnm.ifrs",
                IsChild = true,
                Order = 6
            },

            // ============================================
            // PAYROLL
            // ============================================
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.payroll",
                Label = "Payroll",
                Path = "/finance/payroll",
                Icon = "Users",
                ParKey = "",
                IsChild = false,
                Order = 12
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.payroll.dashboard",
                Label = "Payroll Dashboard",
                Path = "/finance/payroll/dashboard",
                Icon = "LayoutDashboard",
                ParKey = "fnm.payroll",
                IsChild = true,
                Order = 1
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.payroll.run",
                Label = "Run Payroll",
                Path = "/finance/payroll/run",
                Icon = "Calculator",
                ParKey = "fnm.payroll",
                IsChild = true,
                Order = 2
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.payroll.history",
                Label = "Payroll History",
                Path = "/finance/payroll/history",
                Icon = "History",
                ParKey = "fnm.payroll",
                IsChild = true,
                Order = 3
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.payroll.calendar",
                Label = "Payroll Calendar",
                Path = "/finance/payroll/calendar",
                Icon = "Calendar",
                ParKey = "fnm.payroll",
                IsChild = true,
                Order = 4
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.payroll.salaries",
                Label = "Employee Salaries",
                Path = "/finance/payroll/salaries",
                Icon = "Users",
                ParKey = "fnm.payroll",
                IsChild = true,
                Order = 5
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.payroll.structure",
                Label = "Salary Structure",
                Path = "/finance/payroll/salary-structure",
                Icon = "Database",
                ParKey = "fnm.payroll",
                IsChild = true,
                Order = 6
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.payroll.payslips",
                Label = "Payslip History",
                Path = "/finance/payroll/payslips",
                Icon = "FileText",
                ParKey = "fnm.payroll",
                IsChild = true,
                Order = 7
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.payroll.tax",
                Label = "Tax Configurations",
                Path = "/finance/payroll/tax-config",
                Icon = "Shield",
                ParKey = "fnm.payroll",
                IsChild = true,
                Order = 8
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.payroll.reports",
                Label = "Payroll Reports",
                Path = "/finance/payroll/reports",
                Icon = "BarChart3",
                ParKey = "fnm.payroll",
                IsChild = true,
                Order = 9
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.payroll.settings",
                Label = "Payroll Settings",
                Path = "/finance/payroll/settings",
                Icon = "Settings",
                ParKey = "fnm.payroll",
                IsChild = true,
                Order = 10
            },

            // ============================================
            // FIXED ASSETS
            // ============================================
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.assets",
                Label = "Fixed Assets",
                Path = "/finance/assets",
                Icon = "Briefcase",
                ParKey = "",
                IsChild = false,
                Order = 13
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.assets.register",
                Label = "Asset Register",
                Path = "/finance/assets/register",
                Icon = "List",
                ParKey = "fnm.assets",
                IsChild = true,
                Order = 1
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.assets.depreciation",
                Label = "Depreciation",
                Path = "/finance/depreciation",
                Icon = "TrendingDown",
                ParKey = "fnm.assets",
                IsChild = true,
                Order = 2
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.assets.disposal",
                Label = "Asset Disposal",
                Path = "/finance/asset-disposal",
                Icon = "Trash2",
                ParKey = "fnm.assets",
                IsChild = true,
                Order = 3
            },

            // ============================================
            // TAX MANAGEMENT
            // ============================================
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.tax",
                Label = "Tax Management",
                Path = "/finance/vat-management",
                Icon = "Calculator",
                ParKey = "",
                IsChild = false,
                Order = 14
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.tax.vat",
                Label = "VAT Management",
                Path = "/finance/vat-management",
                Icon = "FileText",
                ParKey = "fnm.tax",
                IsChild = true,
                Order = 1
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.tax.withholding",
                Label = "Withholding Tax",
                Path = "/finance/withholding-tax",
                Icon = "FileText",
                ParKey = "fnm.tax",
                IsChild = true,
                Order = 2
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.tax.report",
                Label = "Tax Reports",
                Path = "/finance/tax-reports",
                Icon = "BarChart",
                ParKey = "fnm.tax",
                IsChild = true,
                Order = 3
            },

            // ============================================
            // FINANCIAL REPORTS
            // ============================================
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.reports",
                Label = "Financial Reports",
                Path = "/finance/reports",
                Icon = "LineChart",
                ParKey = "",
                IsChild = false,
                Order = 15
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.reports.balance",
                Label = "Balance Sheet",
                Path = "/finance/reports/balance-sheet",
                Icon = "FileText",
                ParKey = "fnm.reports",
                IsChild = true,
                Order = 1
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.reports.income",
                Label = "Income Statement",
                Path = "/finance/reports/income-statement",
                Icon = "TrendingUp",
                ParKey = "fnm.reports",
                IsChild = true,
                Order = 2
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.reports.cashflow",
                Label = "Cash Flow",
                Path = "/finance/reports/cash-flow",
                Icon = "Activity",
                ParKey = "fnm.reports",
                IsChild = true,
                Order = 3
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.reports.trial",
                Label = "Trial Balance",
                Path = "/finance/reports/trial-balance",
                Icon = "FileText",
                ParKey = "fnm.reports",
                IsChild = true,
                Order = 4
            },
            new() {
                ModKey = "mod.fnm",
                Key = "fnm.reports.ledger",
                Label = "General Ledger",
                Path = "/finance/reports/general-ledger",
                Icon = "BookOpen",
                ParKey = "fnm.reports",
                IsChild = true,
                Order = 5
            },
        };
    }
}