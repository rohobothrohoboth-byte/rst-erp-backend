using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Leave.Utility.Migrations
{
    /// <inheritdoc />
    public partial class MigHrmLeave06 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LeaveTypeId",
                table: "EmpLeavePolicy",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_EmpLeavePolicy_LeaveTypeId",
                table: "EmpLeavePolicy",
                column: "LeaveTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmpLeavePolicy_LeaveType_LeaveTypeId",
                table: "EmpLeavePolicy",
                column: "LeaveTypeId",
                principalTable: "LeaveType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmpLeavePolicy_LeaveType_LeaveTypeId",
                table: "EmpLeavePolicy");

            migrationBuilder.DropIndex(
                name: "IX_EmpLeavePolicy_LeaveTypeId",
                table: "EmpLeavePolicy");

            migrationBuilder.DropColumn(
                name: "LeaveTypeId",
                table: "EmpLeavePolicy");
        }
    }
}
