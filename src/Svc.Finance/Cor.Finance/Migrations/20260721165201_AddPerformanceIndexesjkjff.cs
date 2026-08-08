using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Finance.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexesjkjff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoices_InvoiceDate_IsDeleted",
                table: "Invoices");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceDate_IsDeleted_Include",
                table: "Invoices",
                columns: new[] { "InvoiceDate", "IsDeleted" })
                .Annotation("Npgsql:IndexInclude", new[] { "InvoiceNumber", "TotalAmount", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoices_InvoiceDate_IsDeleted_Include",
                table: "Invoices");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceDate_IsDeleted",
                table: "Invoices",
                columns: new[] { "InvoiceDate", "IsDeleted" });
        }
    }
}
