using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.CRM.Migrations
{
    /// <inheritdoc />
    public partial class ChangeLocalEmployeeAppUserIdToGuidjhhjkdfdf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_SalesOrder_OrderId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderLine_Products_ProductId",
                table: "OrderLine");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderLine_QuoteLines_QuoteLineId",
                table: "OrderLine");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderLine_SalesOrder_OrderId",
                table: "OrderLine");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrder_Customers_CustomerId",
                table: "SalesOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrder_Invoices_InvoiceId",
                table: "SalesOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrder_Opportunities_OpportunityId",
                table: "SalesOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrder_Quotes_QuoteId",
                table: "SalesOrder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SalesOrder",
                table: "SalesOrder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderLine",
                table: "OrderLine");

            migrationBuilder.RenameTable(
                name: "SalesOrder",
                newName: "SalesOrders");

            migrationBuilder.RenameTable(
                name: "OrderLine",
                newName: "OrderLines");

            migrationBuilder.RenameIndex(
                name: "IX_SalesOrder_QuoteId",
                table: "SalesOrders",
                newName: "IX_SalesOrders_QuoteId");

            migrationBuilder.RenameIndex(
                name: "IX_SalesOrder_OpportunityId",
                table: "SalesOrders",
                newName: "IX_SalesOrders_OpportunityId");

            migrationBuilder.RenameIndex(
                name: "IX_SalesOrder_InvoiceId",
                table: "SalesOrders",
                newName: "IX_SalesOrders_InvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_SalesOrder_CustomerId",
                table: "SalesOrders",
                newName: "IX_SalesOrders_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderLine_QuoteLineId",
                table: "OrderLines",
                newName: "IX_OrderLines_QuoteLineId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderLine_ProductId",
                table: "OrderLines",
                newName: "IX_OrderLines_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderLine_OrderId",
                table: "OrderLines",
                newName: "IX_OrderLines_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SalesOrders",
                table: "SalesOrders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderLines",
                table: "OrderLines",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_SalesOrders_OrderId",
                table: "Invoices",
                column: "OrderId",
                principalTable: "SalesOrders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLines_Products_ProductId",
                table: "OrderLines",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLines_QuoteLines_QuoteLineId",
                table: "OrderLines",
                column: "QuoteLineId",
                principalTable: "QuoteLines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLines_SalesOrders_OrderId",
                table: "OrderLines",
                column: "OrderId",
                principalTable: "SalesOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrders_Customers_CustomerId",
                table: "SalesOrders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrders_Invoices_InvoiceId",
                table: "SalesOrders",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrders_Opportunities_OpportunityId",
                table: "SalesOrders",
                column: "OpportunityId",
                principalTable: "Opportunities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrders_Quotes_QuoteId",
                table: "SalesOrders",
                column: "QuoteId",
                principalTable: "Quotes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_SalesOrders_OrderId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderLines_Products_ProductId",
                table: "OrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderLines_QuoteLines_QuoteLineId",
                table: "OrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderLines_SalesOrders_OrderId",
                table: "OrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrders_Customers_CustomerId",
                table: "SalesOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrders_Invoices_InvoiceId",
                table: "SalesOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrders_Opportunities_OpportunityId",
                table: "SalesOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrders_Quotes_QuoteId",
                table: "SalesOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SalesOrders",
                table: "SalesOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderLines",
                table: "OrderLines");

            migrationBuilder.RenameTable(
                name: "SalesOrders",
                newName: "SalesOrder");

            migrationBuilder.RenameTable(
                name: "OrderLines",
                newName: "OrderLine");

            migrationBuilder.RenameIndex(
                name: "IX_SalesOrders_QuoteId",
                table: "SalesOrder",
                newName: "IX_SalesOrder_QuoteId");

            migrationBuilder.RenameIndex(
                name: "IX_SalesOrders_OpportunityId",
                table: "SalesOrder",
                newName: "IX_SalesOrder_OpportunityId");

            migrationBuilder.RenameIndex(
                name: "IX_SalesOrders_InvoiceId",
                table: "SalesOrder",
                newName: "IX_SalesOrder_InvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_SalesOrders_CustomerId",
                table: "SalesOrder",
                newName: "IX_SalesOrder_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderLines_QuoteLineId",
                table: "OrderLine",
                newName: "IX_OrderLine_QuoteLineId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderLines_ProductId",
                table: "OrderLine",
                newName: "IX_OrderLine_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderLines_OrderId",
                table: "OrderLine",
                newName: "IX_OrderLine_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SalesOrder",
                table: "SalesOrder",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderLine",
                table: "OrderLine",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_SalesOrder_OrderId",
                table: "Invoices",
                column: "OrderId",
                principalTable: "SalesOrder",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLine_Products_ProductId",
                table: "OrderLine",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLine_QuoteLines_QuoteLineId",
                table: "OrderLine",
                column: "QuoteLineId",
                principalTable: "QuoteLines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLine_SalesOrder_OrderId",
                table: "OrderLine",
                column: "OrderId",
                principalTable: "SalesOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrder_Customers_CustomerId",
                table: "SalesOrder",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrder_Invoices_InvoiceId",
                table: "SalesOrder",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrder_Opportunities_OpportunityId",
                table: "SalesOrder",
                column: "OpportunityId",
                principalTable: "Opportunities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrder_Quotes_QuoteId",
                table: "SalesOrder",
                column: "QuoteId",
                principalTable: "Quotes",
                principalColumn: "Id");
        }
    }
}
