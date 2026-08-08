using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Svc.HRM.Payroll.Migrations
{
    /// <inheritdoc />
    public partial class AddFinancePostingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FinanceJournalEntryId",
                table: "PayrollRuns",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FinancePostedAt",
                table: "PayrollRuns",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinancePostingError",
                table: "PayrollRuns",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinancePostingStatus",
                table: "PayrollRuns",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinanceJournalEntryId",
                table: "PayrollRuns");

            migrationBuilder.DropColumn(
                name: "FinancePostedAt",
                table: "PayrollRuns");

            migrationBuilder.DropColumn(
                name: "FinancePostingError",
                table: "PayrollRuns");

            migrationBuilder.DropColumn(
                name: "FinancePostingStatus",
                table: "PayrollRuns");
        }
    }
}
