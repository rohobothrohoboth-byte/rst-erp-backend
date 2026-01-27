using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Leave.Utility.Migrations
{
    /// <inheritdoc />
    public partial class MigHrmLeave05 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmpLeavePolicy_LeavePolicyConfig_LeavePolicyConfigId",
                table: "EmpLeavePolicy");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpLeavePolicy_LeaveType_LeaveTypeId",
                table: "EmpLeavePolicy");

            migrationBuilder.DropIndex(
                name: "IX_EmpLeavePolicy_EmployeeId_LeaveTypeId_LeavePolicyConfigId",
                table: "EmpLeavePolicy");

            migrationBuilder.DropIndex(
                name: "IX_EmpLeavePolicy_LeavePolicyConfigId",
                table: "EmpLeavePolicy");

            migrationBuilder.DropColumn(
                name: "LeavePolicyConfigId",
                table: "EmpLeavePolicy");

            migrationBuilder.RenameColumn(
                name: "LeaveTypeId",
                table: "EmpLeavePolicy",
                newName: "LeavePolicyId");

            migrationBuilder.RenameIndex(
                name: "IX_EmpLeavePolicy_LeaveTypeId",
                table: "EmpLeavePolicy",
                newName: "IX_EmpLeavePolicy_LeavePolicyId");

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveTo",
                table: "EmpLeavePolicy",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpLeavePolicy_EmployeeId_LeavePolicyId",
                table: "EmpLeavePolicy",
                columns: new[] { "EmployeeId", "LeavePolicyId" });

            migrationBuilder.AddForeignKey(
                name: "FK_EmpLeavePolicy_LeavePolicy_LeavePolicyId",
                table: "EmpLeavePolicy",
                column: "LeavePolicyId",
                principalTable: "LeavePolicy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmpLeavePolicy_LeavePolicy_LeavePolicyId",
                table: "EmpLeavePolicy");

            migrationBuilder.DropIndex(
                name: "IX_EmpLeavePolicy_EmployeeId_LeavePolicyId",
                table: "EmpLeavePolicy");

            migrationBuilder.DropColumn(
                name: "EffectiveTo",
                table: "EmpLeavePolicy");

            migrationBuilder.RenameColumn(
                name: "LeavePolicyId",
                table: "EmpLeavePolicy",
                newName: "LeaveTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_EmpLeavePolicy_LeavePolicyId",
                table: "EmpLeavePolicy",
                newName: "IX_EmpLeavePolicy_LeaveTypeId");

            migrationBuilder.AddColumn<Guid>(
                name: "LeavePolicyConfigId",
                table: "EmpLeavePolicy",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_EmpLeavePolicy_EmployeeId_LeaveTypeId_LeavePolicyConfigId",
                table: "EmpLeavePolicy",
                columns: new[] { "EmployeeId", "LeaveTypeId", "LeavePolicyConfigId" });

            migrationBuilder.CreateIndex(
                name: "IX_EmpLeavePolicy_LeavePolicyConfigId",
                table: "EmpLeavePolicy",
                column: "LeavePolicyConfigId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmpLeavePolicy_LeavePolicyConfig_LeavePolicyConfigId",
                table: "EmpLeavePolicy",
                column: "LeavePolicyConfigId",
                principalTable: "LeavePolicyConfig",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpLeavePolicy_LeaveType_LeaveTypeId",
                table: "EmpLeavePolicy",
                column: "LeaveTypeId",
                principalTable: "LeaveType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
