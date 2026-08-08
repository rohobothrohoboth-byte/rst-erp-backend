using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Leave.Utility.Migrations
{
    /// <inheritdoc />
    public partial class CreateIntervi3Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,");

            migrationBuilder.CreateTable(
                name: "EmpLeavePolicyHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeaveTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeavePolicyId = table.Column<Guid>(type: "uuid", nullable: true),
                    AssignedEntitlement = table.Column<decimal>(type: "numeric", nullable: false),
                    UsedEntitlement = table.Column<decimal>(type: "numeric", nullable: false),
                    CarryForward = table.Column<decimal>(type: "numeric", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    AssignmentReason = table.Column<string>(type: "text", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ArchivedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ArchiveReason = table.Column<string>(type: "text", nullable: false),
                    ProcessedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ProcessedYear = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpLeavePolicyHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeaveType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    NameAm = table.Column<string>(type: "text", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: false),
                    LeaveCategory = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    AccrualFrequency = table.Column<string>(type: "text", nullable: false),
                    AccrualRate = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxAccrual = table.Column<decimal>(type: "numeric", nullable: false),
                    AllowCarryover = table.Column<bool>(type: "boolean", nullable: false),
                    MaxCarryoverDays = table.Column<decimal>(type: "numeric", nullable: false),
                    CarryoverExpiryMonths = table.Column<int>(type: "integer", nullable: false),
                    MaxDaysPerRequest = table.Column<int>(type: "integer", nullable: false),
                    MaxDaysPerYear = table.Column<int>(type: "integer", nullable: false),
                    MinDaysPerRequest = table.Column<decimal>(type: "numeric", nullable: false),
                    RequiresAttachment = table.Column<bool>(type: "boolean", nullable: false),
                    RequiresDoctorNote = table.Column<bool>(type: "boolean", nullable: false),
                    RequiresApproval = table.Column<bool>(type: "boolean", nullable: false),
                    AllowHalfDay = table.Column<bool>(type: "boolean", nullable: false),
                    AllowNegativeBalance = table.Column<bool>(type: "boolean", nullable: false),
                    MinServiceMonths = table.Column<int>(type: "integer", nullable: false),
                    ProbationPeriodOnly = table.Column<bool>(type: "boolean", nullable: false),
                    EligibleEmploymentTypes = table.Column<string[]>(type: "text[]", nullable: false),
                    SendReminderDays = table.Column<int[]>(type: "integer[]", nullable: false),
                    NotifyManagerOnRequest = table.Column<bool>(type: "boolean", nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    HolidaysAsLeave = table.Column<bool>(type: "boolean", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeavePolicy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    AllowEncashment = table.Column<bool>(type: "boolean", nullable: false),
                    MaxEncashableDays = table.Column<decimal>(type: "numeric", nullable: false),
                    EncashmentRate = table.Column<decimal>(type: "numeric", nullable: false),
                    RequiresAttachment = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    LeaveTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeavePolicy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeavePolicy_LeaveType_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmpLeavePolicy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeaveTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeavePolicyId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedEntitlement = table.Column<decimal>(type: "numeric", nullable: false),
                    UsedEntitlement = table.Column<decimal>(type: "numeric", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AssignmentReason = table.Column<string>(type: "text", nullable: false),
                    Reason = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CarryForward = table.Column<decimal>(type: "numeric", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpLeavePolicy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpLeavePolicy_LeavePolicy_LeavePolicyId",
                        column: x => x.LeavePolicyId,
                        principalTable: "LeavePolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpLeavePolicy_LeaveType_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveAppChain",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LeavePolicyId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeaveTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveAppChain", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveAppChain_LeavePolicy_LeavePolicyId",
                        column: x => x.LeavePolicyId,
                        principalTable: "LeavePolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeaveAppChain_LeaveType_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveBalance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Balance = table.Column<double>(type: "double precision", nullable: false),
                    AsOf = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeaveTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeavePolicyId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveBalance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveBalance_LeavePolicy_LeavePolicyId",
                        column: x => x.LeavePolicyId,
                        principalTable: "LeavePolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveBalance_LeaveType_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveEncashment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DaysEncashed = table.Column<double>(type: "double precision", nullable: false),
                    RatePerDay = table.Column<double>(type: "double precision", nullable: false),
                    TotalAmount = table.Column<double>(type: "double precision", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CurrentAppStep = table.Column<int>(type: "integer", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeaveTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeavePolicyId = table.Column<Guid>(type: "uuid", nullable: true),
                    LeaveAppChainId = table.Column<Guid>(type: "uuid", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveEncashment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveEncashment_LeavePolicy_LeavePolicyId",
                        column: x => x.LeavePolicyId,
                        principalTable: "LeavePolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveEncashment_LeaveType_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveLedger",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<double>(type: "double precision", nullable: false),
                    EntryType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SourceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeaveTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeavePolicyId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReferenceId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveLedger", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveLedger_LeavePolicy_LeavePolicyId",
                        column: x => x.LeavePolicyId,
                        principalTable: "LeavePolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveLedger_LeaveType_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeavePolicyConfig",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnnualEntitlement = table.Column<double>(type: "double precision", nullable: false),
                    AccrualFrequency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AccrualRate = table.Column<double>(type: "double precision", nullable: false),
                    MaxDaysPerReq = table.Column<double>(type: "double precision", nullable: false),
                    MaxCarryOverDays = table.Column<double>(type: "double precision", nullable: false),
                    MinServiceMonths = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    FiscalYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeavePolicyId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeavePolicyConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeavePolicyConfig_LeavePolicy_LeavePolicyId",
                        column: x => x.LeavePolicyId,
                        principalTable: "LeavePolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PolicyAssignmentRule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LeavePolicyId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyAssignmentRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PolicyAssignmentRule_LeavePolicy_LeavePolicyId",
                        column: x => x.LeavePolicyId,
                        principalTable: "LeavePolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeaveAppStep",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StepName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StepOrder = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsFinal = table.Column<bool>(type: "boolean", nullable: false),
                    LeaveAppChainId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeoutHours = table.Column<int>(type: "integer", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveAppStep", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveAppStep_LeaveAppChain_LeaveAppChainId",
                        column: x => x.LeaveAppChainId,
                        principalTable: "LeaveAppChain",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeaveRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DaysRequested = table.Column<double>(type: "double precision", nullable: false),
                    IsHalfDay = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DateApproved = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Comments = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CurrentAppStep = table.Column<int>(type: "integer", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovedById = table.Column<Guid>(type: "uuid", nullable: true),
                    LeaveTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovalChainId = table.Column<Guid>(type: "uuid", nullable: true),
                    CurrentStepId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveRequest_LeaveAppChain_ApprovalChainId",
                        column: x => x.ApprovalChainId,
                        principalTable: "LeaveAppChain",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveRequest_LeaveType_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EncashmentAppAction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StepOrder = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Comment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ActionAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LeaveEncashmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovedById = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncashmentAppAction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EncashmentAppAction_LeaveEncashment_LeaveEncashmentId",
                        column: x => x.LeaveEncashmentId,
                        principalTable: "LeaveEncashment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccrualHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Frequency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AccruedAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PeriodStart = table.Column<DateOnly>(type: "date", nullable: false),
                    PeriodEnd = table.Column<DateOnly>(type: "date", nullable: false),
                    Source = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeaveTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeavePolicyId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeaveLedgerId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccrualHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccrualHistory_LeaveLedger_LeaveLedgerId",
                        column: x => x.LeaveLedgerId,
                        principalTable: "LeaveLedger",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccrualHistory_LeavePolicy_LeavePolicyId",
                        column: x => x.LeavePolicyId,
                        principalTable: "LeavePolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccrualHistory_LeaveType_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PolicyRuleCondition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Field = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Operator = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Value = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PolicyAssignmentRuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyRuleCondition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PolicyRuleCondition_PolicyAssignmentRule_PolicyAssignmentRu~",
                        column: x => x.PolicyAssignmentRuleId,
                        principalTable: "PolicyAssignmentRule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attachment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    DateUpload = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LeaveRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachment_LeaveRequest_LeaveRequestId",
                        column: x => x.LeaveRequestId,
                        principalTable: "LeaveRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeaveAppAction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StepOrder = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Comment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ActionAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LeaveRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovedById = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveAppAction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveAppAction_LeaveRequest_LeaveRequestId",
                        column: x => x.LeaveRequestId,
                        principalTable: "LeaveRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttachmentBlob",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<byte[]>(type: "bytea", nullable: false),
                    AttachmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentBlob", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttachmentBlob_Attachment_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccrualHistory_EmployeeId",
                table: "AccrualHistory",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AccrualHistory_EmployeeId_LeaveTypeId_PeriodStart",
                table: "AccrualHistory",
                columns: new[] { "EmployeeId", "LeaveTypeId", "PeriodStart" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccrualHistory_IsDeleted",
                table: "AccrualHistory",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_AccrualHistory_LeaveLedgerId",
                table: "AccrualHistory",
                column: "LeaveLedgerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccrualHistory_LeavePolicyId",
                table: "AccrualHistory",
                column: "LeavePolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_AccrualHistory_LeaveTypeId",
                table: "AccrualHistory",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_IsDeleted",
                table: "Attachment",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_LeaveRequestId",
                table: "Attachment",
                column: "LeaveRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentBlob_AttachmentId",
                table: "AttachmentBlob",
                column: "AttachmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentBlob_IsDeleted",
                table: "AttachmentBlob",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpLeavePolicy_EmployeeId",
                table: "EmpLeavePolicy",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpLeavePolicy_EmployeeId_LeaveTypeId_LeavePolicyId",
                table: "EmpLeavePolicy",
                columns: new[] { "EmployeeId", "LeaveTypeId", "LeavePolicyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpLeavePolicy_IsDeleted",
                table: "EmpLeavePolicy",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpLeavePolicy_LeavePolicyId",
                table: "EmpLeavePolicy",
                column: "LeavePolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpLeavePolicy_LeaveTypeId",
                table: "EmpLeavePolicy",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EncashmentAppAction_ApprovedById",
                table: "EncashmentAppAction",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_EncashmentAppAction_IsDeleted",
                table: "EncashmentAppAction",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EncashmentAppAction_LeaveEncashmentId",
                table: "EncashmentAppAction",
                column: "LeaveEncashmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EncashmentAppAction_LeaveEncashmentId_StepOrder",
                table: "EncashmentAppAction",
                columns: new[] { "LeaveEncashmentId", "StepOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAppAction_ApprovedById",
                table: "LeaveAppAction",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAppAction_IsDeleted",
                table: "LeaveAppAction",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAppAction_LeaveRequestId",
                table: "LeaveAppAction",
                column: "LeaveRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAppAction_LeaveRequestId_StepOrder",
                table: "LeaveAppAction",
                columns: new[] { "LeaveRequestId", "StepOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAppChain_IsDeleted",
                table: "LeaveAppChain",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAppChain_LeavePolicyId",
                table: "LeaveAppChain",
                column: "LeavePolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAppChain_LeavePolicyId_EffectiveFrom",
                table: "LeaveAppChain",
                columns: new[] { "LeavePolicyId", "EffectiveFrom" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAppChain_LeaveTypeId",
                table: "LeaveAppChain",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAppStep_IsDeleted",
                table: "LeaveAppStep",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAppStep_LeaveAppChainId",
                table: "LeaveAppStep",
                column: "LeaveAppChainId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAppStep_LeaveAppChainId_StepOrder",
                table: "LeaveAppStep",
                columns: new[] { "LeaveAppChainId", "StepOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalance_EmployeeId",
                table: "LeaveBalance",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalance_IsDeleted",
                table: "LeaveBalance",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalance_LeavePolicyId_LeaveTypeId_EmployeeId",
                table: "LeaveBalance",
                columns: new[] { "LeavePolicyId", "LeaveTypeId", "EmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalance_LeaveTypeId",
                table: "LeaveBalance",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveEncashment_EmployeeId",
                table: "LeaveEncashment",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveEncashment_IsDeleted",
                table: "LeaveEncashment",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveEncashment_LeavePolicyId",
                table: "LeaveEncashment",
                column: "LeavePolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveEncashment_LeaveTypeId",
                table: "LeaveEncashment",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveLedger_EmployeeId",
                table: "LeaveLedger",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveLedger_EmployeeId_LeaveTypeId_Date",
                table: "LeaveLedger",
                columns: new[] { "EmployeeId", "LeaveTypeId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveLedger_IsDeleted",
                table: "LeaveLedger",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveLedger_LeavePolicyId",
                table: "LeaveLedger",
                column: "LeavePolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveLedger_LeaveTypeId",
                table: "LeaveLedger",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicy_Code",
                table: "LeavePolicy",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicy_IsDeleted",
                table: "LeavePolicy",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicy_LeaveTypeId",
                table: "LeavePolicy",
                column: "LeaveTypeId",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicy_Name",
                table: "LeavePolicy",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicyConfig_FiscalYearId",
                table: "LeavePolicyConfig",
                column: "FiscalYearId");

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicyConfig_IsDeleted",
                table: "LeavePolicyConfig",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicyConfig_LeavePolicyId",
                table: "LeavePolicyConfig",
                column: "LeavePolicyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequest_ApprovalChainId",
                table: "LeaveRequest",
                column: "ApprovalChainId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequest_EmployeeId",
                table: "LeaveRequest",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequest_EmployeeId_LeaveTypeId_StartDate",
                table: "LeaveRequest",
                columns: new[] { "EmployeeId", "LeaveTypeId", "StartDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequest_IsDeleted",
                table: "LeaveRequest",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequest_LeaveTypeId",
                table: "LeaveRequest",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveType_IsActive",
                table: "LeaveType",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveType_IsDeleted",
                table: "LeaveType",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveType_LeaveCategory",
                table: "LeaveType",
                column: "LeaveCategory");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveType_Name",
                table: "LeaveType",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PolicyAssignmentRule_IsDeleted",
                table: "PolicyAssignmentRule",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyAssignmentRule_LeavePolicyId",
                table: "PolicyAssignmentRule",
                column: "LeavePolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyAssignmentRule_LeavePolicyId_Code",
                table: "PolicyAssignmentRule",
                columns: new[] { "LeavePolicyId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PolicyRuleCondition_IsDeleted",
                table: "PolicyRuleCondition",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyRuleCondition_PolicyAssignmentRuleId",
                table: "PolicyRuleCondition",
                column: "PolicyAssignmentRuleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccrualHistory");

            migrationBuilder.DropTable(
                name: "AttachmentBlob");

            migrationBuilder.DropTable(
                name: "EmpLeavePolicy");

            migrationBuilder.DropTable(
                name: "EmpLeavePolicyHistories");

            migrationBuilder.DropTable(
                name: "EncashmentAppAction");

            migrationBuilder.DropTable(
                name: "LeaveAppAction");

            migrationBuilder.DropTable(
                name: "LeaveAppStep");

            migrationBuilder.DropTable(
                name: "LeaveBalance");

            migrationBuilder.DropTable(
                name: "LeavePolicyConfig");

            migrationBuilder.DropTable(
                name: "PolicyRuleCondition");

            migrationBuilder.DropTable(
                name: "LeaveLedger");

            migrationBuilder.DropTable(
                name: "Attachment");

            migrationBuilder.DropTable(
                name: "LeaveEncashment");

            migrationBuilder.DropTable(
                name: "PolicyAssignmentRule");

            migrationBuilder.DropTable(
                name: "LeaveRequest");

            migrationBuilder.DropTable(
                name: "LeaveAppChain");

            migrationBuilder.DropTable(
                name: "LeavePolicy");

            migrationBuilder.DropTable(
                name: "LeaveType");
        }
    }
}
