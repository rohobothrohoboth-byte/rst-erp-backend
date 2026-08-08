using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Finance.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexesneddssjfffjgjdffgdttw : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Invoices_BranchId",
                table: "Invoices",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_DepartmentId",
                table: "Invoices",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_EmployeeId",
                table: "Invoices",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_LocalBranches_BranchId",
                table: "Invoices",
                column: "BranchId",
                principalTable: "LocalBranches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_LocalDepartments_DepartmentId",
                table: "Invoices",
                column: "DepartmentId",
                principalTable: "LocalDepartments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_LocalEmployees_EmployeeId",
                table: "Invoices",
                column: "EmployeeId",
                principalTable: "LocalEmployees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_LocalBranches_BranchId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_LocalDepartments_DepartmentId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_LocalEmployees_EmployeeId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_BranchId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_DepartmentId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_EmployeeId",
                table: "Invoices");
        }
    }
}
