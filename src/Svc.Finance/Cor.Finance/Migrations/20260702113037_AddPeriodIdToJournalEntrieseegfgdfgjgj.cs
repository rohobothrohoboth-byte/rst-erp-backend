using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Finance.Migrations
{
    /// <inheritdoc />
    public partial class AddPeriodIdToJournalEntrieseegfgdfgjgj : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PeriodName",
                table: "JournalEntries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PeriodName",
                table: "JournalEntries",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
