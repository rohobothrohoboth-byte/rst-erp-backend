using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Svc.HRM.Attendance.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexesjkjffkkdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_LocalEmployees_LocalEmployeeId",
                table: "AttendanceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_Shifts_ShiftId",
                table: "AttendanceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_LocalEmployees_LocalDepartments_DepartmentId",
                table: "LocalEmployees");

            migrationBuilder.DropForeignKey(
                name: "FK_LocalEmployees_LocalJobGrades_JobGradeId",
                table: "LocalEmployees");

            migrationBuilder.DropForeignKey(
                name: "FK_LocalEmployees_LocalPositions_PositionId",
                table: "LocalEmployees");

            migrationBuilder.DropForeignKey(
                name: "FK_LocalPositions_LocalDepartments_DepartmentId",
                table: "LocalPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_LocalPositions_LocalJobGrades_JobGradeId",
                table: "LocalPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftAssignments_Shifts_ShiftId",
                table: "ShiftAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_IsActive",
                table: "Shifts",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftAssignments_IsActive",
                table: "ShiftAssignments",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_OvertimeRequests_Status",
                table: "OvertimeRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_LocalPositions_IsDeleted",
                table: "LocalPositions",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_LocalJobGrades_IsDeleted",
                table: "LocalJobGrades",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_LocalEmployees_Code",
                table: "LocalEmployees",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocalEmployees_IsDeleted",
                table: "LocalEmployees",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_LocalDepartments_IsDeleted",
                table: "LocalDepartments",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_LocalCompanies_IsDeleted",
                table: "LocalCompanies",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_LocalBranches_IsDeleted",
                table: "LocalBranches",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_Status",
                table: "LeaveRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalances_EmployeeId",
                table: "LeaveBalances",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Holidays_IsActive",
                table: "Holidays",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_EmployeeId",
                table: "AttendanceRecords",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_Status",
                table: "AttendanceRecords",
                column: "Status");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_LocalEmployees_LocalEmployeeId",
                table: "AttendanceRecords",
                column: "LocalEmployeeId",
                principalTable: "LocalEmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_Shifts_ShiftId",
                table: "AttendanceRecords",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_LocalEmployees_LocalDepartments_DepartmentId",
                table: "LocalEmployees",
                column: "DepartmentId",
                principalTable: "LocalDepartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LocalEmployees_LocalJobGrades_JobGradeId",
                table: "LocalEmployees",
                column: "JobGradeId",
                principalTable: "LocalJobGrades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LocalEmployees_LocalPositions_PositionId",
                table: "LocalEmployees",
                column: "PositionId",
                principalTable: "LocalPositions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LocalPositions_LocalDepartments_DepartmentId",
                table: "LocalPositions",
                column: "DepartmentId",
                principalTable: "LocalDepartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LocalPositions_LocalJobGrades_JobGradeId",
                table: "LocalPositions",
                column: "JobGradeId",
                principalTable: "LocalJobGrades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftAssignments_Shifts_ShiftId",
                table: "ShiftAssignments",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_LocalEmployees_LocalEmployeeId",
                table: "AttendanceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_Shifts_ShiftId",
                table: "AttendanceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_LocalEmployees_LocalDepartments_DepartmentId",
                table: "LocalEmployees");

            migrationBuilder.DropForeignKey(
                name: "FK_LocalEmployees_LocalJobGrades_JobGradeId",
                table: "LocalEmployees");

            migrationBuilder.DropForeignKey(
                name: "FK_LocalEmployees_LocalPositions_PositionId",
                table: "LocalEmployees");

            migrationBuilder.DropForeignKey(
                name: "FK_LocalPositions_LocalDepartments_DepartmentId",
                table: "LocalPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_LocalPositions_LocalJobGrades_JobGradeId",
                table: "LocalPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftAssignments_Shifts_ShiftId",
                table: "ShiftAssignments");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_IsActive",
                table: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_ShiftAssignments_IsActive",
                table: "ShiftAssignments");

            migrationBuilder.DropIndex(
                name: "IX_OvertimeRequests_Status",
                table: "OvertimeRequests");

            migrationBuilder.DropIndex(
                name: "IX_LocalPositions_IsDeleted",
                table: "LocalPositions");

            migrationBuilder.DropIndex(
                name: "IX_LocalJobGrades_IsDeleted",
                table: "LocalJobGrades");

            migrationBuilder.DropIndex(
                name: "IX_LocalEmployees_Code",
                table: "LocalEmployees");

            migrationBuilder.DropIndex(
                name: "IX_LocalEmployees_IsDeleted",
                table: "LocalEmployees");

            migrationBuilder.DropIndex(
                name: "IX_LocalDepartments_IsDeleted",
                table: "LocalDepartments");

            migrationBuilder.DropIndex(
                name: "IX_LocalCompanies_IsDeleted",
                table: "LocalCompanies");

            migrationBuilder.DropIndex(
                name: "IX_LocalBranches_IsDeleted",
                table: "LocalBranches");

            migrationBuilder.DropIndex(
                name: "IX_LeaveRequests_Status",
                table: "LeaveRequests");

            migrationBuilder.DropIndex(
                name: "IX_LeaveBalances_EmployeeId",
                table: "LeaveBalances");

            migrationBuilder.DropIndex(
                name: "IX_Holidays_IsActive",
                table: "Holidays");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_EmployeeId",
                table: "AttendanceRecords");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_Status",
                table: "AttendanceRecords");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_LocalEmployees_LocalEmployeeId",
                table: "AttendanceRecords",
                column: "LocalEmployeeId",
                principalTable: "LocalEmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_Shifts_ShiftId",
                table: "AttendanceRecords",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LocalEmployees_LocalDepartments_DepartmentId",
                table: "LocalEmployees",
                column: "DepartmentId",
                principalTable: "LocalDepartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LocalEmployees_LocalJobGrades_JobGradeId",
                table: "LocalEmployees",
                column: "JobGradeId",
                principalTable: "LocalJobGrades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LocalEmployees_LocalPositions_PositionId",
                table: "LocalEmployees",
                column: "PositionId",
                principalTable: "LocalPositions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LocalPositions_LocalDepartments_DepartmentId",
                table: "LocalPositions",
                column: "DepartmentId",
                principalTable: "LocalDepartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LocalPositions_LocalJobGrades_JobGradeId",
                table: "LocalPositions",
                column: "JobGradeId",
                principalTable: "LocalJobGrades",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftAssignments_Shifts_ShiftId",
                table: "ShiftAssignments",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
