using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Svc.Auth.Migrations
{
    /// <inheritdoc />
    public partial class MigAuth02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PerModuleId",
                table: "PerMenu",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PerMenuId",
                table: "PerApi",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PerMenu_Id",
                table: "PerMenu",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerMenu_PerModuleId",
                table: "PerMenu",
                column: "PerModuleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerApi_Id",
                table: "PerApi",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerApi_PerMenuId",
                table: "PerApi",
                column: "PerMenuId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PerApi_PerMenu_PerMenuId",
                table: "PerApi",
                column: "PerMenuId",
                principalTable: "PerMenu",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PerMenu_PerModule_PerModuleId",
                table: "PerMenu",
                column: "PerModuleId",
                principalTable: "PerModule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PerApi_PerMenu_PerMenuId",
                table: "PerApi");

            migrationBuilder.DropForeignKey(
                name: "FK_PerMenu_PerModule_PerModuleId",
                table: "PerMenu");

            migrationBuilder.DropIndex(
                name: "IX_PerMenu_Id",
                table: "PerMenu");

            migrationBuilder.DropIndex(
                name: "IX_PerMenu_PerModuleId",
                table: "PerMenu");

            migrationBuilder.DropIndex(
                name: "IX_PerApi_Id",
                table: "PerApi");

            migrationBuilder.DropIndex(
                name: "IX_PerApi_PerMenuId",
                table: "PerApi");

            migrationBuilder.DropColumn(
                name: "PerModuleId",
                table: "PerMenu");

            migrationBuilder.DropColumn(
                name: "PerMenuId",
                table: "PerApi");
        }
    }
}
