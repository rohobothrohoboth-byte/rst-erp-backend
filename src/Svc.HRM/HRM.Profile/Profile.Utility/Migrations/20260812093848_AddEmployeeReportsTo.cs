using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Profile.Utility.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeReportsTo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReportsToId",
                table: "Employee",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportsToId",
                table: "Employee");
        }
    }
}
