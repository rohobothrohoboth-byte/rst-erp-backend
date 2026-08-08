using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.FileManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIncdddfddddsdsdsd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "FileShareTokens",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "FileShareTokens");
        }
    }
}
