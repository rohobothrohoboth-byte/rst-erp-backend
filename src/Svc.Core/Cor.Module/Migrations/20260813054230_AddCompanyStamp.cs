using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Module.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyStamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StampUrl",
                table: "Company",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StampUrl",
                table: "Company");
        }
    }
}
