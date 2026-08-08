using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Finance.Migrations
{
    /// <inheritdoc />
    public partial class AddVoucherAndConsolidationghjghjkjjkjk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "LocalDepartments");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "LocalCompanies");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "LocalBranches");

            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                table: "LocalEmployees",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "LocalEmployees",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateMod",
                table: "FinancialPeriods",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<string>(
                name: "RowVersion",
                table: "FinancialPeriods",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateMod",
                table: "AuditLogs",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<string>(
                name: "RowVersion",
                table: "AuditLogs",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocalEmployees_BranchId",
                table: "LocalEmployees",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_LocalEmployees_LocalBranches_BranchId",
                table: "LocalEmployees",
                column: "BranchId",
                principalTable: "LocalBranches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocalEmployees_LocalBranches_BranchId",
                table: "LocalEmployees");

            migrationBuilder.DropIndex(
                name: "IX_LocalEmployees_BranchId",
                table: "LocalEmployees");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "LocalEmployees");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "LocalEmployees");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "FinancialPeriods");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "AuditLogs");

            migrationBuilder.AddColumn<string>(
                name: "RowVersion",
                table: "LocalDepartments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RowVersion",
                table: "LocalCompanies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RowVersion",
                table: "LocalBranches",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateMod",
                table: "FinancialPeriods",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateMod",
                table: "AuditLogs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
