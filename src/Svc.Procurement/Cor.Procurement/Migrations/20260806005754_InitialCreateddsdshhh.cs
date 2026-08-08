using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Procurement.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateddsdshhh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RowVersion",
                table: "PurchaseOrders",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledDate",
                table: "PurchaseOrders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveredDate",
                table: "PurchaseOrders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ShippedDate",
                table: "PurchaseOrders",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelledDate",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "DeliveredDate",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "ShippedDate",
                table: "PurchaseOrders");

            migrationBuilder.AlterColumn<string>(
                name: "RowVersion",
                table: "PurchaseOrders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
