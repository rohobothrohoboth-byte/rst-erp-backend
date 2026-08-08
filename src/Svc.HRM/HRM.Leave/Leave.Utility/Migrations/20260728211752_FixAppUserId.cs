using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Leave.Utility.Migrations
{
    /// <inheritdoc />
    public partial class FixAppUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                table: "LeaveRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateApp",
                table: "LeaveRequest",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeptId",
                table: "LeaveRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<double>(
                name: "PerApp",
                table: "LeaveRequest",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<Guid>(
                name: "LeaveAppStepId",
                table: "LeaveAppStep",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LeavePolicyId",
                table: "LeaveAppStep",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateApp",
                table: "LeaveAppAction",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LeaveAppStepId",
                table: "LeaveAppAction",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "LeaveRequest");

            migrationBuilder.DropColumn(
                name: "DateApp",
                table: "LeaveRequest");

            migrationBuilder.DropColumn(
                name: "DeptId",
                table: "LeaveRequest");

            migrationBuilder.DropColumn(
                name: "PerApp",
                table: "LeaveRequest");

            migrationBuilder.DropColumn(
                name: "LeaveAppStepId",
                table: "LeaveAppStep");

            migrationBuilder.DropColumn(
                name: "LeavePolicyId",
                table: "LeaveAppStep");

            migrationBuilder.DropColumn(
                name: "DateApp",
                table: "LeaveAppAction");

            migrationBuilder.DropColumn(
                name: "LeaveAppStepId",
                table: "LeaveAppAction");
        }
    }
}
