using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Finance.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Expenses_ExpenseCategoryId",
                table: "Expenses");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CustomerId_IsDeleted",
                table: "Payments",
                columns: new[] { "CustomerId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentDate_IsDeleted",
                table: "Payments",
                columns: new[] { "PaymentDate", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PeriodId_IsDeleted",
                table: "Payments",
                columns: new[] { "PeriodId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Status_PaymentDate",
                table: "Payments",
                columns: new[] { "Status", "PaymentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_VendorId_IsDeleted",
                table: "Payments",
                columns: new[] { "VendorId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_JournalLines_AccountId_PeriodId",
                table: "JournalLines",
                columns: new[] { "AccountId", "PeriodId" });

            migrationBuilder.CreateIndex(
                name: "IX_JournalLines_JournalEntryId_AccountId",
                table: "JournalLines",
                columns: new[] { "JournalEntryId", "AccountId" });

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_EntryDate_IsDeleted",
                table: "JournalEntries",
                columns: new[] { "EntryDate", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_IsPosted_EntryDate",
                table: "JournalEntries",
                columns: new[] { "IsPosted", "EntryDate" });

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_PeriodId_IsDeleted",
                table: "JournalEntries",
                columns: new[] { "PeriodId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CustomerId_IsDeleted",
                table: "Invoices",
                columns: new[] { "CustomerId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceDate_IsDeleted",
                table: "Invoices",
                columns: new[] { "InvoiceDate", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_PeriodId_IsDeleted",
                table: "Invoices",
                columns: new[] { "PeriodId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Status_InvoiceDate",
                table: "Invoices",
                columns: new[] { "Status", "InvoiceDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_VendorId_IsDeleted",
                table: "Invoices",
                columns: new[] { "VendorId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_CategoryId_IsDeleted",
                table: "Expenses",
                columns: new[] { "ExpenseCategoryId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseDate_IsDeleted",
                table: "Expenses",
                columns: new[] { "ExpenseDate", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_PeriodId_IsDeleted",
                table: "Expenses",
                columns: new[] { "PeriodId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_Status_ExpenseDate",
                table: "Expenses",
                columns: new[] { "Status", "ExpenseDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_PeriodId_IsDeleted",
                table: "Budgets",
                columns: new[] { "PeriodId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_StartDate_EndDate",
                table: "Budgets",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_Status_StartDate",
                table: "Budgets",
                columns: new[] { "Status", "StartDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_CustomerId_IsDeleted",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PaymentDate_IsDeleted",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PeriodId_IsDeleted",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_Status_PaymentDate",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_VendorId_IsDeleted",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_JournalLines_AccountId_PeriodId",
                table: "JournalLines");

            migrationBuilder.DropIndex(
                name: "IX_JournalLines_JournalEntryId_AccountId",
                table: "JournalLines");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_EntryDate_IsDeleted",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_IsPosted_EntryDate",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_PeriodId_IsDeleted",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_CustomerId_IsDeleted",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_InvoiceDate_IsDeleted",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_PeriodId_IsDeleted",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_Status_InvoiceDate",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_VendorId_IsDeleted",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_CategoryId_IsDeleted",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_ExpenseDate_IsDeleted",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_PeriodId_IsDeleted",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_Status_ExpenseDate",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Budgets_PeriodId_IsDeleted",
                table: "Budgets");

            migrationBuilder.DropIndex(
                name: "IX_Budgets_StartDate_EndDate",
                table: "Budgets");

            migrationBuilder.DropIndex(
                name: "IX_Budgets_Status_StartDate",
                table: "Budgets");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseCategoryId",
                table: "Expenses",
                column: "ExpenseCategoryId");
        }
    }
}
