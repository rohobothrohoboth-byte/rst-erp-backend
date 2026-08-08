using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Leave.Utility.Migrations
{
    /// <inheritdoc />
    public partial class FixAppUserIdOdddnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocalBranches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NameAm = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    OpenDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BranchType = table.Column<string>(type: "text", nullable: false),
                    BranchStat = table.Column<string>(type: "text", nullable: false),
                    CompId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalBranches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocalCompanies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NameAm = table.Column<string>(type: "text", nullable: false),
                    TaxId = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    LogoUrl = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalCompanies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocalDepartments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NameAm = table.Column<string>(type: "text", nullable: false),
                    DeptStat = table.Column<string>(type: "text", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalDepartments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocalJgStep",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Salary = table.Column<double>(type: "double precision", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    SalaryPayFreq = table.Column<string>(type: "text", nullable: false),
                    JobGradeId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalJgStep", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocalJobGrades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StartSalary = table.Column<double>(type: "double precision", nullable: false),
                    MaxSalary = table.Column<double>(type: "double precision", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalJobGrades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocalPositions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NameAm = table.Column<string>(type: "text", nullable: false),
                    NoOfPosition = table.Column<int>(type: "integer", nullable: false),
                    IsVacant = table.Column<string>(type: "text", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    JobGradeId = table.Column<Guid>(type: "uuid", nullable: true),
                    SaturdayWorkOption = table.Column<string>(type: "text", nullable: false),
                    SundayWorkOption = table.Column<string>(type: "text", nullable: false),
                    WorkingHours = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    ProfessionType = table.Column<string>(type: "text", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalPositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocalPositions_LocalDepartments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "LocalDepartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LocalPositions_LocalJobGrades_JobGradeId",
                        column: x => x.JobGradeId,
                        principalTable: "LocalJobGrades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LocalEmployees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    EmploymentType = table.Column<string>(type: "text", nullable: false),
                    EmploymentNature = table.Column<string>(type: "text", nullable: false),
                    WorkArrangement = table.Column<string>(type: "text", nullable: false),
                    EmpState = table.Column<string>(type: "text", nullable: false),
                    EmploymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PersonId = table.Column<Guid>(type: "uuid", nullable: false),
                    JobGradeId = table.Column<Guid>(type: "uuid", nullable: false),
                    PositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    FirstNameAm = table.Column<string>(type: "text", nullable: false),
                    MiddleName = table.Column<string>(type: "text", nullable: false),
                    MiddleNameAm = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    LastNameAm = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    Nationality = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocalEmployees_LocalBranches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "LocalBranches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LocalEmployees_LocalDepartments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "LocalDepartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LocalEmployees_LocalJobGrades_JobGradeId",
                        column: x => x.JobGradeId,
                        principalTable: "LocalJobGrades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LocalEmployees_LocalPositions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "LocalPositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocalEmployees_BranchId",
                table: "LocalEmployees",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalEmployees_DepartmentId",
                table: "LocalEmployees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalEmployees_JobGradeId",
                table: "LocalEmployees",
                column: "JobGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalEmployees_PositionId",
                table: "LocalEmployees",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalPositions_DepartmentId",
                table: "LocalPositions",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalPositions_JobGradeId",
                table: "LocalPositions",
                column: "JobGradeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocalCompanies");

            migrationBuilder.DropTable(
                name: "LocalEmployees");

            migrationBuilder.DropTable(
                name: "LocalJgStep");

            migrationBuilder.DropTable(
                name: "LocalBranches");

            migrationBuilder.DropTable(
                name: "LocalPositions");

            migrationBuilder.DropTable(
                name: "LocalDepartments");

            migrationBuilder.DropTable(
                name: "LocalJobGrades");
        }
    }
}
