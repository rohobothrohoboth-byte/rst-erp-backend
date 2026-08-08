using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Finance.Migrations
{
    /// <inheritdoc />
    public partial class AddLastOverdueNotifidddffdss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountCategories_AccountCategories_ParentId",
                table: "AccountCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_FinancialPeriods_FinancialPeriodId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_BankTransactions_FinancialPeriods_PeriodId",
                table: "BankTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetLines_ChartOfAccounts_AccountId",
                table: "BudgetLines");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetLines_FinancialPeriods_PeriodId",
                table: "BudgetLines");

            migrationBuilder.DropForeignKey(
                name: "FK_Budgets_FinancialPeriods_PeriodId",
                table: "Budgets");

            migrationBuilder.DropForeignKey(
                name: "FK_ComplianceRequirementControl_ComplianceRequirements_Complia~",
                table: "ComplianceRequirementControl");

            migrationBuilder.DropForeignKey(
                name: "FK_ComplianceRequirementEvidence_ComplianceRequirements_Compli~",
                table: "ComplianceRequirementEvidence");

            migrationBuilder.DropForeignKey(
                name: "FK_ConsolidationGroupEntities_Entities_EntityId",
                table: "ConsolidationGroupEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_ConsolidationGroups_Entities_ParentEntityId",
                table: "ConsolidationGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_ConsolidationReports_ConsolidationGroups_ConsolidationGroup~",
                table: "ConsolidationReports");

            migrationBuilder.DropForeignKey(
                name: "FK_CostCenters_CostCenters_ParentId",
                table: "CostCenters");

            migrationBuilder.DropForeignKey(
                name: "FK_EliminationEntries_ConsolidationGroups_ConsolidationGroupId",
                table: "EliminationEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_EliminationEntries_Entities_FromEntityId",
                table: "EliminationEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_EliminationEntries_Entities_ToEntityId",
                table: "EliminationEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_Entities_Entities_ParentEntityId",
                table: "Entities");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpenseCategories_ExpenseCategoryId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_FinancialPeriods_PeriodId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_IFRMetrics_IFRSReports_ReportId",
                table: "IFRMetrics");

            migrationBuilder.DropForeignKey(
                name: "FK_InternalOrders_CostCenters_CostCenterId",
                table: "InternalOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceLines_FinancialPeriods_PeriodId",
                table: "InvoiceLines");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_FinancialPeriods_PeriodId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntries_FinancialPeriods_PeriodId",
                table: "JournalEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalLines_ChartOfAccounts_AccountId",
                table: "JournalLines");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalLines_FinancialPeriods_PeriodId",
                table: "JournalLines");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_FinancialPeriods_PeriodId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_PortalInvoices_Vendors_VendorId",
                table: "PortalInvoices");

            migrationBuilder.DropForeignKey(
                name: "FK_PortalPayments_PortalInvoices_InvoiceId",
                table: "PortalPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_PortalPayments_Vendors_VendorId",
                table: "PortalPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfitCenters_ProfitCenters_ParentId",
                table: "ProfitCenters");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderLines_FinancialPeriods_PeriodId",
                table: "PurchaseOrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_FinancialPeriods_PeriodId",
                table: "PurchaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Vendors_VendorId",
                table: "PurchaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_VoucherLines_ChartOfAccounts_AccountId",
                table: "VoucherLines");

            migrationBuilder.DropForeignKey(
                name: "FK_Vouchers_Vendors_VendorId",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_PaymentApprovalChains_Name",
                table: "PaymentApprovalChains");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceLines_IsDeleted",
                table: "InvoiceLines");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceAmendments_RequestedAt",
                table: "InvoiceAmendments");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_FinancialPeriodId",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_ApprovalSteps_IsDeleted",
                table: "ApprovalSteps");

            migrationBuilder.DropIndex(
                name: "IX_ApprovalSteps_Role",
                table: "ApprovalSteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IFRMetrics",
                table: "IFRMetrics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ComplianceRequirementEvidence",
                table: "ComplianceRequirementEvidence");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ComplianceRequirementControl",
                table: "ComplianceRequirementControl");

            migrationBuilder.RenameTable(
                name: "IFRMetrics",
                newName: "IFRSMetrics");

            migrationBuilder.RenameTable(
                name: "ComplianceRequirementEvidence",
                newName: "ComplianceRequirementEvidences");

            migrationBuilder.RenameTable(
                name: "ComplianceRequirementControl",
                newName: "ComplianceRequirementControls");

            migrationBuilder.RenameColumn(
                name: "FinancialPeriodId",
                table: "AuditLogs",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_IFRMetrics_ReportId",
                table: "IFRSMetrics",
                newName: "IX_IFRSMetrics_ReportId");

            migrationBuilder.RenameIndex(
                name: "IX_ComplianceRequirementEvidence_ComplianceRequirementId",
                table: "ComplianceRequirementEvidences",
                newName: "IX_ComplianceRequirementEvidences_ComplianceRequirementId");

            migrationBuilder.RenameIndex(
                name: "IX_ComplianceRequirementControl_ComplianceRequirementId",
                table: "ComplianceRequirementControls",
                newName: "IX_ComplianceRequirementControls_ComplianceRequirementId");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalDebit",
                table: "Vouchers",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalCredit",
                table: "Vouchers",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "PostedBy",
                table: "Vouchers",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApprovedBy",
                table: "Vouchers",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Vouchers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "Vouchers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "Vouchers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "Vouchers",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DebitAmount",
                table: "VoucherLines",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "CreditAmount",
                table: "VoucherLines",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "VoucherLines",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "VoucherLines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "VoucherLines",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "VoucherLines",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Website",
                table: "Vendors",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalSpent",
                table: "Vendors",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Rating",
                table: "Vendors",
                type: "numeric(3,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Vendors",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "Vendors",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "Vendors",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "Vendors",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "Vendors",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "VendorPortalUsers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "VendorPortalUsers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "VendorPortalUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "VendorPortalUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "VendorPortalUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "VendorPortalUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "VendorPortalUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TaxableAmount",
                table: "TaxReturns",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "TaxAmount",
                table: "TaxReturns",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "BalanceDue",
                table: "TaxReturns",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountPaid",
                table: "TaxReturns",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "TaxReturns",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "TaxReturns",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "TaxReturns",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "TaxReturns",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "TaxReturns",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Rate",
                table: "TaxRates",
                type: "numeric(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "TaxRates",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "TaxRates",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "TaxRates",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "TaxRates",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "TaxRates",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PeriodId",
                table: "PurchaseOrders",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "PurchaseOrders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "PurchaseOrders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "PurchaseOrders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "PurchaseOrders",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PeriodId",
                table: "PurchaseOrderLines",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "PurchaseOrderLines",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "PurchaseOrderLines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "PurchaseOrderLines",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "PurchaseOrderLines",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Region",
                table: "ProfitCenters",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Manager",
                table: "ProfitCenters",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ProfitCenters",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "ProfitCenters",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "ProfitCenters",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ProfitCenters",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "ProfitCenters",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "PortalPayments",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "PortalPayments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "PortalPayments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "PortalPayments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "PortalPayments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "PortalPayments",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "PortalNotifications",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Link",
                table: "PortalNotifications",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "PortalNotifications",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "PortalNotifications",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PortalNotifications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "PortalNotifications",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "PortalNotifications",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "PortalNotifications",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SubmittedBy",
                table: "PortalInvoices",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "PortalInvoices",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentReference",
                table: "PortalInvoices",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApprovedBy",
                table: "PortalInvoices",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "PortalInvoices",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "PortalInvoices",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "PortalInvoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "PortalInvoices",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "PortalInvoices",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "PortalInvoices",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PeriodId",
                table: "Payments",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Payments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "Payments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "Payments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "Payments",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MinAmount",
                table: "PaymentApprovalChains",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxAmount",
                table: "PaymentApprovalChains",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "PaymentApprovalChains",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "PaymentApprovalChains",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "PaymentApprovalChains",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "PaymentApprovalChains",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "PaymentApprovalChains",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PeriodId",
                table: "JournalLines",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<decimal>(
                name: "Debit",
                table: "JournalLines",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "Credit",
                table: "JournalLines",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "JournalLines",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "JournalLines",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "JournalLines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "JournalLines",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "JournalLines",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalDebit",
                table: "JournalEntries",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalCredit",
                table: "JournalEntries",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<Guid>(
                name: "PeriodId",
                table: "JournalEntries",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "JournalEntries",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "JournalEntries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "JournalEntries",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "JournalEntries",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PeriodId",
                table: "Invoices",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Invoices",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "Invoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "Invoices",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "Invoices",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitPrice",
                table: "InvoiceLines",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "InvoiceLines",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "TaxRate",
                table: "InvoiceLines",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<Guid>(
                name: "PeriodId",
                table: "InvoiceLines",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "InvoiceLines",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "InvoiceLines",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "InvoiceLines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "InvoiceLines",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "InvoiceLines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "InvoiceAmendments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "InvoiceAmendments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "InvoiceAmendments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "InvoiceAmendments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "InvoiceAmendments",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "InternalOrders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "InternalOrders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ResponsiblePerson",
                table: "InternalOrders",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProjectManager",
                table: "InternalOrders",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Priority",
                table: "InternalOrders",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CommittedAmount",
                table: "InternalOrders",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "BudgetAmount",
                table: "InternalOrders",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "ActualAmount",
                table: "InternalOrders",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "InternalOrders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "InternalOrders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "InternalOrders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "InternalOrders",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "InternalControls",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "InternalControls",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Owner",
                table: "InternalControls",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Frequency",
                table: "InternalControls",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Effectiveness",
                table: "InternalControls",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Department",
                table: "InternalControls",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "InternalControls",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "InternalControls",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "InternalControls",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "InternalControls",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "InternalControls",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "InternalControls",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "IFRSReports",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "IFRSReports",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "IFRSReports",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "IFRSReports",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "IFRSReports",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "FinancialPeriods",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "FinancialPeriods",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "FinancialPeriods",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "FinancialPeriods",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "FinancialPeriods",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PeriodId",
                table: "Expenses",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Expenses",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Expenses",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "Expenses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "Expenses",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "Expenses",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameAm",
                table: "ExpenseCategories",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ExpenseCategories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "ExpenseCategories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "ExpenseCategories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ExpenseCategories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "ExpenseCategories",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Entities",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "OwnershipPercentage",
                table: "Entities",
                type: "numeric(5,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConsolidationMethod",
                table: "Entities",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Entities",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "Entities",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "Entities",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "Entities",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "Entities",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PostedBy",
                table: "EliminationEntries",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ExchangeRate",
                table: "EliminationEntries",
                type: "numeric(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountInReportingCurrency",
                table: "EliminationEntries",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "EliminationEntries",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "EliminationEntries",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "EliminationEntries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "EliminationEntries",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "EliminationEntries",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "EliminationEntries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Customers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "Customers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "Customers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BudgetHolder",
                table: "CostCenters",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "CostCenters",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "CostCenters",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "CostCenters",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "CostCenters",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "CostCenters",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalRevenue",
                table: "ConsolidationReports",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalLiabilities",
                table: "ConsolidationReports",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalEquity",
                table: "ConsolidationReports",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAssets",
                table: "ConsolidationReports",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "NetIncome",
                table: "ConsolidationReports",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ConsolidationReports",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "ConsolidationReports",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "ConsolidationReports",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ConsolidationReports",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "ConsolidationReports",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalRevenue",
                table: "ConsolidationGroups",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalProfit",
                table: "ConsolidationGroups",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalLiabilities",
                table: "ConsolidationGroups",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalExpenses",
                table: "ConsolidationGroups",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalEquity",
                table: "ConsolidationGroups",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAssets",
                table: "ConsolidationGroups",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ConsolidationGroups",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ConsolidationGroups",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "ConsolidationGroups",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ConsolidationGroups",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "ConsolidationGroups",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ConsolidationGroupEntities",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "ConsolidationGroupEntities",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAdd",
                table: "ConsolidationGroupEntities",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateMod",
                table: "ConsolidationGroupEntities",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ConsolidationGroupEntities",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "ConsolidationGroupEntities",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RowVersion",
                table: "ConsolidationGroupEntities",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ConsolidationGroupEntities",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "ConsolidationGroupEntities",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Section",
                table: "ComplianceRequirements",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RiskLevel",
                table: "ComplianceRequirements",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Regulation",
                table: "ComplianceRequirements",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Owner",
                table: "ComplianceRequirements",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "ComplianceRequirements",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ComplianceStatus",
                table: "ComplianceRequirements",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ComplianceRequirements",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "ComplianceRequirements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "ComplianceRequirements",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ComplianceRequirements",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "ComplianceRequirements",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ComplianceScore",
                table: "ComplianceReports",
                type: "numeric(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ComplianceReports",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "ComplianceReports",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "ComplianceReports",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ComplianceReports",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "ComplianceReports",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SerialNumber",
                table: "ChartOfAccounts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "SalvageValue",
                table: "ChartOfAccounts",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "OpeningBalance",
                table: "ChartOfAccounts",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameAm",
                table: "ChartOfAccounts",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Model",
                table: "ChartOfAccounts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Manufacturer",
                table: "ChartOfAccounts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "ChartOfAccounts",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AssignedTo",
                table: "ChartOfAccounts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ChartOfAccounts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "ChartOfAccounts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "ChartOfAccounts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ChartOfAccounts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "ChartOfAccounts",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "Budgets",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<Guid>(
                name: "PeriodId",
                table: "Budgets",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Budgets",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "Budgets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "Budgets",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "Budgets",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "SpentAmount",
                table: "BudgetLines",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<Guid>(
                name: "PeriodId",
                table: "BudgetLines",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<decimal>(
                name: "AllocatedAmount",
                table: "BudgetLines",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "BudgetLines",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "BudgetLines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "BudgetLines",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "BudgetLines",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "BudgetCodes",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "BudgetCodes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "BudgetCodes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "BudgetCodes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "BudgetCodes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "BudgetCodes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "BudgetCategories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "BudgetCategories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "BudgetCategories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "BudgetCategories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "BudgetCategories",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TransactionType",
                table: "BankTransactions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Reference",
                table: "BankTransactions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<Guid>(
                name: "PeriodId",
                table: "BankTransactions",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "BankTransactions",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "BankTransactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "BankTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "BankTransactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "BankTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "BankAccounts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "BankAccounts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "BankAccounts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "BankAccounts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "BankAccounts",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserRole",
                table: "AuditLogs",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "AuditLogs",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "OldValues",
                table: "AuditLogs",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "NewValues",
                table: "AuditLogs",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "MetadataJson",
                table: "AuditLogs",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "AuditLogs",
                type: "character varying(45)",
                maxLength: 45,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(45)",
                oldMaxLength: 45);

            migrationBuilder.AlterColumn<string>(
                name: "ErrorMessage",
                table: "AuditLogs",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ChangesJson",
                table: "AuditLogs",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "jsonb");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "AuditLogs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "AuditLogs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AuditLogs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "AuditLogs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "AuditLogs",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "ApprovalSteps",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ApproverName",
                table: "ApprovalSteps",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ApprovalSteps",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "ApprovalSteps",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "ApprovalSteps",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ApprovalSteps",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "ApprovalSteps",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "AccountCategories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "AccountCategories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "AccountCategories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "AccountCategories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "AccountCategories",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "IFRSMetrics",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "PreviousValue",
                table: "IFRSMetrics",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ChangePercentage",
                table: "IFRSMetrics",
                type: "numeric(5,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Change",
                table: "IFRSMetrics",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "IFRSMetrics",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "IFRSMetrics",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "IFRSMetrics",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "IFRSMetrics",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "IFRSMetrics",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "ComplianceRequirementEvidences",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "ComplianceRequirementEvidences",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ComplianceRequirementEvidences",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "ComplianceRequirementEvidences",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAdd",
                table: "ComplianceRequirementEvidences",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateMod",
                table: "ComplianceRequirementEvidences",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ComplianceRequirementEvidences",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "ComplianceRequirementEvidences",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "ComplianceRequirementEvidences",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ComplianceRequirementEvidences",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "ComplianceRequirementEvidences",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RowVersion",
                table: "ComplianceRequirementEvidences",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ComplianceRequirementEvidences",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "ComplianceRequirementEvidences",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UploadedBy",
                table: "ComplianceRequirementEvidences",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ComplianceRequirementControls",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ControlName",
                table: "ComplianceRequirementControls",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "ControlType",
                table: "ComplianceRequirementControls",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ComplianceRequirementControls",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "ComplianceRequirementControls",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAdd",
                table: "ComplianceRequirementControls",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateMod",
                table: "ComplianceRequirementControls",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Frequency",
                table: "ComplianceRequirementControls",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ImplementationDate",
                table: "ComplianceRequirementControls",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ComplianceRequirementControls",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsImplemented",
                table: "ComplianceRequirementControls",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "ComplianceRequirementControls",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RowVersion",
                table: "ComplianceRequirementControls",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ComplianceRequirementControls",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "ComplianceRequirementControls",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_IFRSMetrics",
                table: "IFRSMetrics",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ComplianceRequirementEvidences",
                table: "ComplianceRequirementEvidences",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ComplianceRequirementControls",
                table: "ComplianceRequirementControls",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "BudgetControls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BudgetId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    AllocatedAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SpentAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AvailableAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PercentageUsed = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    PeriodId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetControls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetControls_Budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetControls_ChartOfAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "ChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetControls_FinancialPeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "FinancialPeriods",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CreditNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NoteNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    NoteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: true),
                    InvoiceId = table.Column<Guid>(type: "uuid", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Reason = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ApprovedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PostedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    PostedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    PeriodId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditNotes_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CreditNotes_FinancialPeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "FinancialPeriods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CreditNotes_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CreditNotes_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FromCurrency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ToCurrency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Rate = table.Column<decimal>(type: "numeric(18,6)", nullable: false),
                    RateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RateType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Source = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Provider = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Metadata = table.Column<string>(type: "jsonb", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    PeriodId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrencyRates_FinancialPeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "FinancialPeriods",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DebitNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NoteNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    NoteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    InvoiceId = table.Column<Guid>(type: "uuid", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Reason = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ApprovedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PostedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    PostedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    PeriodId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebitNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DebitNotes_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DebitNotes_FinancialPeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "FinancialPeriods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DebitNotes_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DebitNotes_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    PaymentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Frequency = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NextPaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OccurrenceCount = table.Column<int>(type: "integer", nullable: true),
                    CurrentOccurrence = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PaymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AccountNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BankAccountId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    PeriodId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentSchedules_BankAccounts_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentSchedules_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentSchedules_FinancialPeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "FinancialPeriods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaymentSchedules_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NameAm = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SubCategory = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UnitOfMeasure = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TaxRate = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    StockQuantity = table.Column<int>(type: "integer", nullable: true),
                    ReorderLevel = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    Attributes = table.Column<string>(type: "jsonb", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    PeriodId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_FinancialPeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "FinancialPeriods",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Receipts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiptNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ReceiptDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReceiptType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: true),
                    InvoiceId = table.Column<Guid>(type: "uuid", nullable: true),
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Reference = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BankAccountId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    PeriodId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Receipts_BankAccounts_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Receipts_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Receipts_FinancialPeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "FinancialPeriods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Receipts_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SubTotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: true),
                    SalesRepId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    PeriodId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesOrders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrders_FinancialPeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "FinancialPeriods",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PaymentScheduleHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    PeriodId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentScheduleHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentScheduleHistories_FinancialPeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "FinancialPeriods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaymentScheduleHistories_PaymentSchedules_PaymentScheduleId",
                        column: x => x.PaymentScheduleId,
                        principalTable: "PaymentSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditNoteLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditNoteId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    TaxRate = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    PeriodId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditNoteLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditNoteLines_CreditNotes_CreditNoteId",
                        column: x => x.CreditNoteId,
                        principalTable: "CreditNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreditNoteLines_FinancialPeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "FinancialPeriods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CreditNoteLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DebitNoteLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DebitNoteId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    TaxRate = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    PeriodId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebitNoteLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DebitNoteLines_DebitNotes_DebitNoteId",
                        column: x => x.DebitNoteId,
                        principalTable: "DebitNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DebitNoteLines_FinancialPeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "FinancialPeriods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DebitNoteLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductInventories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    TransactionType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReferenceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    PeriodId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductInventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductInventories_FinancialPeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "FinancialPeriods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductInventories_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductPrices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PriceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    PeriodId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPrices_FinancialPeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "FinancialPeriods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductPrices_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SalesOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TaxRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    PeriodId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesOrderLines_FinancialPeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "FinancialPeriods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SalesOrderLines_SalesOrders_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "SalesOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_Status",
                table: "Vouchers",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_VoucherDate",
                table: "Vouchers",
                column: "VoucherDate");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_VoucherNumber",
                table: "Vouchers",
                column: "VoucherNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_VoucherType",
                table: "Vouchers",
                column: "VoucherType");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_PeriodId",
                table: "Vendors",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorPortalUsers_Email",
                table: "VendorPortalUsers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_VendorPortalUsers_PeriodId",
                table: "VendorPortalUsers",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorPortalUsers_Status",
                table: "VendorPortalUsers",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TaxReturns_Code",
                table: "TaxReturns",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaxReturns_Period",
                table: "TaxReturns",
                column: "Period");

            migrationBuilder.CreateIndex(
                name: "IX_TaxReturns_PeriodId",
                table: "TaxReturns",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxReturns_Status",
                table: "TaxReturns",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TaxReturns_TaxType",
                table: "TaxReturns",
                column: "TaxType");

            migrationBuilder.CreateIndex(
                name: "IX_TaxRates_Code",
                table: "TaxRates",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaxRates_IsActive",
                table: "TaxRates",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TaxRates_PeriodId",
                table: "TaxRates",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_OrderDate",
                table: "PurchaseOrders",
                column: "OrderDate");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_PurchaseOrderNumber",
                table: "PurchaseOrders",
                column: "PurchaseOrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_Status",
                table: "PurchaseOrders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ProfitCenters_Code",
                table: "ProfitCenters",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfitCenters_IsActive",
                table: "ProfitCenters",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ProfitCenters_PeriodId",
                table: "ProfitCenters",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_PortalPayments_PaymentDate",
                table: "PortalPayments",
                column: "PaymentDate");

            migrationBuilder.CreateIndex(
                name: "IX_PortalPayments_PaymentNumber",
                table: "PortalPayments",
                column: "PaymentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PortalPayments_PeriodId",
                table: "PortalPayments",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_PortalPayments_Status",
                table: "PortalPayments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PortalNotifications_IsRead",
                table: "PortalNotifications",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_PortalNotifications_PeriodId",
                table: "PortalNotifications",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_PortalNotifications_Type",
                table: "PortalNotifications",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_PortalInvoices_InvoiceDate",
                table: "PortalInvoices",
                column: "InvoiceDate");

            migrationBuilder.CreateIndex(
                name: "IX_PortalInvoices_InvoiceNumber",
                table: "PortalInvoices",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PortalInvoices_PeriodId",
                table: "PortalInvoices",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_PortalInvoices_Status",
                table: "PortalInvoices",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentApprovalChains_PeriodId",
                table: "PaymentApprovalChains",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalEmployees_FirstName",
                table: "LocalEmployees",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_LocalBranches_Name",
                table: "LocalBranches",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_EntryDate",
                table: "JournalEntries",
                column: "EntryDate");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_IsPosted",
                table: "JournalEntries",
                column: "IsPosted");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_Reference",
                table: "JournalEntries",
                column: "Reference");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceAmendments_PeriodId",
                table: "InvoiceAmendments",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_InternalOrders_Code",
                table: "InternalOrders",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InternalOrders_EndDate",
                table: "InternalOrders",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_InternalOrders_StartDate",
                table: "InternalOrders",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_InternalOrders_Status",
                table: "InternalOrders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InternalControls_Category",
                table: "InternalControls",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_InternalControls_Code",
                table: "InternalControls",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InternalControls_PeriodId",
                table: "InternalControls",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_InternalControls_Status",
                table: "InternalControls",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InternalControls_Type",
                table: "InternalControls",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_IFRSReports_PeriodId",
                table: "IFRSReports",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_IFRSReports_Standard",
                table: "IFRSReports",
                column: "Standard");

            migrationBuilder.CreateIndex(
                name: "IX_IFRSReports_Status",
                table: "IFRSReports",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialPeriods_EndDate",
                table: "FinancialPeriods",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialPeriods_IsClosed",
                table: "FinancialPeriods",
                column: "IsClosed");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialPeriods_PeriodId",
                table: "FinancialPeriods",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialPeriods_StartDate",
                table: "FinancialPeriods",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialPeriods_Status",
                table: "FinancialPeriods",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseDate",
                table: "Expenses",
                column: "ExpenseDate");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_Status",
                table: "Expenses",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategories_CategoryType",
                table: "ExpenseCategories",
                column: "CategoryType");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategories_IsActive",
                table: "ExpenseCategories",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategories_PeriodId",
                table: "ExpenseCategories",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_Code",
                table: "Entities",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Entities_IsActive",
                table: "Entities",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_IsDeleted",
                table: "Entities",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_PeriodId",
                table: "Entities",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_Type",
                table: "Entities",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_EliminationEntries_Code",
                table: "EliminationEntries",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EliminationEntries_PeriodId",
                table: "EliminationEntries",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_EliminationEntries_Status",
                table: "EliminationEntries",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EliminationEntries_Type",
                table: "EliminationEntries",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PeriodId",
                table: "Customers",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenters_Code",
                table: "CostCenters",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CostCenters_IsActive",
                table: "CostCenters",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenters_PeriodId",
                table: "CostCenters",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidationReports_Code",
                table: "ConsolidationReports",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidationReports_PeriodId",
                table: "ConsolidationReports",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidationReports_Status",
                table: "ConsolidationReports",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidationReports_Type",
                table: "ConsolidationReports",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidationGroups_ConsolidationDate",
                table: "ConsolidationGroups",
                column: "ConsolidationDate");

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidationGroups_Name",
                table: "ConsolidationGroups",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidationGroups_Status",
                table: "ConsolidationGroups",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidationGroupEntities_PeriodId",
                table: "ConsolidationGroupEntities",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceRequirements_Code",
                table: "ComplianceRequirements",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceRequirements_ComplianceStatus",
                table: "ComplianceRequirements",
                column: "ComplianceStatus");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceRequirements_PeriodId",
                table: "ComplianceRequirements",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceRequirements_RiskLevel",
                table: "ComplianceRequirements",
                column: "RiskLevel");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceReports_Code",
                table: "ComplianceReports",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceReports_PeriodId",
                table: "ComplianceReports",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceReports_Status",
                table: "ComplianceReports",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceReports_Type",
                table: "ComplianceReports",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ChartOfAccounts_AccountType",
                table: "ChartOfAccounts",
                column: "AccountType");

            migrationBuilder.CreateIndex(
                name: "IX_ChartOfAccounts_IsActive",
                table: "ChartOfAccounts",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ChartOfAccounts_IsDeleted",
                table: "ChartOfAccounts",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ChartOfAccounts_PeriodId",
                table: "ChartOfAccounts",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_EndDate",
                table: "Budgets",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_Name",
                table: "Budgets",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_StartDate",
                table: "Budgets",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_Status",
                table: "Budgets",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetCodes_PeriodId",
                table: "BudgetCodes",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetCategories_PeriodId",
                table: "BudgetCategories",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_Status",
                table: "BankTransactions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_TransactionDate",
                table: "BankTransactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_TransactionType",
                table: "BankTransactions",
                column: "TransactionType");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_AccountNumber",
                table: "BankAccounts",
                column: "AccountNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_AccountType",
                table: "BankAccounts",
                column: "AccountType");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_IsActive",
                table: "BankAccounts",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_PeriodId",
                table: "BankAccounts",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Action",
                table: "AuditLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ActionDate",
                table: "AuditLogs",
                column: "ActionDate");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityId",
                table: "AuditLogs",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType",
                table: "AuditLogs",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_PeriodId",
                table: "AuditLogs",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalSteps_PeriodId",
                table: "ApprovalSteps",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCategories_Code",
                table: "AccountCategories",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountCategories_PeriodId",
                table: "AccountCategories",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCategories_Type",
                table: "AccountCategories",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_IFRSMetrics_PeriodId",
                table: "IFRSMetrics",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceRequirementEvidences_PeriodId",
                table: "ComplianceRequirementEvidences",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceRequirementControls_PeriodId",
                table: "ComplianceRequirementControls",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetControls_AccountId",
                table: "BudgetControls",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetControls_BudgetId",
                table: "BudgetControls",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetControls_PeriodId",
                table: "BudgetControls",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetControls_Status",
                table: "BudgetControls",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNoteLines_CreditNoteId",
                table: "CreditNoteLines",
                column: "CreditNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNoteLines_PeriodId",
                table: "CreditNoteLines",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNoteLines_ProductId",
                table: "CreditNoteLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNotes_CustomerId",
                table: "CreditNotes",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNotes_InvoiceId",
                table: "CreditNotes",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNotes_NoteDate",
                table: "CreditNotes",
                column: "NoteDate");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNotes_NoteNumber",
                table: "CreditNotes",
                column: "NoteNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditNotes_PeriodId",
                table: "CreditNotes",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNotes_Status",
                table: "CreditNotes",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNotes_VendorId",
                table: "CreditNotes",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyRates_FromCurrency_ToCurrency_RateDate",
                table: "CurrencyRates",
                columns: new[] { "FromCurrency", "ToCurrency", "RateDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyRates_PeriodId",
                table: "CurrencyRates",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_DebitNoteLines_DebitNoteId",
                table: "DebitNoteLines",
                column: "DebitNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_DebitNoteLines_PeriodId",
                table: "DebitNoteLines",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_DebitNoteLines_ProductId",
                table: "DebitNoteLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_DebitNotes_CustomerId",
                table: "DebitNotes",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_DebitNotes_InvoiceId",
                table: "DebitNotes",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_DebitNotes_NoteDate",
                table: "DebitNotes",
                column: "NoteDate");

            migrationBuilder.CreateIndex(
                name: "IX_DebitNotes_NoteNumber",
                table: "DebitNotes",
                column: "NoteNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DebitNotes_PeriodId",
                table: "DebitNotes",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_DebitNotes_Status",
                table: "DebitNotes",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_DebitNotes_VendorId",
                table: "DebitNotes",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentScheduleHistories_PaymentScheduleId",
                table: "PaymentScheduleHistories",
                column: "PaymentScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentScheduleHistories_PeriodId",
                table: "PaymentScheduleHistories",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentScheduleHistories_ScheduledDate",
                table: "PaymentScheduleHistories",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentScheduleHistories_Status",
                table: "PaymentScheduleHistories",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSchedules_BankAccountId",
                table: "PaymentSchedules",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSchedules_CustomerId",
                table: "PaymentSchedules",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSchedules_Name",
                table: "PaymentSchedules",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSchedules_NextPaymentDate",
                table: "PaymentSchedules",
                column: "NextPaymentDate");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSchedules_PeriodId",
                table: "PaymentSchedules",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSchedules_Status",
                table: "PaymentSchedules",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSchedules_VendorId",
                table: "PaymentSchedules",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductInventories_PeriodId",
                table: "ProductInventories",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductInventories_ProductId",
                table: "ProductInventories",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductInventories_TransactionDate",
                table: "ProductInventories",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPrices_EffectiveDate",
                table: "ProductPrices",
                column: "EffectiveDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPrices_PeriodId",
                table: "ProductPrices",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPrices_ProductId",
                table: "ProductPrices",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Category",
                table: "Products",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Code",
                table: "Products",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsActive",
                table: "Products",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Products_PeriodId",
                table: "Products",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_BankAccountId",
                table: "Receipts",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_CustomerId",
                table: "Receipts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_PeriodId",
                table: "Receipts",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_ReceiptDate",
                table: "Receipts",
                column: "ReceiptDate");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_ReceiptNumber",
                table: "Receipts",
                column: "ReceiptNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_Status",
                table: "Receipts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_VendorId",
                table: "Receipts",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderLines_PeriodId",
                table: "SalesOrderLines",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderLines_SalesOrderId",
                table: "SalesOrderLines",
                column: "SalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_CustomerId",
                table: "SalesOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_OrderDate",
                table: "SalesOrders",
                column: "OrderDate");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_OrderNumber",
                table: "SalesOrders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_PeriodId",
                table: "SalesOrders",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_Status",
                table: "SalesOrders",
                column: "Status");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountCategories_AccountCategories_ParentId",
                table: "AccountCategories",
                column: "ParentId",
                principalTable: "AccountCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountCategories_FinancialPeriods_PeriodId",
                table: "AccountCategories",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalSteps_FinancialPeriods_PeriodId",
                table: "ApprovalSteps",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_FinancialPeriods_PeriodId",
                table: "AuditLogs",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_FinancialPeriods_PeriodId",
                table: "BankAccounts",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankTransactions_FinancialPeriods_PeriodId",
                table: "BankTransactions",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetCategories_FinancialPeriods_PeriodId",
                table: "BudgetCategories",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetCodes_FinancialPeriods_PeriodId",
                table: "BudgetCodes",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetLines_ChartOfAccounts_AccountId",
                table: "BudgetLines",
                column: "AccountId",
                principalTable: "ChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetLines_FinancialPeriods_PeriodId",
                table: "BudgetLines",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Budgets_FinancialPeriods_PeriodId",
                table: "Budgets",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChartOfAccounts_FinancialPeriods_PeriodId",
                table: "ChartOfAccounts",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ComplianceReports_FinancialPeriods_PeriodId",
                table: "ComplianceReports",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ComplianceRequirementControls_ComplianceRequirements_Compli~",
                table: "ComplianceRequirementControls",
                column: "ComplianceRequirementId",
                principalTable: "ComplianceRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ComplianceRequirementControls_FinancialPeriods_PeriodId",
                table: "ComplianceRequirementControls",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ComplianceRequirementEvidences_ComplianceRequirements_Compl~",
                table: "ComplianceRequirementEvidences",
                column: "ComplianceRequirementId",
                principalTable: "ComplianceRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ComplianceRequirementEvidences_FinancialPeriods_PeriodId",
                table: "ComplianceRequirementEvidences",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ComplianceRequirements_FinancialPeriods_PeriodId",
                table: "ComplianceRequirements",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsolidationGroupEntities_Entities_EntityId",
                table: "ConsolidationGroupEntities",
                column: "EntityId",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ConsolidationGroupEntities_FinancialPeriods_PeriodId",
                table: "ConsolidationGroupEntities",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsolidationGroups_Entities_ParentEntityId",
                table: "ConsolidationGroups",
                column: "ParentEntityId",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ConsolidationReports_ConsolidationGroups_ConsolidationGroup~",
                table: "ConsolidationReports",
                column: "ConsolidationGroupId",
                principalTable: "ConsolidationGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenters_CostCenters_ParentId",
                table: "CostCenters",
                column: "ParentId",
                principalTable: "CostCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenters_FinancialPeriods_PeriodId",
                table: "CostCenters",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_FinancialPeriods_PeriodId",
                table: "Customers",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EliminationEntries_ConsolidationGroups_ConsolidationGroupId",
                table: "EliminationEntries",
                column: "ConsolidationGroupId",
                principalTable: "ConsolidationGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EliminationEntries_Entities_FromEntityId",
                table: "EliminationEntries",
                column: "FromEntityId",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EliminationEntries_Entities_ToEntityId",
                table: "EliminationEntries",
                column: "ToEntityId",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Entities_Entities_ParentEntityId",
                table: "Entities",
                column: "ParentEntityId",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Entities_FinancialPeriods_PeriodId",
                table: "Entities",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseCategories_FinancialPeriods_PeriodId",
                table: "ExpenseCategories",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpenseCategories_ExpenseCategoryId",
                table: "Expenses",
                column: "ExpenseCategoryId",
                principalTable: "ExpenseCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_FinancialPeriods_PeriodId",
                table: "Expenses",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialPeriods_FinancialPeriods_PeriodId",
                table: "FinancialPeriods",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IFRSMetrics_FinancialPeriods_PeriodId",
                table: "IFRSMetrics",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IFRSMetrics_IFRSReports_ReportId",
                table: "IFRSMetrics",
                column: "ReportId",
                principalTable: "IFRSReports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InternalControls_FinancialPeriods_PeriodId",
                table: "InternalControls",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InternalOrders_CostCenters_CostCenterId",
                table: "InternalOrders",
                column: "CostCenterId",
                principalTable: "CostCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceAmendments_FinancialPeriods_PeriodId",
                table: "InvoiceAmendments",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceLines_FinancialPeriods_PeriodId",
                table: "InvoiceLines",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_FinancialPeriods_PeriodId",
                table: "Invoices",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntries_FinancialPeriods_PeriodId",
                table: "JournalEntries",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalLines_ChartOfAccounts_AccountId",
                table: "JournalLines",
                column: "AccountId",
                principalTable: "ChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalLines_FinancialPeriods_PeriodId",
                table: "JournalLines",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentApprovalChains_FinancialPeriods_PeriodId",
                table: "PaymentApprovalChains",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_FinancialPeriods_PeriodId",
                table: "Payments",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PortalInvoices_FinancialPeriods_PeriodId",
                table: "PortalInvoices",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PortalInvoices_Vendors_VendorId",
                table: "PortalInvoices",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PortalNotifications_FinancialPeriods_PeriodId",
                table: "PortalNotifications",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PortalPayments_FinancialPeriods_PeriodId",
                table: "PortalPayments",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PortalPayments_PortalInvoices_InvoiceId",
                table: "PortalPayments",
                column: "InvoiceId",
                principalTable: "PortalInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PortalPayments_Vendors_VendorId",
                table: "PortalPayments",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfitCenters_FinancialPeriods_PeriodId",
                table: "ProfitCenters",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfitCenters_ProfitCenters_ParentId",
                table: "ProfitCenters",
                column: "ParentId",
                principalTable: "ProfitCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderLines_FinancialPeriods_PeriodId",
                table: "PurchaseOrderLines",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_FinancialPeriods_PeriodId",
                table: "PurchaseOrders",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Vendors_VendorId",
                table: "PurchaseOrders",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaxRates_FinancialPeriods_PeriodId",
                table: "TaxRates",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorPortalUsers_FinancialPeriods_PeriodId",
                table: "VendorPortalUsers",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_FinancialPeriods_PeriodId",
                table: "Vendors",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherLines_ChartOfAccounts_AccountId",
                table: "VoucherLines",
                column: "AccountId",
                principalTable: "ChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vouchers_Vendors_VendorId",
                table: "Vouchers",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountCategories_AccountCategories_ParentId",
                table: "AccountCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountCategories_FinancialPeriods_PeriodId",
                table: "AccountCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalSteps_FinancialPeriods_PeriodId",
                table: "ApprovalSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_FinancialPeriods_PeriodId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_FinancialPeriods_PeriodId",
                table: "BankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_BankTransactions_FinancialPeriods_PeriodId",
                table: "BankTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetCategories_FinancialPeriods_PeriodId",
                table: "BudgetCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetCodes_FinancialPeriods_PeriodId",
                table: "BudgetCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetLines_ChartOfAccounts_AccountId",
                table: "BudgetLines");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetLines_FinancialPeriods_PeriodId",
                table: "BudgetLines");

            migrationBuilder.DropForeignKey(
                name: "FK_Budgets_FinancialPeriods_PeriodId",
                table: "Budgets");

            migrationBuilder.DropForeignKey(
                name: "FK_ChartOfAccounts_FinancialPeriods_PeriodId",
                table: "ChartOfAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_ComplianceReports_FinancialPeriods_PeriodId",
                table: "ComplianceReports");

            migrationBuilder.DropForeignKey(
                name: "FK_ComplianceRequirementControls_ComplianceRequirements_Compli~",
                table: "ComplianceRequirementControls");

            migrationBuilder.DropForeignKey(
                name: "FK_ComplianceRequirementControls_FinancialPeriods_PeriodId",
                table: "ComplianceRequirementControls");

            migrationBuilder.DropForeignKey(
                name: "FK_ComplianceRequirementEvidences_ComplianceRequirements_Compl~",
                table: "ComplianceRequirementEvidences");

            migrationBuilder.DropForeignKey(
                name: "FK_ComplianceRequirementEvidences_FinancialPeriods_PeriodId",
                table: "ComplianceRequirementEvidences");

            migrationBuilder.DropForeignKey(
                name: "FK_ComplianceRequirements_FinancialPeriods_PeriodId",
                table: "ComplianceRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_ConsolidationGroupEntities_Entities_EntityId",
                table: "ConsolidationGroupEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_ConsolidationGroupEntities_FinancialPeriods_PeriodId",
                table: "ConsolidationGroupEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_ConsolidationGroups_Entities_ParentEntityId",
                table: "ConsolidationGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_ConsolidationReports_ConsolidationGroups_ConsolidationGroup~",
                table: "ConsolidationReports");

            migrationBuilder.DropForeignKey(
                name: "FK_CostCenters_CostCenters_ParentId",
                table: "CostCenters");

            migrationBuilder.DropForeignKey(
                name: "FK_CostCenters_FinancialPeriods_PeriodId",
                table: "CostCenters");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_FinancialPeriods_PeriodId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_EliminationEntries_ConsolidationGroups_ConsolidationGroupId",
                table: "EliminationEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_EliminationEntries_Entities_FromEntityId",
                table: "EliminationEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_EliminationEntries_Entities_ToEntityId",
                table: "EliminationEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_Entities_Entities_ParentEntityId",
                table: "Entities");

            migrationBuilder.DropForeignKey(
                name: "FK_Entities_FinancialPeriods_PeriodId",
                table: "Entities");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseCategories_FinancialPeriods_PeriodId",
                table: "ExpenseCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpenseCategories_ExpenseCategoryId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_FinancialPeriods_PeriodId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_FinancialPeriods_FinancialPeriods_PeriodId",
                table: "FinancialPeriods");

            migrationBuilder.DropForeignKey(
                name: "FK_IFRSMetrics_FinancialPeriods_PeriodId",
                table: "IFRSMetrics");

            migrationBuilder.DropForeignKey(
                name: "FK_IFRSMetrics_IFRSReports_ReportId",
                table: "IFRSMetrics");

            migrationBuilder.DropForeignKey(
                name: "FK_InternalControls_FinancialPeriods_PeriodId",
                table: "InternalControls");

            migrationBuilder.DropForeignKey(
                name: "FK_InternalOrders_CostCenters_CostCenterId",
                table: "InternalOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceAmendments_FinancialPeriods_PeriodId",
                table: "InvoiceAmendments");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceLines_FinancialPeriods_PeriodId",
                table: "InvoiceLines");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_FinancialPeriods_PeriodId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntries_FinancialPeriods_PeriodId",
                table: "JournalEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalLines_ChartOfAccounts_AccountId",
                table: "JournalLines");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalLines_FinancialPeriods_PeriodId",
                table: "JournalLines");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentApprovalChains_FinancialPeriods_PeriodId",
                table: "PaymentApprovalChains");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_FinancialPeriods_PeriodId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_PortalInvoices_FinancialPeriods_PeriodId",
                table: "PortalInvoices");

            migrationBuilder.DropForeignKey(
                name: "FK_PortalInvoices_Vendors_VendorId",
                table: "PortalInvoices");

            migrationBuilder.DropForeignKey(
                name: "FK_PortalNotifications_FinancialPeriods_PeriodId",
                table: "PortalNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_PortalPayments_FinancialPeriods_PeriodId",
                table: "PortalPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_PortalPayments_PortalInvoices_InvoiceId",
                table: "PortalPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_PortalPayments_Vendors_VendorId",
                table: "PortalPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfitCenters_FinancialPeriods_PeriodId",
                table: "ProfitCenters");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfitCenters_ProfitCenters_ParentId",
                table: "ProfitCenters");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderLines_FinancialPeriods_PeriodId",
                table: "PurchaseOrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_FinancialPeriods_PeriodId",
                table: "PurchaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Vendors_VendorId",
                table: "PurchaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxRates_FinancialPeriods_PeriodId",
                table: "TaxRates");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorPortalUsers_FinancialPeriods_PeriodId",
                table: "VendorPortalUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_FinancialPeriods_PeriodId",
                table: "Vendors");

            migrationBuilder.DropForeignKey(
                name: "FK_VoucherLines_ChartOfAccounts_AccountId",
                table: "VoucherLines");

            migrationBuilder.DropForeignKey(
                name: "FK_Vouchers_Vendors_VendorId",
                table: "Vouchers");

            migrationBuilder.DropTable(
                name: "BudgetControls");

            migrationBuilder.DropTable(
                name: "CreditNoteLines");

            migrationBuilder.DropTable(
                name: "CurrencyRates");

            migrationBuilder.DropTable(
                name: "DebitNoteLines");

            migrationBuilder.DropTable(
                name: "PaymentScheduleHistories");

            migrationBuilder.DropTable(
                name: "ProductInventories");

            migrationBuilder.DropTable(
                name: "ProductPrices");

            migrationBuilder.DropTable(
                name: "Receipts");

            migrationBuilder.DropTable(
                name: "SalesOrderLines");

            migrationBuilder.DropTable(
                name: "CreditNotes");

            migrationBuilder.DropTable(
                name: "DebitNotes");

            migrationBuilder.DropTable(
                name: "PaymentSchedules");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "SalesOrders");

            migrationBuilder.DropIndex(
                name: "IX_Vouchers_Status",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_Vouchers_VoucherDate",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_Vouchers_VoucherNumber",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_Vouchers_VoucherType",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_PeriodId",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_VendorPortalUsers_Email",
                table: "VendorPortalUsers");

            migrationBuilder.DropIndex(
                name: "IX_VendorPortalUsers_PeriodId",
                table: "VendorPortalUsers");

            migrationBuilder.DropIndex(
                name: "IX_VendorPortalUsers_Status",
                table: "VendorPortalUsers");

            migrationBuilder.DropIndex(
                name: "IX_TaxReturns_Code",
                table: "TaxReturns");

            migrationBuilder.DropIndex(
                name: "IX_TaxReturns_Period",
                table: "TaxReturns");

            migrationBuilder.DropIndex(
                name: "IX_TaxReturns_PeriodId",
                table: "TaxReturns");

            migrationBuilder.DropIndex(
                name: "IX_TaxReturns_Status",
                table: "TaxReturns");

            migrationBuilder.DropIndex(
                name: "IX_TaxReturns_TaxType",
                table: "TaxReturns");

            migrationBuilder.DropIndex(
                name: "IX_TaxRates_Code",
                table: "TaxRates");

            migrationBuilder.DropIndex(
                name: "IX_TaxRates_IsActive",
                table: "TaxRates");

            migrationBuilder.DropIndex(
                name: "IX_TaxRates_PeriodId",
                table: "TaxRates");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_OrderDate",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_PurchaseOrderNumber",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_Status",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_ProfitCenters_Code",
                table: "ProfitCenters");

            migrationBuilder.DropIndex(
                name: "IX_ProfitCenters_IsActive",
                table: "ProfitCenters");

            migrationBuilder.DropIndex(
                name: "IX_ProfitCenters_PeriodId",
                table: "ProfitCenters");

            migrationBuilder.DropIndex(
                name: "IX_PortalPayments_PaymentDate",
                table: "PortalPayments");

            migrationBuilder.DropIndex(
                name: "IX_PortalPayments_PaymentNumber",
                table: "PortalPayments");

            migrationBuilder.DropIndex(
                name: "IX_PortalPayments_PeriodId",
                table: "PortalPayments");

            migrationBuilder.DropIndex(
                name: "IX_PortalPayments_Status",
                table: "PortalPayments");

            migrationBuilder.DropIndex(
                name: "IX_PortalNotifications_IsRead",
                table: "PortalNotifications");

            migrationBuilder.DropIndex(
                name: "IX_PortalNotifications_PeriodId",
                table: "PortalNotifications");

            migrationBuilder.DropIndex(
                name: "IX_PortalNotifications_Type",
                table: "PortalNotifications");

            migrationBuilder.DropIndex(
                name: "IX_PortalInvoices_InvoiceDate",
                table: "PortalInvoices");

            migrationBuilder.DropIndex(
                name: "IX_PortalInvoices_InvoiceNumber",
                table: "PortalInvoices");

            migrationBuilder.DropIndex(
                name: "IX_PortalInvoices_PeriodId",
                table: "PortalInvoices");

            migrationBuilder.DropIndex(
                name: "IX_PortalInvoices_Status",
                table: "PortalInvoices");

            migrationBuilder.DropIndex(
                name: "IX_PaymentApprovalChains_PeriodId",
                table: "PaymentApprovalChains");

            migrationBuilder.DropIndex(
                name: "IX_LocalEmployees_FirstName",
                table: "LocalEmployees");

            migrationBuilder.DropIndex(
                name: "IX_LocalBranches_Name",
                table: "LocalBranches");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_EntryDate",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_IsPosted",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_Reference",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceAmendments_PeriodId",
                table: "InvoiceAmendments");

            migrationBuilder.DropIndex(
                name: "IX_InternalOrders_Code",
                table: "InternalOrders");

            migrationBuilder.DropIndex(
                name: "IX_InternalOrders_EndDate",
                table: "InternalOrders");

            migrationBuilder.DropIndex(
                name: "IX_InternalOrders_StartDate",
                table: "InternalOrders");

            migrationBuilder.DropIndex(
                name: "IX_InternalOrders_Status",
                table: "InternalOrders");

            migrationBuilder.DropIndex(
                name: "IX_InternalControls_Category",
                table: "InternalControls");

            migrationBuilder.DropIndex(
                name: "IX_InternalControls_Code",
                table: "InternalControls");

            migrationBuilder.DropIndex(
                name: "IX_InternalControls_PeriodId",
                table: "InternalControls");

            migrationBuilder.DropIndex(
                name: "IX_InternalControls_Status",
                table: "InternalControls");

            migrationBuilder.DropIndex(
                name: "IX_InternalControls_Type",
                table: "InternalControls");

            migrationBuilder.DropIndex(
                name: "IX_IFRSReports_PeriodId",
                table: "IFRSReports");

            migrationBuilder.DropIndex(
                name: "IX_IFRSReports_Standard",
                table: "IFRSReports");

            migrationBuilder.DropIndex(
                name: "IX_IFRSReports_Status",
                table: "IFRSReports");

            migrationBuilder.DropIndex(
                name: "IX_FinancialPeriods_EndDate",
                table: "FinancialPeriods");

            migrationBuilder.DropIndex(
                name: "IX_FinancialPeriods_IsClosed",
                table: "FinancialPeriods");

            migrationBuilder.DropIndex(
                name: "IX_FinancialPeriods_PeriodId",
                table: "FinancialPeriods");

            migrationBuilder.DropIndex(
                name: "IX_FinancialPeriods_StartDate",
                table: "FinancialPeriods");

            migrationBuilder.DropIndex(
                name: "IX_FinancialPeriods_Status",
                table: "FinancialPeriods");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_ExpenseDate",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_Status",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseCategories_CategoryType",
                table: "ExpenseCategories");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseCategories_IsActive",
                table: "ExpenseCategories");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseCategories_PeriodId",
                table: "ExpenseCategories");

            migrationBuilder.DropIndex(
                name: "IX_Entities_Code",
                table: "Entities");

            migrationBuilder.DropIndex(
                name: "IX_Entities_IsActive",
                table: "Entities");

            migrationBuilder.DropIndex(
                name: "IX_Entities_IsDeleted",
                table: "Entities");

            migrationBuilder.DropIndex(
                name: "IX_Entities_PeriodId",
                table: "Entities");

            migrationBuilder.DropIndex(
                name: "IX_Entities_Type",
                table: "Entities");

            migrationBuilder.DropIndex(
                name: "IX_EliminationEntries_Code",
                table: "EliminationEntries");

            migrationBuilder.DropIndex(
                name: "IX_EliminationEntries_PeriodId",
                table: "EliminationEntries");

            migrationBuilder.DropIndex(
                name: "IX_EliminationEntries_Status",
                table: "EliminationEntries");

            migrationBuilder.DropIndex(
                name: "IX_EliminationEntries_Type",
                table: "EliminationEntries");

            migrationBuilder.DropIndex(
                name: "IX_Customers_PeriodId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_CostCenters_Code",
                table: "CostCenters");

            migrationBuilder.DropIndex(
                name: "IX_CostCenters_IsActive",
                table: "CostCenters");

            migrationBuilder.DropIndex(
                name: "IX_CostCenters_PeriodId",
                table: "CostCenters");

            migrationBuilder.DropIndex(
                name: "IX_ConsolidationReports_Code",
                table: "ConsolidationReports");

            migrationBuilder.DropIndex(
                name: "IX_ConsolidationReports_PeriodId",
                table: "ConsolidationReports");

            migrationBuilder.DropIndex(
                name: "IX_ConsolidationReports_Status",
                table: "ConsolidationReports");

            migrationBuilder.DropIndex(
                name: "IX_ConsolidationReports_Type",
                table: "ConsolidationReports");

            migrationBuilder.DropIndex(
                name: "IX_ConsolidationGroups_ConsolidationDate",
                table: "ConsolidationGroups");

            migrationBuilder.DropIndex(
                name: "IX_ConsolidationGroups_Name",
                table: "ConsolidationGroups");

            migrationBuilder.DropIndex(
                name: "IX_ConsolidationGroups_Status",
                table: "ConsolidationGroups");

            migrationBuilder.DropIndex(
                name: "IX_ConsolidationGroupEntities_PeriodId",
                table: "ConsolidationGroupEntities");

            migrationBuilder.DropIndex(
                name: "IX_ComplianceRequirements_Code",
                table: "ComplianceRequirements");

            migrationBuilder.DropIndex(
                name: "IX_ComplianceRequirements_ComplianceStatus",
                table: "ComplianceRequirements");

            migrationBuilder.DropIndex(
                name: "IX_ComplianceRequirements_PeriodId",
                table: "ComplianceRequirements");

            migrationBuilder.DropIndex(
                name: "IX_ComplianceRequirements_RiskLevel",
                table: "ComplianceRequirements");

            migrationBuilder.DropIndex(
                name: "IX_ComplianceReports_Code",
                table: "ComplianceReports");

            migrationBuilder.DropIndex(
                name: "IX_ComplianceReports_PeriodId",
                table: "ComplianceReports");

            migrationBuilder.DropIndex(
                name: "IX_ComplianceReports_Status",
                table: "ComplianceReports");

            migrationBuilder.DropIndex(
                name: "IX_ComplianceReports_Type",
                table: "ComplianceReports");

            migrationBuilder.DropIndex(
                name: "IX_ChartOfAccounts_AccountType",
                table: "ChartOfAccounts");

            migrationBuilder.DropIndex(
                name: "IX_ChartOfAccounts_IsActive",
                table: "ChartOfAccounts");

            migrationBuilder.DropIndex(
                name: "IX_ChartOfAccounts_IsDeleted",
                table: "ChartOfAccounts");

            migrationBuilder.DropIndex(
                name: "IX_ChartOfAccounts_PeriodId",
                table: "ChartOfAccounts");

            migrationBuilder.DropIndex(
                name: "IX_Budgets_EndDate",
                table: "Budgets");

            migrationBuilder.DropIndex(
                name: "IX_Budgets_Name",
                table: "Budgets");

            migrationBuilder.DropIndex(
                name: "IX_Budgets_StartDate",
                table: "Budgets");

            migrationBuilder.DropIndex(
                name: "IX_Budgets_Status",
                table: "Budgets");

            migrationBuilder.DropIndex(
                name: "IX_BudgetCodes_PeriodId",
                table: "BudgetCodes");

            migrationBuilder.DropIndex(
                name: "IX_BudgetCategories_PeriodId",
                table: "BudgetCategories");

            migrationBuilder.DropIndex(
                name: "IX_BankTransactions_Status",
                table: "BankTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BankTransactions_TransactionDate",
                table: "BankTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BankTransactions_TransactionType",
                table: "BankTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_AccountNumber",
                table: "BankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_AccountType",
                table: "BankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_IsActive",
                table: "BankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_PeriodId",
                table: "BankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_Action",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_ActionDate",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_EntityId",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_EntityType",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_PeriodId",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_ApprovalSteps_PeriodId",
                table: "ApprovalSteps");

            migrationBuilder.DropIndex(
                name: "IX_AccountCategories_Code",
                table: "AccountCategories");

            migrationBuilder.DropIndex(
                name: "IX_AccountCategories_PeriodId",
                table: "AccountCategories");

            migrationBuilder.DropIndex(
                name: "IX_AccountCategories_Type",
                table: "AccountCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IFRSMetrics",
                table: "IFRSMetrics");

            migrationBuilder.DropIndex(
                name: "IX_IFRSMetrics_PeriodId",
                table: "IFRSMetrics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ComplianceRequirementEvidences",
                table: "ComplianceRequirementEvidences");

            migrationBuilder.DropIndex(
                name: "IX_ComplianceRequirementEvidences_PeriodId",
                table: "ComplianceRequirementEvidences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ComplianceRequirementControls",
                table: "ComplianceRequirementControls");

            migrationBuilder.DropIndex(
                name: "IX_ComplianceRequirementControls_PeriodId",
                table: "ComplianceRequirementControls");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "VoucherLines");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "VoucherLines");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "VoucherLines");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "VoucherLines");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "VendorPortalUsers");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "VendorPortalUsers");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "VendorPortalUsers");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "VendorPortalUsers");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "VendorPortalUsers");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "TaxReturns");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "TaxReturns");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "TaxReturns");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "TaxReturns");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "TaxReturns");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "TaxRates");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "TaxRates");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "TaxRates");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "TaxRates");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "TaxRates");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "PurchaseOrderLines");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "PurchaseOrderLines");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "PurchaseOrderLines");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "PurchaseOrderLines");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ProfitCenters");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "ProfitCenters");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "ProfitCenters");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ProfitCenters");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "ProfitCenters");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "PortalPayments");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "PortalPayments");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "PortalPayments");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "PortalPayments");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "PortalPayments");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "PortalNotifications");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "PortalNotifications");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PortalNotifications");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "PortalNotifications");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "PortalNotifications");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "PortalNotifications");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "PortalInvoices");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "PortalInvoices");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "PortalInvoices");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "PortalInvoices");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "PortalInvoices");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "PaymentApprovalChains");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "PaymentApprovalChains");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "PaymentApprovalChains");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "PaymentApprovalChains");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "PaymentApprovalChains");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "JournalLines");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "JournalLines");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "JournalLines");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "JournalLines");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "InvoiceAmendments");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "InvoiceAmendments");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "InvoiceAmendments");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "InvoiceAmendments");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "InvoiceAmendments");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "InternalOrders");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "InternalOrders");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "InternalOrders");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "InternalOrders");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "InternalControls");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "InternalControls");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "InternalControls");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "InternalControls");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "InternalControls");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "IFRSReports");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "IFRSReports");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "IFRSReports");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "IFRSReports");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "IFRSReports");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "FinancialPeriods");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "FinancialPeriods");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "FinancialPeriods");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "FinancialPeriods");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "FinancialPeriods");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ExpenseCategories");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "ExpenseCategories");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "ExpenseCategories");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ExpenseCategories");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "ExpenseCategories");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "EliminationEntries");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "EliminationEntries");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "EliminationEntries");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "EliminationEntries");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "EliminationEntries");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "CostCenters");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "CostCenters");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "CostCenters");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "CostCenters");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "CostCenters");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ConsolidationReports");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "ConsolidationReports");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "ConsolidationReports");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ConsolidationReports");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "ConsolidationReports");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ConsolidationGroups");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "ConsolidationGroups");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ConsolidationGroups");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "ConsolidationGroups");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ConsolidationGroupEntities");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "ConsolidationGroupEntities");

            migrationBuilder.DropColumn(
                name: "DateAdd",
                table: "ConsolidationGroupEntities");

            migrationBuilder.DropColumn(
                name: "DateMod",
                table: "ConsolidationGroupEntities");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ConsolidationGroupEntities");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "ConsolidationGroupEntities");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "ConsolidationGroupEntities");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ConsolidationGroupEntities");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "ConsolidationGroupEntities");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ComplianceRequirements");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "ComplianceRequirements");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "ComplianceRequirements");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ComplianceRequirements");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "ComplianceRequirements");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ComplianceReports");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "ComplianceReports");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "ComplianceReports");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ComplianceReports");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "ComplianceReports");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ChartOfAccounts");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "ChartOfAccounts");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "ChartOfAccounts");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ChartOfAccounts");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "ChartOfAccounts");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "BudgetLines");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "BudgetLines");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "BudgetLines");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "BudgetLines");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "BudgetCodes");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "BudgetCodes");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "BudgetCodes");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "BudgetCodes");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "BudgetCodes");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "BudgetCategories");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "BudgetCategories");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "BudgetCategories");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "BudgetCategories");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "BudgetCategories");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "BankTransactions");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "BankTransactions");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "BankTransactions");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "BankTransactions");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ApprovalSteps");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "ApprovalSteps");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "ApprovalSteps");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ApprovalSteps");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "ApprovalSteps");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "AccountCategories");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "AccountCategories");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "AccountCategories");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "AccountCategories");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "AccountCategories");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "IFRSMetrics");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "IFRSMetrics");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "IFRSMetrics");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "IFRSMetrics");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "IFRSMetrics");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ComplianceRequirementEvidences");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "ComplianceRequirementEvidences");

            migrationBuilder.DropColumn(
                name: "DateAdd",
                table: "ComplianceRequirementEvidences");

            migrationBuilder.DropColumn(
                name: "DateMod",
                table: "ComplianceRequirementEvidences");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ComplianceRequirementEvidences");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "ComplianceRequirementEvidences");

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "ComplianceRequirementEvidences");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ComplianceRequirementEvidences");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "ComplianceRequirementEvidences");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "ComplianceRequirementEvidences");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ComplianceRequirementEvidences");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "ComplianceRequirementEvidences");

            migrationBuilder.DropColumn(
                name: "UploadedBy",
                table: "ComplianceRequirementEvidences");

            migrationBuilder.DropColumn(
                name: "ControlType",
                table: "ComplianceRequirementControls");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ComplianceRequirementControls");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "ComplianceRequirementControls");

            migrationBuilder.DropColumn(
                name: "DateAdd",
                table: "ComplianceRequirementControls");

            migrationBuilder.DropColumn(
                name: "DateMod",
                table: "ComplianceRequirementControls");

            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "ComplianceRequirementControls");

            migrationBuilder.DropColumn(
                name: "ImplementationDate",
                table: "ComplianceRequirementControls");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ComplianceRequirementControls");

            migrationBuilder.DropColumn(
                name: "IsImplemented",
                table: "ComplianceRequirementControls");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "ComplianceRequirementControls");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "ComplianceRequirementControls");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ComplianceRequirementControls");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "ComplianceRequirementControls");

            migrationBuilder.RenameTable(
                name: "IFRSMetrics",
                newName: "IFRMetrics");

            migrationBuilder.RenameTable(
                name: "ComplianceRequirementEvidences",
                newName: "ComplianceRequirementEvidence");

            migrationBuilder.RenameTable(
                name: "ComplianceRequirementControls",
                newName: "ComplianceRequirementControl");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "AuditLogs",
                newName: "FinancialPeriodId");

            migrationBuilder.RenameIndex(
                name: "IX_IFRSMetrics_ReportId",
                table: "IFRMetrics",
                newName: "IX_IFRMetrics_ReportId");

            migrationBuilder.RenameIndex(
                name: "IX_ComplianceRequirementEvidences_ComplianceRequirementId",
                table: "ComplianceRequirementEvidence",
                newName: "IX_ComplianceRequirementEvidence_ComplianceRequirementId");

            migrationBuilder.RenameIndex(
                name: "IX_ComplianceRequirementControls_ComplianceRequirementId",
                table: "ComplianceRequirementControl",
                newName: "IX_ComplianceRequirementControl_ComplianceRequirementId");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalDebit",
                table: "Vouchers",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalCredit",
                table: "Vouchers",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "PostedBy",
                table: "Vouchers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApprovedBy",
                table: "Vouchers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DebitAmount",
                table: "VoucherLines",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CreditAmount",
                table: "VoucherLines",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Website",
                table: "Vendors",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalSpent",
                table: "Vendors",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Rating",
                table: "Vendors",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(3,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "VendorPortalUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "VendorPortalUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TaxableAmount",
                table: "TaxReturns",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TaxAmount",
                table: "TaxReturns",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BalanceDue",
                table: "TaxReturns",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountPaid",
                table: "TaxReturns",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Rate",
                table: "TaxRates",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)");

            migrationBuilder.AlterColumn<Guid>(
                name: "PeriodId",
                table: "PurchaseOrders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PeriodId",
                table: "PurchaseOrderLines",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Region",
                table: "ProfitCenters",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Manager",
                table: "ProfitCenters",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "PortalPayments",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "PortalNotifications",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Link",
                table: "PortalNotifications",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SubmittedBy",
                table: "PortalInvoices",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "PortalInvoices",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentReference",
                table: "PortalInvoices",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApprovedBy",
                table: "PortalInvoices",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "PortalInvoices",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<Guid>(
                name: "PeriodId",
                table: "Payments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MinAmount",
                table: "PaymentApprovalChains",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxAmount",
                table: "PaymentApprovalChains",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PeriodId",
                table: "JournalLines",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Debit",
                table: "JournalLines",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Credit",
                table: "JournalLines",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "JournalLines",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalDebit",
                table: "JournalEntries",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalCredit",
                table: "JournalEntries",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<Guid>(
                name: "PeriodId",
                table: "JournalEntries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PeriodId",
                table: "Invoices",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitPrice",
                table: "InvoiceLines",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "InvoiceLines",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TaxRate",
                table: "InvoiceLines",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<Guid>(
                name: "PeriodId",
                table: "InvoiceLines",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "InvoiceLines",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "InternalOrders",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "InternalOrders",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ResponsiblePerson",
                table: "InternalOrders",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProjectManager",
                table: "InternalOrders",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Priority",
                table: "InternalOrders",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CommittedAmount",
                table: "InternalOrders",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BudgetAmount",
                table: "InternalOrders",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ActualAmount",
                table: "InternalOrders",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "InternalControls",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "InternalControls",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Owner",
                table: "InternalControls",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Frequency",
                table: "InternalControls",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Effectiveness",
                table: "InternalControls",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Department",
                table: "InternalControls",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "InternalControls",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PeriodId",
                table: "Expenses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Expenses",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "NameAm",
                table: "ExpenseCategories",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Entities",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "OwnershipPercentage",
                table: "Entities",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConsolidationMethod",
                table: "Entities",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PostedBy",
                table: "EliminationEntries",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ExchangeRate",
                table: "EliminationEntries",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountInReportingCurrency",
                table: "EliminationEntries",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "EliminationEntries",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "BudgetHolder",
                table: "CostCenters",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalRevenue",
                table: "ConsolidationReports",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalLiabilities",
                table: "ConsolidationReports",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalEquity",
                table: "ConsolidationReports",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAssets",
                table: "ConsolidationReports",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "NetIncome",
                table: "ConsolidationReports",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalRevenue",
                table: "ConsolidationGroups",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalProfit",
                table: "ConsolidationGroups",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalLiabilities",
                table: "ConsolidationGroups",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalExpenses",
                table: "ConsolidationGroups",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalEquity",
                table: "ConsolidationGroups",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAssets",
                table: "ConsolidationGroups",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ConsolidationGroups",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Section",
                table: "ComplianceRequirements",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RiskLevel",
                table: "ComplianceRequirements",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Regulation",
                table: "ComplianceRequirements",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Owner",
                table: "ComplianceRequirements",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "ComplianceRequirements",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ComplianceStatus",
                table: "ComplianceRequirements",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ComplianceScore",
                table: "ComplianceReports",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)");

            migrationBuilder.AlterColumn<string>(
                name: "SerialNumber",
                table: "ChartOfAccounts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "SalvageValue",
                table: "ChartOfAccounts",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "OpeningBalance",
                table: "ChartOfAccounts",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameAm",
                table: "ChartOfAccounts",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Model",
                table: "ChartOfAccounts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Manufacturer",
                table: "ChartOfAccounts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "ChartOfAccounts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AssignedTo",
                table: "ChartOfAccounts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "Budgets",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<Guid>(
                name: "PeriodId",
                table: "Budgets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "SpentAmount",
                table: "BudgetLines",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<Guid>(
                name: "PeriodId",
                table: "BudgetLines",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AllocatedAmount",
                table: "BudgetLines",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "BudgetCodes",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TransactionType",
                table: "BankTransactions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Reference",
                table: "BankTransactions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "PeriodId",
                table: "BankTransactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "BankTransactions",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "UserRole",
                table: "AuditLogs",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "AuditLogs",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OldValues",
                table: "AuditLogs",
                type: "jsonb",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NewValues",
                table: "AuditLogs",
                type: "jsonb",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MetadataJson",
                table: "AuditLogs",
                type: "jsonb",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "AuditLogs",
                type: "character varying(45)",
                maxLength: 45,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(45)",
                oldMaxLength: 45,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ErrorMessage",
                table: "AuditLogs",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ChangesJson",
                table: "AuditLogs",
                type: "jsonb",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "ApprovalSteps",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "ApproverName",
                table: "ApprovalSteps",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "IFRMetrics",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PreviousValue",
                table: "IFRMetrics",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ChangePercentage",
                table: "IFRMetrics",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Change",
                table: "IFRMetrics",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "ComplianceRequirementEvidence",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "ComplianceRequirementEvidence",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ComplianceRequirementControl",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ControlName",
                table: "ComplianceRequirementControl",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AddPrimaryKey(
                name: "PK_IFRMetrics",
                table: "IFRMetrics",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ComplianceRequirementEvidence",
                table: "ComplianceRequirementEvidence",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ComplianceRequirementControl",
                table: "ComplianceRequirementControl",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentApprovalChains_Name",
                table: "PaymentApprovalChains",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLines_IsDeleted",
                table: "InvoiceLines",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceAmendments_RequestedAt",
                table: "InvoiceAmendments",
                column: "RequestedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_FinancialPeriodId",
                table: "AuditLogs",
                column: "FinancialPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalSteps_IsDeleted",
                table: "ApprovalSteps",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalSteps_Role",
                table: "ApprovalSteps",
                column: "Role");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountCategories_AccountCategories_ParentId",
                table: "AccountCategories",
                column: "ParentId",
                principalTable: "AccountCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_FinancialPeriods_FinancialPeriodId",
                table: "AuditLogs",
                column: "FinancialPeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankTransactions_FinancialPeriods_PeriodId",
                table: "BankTransactions",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetLines_ChartOfAccounts_AccountId",
                table: "BudgetLines",
                column: "AccountId",
                principalTable: "ChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetLines_FinancialPeriods_PeriodId",
                table: "BudgetLines",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Budgets_FinancialPeriods_PeriodId",
                table: "Budgets",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ComplianceRequirementControl_ComplianceRequirements_Complia~",
                table: "ComplianceRequirementControl",
                column: "ComplianceRequirementId",
                principalTable: "ComplianceRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ComplianceRequirementEvidence_ComplianceRequirements_Compli~",
                table: "ComplianceRequirementEvidence",
                column: "ComplianceRequirementId",
                principalTable: "ComplianceRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConsolidationGroupEntities_Entities_EntityId",
                table: "ConsolidationGroupEntities",
                column: "EntityId",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConsolidationGroups_Entities_ParentEntityId",
                table: "ConsolidationGroups",
                column: "ParentEntityId",
                principalTable: "Entities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsolidationReports_ConsolidationGroups_ConsolidationGroup~",
                table: "ConsolidationReports",
                column: "ConsolidationGroupId",
                principalTable: "ConsolidationGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenters_CostCenters_ParentId",
                table: "CostCenters",
                column: "ParentId",
                principalTable: "CostCenters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EliminationEntries_ConsolidationGroups_ConsolidationGroupId",
                table: "EliminationEntries",
                column: "ConsolidationGroupId",
                principalTable: "ConsolidationGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EliminationEntries_Entities_FromEntityId",
                table: "EliminationEntries",
                column: "FromEntityId",
                principalTable: "Entities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EliminationEntries_Entities_ToEntityId",
                table: "EliminationEntries",
                column: "ToEntityId",
                principalTable: "Entities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Entities_Entities_ParentEntityId",
                table: "Entities",
                column: "ParentEntityId",
                principalTable: "Entities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpenseCategories_ExpenseCategoryId",
                table: "Expenses",
                column: "ExpenseCategoryId",
                principalTable: "ExpenseCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_FinancialPeriods_PeriodId",
                table: "Expenses",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IFRMetrics_IFRSReports_ReportId",
                table: "IFRMetrics",
                column: "ReportId",
                principalTable: "IFRSReports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InternalOrders_CostCenters_CostCenterId",
                table: "InternalOrders",
                column: "CostCenterId",
                principalTable: "CostCenters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceLines_FinancialPeriods_PeriodId",
                table: "InvoiceLines",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_FinancialPeriods_PeriodId",
                table: "Invoices",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntries_FinancialPeriods_PeriodId",
                table: "JournalEntries",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalLines_ChartOfAccounts_AccountId",
                table: "JournalLines",
                column: "AccountId",
                principalTable: "ChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalLines_FinancialPeriods_PeriodId",
                table: "JournalLines",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_FinancialPeriods_PeriodId",
                table: "Payments",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PortalInvoices_Vendors_VendorId",
                table: "PortalInvoices",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PortalPayments_PortalInvoices_InvoiceId",
                table: "PortalPayments",
                column: "InvoiceId",
                principalTable: "PortalInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PortalPayments_Vendors_VendorId",
                table: "PortalPayments",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfitCenters_ProfitCenters_ParentId",
                table: "ProfitCenters",
                column: "ParentId",
                principalTable: "ProfitCenters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderLines_FinancialPeriods_PeriodId",
                table: "PurchaseOrderLines",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_FinancialPeriods_PeriodId",
                table: "PurchaseOrders",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Vendors_VendorId",
                table: "PurchaseOrders",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherLines_ChartOfAccounts_AccountId",
                table: "VoucherLines",
                column: "AccountId",
                principalTable: "ChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vouchers_Vendors_VendorId",
                table: "Vouchers",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id");
        }
    }
}
