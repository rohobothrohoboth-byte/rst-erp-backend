using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Svc.HRM.Attendance.Migrations
{
    /// <inheritdoc />
    public partial class AddContractEntitijhj : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "LocalEmployees");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "LocalEmployees",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
