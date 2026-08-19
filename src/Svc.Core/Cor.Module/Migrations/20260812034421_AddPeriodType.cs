using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Module.Migrations
{
    /// <inheritdoc />
    public partial class AddPeriodType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PeriodType",
                table: "Period",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PeriodType",
                table: "Period");
        }
    }
}
