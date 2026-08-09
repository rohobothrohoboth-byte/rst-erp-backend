using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Profile.Utility.Migrations
{
    /// <inheritdoc />
    public partial class AddTerminationOffboarding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmpTermination",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TerminationType = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    LastWorkingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NoticeDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Comments = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ExitInterviewNotes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    ApprovedById = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AppliedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RequestFinalPay = table.Column<bool>(type: "boolean", nullable: false),
                    RequestLeaveSettlement = table.Column<bool>(type: "boolean", nullable: false),
                    SettlementPayrollRunId = table.Column<Guid>(type: "uuid", nullable: true),
                    SettlementStatus = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    SettlementNotes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    LeaveUnpaidDaysSnapshot = table.Column<decimal>(type: "numeric", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpTermination", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpTermination_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmpOffboardingTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerminationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Category = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    AssignedToId = table.Column<Guid>(type: "uuid", nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpOffboardingTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpOffboardingTask_EmpTermination_TerminationId",
                        column: x => x.TerminationId,
                        principalTable: "EmpTermination",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmpOffboardingTask_IsDeleted",
                table: "EmpOffboardingTask",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpOffboardingTask_Status",
                table: "EmpOffboardingTask",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EmpOffboardingTask_TerminationId",
                table: "EmpOffboardingTask",
                column: "TerminationId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpTermination_EmployeeId",
                table: "EmpTermination",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpTermination_IsDeleted",
                table: "EmpTermination",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpTermination_Status",
                table: "EmpTermination",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmpOffboardingTask");

            migrationBuilder.DropTable(
                name: "EmpTermination");
        }
    }
}
