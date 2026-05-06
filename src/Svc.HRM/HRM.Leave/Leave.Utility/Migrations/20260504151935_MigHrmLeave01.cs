using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Leave.Utility.Migrations
{
    /// <inheritdoc />
    public partial class MigHrmLeave01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveBalance_LeaveLedger_LeaveLedgerId",
                table: "LeaveBalance");

            migrationBuilder.DropIndex(
                name: "IX_LeaveBalance_EmployeeId_LeaveTypeId",
                table: "LeaveBalance");

            migrationBuilder.DropIndex(
                name: "IX_LeaveBalance_LeaveLedgerId",
                table: "LeaveBalance");

            migrationBuilder.DropColumn(
                name: "LeaveLedgerId",
                table: "LeaveBalance");

            migrationBuilder.AddColumn<Guid>(
                name: "LeavePolicyId",
                table: "LeaveBalance",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalance_LeavePolicyId_LeaveTypeId_EmployeeId",
                table: "LeaveBalance",
                columns: new[] { "LeavePolicyId", "LeaveTypeId", "EmployeeId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveBalance_LeavePolicy_LeavePolicyId",
                table: "LeaveBalance",
                column: "LeavePolicyId",
                principalTable: "LeavePolicy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveBalance_LeavePolicy_LeavePolicyId",
                table: "LeaveBalance");

            migrationBuilder.DropIndex(
                name: "IX_LeaveBalance_LeavePolicyId_LeaveTypeId_EmployeeId",
                table: "LeaveBalance");

            migrationBuilder.DropColumn(
                name: "LeavePolicyId",
                table: "LeaveBalance");

            migrationBuilder.AddColumn<Guid>(
                name: "LeaveLedgerId",
                table: "LeaveBalance",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalance_EmployeeId_LeaveTypeId",
                table: "LeaveBalance",
                columns: new[] { "EmployeeId", "LeaveTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalance_LeaveLedgerId",
                table: "LeaveBalance",
                column: "LeaveLedgerId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveBalance_LeaveLedger_LeaveLedgerId",
                table: "LeaveBalance",
                column: "LeaveLedgerId",
                principalTable: "LeaveLedger",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
