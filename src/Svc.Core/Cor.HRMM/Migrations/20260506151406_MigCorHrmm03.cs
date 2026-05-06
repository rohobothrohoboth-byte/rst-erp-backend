using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.HRMM.Migrations
{
    /// <inheritdoc />
    public partial class MigCorHrmm03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "JgStep",
                type: "character varying(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SalaryPayFreq",
                table: "JgStep",
                type: "character varying(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "JgStep");

            migrationBuilder.DropColumn(
                name: "SalaryPayFreq",
                table: "JgStep");
        }
    }
}
