using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.CRM.Migrations
{
    /// <inheritdoc />
    public partial class AddContractEntitiesjhhjhddsdsdsddsddsdssddddffd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_SalesOrders_OrderId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_SalesOrders_InvoiceId",
                table: "SalesOrders");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_OrderId",
                table: "Invoices");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_InvoiceId",
                table: "SalesOrders",
                column: "InvoiceId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SalesOrders_InvoiceId",
                table: "SalesOrders");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_InvoiceId",
                table: "SalesOrders",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_OrderId",
                table: "Invoices",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_SalesOrders_OrderId",
                table: "Invoices",
                column: "OrderId",
                principalTable: "SalesOrders",
                principalColumn: "Id");
        }
    }
}
