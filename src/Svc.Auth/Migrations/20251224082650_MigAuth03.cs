using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Svc.Auth.Migrations
{
    /// <inheritdoc />
    public partial class MigAuth03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PerMenu_PerModuleId",
                table: "PerMenu");

            migrationBuilder.DropIndex(
                name: "IX_PerApi_PerMenuId",
                table: "PerApi");

            migrationBuilder.CreateIndex(
                name: "IX_PerMenu_PerModuleId",
                table: "PerMenu",
                column: "PerModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_PerApi_PerMenuId",
                table: "PerApi",
                column: "PerMenuId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PerMenu_PerModuleId",
                table: "PerMenu");

            migrationBuilder.DropIndex(
                name: "IX_PerApi_PerMenuId",
                table: "PerApi");

            migrationBuilder.CreateIndex(
                name: "IX_PerMenu_PerModuleId",
                table: "PerMenu",
                column: "PerModuleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerApi_PerMenuId",
                table: "PerApi",
                column: "PerMenuId",
                unique: true);
        }
    }
}
