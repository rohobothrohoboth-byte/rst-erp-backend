using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Svc.Task.Migrations
{
    /// <inheritdoc />
    public partial class AddLastOverdueNotifiedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastOverdueNotified",
                table: "Tasks",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastOverdueNotified",
                table: "Tasks");
        }
    }
}
