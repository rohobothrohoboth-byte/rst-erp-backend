using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Finance.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIncdddfgfgfgfddddgsdstw : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoices_Status_InvoiceDate",
                table: "Invoices");

            migrationBuilder.RenameIndex(
                name: "IX_Expenses_ExpenseDate_IsDeleted_Include",
                table: "Expenses",
                newName: "IX_Expenses_Date_IsDeleted_Include");

            migrationBuilder.RenameIndex(
                name: "IX_Assets_DateAdd_IsDeleted_Include",
                table: "Assets",
                newName: "IX_Assets_Date_IsDeleted_Include");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_AP_Status_Date",
                table: "Invoices",
                columns: new[] { "Status", "InvoiceDate" },
                filter: "\"InvoiceType\" = 'Purchase' AND \"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Customer_Date_Amount",
                table: "Invoices",
                columns: new[] { "CustomerId", "InvoiceDate", "TotalAmount" },
                filter: "\"InvoiceType\" = 'Sales' AND \"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Vendor_Date_Amount",
                table: "Invoices",
                columns: new[] { "VendorId", "InvoiceDate", "TotalAmount" },
                filter: "\"InvoiceType\" = 'Purchase' AND \"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_Date_Category_Include",
                table: "Expenses",
                columns: new[] { "ExpenseDate", "ExpenseCategoryId", "IsDeleted" },
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_Status_Date_Include",
                table: "Budgets",
                columns: new[] { "Status", "StartDate", "EndDate", "IsDeleted" },
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_Date_Status_Include",
                table: "Assets",
                columns: new[] { "DateAdd", "Status", "IsDeleted" },
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoices_AP_Status_Date",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_Customer_Date_Amount",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_Vendor_Date_Amount",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_Date_Category_Include",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Budgets_Status_Date_Include",
                table: "Budgets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_Date_Status_Include",
                table: "Assets");

            migrationBuilder.RenameIndex(
                name: "IX_Expenses_Date_IsDeleted_Include",
                table: "Expenses",
                newName: "IX_Expenses_ExpenseDate_IsDeleted_Include");

            migrationBuilder.RenameIndex(
                name: "IX_Assets_Date_IsDeleted_Include",
                table: "Assets",
                newName: "IX_Assets_DateAdd_IsDeleted_Include");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Status_InvoiceDate",
                table: "Invoices",
                columns: new[] { "Status", "InvoiceDate" });
        }
    }
}
