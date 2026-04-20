using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recruit.Utility.Migrations
{
    /// <inheritdoc />
    public partial class MigHrmRecruit05 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveFrom",
                table: "JobPostEvalFlow",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveTo",
                table: "JobPostEvalFlow",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EffectiveFrom",
                table: "JobPostEvalFlow");

            migrationBuilder.DropColumn(
                name: "EffectiveTo",
                table: "JobPostEvalFlow");
        }
    }
}
