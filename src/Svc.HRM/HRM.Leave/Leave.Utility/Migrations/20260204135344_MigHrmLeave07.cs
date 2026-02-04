using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Leave.Utility.Migrations
{
    /// <inheritdoc />
    public partial class MigHrmLeave07 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EncashmentAppAction_LeaveEncashment_LeaveEncashmentId",
                table: "EncashmentAppAction");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveAppAction_LeaveRequest_LeaveRequestId",
                table: "LeaveAppAction");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveAppStep_LeaveAppChain_LeaveAppChainId",
                table: "LeaveAppStep");

            migrationBuilder.DropForeignKey(
                name: "FK_LeavePolicyConfig_LeavePolicy_LeavePolicyId",
                table: "LeavePolicyConfig");

            migrationBuilder.DropIndex(
                name: "IX_LeaveRequest_EmployeeId_LeaveTypeId_ApprovedById_Status",
                table: "LeaveRequest");

            migrationBuilder.DropIndex(
                name: "IX_LeaveRequest_Id",
                table: "LeaveRequest");

            migrationBuilder.DropIndex(
                name: "IX_LeavePolicyConfig_FiscalYearId_LeavePolicyId",
                table: "LeavePolicyConfig");

            migrationBuilder.DropIndex(
                name: "IX_LeavePolicyConfig_Id",
                table: "LeavePolicyConfig");

            migrationBuilder.DropIndex(
                name: "IX_LeavePolicyConfig_LeavePolicyId",
                table: "LeavePolicyConfig");

            migrationBuilder.DropIndex(
                name: "IX_LeavePolicy_Id",
                table: "LeavePolicy");

            migrationBuilder.DropIndex(
                name: "IX_LeaveLedger_EmployeeId_LeaveTypeId_LeavePolicyId_ReferenceId",
                table: "LeaveLedger");

            migrationBuilder.DropIndex(
                name: "IX_LeaveLedger_Id",
                table: "LeaveLedger");

            migrationBuilder.DropIndex(
                name: "IX_LeaveEncashment_EmployeeId_LeaveTypeId_LeavePolicyId",
                table: "LeaveEncashment");

            migrationBuilder.DropIndex(
                name: "IX_LeaveEncashment_Id",
                table: "LeaveEncashment");

            migrationBuilder.DropIndex(
                name: "IX_LeaveBalance_EmployeeId_LeaveTypeId_LeaveLedgerId",
                table: "LeaveBalance");

            migrationBuilder.DropIndex(
                name: "IX_LeaveBalance_Id",
                table: "LeaveBalance");

            migrationBuilder.DropIndex(
                name: "IX_LeaveAppStep_Id",
                table: "LeaveAppStep");

            migrationBuilder.DropIndex(
                name: "IX_LeaveAppStep_LeaveAppChainId",
                table: "LeaveAppStep");

            migrationBuilder.DropIndex(
                name: "IX_LeaveAppStep_StepName",
                table: "LeaveAppStep");

            migrationBuilder.DropIndex(
                name: "IX_LeaveAppChain_Id",
                table: "LeaveAppChain");

            migrationBuilder.DropIndex(
                name: "IX_LeaveAppAction_Id",
                table: "LeaveAppAction");

            migrationBuilder.DropIndex(
                name: "IX_LeaveAppAction_LeaveRequestId_ApprovedById",
                table: "LeaveAppAction");

            migrationBuilder.DropIndex(
                name: "IX_EncashmentAppAction_Id",
                table: "EncashmentAppAction");

            migrationBuilder.DropIndex(
                name: "IX_EncashmentAppAction_LeaveEncashmentId_ApprovedById",
                table: "EncashmentAppAction");

            migrationBuilder.DropIndex(
                name: "IX_EmpLeavePolicy_EmployeeId_LeavePolicyId",
                table: "EmpLeavePolicy");

            migrationBuilder.DropIndex(
                name: "IX_EmpLeavePolicy_Id",
                table: "EmpLeavePolicy");

            migrationBuilder.DropIndex(
                name: "IX_AccrualHistory_EmployeeId_LeaveTypeId_LeavePolicyId_LeaveLe~",
                table: "AccrualHistory");

            migrationBuilder.DropIndex(
                name: "IX_AccrualHistory_Id",
                table: "AccrualHistory");

            migrationBuilder.AlterColumn<decimal>(
                name: "AccruedAmount",
                table: "AccrualHistory",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequest_EmployeeId_StartDate_EndDate",
                table: "LeaveRequest",
                columns: new[] { "EmployeeId", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequest_Status",
                table: "LeaveRequest",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicyConfig_LeavePolicyId_FiscalYearId_IsActive",
                table: "LeavePolicyConfig",
                columns: new[] { "LeavePolicyId", "FiscalYearId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicy_Status",
                table: "LeavePolicy",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveLedger_EmployeeId_LeaveTypeId_Date",
                table: "LeaveLedger",
                columns: new[] { "EmployeeId", "LeaveTypeId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveLedger_ReferenceId",
                table: "LeaveLedger",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveLedger_SourceType_Date",
                table: "LeaveLedger",
                columns: new[] { "SourceType", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveEncashment_EmployeeId_LeaveTypeId_Status",
                table: "LeaveEncashment",
                columns: new[] { "EmployeeId", "LeaveTypeId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalance_EmployeeId_LeaveTypeId",
                table: "LeaveBalance",
                columns: new[] { "EmployeeId", "LeaveTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAppStep_LeaveAppChainId_StepOrder",
                table: "LeaveAppStep",
                columns: new[] { "LeaveAppChainId", "StepOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAppAction_LeaveRequestId_StepOrder",
                table: "LeaveAppAction",
                columns: new[] { "LeaveRequestId", "StepOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_EncashmentAppAction_LeaveEncashmentId_StepOrder",
                table: "EncashmentAppAction",
                columns: new[] { "LeaveEncashmentId", "StepOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_EmpLeavePolicy_EmployeeId_LeaveTypeId_EffectiveFrom",
                table: "EmpLeavePolicy",
                columns: new[] { "EmployeeId", "LeaveTypeId", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_AccrualHistory_EmployeeId_LeaveTypeId_PeriodStart_PeriodEnd",
                table: "AccrualHistory",
                columns: new[] { "EmployeeId", "LeaveTypeId", "PeriodStart", "PeriodEnd" });

            migrationBuilder.CreateIndex(
                name: "IX_AccrualHistory_Frequency",
                table: "AccrualHistory",
                column: "Frequency");

            migrationBuilder.AddForeignKey(
                name: "FK_EncashmentAppAction_LeaveEncashment_LeaveEncashmentId",
                table: "EncashmentAppAction",
                column: "LeaveEncashmentId",
                principalTable: "LeaveEncashment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveAppAction_LeaveRequest_LeaveRequestId",
                table: "LeaveAppAction",
                column: "LeaveRequestId",
                principalTable: "LeaveRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveAppStep_LeaveAppChain_LeaveAppChainId",
                table: "LeaveAppStep",
                column: "LeaveAppChainId",
                principalTable: "LeaveAppChain",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeavePolicyConfig_LeavePolicy_LeavePolicyId",
                table: "LeavePolicyConfig",
                column: "LeavePolicyId",
                principalTable: "LeavePolicy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EncashmentAppAction_LeaveEncashment_LeaveEncashmentId",
                table: "EncashmentAppAction");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveAppAction_LeaveRequest_LeaveRequestId",
                table: "LeaveAppAction");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveAppStep_LeaveAppChain_LeaveAppChainId",
                table: "LeaveAppStep");

            migrationBuilder.DropForeignKey(
                name: "FK_LeavePolicyConfig_LeavePolicy_LeavePolicyId",
                table: "LeavePolicyConfig");

            migrationBuilder.DropIndex(
                name: "IX_LeaveRequest_EmployeeId_StartDate_EndDate",
                table: "LeaveRequest");

            migrationBuilder.DropIndex(
                name: "IX_LeaveRequest_Status",
                table: "LeaveRequest");

            migrationBuilder.DropIndex(
                name: "IX_LeavePolicyConfig_LeavePolicyId_FiscalYearId_IsActive",
                table: "LeavePolicyConfig");

            migrationBuilder.DropIndex(
                name: "IX_LeavePolicy_Status",
                table: "LeavePolicy");

            migrationBuilder.DropIndex(
                name: "IX_LeaveLedger_EmployeeId_LeaveTypeId_Date",
                table: "LeaveLedger");

            migrationBuilder.DropIndex(
                name: "IX_LeaveLedger_ReferenceId",
                table: "LeaveLedger");

            migrationBuilder.DropIndex(
                name: "IX_LeaveLedger_SourceType_Date",
                table: "LeaveLedger");

            migrationBuilder.DropIndex(
                name: "IX_LeaveEncashment_EmployeeId_LeaveTypeId_Status",
                table: "LeaveEncashment");

            migrationBuilder.DropIndex(
                name: "IX_LeaveBalance_EmployeeId_LeaveTypeId",
                table: "LeaveBalance");

            migrationBuilder.DropIndex(
                name: "IX_LeaveAppStep_LeaveAppChainId_StepOrder",
                table: "LeaveAppStep");

            migrationBuilder.DropIndex(
                name: "IX_LeaveAppAction_LeaveRequestId_StepOrder",
                table: "LeaveAppAction");

            migrationBuilder.DropIndex(
                name: "IX_EncashmentAppAction_LeaveEncashmentId_StepOrder",
                table: "EncashmentAppAction");

            migrationBuilder.DropIndex(
                name: "IX_EmpLeavePolicy_EmployeeId_LeaveTypeId_EffectiveFrom",
                table: "EmpLeavePolicy");

            migrationBuilder.DropIndex(
                name: "IX_AccrualHistory_EmployeeId_LeaveTypeId_PeriodStart_PeriodEnd",
                table: "AccrualHistory");

            migrationBuilder.DropIndex(
                name: "IX_AccrualHistory_Frequency",
                table: "AccrualHistory");

            migrationBuilder.AlterColumn<decimal>(
                name: "AccruedAmount",
                table: "AccrualHistory",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequest_EmployeeId_LeaveTypeId_ApprovedById_Status",
                table: "LeaveRequest",
                columns: new[] { "EmployeeId", "LeaveTypeId", "ApprovedById", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequest_Id",
                table: "LeaveRequest",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicyConfig_FiscalYearId_LeavePolicyId",
                table: "LeavePolicyConfig",
                columns: new[] { "FiscalYearId", "LeavePolicyId" });

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicyConfig_Id",
                table: "LeavePolicyConfig",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicyConfig_LeavePolicyId",
                table: "LeavePolicyConfig",
                column: "LeavePolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicy_Id",
                table: "LeavePolicy",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveLedger_EmployeeId_LeaveTypeId_LeavePolicyId_ReferenceId",
                table: "LeaveLedger",
                columns: new[] { "EmployeeId", "LeaveTypeId", "LeavePolicyId", "ReferenceId" });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveLedger_Id",
                table: "LeaveLedger",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveEncashment_EmployeeId_LeaveTypeId_LeavePolicyId",
                table: "LeaveEncashment",
                columns: new[] { "EmployeeId", "LeaveTypeId", "LeavePolicyId" });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveEncashment_Id",
                table: "LeaveEncashment",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalance_EmployeeId_LeaveTypeId_LeaveLedgerId",
                table: "LeaveBalance",
                columns: new[] { "EmployeeId", "LeaveTypeId", "LeaveLedgerId" });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalance_Id",
                table: "LeaveBalance",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAppStep_Id",
                table: "LeaveAppStep",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAppStep_LeaveAppChainId",
                table: "LeaveAppStep",
                column: "LeaveAppChainId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAppStep_StepName",
                table: "LeaveAppStep",
                column: "StepName");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAppChain_Id",
                table: "LeaveAppChain",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAppAction_Id",
                table: "LeaveAppAction",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAppAction_LeaveRequestId_ApprovedById",
                table: "LeaveAppAction",
                columns: new[] { "LeaveRequestId", "ApprovedById" });

            migrationBuilder.CreateIndex(
                name: "IX_EncashmentAppAction_Id",
                table: "EncashmentAppAction",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EncashmentAppAction_LeaveEncashmentId_ApprovedById",
                table: "EncashmentAppAction",
                columns: new[] { "LeaveEncashmentId", "ApprovedById" });

            migrationBuilder.CreateIndex(
                name: "IX_EmpLeavePolicy_EmployeeId_LeavePolicyId",
                table: "EmpLeavePolicy",
                columns: new[] { "EmployeeId", "LeavePolicyId" });

            migrationBuilder.CreateIndex(
                name: "IX_EmpLeavePolicy_Id",
                table: "EmpLeavePolicy",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccrualHistory_EmployeeId_LeaveTypeId_LeavePolicyId_LeaveLe~",
                table: "AccrualHistory",
                columns: new[] { "EmployeeId", "LeaveTypeId", "LeavePolicyId", "LeaveLedgerId" });

            migrationBuilder.CreateIndex(
                name: "IX_AccrualHistory_Id",
                table: "AccrualHistory",
                column: "Id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EncashmentAppAction_LeaveEncashment_LeaveEncashmentId",
                table: "EncashmentAppAction",
                column: "LeaveEncashmentId",
                principalTable: "LeaveEncashment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveAppAction_LeaveRequest_LeaveRequestId",
                table: "LeaveAppAction",
                column: "LeaveRequestId",
                principalTable: "LeaveRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveAppStep_LeaveAppChain_LeaveAppChainId",
                table: "LeaveAppStep",
                column: "LeaveAppChainId",
                principalTable: "LeaveAppChain",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LeavePolicyConfig_LeavePolicy_LeavePolicyId",
                table: "LeavePolicyConfig",
                column: "LeavePolicyId",
                principalTable: "LeavePolicy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
