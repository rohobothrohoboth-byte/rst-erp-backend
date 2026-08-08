using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Svc.HRM.Payroll.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PayrollRuns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PayPeriodStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PayPeriodEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PayrollStatus = table.Column<string>(type: "text", nullable: false),
                    TotalGrossPay = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalNetPay = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalTaxes = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalDeductions = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalEmployees = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApprovedBy = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollRuns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalaryStructures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    BaseSalary = table.Column<decimal>(type: "numeric", nullable: false),
                    HousingAllowance = table.Column<decimal>(type: "numeric", nullable: false),
                    TransportAllowance = table.Column<decimal>(type: "numeric", nullable: false),
                    MealAllowance = table.Column<decimal>(type: "numeric", nullable: false),
                    MedicalAllowance = table.Column<decimal>(type: "numeric", nullable: false),
                    OtherAllowances = table.Column<decimal>(type: "numeric", nullable: false),
                    Deductions = table.Column<decimal>(type: "numeric", nullable: false),
                    PensionContribution = table.Column<decimal>(type: "numeric", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryStructures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    MinIncome = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxIncome = table.Column<decimal>(type: "numeric", nullable: true),
                    TaxRate = table.Column<decimal>(type: "numeric", nullable: false),
                    DeductibleAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxYear = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayrollEmployees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PayrollRunId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeCode = table.Column<string>(type: "text", nullable: false),
                    EmployeeName = table.Column<string>(type: "text", nullable: false),
                    Department = table.Column<string>(type: "text", nullable: false),
                    Position = table.Column<string>(type: "text", nullable: false),
                    BaseSalary = table.Column<decimal>(type: "numeric", nullable: false),
                    HousingAllowance = table.Column<decimal>(type: "numeric", nullable: false),
                    TransportAllowance = table.Column<decimal>(type: "numeric", nullable: false),
                    MealAllowance = table.Column<decimal>(type: "numeric", nullable: false),
                    MedicalAllowance = table.Column<decimal>(type: "numeric", nullable: false),
                    OtherAllowances = table.Column<decimal>(type: "numeric", nullable: false),
                    OvertimePay = table.Column<decimal>(type: "numeric", nullable: false),
                    BonusPay = table.Column<decimal>(type: "numeric", nullable: false),
                    CommissionPay = table.Column<decimal>(type: "numeric", nullable: false),
                    GrossPay = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    PensionContribution = table.Column<decimal>(type: "numeric", nullable: false),
                    OtherDeductions = table.Column<decimal>(type: "numeric", nullable: false),
                    NetPay = table.Column<decimal>(type: "numeric", nullable: false),
                    DaysWorked = table.Column<int>(type: "integer", nullable: false),
                    DaysAbsent = table.Column<int>(type: "integer", nullable: false),
                    OvertimeHours = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollEmployees_PayrollRuns_PayrollRunId",
                        column: x => x.PayrollRunId,
                        principalTable: "PayrollRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSalaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    SalaryStructureId = table.Column<Guid>(type: "uuid", nullable: false),
                    BaseSalary = table.Column<decimal>(type: "numeric", nullable: false),
                    HousingAllowance = table.Column<decimal>(type: "numeric", nullable: false),
                    TransportAllowance = table.Column<decimal>(type: "numeric", nullable: false),
                    MealAllowance = table.Column<decimal>(type: "numeric", nullable: false),
                    MedicalAllowance = table.Column<decimal>(type: "numeric", nullable: false),
                    OtherAllowances = table.Column<decimal>(type: "numeric", nullable: false),
                    Deductions = table.Column<decimal>(type: "numeric", nullable: false),
                    PensionContribution = table.Column<decimal>(type: "numeric", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSalaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSalaries_SalaryStructures_SalaryStructureId",
                        column: x => x.SalaryStructureId,
                        principalTable: "SalaryStructures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payslips",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PayrollEmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    PayslipNumber = table.Column<string>(type: "text", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GrossPay = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalAllowances = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalDeductions = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    NetPay = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentMethod = table.Column<string>(type: "text", nullable: true),
                    BankAccount = table.Column<string>(type: "text", nullable: true),
                    IsGenerated = table.Column<bool>(type: "boolean", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GeneratedBy = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payslips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payslips_PayrollEmployees_PayrollEmployeeId",
                        column: x => x.PayrollEmployeeId,
                        principalTable: "PayrollEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaries_EmployeeId_IsActive",
                table: "EmployeeSalaries",
                columns: new[] { "EmployeeId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaries_SalaryStructureId",
                table: "EmployeeSalaries",
                column: "SalaryStructureId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollEmployees_PayrollRunId_EmployeeId",
                table: "PayrollEmployees",
                columns: new[] { "PayrollRunId", "EmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PayrollRuns_PayPeriodEnd",
                table: "PayrollRuns",
                column: "PayPeriodEnd");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollRuns_PayPeriodStart",
                table: "PayrollRuns",
                column: "PayPeriodStart");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollRuns_PayrollStatus",
                table: "PayrollRuns",
                column: "PayrollStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Payslips_PayrollEmployeeId",
                table: "Payslips",
                column: "PayrollEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Payslips_PayslipNumber",
                table: "Payslips",
                column: "PayslipNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalaryStructures_Name",
                table: "SalaryStructures",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaxRates_TaxYear_IsActive",
                table: "TaxRates",
                columns: new[] { "TaxYear", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeSalaries");

            migrationBuilder.DropTable(
                name: "Payslips");

            migrationBuilder.DropTable(
                name: "TaxRates");

            migrationBuilder.DropTable(
                name: "SalaryStructures");

            migrationBuilder.DropTable(
                name: "PayrollEmployees");

            migrationBuilder.DropTable(
                name: "PayrollRuns");
        }
    }
}
