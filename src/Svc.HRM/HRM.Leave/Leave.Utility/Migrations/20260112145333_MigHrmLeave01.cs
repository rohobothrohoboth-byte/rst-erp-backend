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
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,");

            migrationBuilder.CreateTable(
                name: "LeaveType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    LeaveCategory = table.Column<string>(type: "text", nullable: false),
                    RequiresApproval = table.Column<bool>(type: "boolean", nullable: false),
                    AllowHalfDay = table.Column<bool>(type: "boolean", nullable: false),
                    HolidaysAsLeave = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
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
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AllowEncashment = table.Column<bool>(type: "boolean", nullable: false),
                    RequiresAttachment = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    LeaveTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
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
                name: "LeaveRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DaysRequested = table.Column<double>(type: "double precision", nullable: false),
                    IsHalfDay = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    DateApproved = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Comments = table.Column<string>(type: "text", nullable: false),
                    CurrentAppStep = table.Column<int>(type: "integer", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovedById = table.Column<Guid>(type: "uuid", nullable: true),
                    LeaveTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveRequest_LeaveType_LeaveTypeId",
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
                    EffectiveFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveAppChain", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveAppChain_LeavePolicy_LeavePolicyId",
                        column: x => x.LeavePolicyId,
                        principalTable: "LeavePolicy",
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
                    Status = table.Column<string>(type: "text", nullable: false),
                    CurrentAppStep = table.Column<int>(type: "integer", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeaveTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeavePolicyId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
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
                    EntryType = table.Column<string>(type: "text", nullable: false),
                    SourceType = table.Column<string>(type: "text", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeaveTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeavePolicyId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReferenceId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
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
                name: "PolicyAssignmentRule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LeavePolicyId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeaveTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyAssignmentRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PolicyAssignmentRule_LeavePolicy_LeavePolicyId",
                        column: x => x.LeavePolicyId,
                        principalTable: "LeavePolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PolicyAssignmentRule_LeaveType_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Attachment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    DateUpload = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LeaveRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachment_LeaveRequest_LeaveRequestId",
                        column: x => x.LeaveRequestId,
                        principalTable: "LeaveRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveAppAction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StepOrder = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    ActionAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LeaveRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovedById = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveAppAction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveAppAction_LeaveRequest_LeaveRequestId",
                        column: x => x.LeaveRequestId,
                        principalTable: "LeaveRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveAppStep",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StepOrder = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    StepName = table.Column<string>(type: "text", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsFinal = table.Column<bool>(type: "boolean", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LeaveAppChainId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveAppStep", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveAppStep_LeaveAppChain_LeaveAppChainId",
                        column: x => x.LeaveAppChainId,
                        principalTable: "LeaveAppChain",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeavePolicyConfig",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnnualEntitlement = table.Column<double>(type: "double precision", nullable: false),
                    AccrualFrequency = table.Column<string>(type: "text", nullable: false),
                    AccrualRate = table.Column<double>(type: "double precision", nullable: false),
                    MaxDaysPerReq = table.Column<double>(type: "double precision", nullable: false),
                    MaxCarryOverDays = table.Column<double>(type: "double precision", nullable: false),
                    MinServiceMonths = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    FiscalYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeavePolicyId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeaveAppChainId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeavePolicyConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeavePolicyConfig_LeaveAppChain_LeaveAppChainId",
                        column: x => x.LeaveAppChainId,
                        principalTable: "LeaveAppChain",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeavePolicyConfig_LeavePolicy_LeavePolicyId",
                        column: x => x.LeavePolicyId,
                        principalTable: "LeavePolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EncashmentAppAction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StepOrder = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    ActionAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LeaveEncashmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovedById = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncashmentAppAction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EncashmentAppAction_LeaveEncashment_LeaveEncashmentId",
                        column: x => x.LeaveEncashmentId,
                        principalTable: "LeaveEncashment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccrualHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Frequency = table.Column<string>(type: "text", nullable: false),
                    AccruedAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    PeriodStart = table.Column<DateOnly>(type: "date", nullable: false),
                    PeriodEnd = table.Column<DateOnly>(type: "date", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeaveTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeavePolicyId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeaveLedgerId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
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
                name: "LeaveBalance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Balance = table.Column<double>(type: "double precision", nullable: false),
                    AsOf = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeaveTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeaveLedgerId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveBalance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveBalance_LeaveLedger_LeaveLedgerId",
                        column: x => x.LeaveLedgerId,
                        principalTable: "LeaveLedger",
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
                name: "PolicyRuleCondition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Field = table.Column<string>(type: "text", nullable: false),
                    Operator = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    PolicyAssignmentRuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyRuleCondition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PolicyRuleCondition_PolicyAssignmentRule_PolicyAssignmentRu~",
                        column: x => x.PolicyAssignmentRuleId,
                        principalTable: "PolicyAssignmentRule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentBlob", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttachmentBlob_Attachment_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmpLeavePolicy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AssignedEntitlement = table.Column<double>(type: "double precision", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeaveTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeavePolicyConfigId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpLeavePolicy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpLeavePolicy_LeavePolicyConfig_LeavePolicyConfigId",
                        column: x => x.LeavePolicyConfigId,
                        principalTable: "LeavePolicyConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpLeavePolicy_LeaveType_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccrualHistory_EmployeeId_LeaveTypeId_LeavePolicyId_LeaveLe~",
                table: "AccrualHistory",
                columns: new[] { "EmployeeId", "LeaveTypeId", "LeavePolicyId", "LeaveLedgerId" });

            migrationBuilder.CreateIndex(
                name: "IX_AccrualHistory_Id",
                table: "AccrualHistory",
                column: "Id",
                unique: true);

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
                name: "IX_Attachment_Id",
                table: "Attachment",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_LeaveRequestId",
                table: "Attachment",
                column: "LeaveRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentBlob_AttachmentId",
                table: "AttachmentBlob",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentBlob_Id",
                table: "AttachmentBlob",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpLeavePolicy_EmployeeId_LeaveTypeId_LeavePolicyConfigId",
                table: "EmpLeavePolicy",
                columns: new[] { "EmployeeId", "LeaveTypeId", "LeavePolicyConfigId" });

            migrationBuilder.CreateIndex(
                name: "IX_EmpLeavePolicy_Id",
                table: "EmpLeavePolicy",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpLeavePolicy_LeavePolicyConfigId",
                table: "EmpLeavePolicy",
                column: "LeavePolicyConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpLeavePolicy_LeaveTypeId",
                table: "EmpLeavePolicy",
                column: "LeaveTypeId");

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
                name: "IX_LeaveAppAction_Id",
                table: "LeaveAppAction",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAppAction_LeaveRequestId_ApprovedById",
                table: "LeaveAppAction",
                columns: new[] { "LeaveRequestId", "ApprovedById" });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAppChain_Id",
                table: "LeaveAppChain",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAppChain_LeavePolicyId",
                table: "LeaveAppChain",
                column: "LeavePolicyId");

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
                name: "IX_LeaveBalance_EmployeeId_LeaveTypeId_LeaveLedgerId",
                table: "LeaveBalance",
                columns: new[] { "EmployeeId", "LeaveTypeId", "LeaveLedgerId" });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalance_Id",
                table: "LeaveBalance",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalance_LeaveLedgerId",
                table: "LeaveBalance",
                column: "LeaveLedgerId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalance_LeaveTypeId",
                table: "LeaveBalance",
                column: "LeaveTypeId");

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
                name: "IX_LeaveEncashment_LeavePolicyId",
                table: "LeaveEncashment",
                column: "LeavePolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveEncashment_LeaveTypeId",
                table: "LeaveEncashment",
                column: "LeaveTypeId");

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
                name: "IX_LeavePolicy_Id",
                table: "LeavePolicy",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicy_LeaveTypeId",
                table: "LeavePolicy",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicyConfig_FiscalYearId_LeavePolicyId_LeaveAppChainId",
                table: "LeavePolicyConfig",
                columns: new[] { "FiscalYearId", "LeavePolicyId", "LeaveAppChainId" });

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicyConfig_Id",
                table: "LeavePolicyConfig",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicyConfig_LeaveAppChainId",
                table: "LeavePolicyConfig",
                column: "LeaveAppChainId");

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicyConfig_LeavePolicyId",
                table: "LeavePolicyConfig",
                column: "LeavePolicyId");

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
                name: "IX_LeaveRequest_LeaveTypeId",
                table: "LeaveRequest",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveType_LeaveCategory_IsActive",
                table: "LeaveType",
                columns: new[] { "LeaveCategory", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveType_Name",
                table: "LeaveType",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PolicyAssignmentRule_Code",
                table: "PolicyAssignmentRule",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PolicyAssignmentRule_Id",
                table: "PolicyAssignmentRule",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PolicyAssignmentRule_LeavePolicyId_LeaveTypeId",
                table: "PolicyAssignmentRule",
                columns: new[] { "LeavePolicyId", "LeaveTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_PolicyAssignmentRule_LeaveTypeId",
                table: "PolicyAssignmentRule",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyRuleCondition_Id",
                table: "PolicyRuleCondition",
                column: "Id",
                unique: true);

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
                name: "EncashmentAppAction");

            migrationBuilder.DropTable(
                name: "LeaveAppAction");

            migrationBuilder.DropTable(
                name: "LeaveAppStep");

            migrationBuilder.DropTable(
                name: "LeaveBalance");

            migrationBuilder.DropTable(
                name: "PolicyRuleCondition");

            migrationBuilder.DropTable(
                name: "Attachment");

            migrationBuilder.DropTable(
                name: "LeavePolicyConfig");

            migrationBuilder.DropTable(
                name: "LeaveEncashment");

            migrationBuilder.DropTable(
                name: "LeaveLedger");

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
