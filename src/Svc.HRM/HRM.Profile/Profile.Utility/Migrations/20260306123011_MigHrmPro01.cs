using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Profile.Utility.Migrations
{
    /// <inheritdoc />
    public partial class MigHrmPro01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmpState");

            migrationBuilder.AddColumn<string>(
                name: "EmpState",
                table: "Employee",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "EmpSalary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BaseSalary = table.Column<double>(type: "double precision", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    JgStepId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpSalary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpSalary_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmpSalary_EffectiveFrom",
                table: "EmpSalary",
                column: "EffectiveFrom");

            migrationBuilder.CreateIndex(
                name: "IX_EmpSalary_EmployeeId",
                table: "EmpSalary",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpSalary_IsDeleted",
                table: "EmpSalary",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpSalary_JgStepId",
                table: "EmpSalary",
                column: "JgStepId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmpSalary");

            migrationBuilder.DropColumn(
                name: "EmpState",
                table: "Employee");

            migrationBuilder.CreateTable(
                name: "EmpState",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsApproved = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsRetired = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IsStandBy = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IsTerminated = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IsUnderProbation = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpState", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpState_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmpState_EmployeeId",
                table: "EmpState",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpState_IsDeleted",
                table: "EmpState",
                column: "IsDeleted");
        }
    }
}
