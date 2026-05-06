using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Profile.Utility.Migrations
{
    /// <inheritdoc />
    public partial class MigHrmPro04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "EmpSalary",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SalaryPayFreq",
                table: "EmpSalary",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "EmpSalary");

            migrationBuilder.DropColumn(
                name: "SalaryPayFreq",
                table: "EmpSalary");
        }
    }
}
