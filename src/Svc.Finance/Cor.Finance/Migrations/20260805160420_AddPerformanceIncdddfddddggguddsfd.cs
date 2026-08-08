using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Finance.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIncdddfddddggguddsfd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NormalBalance",
                table: "ChartOfAccounts",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "ChartOfAccounts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChartOfAccounts_CategoryId",
                table: "ChartOfAccounts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartOfAccounts_DepartmentId",
                table: "ChartOfAccounts",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChartOfAccounts_AccountCategories_CategoryId",
                table: "ChartOfAccounts",
                column: "CategoryId",
                principalTable: "AccountCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChartOfAccounts_LocalDepartments_DepartmentId",
                table: "ChartOfAccounts",
                column: "DepartmentId",
                principalTable: "LocalDepartments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChartOfAccounts_AccountCategories_CategoryId",
                table: "ChartOfAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_ChartOfAccounts_LocalDepartments_DepartmentId",
                table: "ChartOfAccounts");

            migrationBuilder.DropIndex(
                name: "IX_ChartOfAccounts_CategoryId",
                table: "ChartOfAccounts");

            migrationBuilder.DropIndex(
                name: "IX_ChartOfAccounts_DepartmentId",
                table: "ChartOfAccounts");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "ChartOfAccounts");

            migrationBuilder.AlterColumn<string>(
                name: "NormalBalance",
                table: "ChartOfAccounts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);
        }
    }
}
