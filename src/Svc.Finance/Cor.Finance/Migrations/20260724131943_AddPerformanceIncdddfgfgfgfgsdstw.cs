using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Finance.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIncdddfgfgfgfgsdstw : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_PaymentDate_IsDeleted",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_EntryDate_IsDeleted",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_InvoiceDate_IsDeleted_Include",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_ExpenseDate_IsDeleted",
                table: "Expenses");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_Status_VoucherDate",
                table: "Vouchers",
                columns: new[] { "Status", "VoucherDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_VoucherDate_IsDeleted",
                table: "Vouchers",
                columns: new[] { "VoucherDate", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_VoucherLines_VoucherId_AccountId",
                table: "VoucherLines",
                columns: new[] { "VoucherId", "AccountId" });

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_Status_IsDeleted",
                table: "Vendors",
                columns: new[] { "Status", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_VendorPortalUsers_VendorId_Status",
                table: "VendorPortalUsers",
                columns: new[] { "VendorId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TaxReturns_Period_Status",
                table: "TaxReturns",
                columns: new[] { "Period", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_CustomerId_OrderDate",
                table: "SalesOrders",
                columns: new[] { "CustomerId", "OrderDate" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_OrderDate_IsDeleted",
                table: "SalesOrders",
                columns: new[] { "OrderDate", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_Status_OrderDate",
                table: "SalesOrders",
                columns: new[] { "Status", "OrderDate" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderLines_SalesOrderId_PeriodId",
                table: "SalesOrderLines",
                columns: new[] { "SalesOrderId", "PeriodId" });

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_ReceiptDate_IsDeleted",
                table: "Receipts",
                columns: new[] { "ReceiptDate", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_OrderDate_IsDeleted",
                table: "PurchaseOrders",
                columns: new[] { "OrderDate", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_Status_OrderDate",
                table: "PurchaseOrders",
                columns: new[] { "Status", "OrderDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_VendorId_OrderDate",
                table: "PurchaseOrders",
                columns: new[] { "VendorId", "OrderDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_PurchaseOrderId_PeriodId",
                table: "PurchaseOrderLines",
                columns: new[] { "PurchaseOrderId", "PeriodId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProfitCenters_IsActive_IsDeleted",
                table: "ProfitCenters",
                columns: new[] { "IsActive", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsActive_IsDeleted",
                table: "Products",
                columns: new[] { "IsActive", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductPrices_ProductId_EffectiveDate",
                table: "ProductPrices",
                columns: new[] { "ProductId", "EffectiveDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductInventories_ProductId_TransactionDate",
                table: "ProductInventories",
                columns: new[] { "ProductId", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PortalPayments_PaymentDate_IsDeleted",
                table: "PortalPayments",
                columns: new[] { "PaymentDate", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_PortalPayments_Status_PaymentDate",
                table: "PortalPayments",
                columns: new[] { "Status", "PaymentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PortalPayments_VendorId_PaymentDate",
                table: "PortalPayments",
                columns: new[] { "VendorId", "PaymentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PortalNotifications_VendorId_IsRead",
                table: "PortalNotifications",
                columns: new[] { "VendorId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_PortalInvoices_InvoiceDate_IsDeleted",
                table: "PortalInvoices",
                columns: new[] { "InvoiceDate", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_PortalInvoices_Status_InvoiceDate",
                table: "PortalInvoices",
                columns: new[] { "Status", "InvoiceDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PortalInvoices_VendorId_InvoiceDate",
                table: "PortalInvoices",
                columns: new[] { "VendorId", "InvoiceDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSchedules_NextPaymentDate_IsDeleted",
                table: "PaymentSchedules",
                columns: new[] { "NextPaymentDate", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSchedules_Status_NextPaymentDate",
                table: "PaymentSchedules",
                columns: new[] { "Status", "NextPaymentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentScheduleHistories_PaymentScheduleId_ScheduledDate",
                table: "PaymentScheduleHistories",
                columns: new[] { "PaymentScheduleId", "ScheduledDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentDate_IsDeleted_Include",
                table: "Payments",
                columns: new[] { "PaymentDate", "IsDeleted" })
                .Annotation("Npgsql:IndexInclude", new[] { "Amount", "PaymentType", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentApprovalChains_IsActive_IsDeleted",
                table: "PaymentApprovalChains",
                columns: new[] { "IsActive", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAggregates_AggregateDate_AggregateType",
                table: "PaymentAggregates",
                columns: new[] { "AggregateDate", "AggregateType" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAggregates_Period_AggregateType",
                table: "PaymentAggregates",
                columns: new[] { "Period", "AggregateType" });

            migrationBuilder.CreateIndex(
                name: "IX_LocalPositions_Name_IsDeleted",
                table: "LocalPositions",
                columns: new[] { "Name", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_LocalJobGrades_Name_IsDeleted",
                table: "LocalJobGrades",
                columns: new[] { "Name", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_LocalEmployees_Code_IsDeleted",
                table: "LocalEmployees",
                columns: new[] { "Code", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_LocalDepartments_Name_IsDeleted",
                table: "LocalDepartments",
                columns: new[] { "Name", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_LocalCompanies_Name_IsDeleted",
                table: "LocalCompanies",
                columns: new[] { "Name", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_LocalBranches_Code_IsDeleted",
                table: "LocalBranches",
                columns: new[] { "Code", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_EntryDate_IsDeleted_Include",
                table: "JournalEntries",
                columns: new[] { "EntryDate", "IsDeleted" })
                .Annotation("Npgsql:IndexInclude", new[] { "IsPosted", "TotalDebit", "TotalCredit" });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceDate_IsDeleted_Include",
                table: "Invoices",
                columns: new[] { "InvoiceDate", "IsDeleted" })
                .Annotation("Npgsql:IndexInclude", new[] { "InvoiceNumber", "TotalAmount", "Status", "PaidAmount", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Purchase_Date_Type",
                table: "Invoices",
                columns: new[] { "InvoiceDate", "InvoiceType" },
                filter: "\"InvoiceType\" = 'Purchase' AND \"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Purchase_Vendor_Date",
                table: "Invoices",
                columns: new[] { "VendorId", "InvoiceDate" },
                filter: "\"InvoiceType\" = 'Purchase' AND \"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Sales_Date_Type_Status",
                table: "Invoices",
                columns: new[] { "InvoiceDate", "InvoiceType", "Status" },
                filter: "\"InvoiceType\" = 'Sales' AND \"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLines_InvoiceId_PeriodId",
                table: "InvoiceLines",
                columns: new[] { "InvoiceId", "PeriodId" });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceAggregates_AggregateDate_AggregateType",
                table: "InvoiceAggregates",
                columns: new[] { "AggregateDate", "AggregateType" });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceAggregates_Period_AggregateType",
                table: "InvoiceAggregates",
                columns: new[] { "Period", "AggregateType" });

            migrationBuilder.CreateIndex(
                name: "IX_InternalOrders_StartDate_EndDate",
                table: "InternalOrders",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_InternalOrders_Status_StartDate",
                table: "InternalOrders",
                columns: new[] { "Status", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_InternalControls_Category_Status",
                table: "InternalControls",
                columns: new[] { "Category", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_InternalControls_Type_Status",
                table: "InternalControls",
                columns: new[] { "Type", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_IFRSReports_PeriodId_IsDeleted",
                table: "IFRSReports",
                columns: new[] { "PeriodId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_IFRSReports_Standard_Status",
                table: "IFRSReports",
                columns: new[] { "Standard", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_IFRSMetrics_ReportId_PeriodId",
                table: "IFRSMetrics",
                columns: new[] { "ReportId", "PeriodId" });

            migrationBuilder.CreateIndex(
                name: "IX_FinancialPeriods_StartDate_EndDate",
                table: "FinancialPeriods",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseDate_IsDeleted_Include",
                table: "Expenses",
                columns: new[] { "ExpenseDate", "IsDeleted" })
                .Annotation("Npgsql:IndexInclude", new[] { "Amount", "ExpenseCategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategories_IsDeleted",
                table: "ExpenseCategories",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_Type_IsActive",
                table: "Entities",
                columns: new[] { "Type", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_EliminationEntries_PeriodId_IsDeleted",
                table: "EliminationEntries",
                columns: new[] { "PeriodId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_EliminationEntries_Type_Status",
                table: "EliminationEntries",
                columns: new[] { "Type", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_DebitNotes_NoteDate_IsDeleted",
                table: "DebitNotes",
                columns: new[] { "NoteDate", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_DebitNotes_Status_NoteDate",
                table: "DebitNotes",
                columns: new[] { "Status", "NoteDate" });

            migrationBuilder.CreateIndex(
                name: "IX_DebitNoteLines_DebitNoteId_PeriodId",
                table: "DebitNoteLines",
                columns: new[] { "DebitNoteId", "PeriodId" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Status_IsDeleted",
                table: "Customers",
                columns: new[] { "Status", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyRates_RateDate_IsDeleted",
                table: "CurrencyRates",
                columns: new[] { "RateDate", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_CreditNotes_NoteDate_IsDeleted",
                table: "CreditNotes",
                columns: new[] { "NoteDate", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_CreditNotes_Status_NoteDate",
                table: "CreditNotes",
                columns: new[] { "Status", "NoteDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CreditNoteLines_CreditNoteId_PeriodId",
                table: "CreditNoteLines",
                columns: new[] { "CreditNoteId", "PeriodId" });

            migrationBuilder.CreateIndex(
                name: "IX_CostCenters_IsActive_IsDeleted",
                table: "CostCenters",
                columns: new[] { "IsActive", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidationReports_PeriodId_IsDeleted",
                table: "ConsolidationReports",
                columns: new[] { "PeriodId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidationReports_Type_Status",
                table: "ConsolidationReports",
                columns: new[] { "Type", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidationGroups_Date_IsDeleted",
                table: "ConsolidationGroups",
                columns: new[] { "ConsolidationDate", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidationGroups_Status_Date",
                table: "ConsolidationGroups",
                columns: new[] { "Status", "ConsolidationDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidationGroupEntities_GroupId_EntityId",
                table: "ConsolidationGroupEntities",
                columns: new[] { "GroupId", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceRequirements_RiskLevel_IsDeleted",
                table: "ComplianceRequirements",
                columns: new[] { "RiskLevel", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceRequirements_Status_IsDeleted",
                table: "ComplianceRequirements",
                columns: new[] { "ComplianceStatus", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceReports_PeriodId_IsDeleted",
                table: "ComplianceReports",
                columns: new[] { "PeriodId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceReports_Type_Status",
                table: "ComplianceReports",
                columns: new[] { "Type", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ChartOfAccounts_AccountType_IsDeleted",
                table: "ChartOfAccounts",
                columns: new[] { "AccountType", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_Status_IsDeleted_Include",
                table: "Budgets",
                columns: new[] { "Status", "IsDeleted" })
                .Annotation("Npgsql:IndexInclude", new[] { "TotalAmount", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetLines_BudgetId_AccountId",
                table: "BudgetLines",
                columns: new[] { "BudgetId", "AccountId" });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetControls_BudgetId_AccountId_PeriodId",
                table: "BudgetControls",
                columns: new[] { "BudgetId", "AccountId", "PeriodId" });

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_BankAccountId_TransactionDate",
                table: "BankTransactions",
                columns: new[] { "BankAccountId", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_Status_TransactionDate",
                table: "BankTransactions",
                columns: new[] { "Status", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_TransactionDate_IsDeleted",
                table: "BankTransactions",
                columns: new[] { "TransactionDate", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_AccountType_IsDeleted",
                table: "BankAccounts",
                columns: new[] { "AccountType", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_DateAdd_IsDeleted_Include",
                table: "BankAccounts",
                columns: new[] { "DateAdd", "IsDeleted" })
                .Annotation("Npgsql:IndexInclude", new[] { "CurrentBalance", "AccountType" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ActionDate_IsDeleted",
                table: "AuditLogs",
                columns: new[] { "ActionDate", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType_Action",
                table: "AuditLogs",
                columns: new[] { "EntityType", "Action" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId_ActionDate",
                table: "AuditLogs",
                columns: new[] { "UserId", "ActionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetType_IsDeleted",
                table: "Assets",
                columns: new[] { "AssetType", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_DateAdd_IsDeleted_Include",
                table: "Assets",
                columns: new[] { "DateAdd", "IsDeleted" })
                .Annotation("Npgsql:IndexInclude", new[] { "CurrentValue", "AccumulatedDepreciation", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_Status_IsDeleted",
                table: "Assets",
                columns: new[] { "Status", "IsDeleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vouchers_Status_VoucherDate",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_Vouchers_VoucherDate_IsDeleted",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_VoucherLines_VoucherId_AccountId",
                table: "VoucherLines");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_Status_IsDeleted",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_VendorPortalUsers_VendorId_Status",
                table: "VendorPortalUsers");

            migrationBuilder.DropIndex(
                name: "IX_TaxReturns_Period_Status",
                table: "TaxReturns");

            migrationBuilder.DropIndex(
                name: "IX_SalesOrders_CustomerId_OrderDate",
                table: "SalesOrders");

            migrationBuilder.DropIndex(
                name: "IX_SalesOrders_OrderDate_IsDeleted",
                table: "SalesOrders");

            migrationBuilder.DropIndex(
                name: "IX_SalesOrders_Status_OrderDate",
                table: "SalesOrders");

            migrationBuilder.DropIndex(
                name: "IX_SalesOrderLines_SalesOrderId_PeriodId",
                table: "SalesOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_ReceiptDate_IsDeleted",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_OrderDate_IsDeleted",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_Status_OrderDate",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_VendorId_OrderDate",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrderLines_PurchaseOrderId_PeriodId",
                table: "PurchaseOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_ProfitCenters_IsActive_IsDeleted",
                table: "ProfitCenters");

            migrationBuilder.DropIndex(
                name: "IX_Products_IsActive_IsDeleted",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_ProductPrices_ProductId_EffectiveDate",
                table: "ProductPrices");

            migrationBuilder.DropIndex(
                name: "IX_ProductInventories_ProductId_TransactionDate",
                table: "ProductInventories");

            migrationBuilder.DropIndex(
                name: "IX_PortalPayments_PaymentDate_IsDeleted",
                table: "PortalPayments");

            migrationBuilder.DropIndex(
                name: "IX_PortalPayments_Status_PaymentDate",
                table: "PortalPayments");

            migrationBuilder.DropIndex(
                name: "IX_PortalPayments_VendorId_PaymentDate",
                table: "PortalPayments");

            migrationBuilder.DropIndex(
                name: "IX_PortalNotifications_VendorId_IsRead",
                table: "PortalNotifications");

            migrationBuilder.DropIndex(
                name: "IX_PortalInvoices_InvoiceDate_IsDeleted",
                table: "PortalInvoices");

            migrationBuilder.DropIndex(
                name: "IX_PortalInvoices_Status_InvoiceDate",
                table: "PortalInvoices");

            migrationBuilder.DropIndex(
                name: "IX_PortalInvoices_VendorId_InvoiceDate",
                table: "PortalInvoices");

            migrationBuilder.DropIndex(
                name: "IX_PaymentSchedules_NextPaymentDate_IsDeleted",
                table: "PaymentSchedules");

            migrationBuilder.DropIndex(
                name: "IX_PaymentSchedules_Status_NextPaymentDate",
                table: "PaymentSchedules");

            migrationBuilder.DropIndex(
                name: "IX_PaymentScheduleHistories_PaymentScheduleId_ScheduledDate",
                table: "PaymentScheduleHistories");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PaymentDate_IsDeleted_Include",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_PaymentApprovalChains_IsActive_IsDeleted",
                table: "PaymentApprovalChains");

            migrationBuilder.DropIndex(
                name: "IX_PaymentAggregates_AggregateDate_AggregateType",
                table: "PaymentAggregates");

            migrationBuilder.DropIndex(
                name: "IX_PaymentAggregates_Period_AggregateType",
                table: "PaymentAggregates");

            migrationBuilder.DropIndex(
                name: "IX_LocalPositions_Name_IsDeleted",
                table: "LocalPositions");

            migrationBuilder.DropIndex(
                name: "IX_LocalJobGrades_Name_IsDeleted",
                table: "LocalJobGrades");

            migrationBuilder.DropIndex(
                name: "IX_LocalEmployees_Code_IsDeleted",
                table: "LocalEmployees");

            migrationBuilder.DropIndex(
                name: "IX_LocalDepartments_Name_IsDeleted",
                table: "LocalDepartments");

            migrationBuilder.DropIndex(
                name: "IX_LocalCompanies_Name_IsDeleted",
                table: "LocalCompanies");

            migrationBuilder.DropIndex(
                name: "IX_LocalBranches_Code_IsDeleted",
                table: "LocalBranches");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_EntryDate_IsDeleted_Include",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_InvoiceDate_IsDeleted_Include",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_Purchase_Date_Type",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_Purchase_Vendor_Date",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_Sales_Date_Type_Status",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceLines_InvoiceId_PeriodId",
                table: "InvoiceLines");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceAggregates_AggregateDate_AggregateType",
                table: "InvoiceAggregates");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceAggregates_Period_AggregateType",
                table: "InvoiceAggregates");

            migrationBuilder.DropIndex(
                name: "IX_InternalOrders_StartDate_EndDate",
                table: "InternalOrders");

            migrationBuilder.DropIndex(
                name: "IX_InternalOrders_Status_StartDate",
                table: "InternalOrders");

            migrationBuilder.DropIndex(
                name: "IX_InternalControls_Category_Status",
                table: "InternalControls");

            migrationBuilder.DropIndex(
                name: "IX_InternalControls_Type_Status",
                table: "InternalControls");

            migrationBuilder.DropIndex(
                name: "IX_IFRSReports_PeriodId_IsDeleted",
                table: "IFRSReports");

            migrationBuilder.DropIndex(
                name: "IX_IFRSReports_Standard_Status",
                table: "IFRSReports");

            migrationBuilder.DropIndex(
                name: "IX_IFRSMetrics_ReportId_PeriodId",
                table: "IFRSMetrics");

            migrationBuilder.DropIndex(
                name: "IX_FinancialPeriods_StartDate_EndDate",
                table: "FinancialPeriods");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_ExpenseDate_IsDeleted_Include",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseCategories_IsDeleted",
                table: "ExpenseCategories");

            migrationBuilder.DropIndex(
                name: "IX_Entities_Type_IsActive",
                table: "Entities");

            migrationBuilder.DropIndex(
                name: "IX_EliminationEntries_PeriodId_IsDeleted",
                table: "EliminationEntries");

            migrationBuilder.DropIndex(
                name: "IX_EliminationEntries_Type_Status",
                table: "EliminationEntries");

            migrationBuilder.DropIndex(
                name: "IX_DebitNotes_NoteDate_IsDeleted",
                table: "DebitNotes");

            migrationBuilder.DropIndex(
                name: "IX_DebitNotes_Status_NoteDate",
                table: "DebitNotes");

            migrationBuilder.DropIndex(
                name: "IX_DebitNoteLines_DebitNoteId_PeriodId",
                table: "DebitNoteLines");

            migrationBuilder.DropIndex(
                name: "IX_Customers_Status_IsDeleted",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_CurrencyRates_RateDate_IsDeleted",
                table: "CurrencyRates");

            migrationBuilder.DropIndex(
                name: "IX_CreditNotes_NoteDate_IsDeleted",
                table: "CreditNotes");

            migrationBuilder.DropIndex(
                name: "IX_CreditNotes_Status_NoteDate",
                table: "CreditNotes");

            migrationBuilder.DropIndex(
                name: "IX_CreditNoteLines_CreditNoteId_PeriodId",
                table: "CreditNoteLines");

            migrationBuilder.DropIndex(
                name: "IX_CostCenters_IsActive_IsDeleted",
                table: "CostCenters");

            migrationBuilder.DropIndex(
                name: "IX_ConsolidationReports_PeriodId_IsDeleted",
                table: "ConsolidationReports");

            migrationBuilder.DropIndex(
                name: "IX_ConsolidationReports_Type_Status",
                table: "ConsolidationReports");

            migrationBuilder.DropIndex(
                name: "IX_ConsolidationGroups_Date_IsDeleted",
                table: "ConsolidationGroups");

            migrationBuilder.DropIndex(
                name: "IX_ConsolidationGroups_Status_Date",
                table: "ConsolidationGroups");

            migrationBuilder.DropIndex(
                name: "IX_ConsolidationGroupEntities_GroupId_EntityId",
                table: "ConsolidationGroupEntities");

            migrationBuilder.DropIndex(
                name: "IX_ComplianceRequirements_RiskLevel_IsDeleted",
                table: "ComplianceRequirements");

            migrationBuilder.DropIndex(
                name: "IX_ComplianceRequirements_Status_IsDeleted",
                table: "ComplianceRequirements");

            migrationBuilder.DropIndex(
                name: "IX_ComplianceReports_PeriodId_IsDeleted",
                table: "ComplianceReports");

            migrationBuilder.DropIndex(
                name: "IX_ComplianceReports_Type_Status",
                table: "ComplianceReports");

            migrationBuilder.DropIndex(
                name: "IX_ChartOfAccounts_AccountType_IsDeleted",
                table: "ChartOfAccounts");

            migrationBuilder.DropIndex(
                name: "IX_Budgets_Status_IsDeleted_Include",
                table: "Budgets");

            migrationBuilder.DropIndex(
                name: "IX_BudgetLines_BudgetId_AccountId",
                table: "BudgetLines");

            migrationBuilder.DropIndex(
                name: "IX_BudgetControls_BudgetId_AccountId_PeriodId",
                table: "BudgetControls");

            migrationBuilder.DropIndex(
                name: "IX_BankTransactions_BankAccountId_TransactionDate",
                table: "BankTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BankTransactions_Status_TransactionDate",
                table: "BankTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BankTransactions_TransactionDate_IsDeleted",
                table: "BankTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_AccountType_IsDeleted",
                table: "BankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_DateAdd_IsDeleted_Include",
                table: "BankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_ActionDate_IsDeleted",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_EntityType_Action",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_UserId_ActionDate",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_Assets_AssetType_IsDeleted",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_DateAdd_IsDeleted_Include",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_Status_IsDeleted",
                table: "Assets");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentDate_IsDeleted",
                table: "Payments",
                columns: new[] { "PaymentDate", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_EntryDate_IsDeleted",
                table: "JournalEntries",
                columns: new[] { "EntryDate", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceDate_IsDeleted_Include",
                table: "Invoices",
                columns: new[] { "InvoiceDate", "IsDeleted" })
                .Annotation("Npgsql:IndexInclude", new[] { "InvoiceNumber", "TotalAmount", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseDate_IsDeleted",
                table: "Expenses",
                columns: new[] { "ExpenseDate", "IsDeleted" });
        }
    }
}
