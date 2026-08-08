using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Cor.Finance.Migrations.FinanceWarehouse
{
    /// <inheritdoc />
    public partial class AddWarehouseIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DimAccounts",
                columns: table => new
                {
                    AccountKey = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AccountType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AccountSubType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ParentKey = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimAccounts", x => x.AccountKey);
                });

            migrationBuilder.CreateTable(
                name: "DimBranches",
                columns: table => new
                {
                    BranchKey = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Location = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimBranches", x => x.BranchKey);
                });

            migrationBuilder.CreateTable(
                name: "DimCustomers",
                columns: table => new
                {
                    CustomerKey = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimCustomers", x => x.CustomerKey);
                });

            migrationBuilder.CreateTable(
                name: "DimDates",
                columns: table => new
                {
                    DateKey = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Day = table.Column<int>(type: "integer", nullable: false),
                    Quarter = table.Column<int>(type: "integer", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    DayOfWeekName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MonthName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    QuarterName = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IsWeekend = table.Column<bool>(type: "boolean", nullable: false),
                    IsHoliday = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimDates", x => x.DateKey);
                });

            migrationBuilder.CreateTable(
                name: "DimDepartments",
                columns: table => new
                {
                    DepartmentKey = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimDepartments", x => x.DepartmentKey);
                });

            migrationBuilder.CreateTable(
                name: "DimEmployees",
                columns: table => new
                {
                    EmployeeKey = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DepartmentKey = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimEmployees", x => x.EmployeeKey);
                });

            migrationBuilder.CreateTable(
                name: "DimVendors",
                columns: table => new
                {
                    VendorKey = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimVendors", x => x.VendorKey);
                });

            migrationBuilder.CreateTable(
                name: "FactBudgets",
                columns: table => new
                {
                    BudgetKey = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateKey = table.Column<int>(type: "integer", nullable: false),
                    DepartmentKey = table.Column<int>(type: "integer", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    SpentAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Utilization = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ETLDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactBudgets", x => x.BudgetKey);
                });

            migrationBuilder.CreateTable(
                name: "FactExpenses",
                columns: table => new
                {
                    ExpenseKey = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExpenseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateKey = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    EmployeeKey = table.Column<int>(type: "integer", nullable: false),
                    DepartmentKey = table.Column<int>(type: "integer", nullable: false),
                    ETLDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactExpenses", x => x.ExpenseKey);
                });

            migrationBuilder.CreateTable(
                name: "FactInvoices",
                columns: table => new
                {
                    InvoiceKey = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InvoiceNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateKey = table.Column<int>(type: "integer", nullable: false),
                    CustomerKey = table.Column<int>(type: "integer", nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    VendorKey = table.Column<int>(type: "integer", nullable: false),
                    VendorName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PaidAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    BalanceAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DaysOverdue = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    InvoiceType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ETLDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactInvoices", x => x.InvoiceKey);
                });

            migrationBuilder.CreateTable(
                name: "FactPayments",
                columns: table => new
                {
                    PaymentKey = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PaymentNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateKey = table.Column<int>(type: "integer", nullable: false),
                    InvoiceKey = table.Column<long>(type: "bigint", nullable: false),
                    CustomerKey = table.Column<int>(type: "integer", nullable: false),
                    VendorKey = table.Column<int>(type: "integer", nullable: false),
                    PaymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PaymentType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ETLDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactPayments", x => x.PaymentKey);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DimAccounts_Code",
                table: "DimAccounts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DimBranches_Code",
                table: "DimBranches",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_DimBranches_Name",
                table: "DimBranches",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_DimCustomers_Code",
                table: "DimCustomers",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_DimCustomers_Name",
                table: "DimCustomers",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_DimDates_Date",
                table: "DimDates",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_DimDates_Month",
                table: "DimDates",
                column: "Month");

            migrationBuilder.CreateIndex(
                name: "IX_DimDates_Quarter",
                table: "DimDates",
                column: "Quarter");

            migrationBuilder.CreateIndex(
                name: "IX_DimDates_Year",
                table: "DimDates",
                column: "Year");

            migrationBuilder.CreateIndex(
                name: "IX_DimDepartments_Code",
                table: "DimDepartments",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_DimDepartments_Name",
                table: "DimDepartments",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_DimEmployees_Code",
                table: "DimEmployees",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_DimEmployees_FirstName",
                table: "DimEmployees",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_DimVendors_Code",
                table: "DimVendors",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_DimVendors_Name",
                table: "DimVendors",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_FactBudgets_DateKey",
                table: "FactBudgets",
                column: "DateKey");

            migrationBuilder.CreateIndex(
                name: "IX_FactBudgets_DepartmentKey",
                table: "FactBudgets",
                column: "DepartmentKey");

            migrationBuilder.CreateIndex(
                name: "IX_FactBudgets_Status",
                table: "FactBudgets",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FactExpenses_Category",
                table: "FactExpenses",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_FactExpenses_DateKey",
                table: "FactExpenses",
                column: "DateKey");

            migrationBuilder.CreateIndex(
                name: "IX_FactExpenses_ExpenseDate",
                table: "FactExpenses",
                column: "ExpenseDate");

            migrationBuilder.CreateIndex(
                name: "IX_FactInvoices_CustomerKey",
                table: "FactInvoices",
                column: "CustomerKey");

            migrationBuilder.CreateIndex(
                name: "IX_FactInvoices_DateKey",
                table: "FactInvoices",
                column: "DateKey");

            migrationBuilder.CreateIndex(
                name: "IX_FactInvoices_InvoiceDate",
                table: "FactInvoices",
                column: "InvoiceDate");

            migrationBuilder.CreateIndex(
                name: "IX_FactInvoices_InvoiceType",
                table: "FactInvoices",
                column: "InvoiceType");

            migrationBuilder.CreateIndex(
                name: "IX_FactInvoices_Status",
                table: "FactInvoices",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FactInvoices_VendorKey",
                table: "FactInvoices",
                column: "VendorKey");

            migrationBuilder.CreateIndex(
                name: "IX_FactPayments_CustomerKey",
                table: "FactPayments",
                column: "CustomerKey");

            migrationBuilder.CreateIndex(
                name: "IX_FactPayments_DateKey",
                table: "FactPayments",
                column: "DateKey");

            migrationBuilder.CreateIndex(
                name: "IX_FactPayments_InvoiceKey",
                table: "FactPayments",
                column: "InvoiceKey");

            migrationBuilder.CreateIndex(
                name: "IX_FactPayments_PaymentDate",
                table: "FactPayments",
                column: "PaymentDate");

            migrationBuilder.CreateIndex(
                name: "IX_FactPayments_PaymentType",
                table: "FactPayments",
                column: "PaymentType");

            migrationBuilder.CreateIndex(
                name: "IX_FactPayments_Status",
                table: "FactPayments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FactPayments_VendorKey",
                table: "FactPayments",
                column: "VendorKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DimAccounts");

            migrationBuilder.DropTable(
                name: "DimBranches");

            migrationBuilder.DropTable(
                name: "DimCustomers");

            migrationBuilder.DropTable(
                name: "DimDates");

            migrationBuilder.DropTable(
                name: "DimDepartments");

            migrationBuilder.DropTable(
                name: "DimEmployees");

            migrationBuilder.DropTable(
                name: "DimVendors");

            migrationBuilder.DropTable(
                name: "FactBudgets");

            migrationBuilder.DropTable(
                name: "FactExpenses");

            migrationBuilder.DropTable(
                name: "FactInvoices");

            migrationBuilder.DropTable(
                name: "FactPayments");
        }
    }
}
