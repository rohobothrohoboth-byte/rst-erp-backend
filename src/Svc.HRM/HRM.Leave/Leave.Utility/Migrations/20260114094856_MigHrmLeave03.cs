using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Leave.Utility.Migrations
{
    /// <inheritdoc />
    public partial class MigHrmLeave03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EffectiveFrom",
                table: "LeaveAppStep");

            migrationBuilder.DropColumn(
                name: "EffectiveTo",
                table: "LeaveAppStep");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAppStep_StepName",
                table: "LeaveAppStep",
                column: "StepName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LeaveAppStep_StepName",
                table: "LeaveAppStep");

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveFrom",
                table: "LeaveAppStep",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveTo",
                table: "LeaveAppStep",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
