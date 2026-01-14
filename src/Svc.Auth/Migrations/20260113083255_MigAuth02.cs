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
            migrationBuilder.DropColumn(
                name: "Parent",
                table: "PerMenu");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "PerMenu",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerMenu_ParentId",
                table: "PerMenu",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_PerMenu_PerMenu_ParentId",
                table: "PerMenu",
                column: "ParentId",
                principalTable: "PerMenu",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PerMenu_PerMenu_ParentId",
                table: "PerMenu");

            migrationBuilder.DropIndex(
                name: "IX_PerMenu_ParentId",
                table: "PerMenu");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "PerMenu");

            migrationBuilder.AddColumn<string>(
                name: "Parent",
                table: "PerMenu",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
