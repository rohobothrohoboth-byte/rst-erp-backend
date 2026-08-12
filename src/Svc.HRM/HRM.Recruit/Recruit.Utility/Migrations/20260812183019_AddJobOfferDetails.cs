using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recruit.Utility.Migrations
{
    /// <inheritdoc />
    public partial class AddJobOfferDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApplicantId",
                table: "JobOffer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Benefits",
                table: "JobOffer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "JobOffer",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "JobOffer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Salary",
                table: "JobOffer",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "JobOffer",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicantId",
                table: "JobOffer");

            migrationBuilder.DropColumn(
                name: "Benefits",
                table: "JobOffer");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "JobOffer");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "JobOffer");

            migrationBuilder.DropColumn(
                name: "Salary",
                table: "JobOffer");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "JobOffer");
        }
    }
}
