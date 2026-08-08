using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Finance.Migrations
{
    /// <inheritdoc />
    public partial class AddLastOverdueNotifidddffdddrdttyty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "JournalEntries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "JournalEntries",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "JournalEntries",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReversed",
                table: "JournalEntries",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "JournalEntries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReversedBy",
                table: "JournalEntries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReversedDate",
                table: "JournalEntries",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "IsReversed",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "ReversedBy",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "ReversedDate",
                table: "JournalEntries");
        }
    }
}
