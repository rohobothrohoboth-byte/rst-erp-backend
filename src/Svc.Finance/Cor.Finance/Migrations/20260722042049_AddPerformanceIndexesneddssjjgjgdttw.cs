using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Finance.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexesneddssjjgjgdttw : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "VendorId",
                table: "Expenses",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_VendorId",
                table: "Expenses",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Vendors_VendorId",
                table: "Expenses",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Vendors_VendorId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_VendorId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "Expenses");
        }
    }
}
