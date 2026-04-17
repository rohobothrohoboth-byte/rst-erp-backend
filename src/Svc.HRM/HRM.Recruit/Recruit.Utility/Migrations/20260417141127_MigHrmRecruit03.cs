using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recruit.Utility.Migrations
{
    /// <inheritdoc />
    public partial class MigHrmRecruit03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxScore",
                table: "EvaluationType");

            migrationBuilder.AddColumn<double>(
                name: "MaxScore",
                table: "EvaluationStep",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MinScore",
                table: "EvaluationStep",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxScore",
                table: "EvaluationStep");

            migrationBuilder.DropColumn(
                name: "MinScore",
                table: "EvaluationStep");

            migrationBuilder.AddColumn<double>(
                name: "MaxScore",
                table: "EvaluationType",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
