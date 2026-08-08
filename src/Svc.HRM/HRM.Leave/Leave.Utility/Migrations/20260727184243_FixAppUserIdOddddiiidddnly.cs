using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Leave.Utility.Migrations
{
    /// <inheritdoc />
    public partial class FixAppUserIdOddddiiidddnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "LocalPositions");

            migrationBuilder.DropColumn(
                name: "ProfessionType",
                table: "LocalPositions");

            migrationBuilder.DropColumn(
                name: "SaturdayWorkOption",
                table: "LocalPositions");

            migrationBuilder.DropColumn(
                name: "SundayWorkOption",
                table: "LocalPositions");

            migrationBuilder.DropColumn(
                name: "WorkingHours",
                table: "LocalPositions");

            migrationBuilder.CreateTable(
                name: "LocalPositionReq",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    ProfessionType = table.Column<string>(type: "text", nullable: false),
                    SaturdayWorkOption = table.Column<string>(type: "text", nullable: false),
                    SundayWorkOption = table.Column<string>(type: "text", nullable: false),
                    WorkingHours = table.Column<double>(type: "double precision", nullable: false),
                    PositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalPositionReq", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocalPositionReq_LocalPositions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "LocalPositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocalPositionReq_PositionId",
                table: "LocalPositionReq",
                column: "PositionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocalPositionReq");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "LocalPositions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProfessionType",
                table: "LocalPositions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SaturdayWorkOption",
                table: "LocalPositions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SundayWorkOption",
                table: "LocalPositions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WorkingHours",
                table: "LocalPositions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
