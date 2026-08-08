using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.FileManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryToFileFolders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "FileFolders",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "FileFolders");
        }
    }
}
