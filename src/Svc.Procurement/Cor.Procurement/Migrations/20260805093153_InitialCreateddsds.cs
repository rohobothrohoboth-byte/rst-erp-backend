using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Procurement.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateddsds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LocalEmployees_Code",
                table: "LocalEmployees",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_LocalEmployees_Code_IsDeleted",
                table: "LocalEmployees",
                columns: new[] { "Code", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_LocalEmployees_FirstName",
                table: "LocalEmployees",
                column: "FirstName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LocalEmployees_Code",
                table: "LocalEmployees");

            migrationBuilder.DropIndex(
                name: "IX_LocalEmployees_Code_IsDeleted",
                table: "LocalEmployees");

            migrationBuilder.DropIndex(
                name: "IX_LocalEmployees_FirstName",
                table: "LocalEmployees");
        }
    }
}
