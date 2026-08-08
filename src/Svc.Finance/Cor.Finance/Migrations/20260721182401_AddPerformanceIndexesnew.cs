using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Finance.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexesnew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvoiceAggregates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AggregateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Period = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AggregateType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TotalInvoices = table.Column<int>(type: "integer", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalPaid = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalBalance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PaidCount = table.Column<int>(type: "integer", nullable: false),
                    OverdueCount = table.Column<int>(type: "integer", nullable: false),
                    DraftCount = table.Column<int>(type: "integer", nullable: false),
                    AverageInvoiceAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AveragePaymentDays = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SalesCount = table.Column<int>(type: "integer", nullable: false),
                    SalesAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PurchaseCount = table.Column<int>(type: "integer", nullable: false),
                    PurchaseAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceAggregates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentAggregates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AggregateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Period = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AggregateType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TotalPayments = table.Column<int>(type: "integer", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    VendorPayments = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CustomerPayments = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ProcessedCount = table.Column<int>(type: "integer", nullable: false),
                    PendingCount = table.Column<int>(type: "integer", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentAggregates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceAggregates_AggregateDate",
                table: "InvoiceAggregates",
                column: "AggregateDate");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceAggregates_AggregateType",
                table: "InvoiceAggregates",
                column: "AggregateType");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceAggregates_Period",
                table: "InvoiceAggregates",
                column: "Period");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAggregates_AggregateDate",
                table: "PaymentAggregates",
                column: "AggregateDate");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAggregates_AggregateType",
                table: "PaymentAggregates",
                column: "AggregateType");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAggregates_Period",
                table: "PaymentAggregates",
                column: "Period");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoiceAggregates");

            migrationBuilder.DropTable(
                name: "PaymentAggregates");
        }
    }
}
