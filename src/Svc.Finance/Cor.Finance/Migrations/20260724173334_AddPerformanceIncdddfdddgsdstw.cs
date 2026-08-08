using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Finance.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIncdddfdddgsdstw : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Aging_DueDate_Status",
                table: "Invoices",
                columns: new[] { "DueDate", "Status", "TotalAmount", "PaidAmount" },
                filter: "\"Status\" != 'Paid' AND \"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Customer_All_Fields",
                table: "Invoices",
                columns: new[] { "CustomerId", "InvoiceDate", "TotalAmount", "PaidAmount" },
                filter: "\"InvoiceType\" = 'Sales' AND \"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Trend_Date_Type_Amount",
                table: "Invoices",
                columns: new[] { "InvoiceDate", "InvoiceType", "TotalAmount" },
                filter: "\"InvoiceType\" IN ('Sales', 'Purchase') AND \"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Vendor_All_Fields",
                table: "Invoices",
                columns: new[] { "VendorId", "InvoiceDate", "TotalAmount", "PaidAmount" },
                filter: "\"InvoiceType\" = 'Purchase' AND \"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoices_Aging_DueDate_Status",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_Customer_All_Fields",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_Trend_Date_Type_Amount",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_Vendor_All_Fields",
                table: "Invoices");
        }
    }
}
