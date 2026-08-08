using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Profile.Utility.Migrations
{
    /// <inheritdoc />
    public partial class AddCareerChangeEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmpContract",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContractNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ContractType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SignedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TerminatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TerminationReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DocumentRef = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    RenewedFromId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpContract", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpContract_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmpPromotion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Comments = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ApprovedById = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AppliedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromJobGradeId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromPositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromDepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromJgStepId = table.Column<Guid>(type: "uuid", nullable: true),
                    ToJobGradeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToPositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToDepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToJgStepId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpPromotion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpPromotion_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmpTransfer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Comments = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ApprovedById = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AppliedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromDepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromPositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromJobGradeId = table.Column<Guid>(type: "uuid", nullable: true),
                    ToDepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToPositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToJobGradeId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpTransfer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpTransfer_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmpContract_ContractNumber",
                table: "EmpContract",
                column: "ContractNumber");

            migrationBuilder.CreateIndex(
                name: "IX_EmpContract_EmployeeId",
                table: "EmpContract",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpContract_IsDeleted",
                table: "EmpContract",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpContract_Status",
                table: "EmpContract",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPromotion_EmployeeId",
                table: "EmpPromotion",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPromotion_IsDeleted",
                table: "EmpPromotion",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPromotion_Status",
                table: "EmpPromotion",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EmpTransfer_EmployeeId",
                table: "EmpTransfer",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpTransfer_IsDeleted",
                table: "EmpTransfer",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpTransfer_Status",
                table: "EmpTransfer",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmpContract");

            migrationBuilder.DropTable(
                name: "EmpPromotion");

            migrationBuilder.DropTable(
                name: "EmpTransfer");
        }
    }
}
