using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Module.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyBranchDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Mission",
                table: "Company",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Structure",
                table: "Company",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Values",
                table: "Company",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Vision",
                table: "Company",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Company",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Branch",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Branch",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Branch",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerName",
                table: "Branch",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Branch",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mission",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "Structure",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "Values",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "Vision",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "ManagerName",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Branch");
        }
    }
}
