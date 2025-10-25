using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Module.Migrations
{
    /// <inheritdoc />
    public partial class MigCorMod00 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuarterId",
                table: "Period");

            migrationBuilder.AddColumn<string>(
                name: "Quarter",
                table: "Period",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quarter",
                table: "Period");

            migrationBuilder.AddColumn<Guid>(
                name: "QuarterId",
                table: "Period",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
