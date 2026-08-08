using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Finance.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexesneddssdw : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "OpeningBalance",
                table: "BankAccounts",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentBalance",
                table: "BankAccounts",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "BankAccounts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AvailableBalance",
                table: "BankAccounts",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "BankAddress",
                table: "BankAccounts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "BankAccounts",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "BankAccounts",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IBAN",
                table: "BankAccounts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "BankAccounts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReconciled",
                table: "BankAccounts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastReconciledDate",
                table: "BankAccounts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OverdraftLimit",
                table: "BankAccounts",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwiftCode",
                table: "BankAccounts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncedAt",
                table: "BankAccounts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_AccountId",
                table: "BankAccounts",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_BranchId",
                table: "BankAccounts",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_ChartOfAccounts_AccountId",
                table: "BankAccounts",
                column: "AccountId",
                principalTable: "ChartOfAccounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_LocalBranches_BranchId",
                table: "BankAccounts",
                column: "BranchId",
                principalTable: "LocalBranches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_ChartOfAccounts_AccountId",
                table: "BankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_LocalBranches_BranchId",
                table: "BankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_AccountId",
                table: "BankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_BranchId",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "AvailableBalance",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "BankAddress",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "IBAN",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "IsReconciled",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "LastReconciledDate",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "OverdraftLimit",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "SwiftCode",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "SyncedAt",
                table: "BankAccounts");

            migrationBuilder.AlterColumn<decimal>(
                name: "OpeningBalance",
                table: "BankAccounts",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentBalance",
                table: "BankAccounts",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");
        }
    }
}
