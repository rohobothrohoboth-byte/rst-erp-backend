using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Profile.Utility.Migrations
{
    /// <inheritdoc />
    public partial class FixAppUserIdOddddiiiddddddnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                table: "EmpExperience");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "EmpEducation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "EmpExperience",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "EmpEducation",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
