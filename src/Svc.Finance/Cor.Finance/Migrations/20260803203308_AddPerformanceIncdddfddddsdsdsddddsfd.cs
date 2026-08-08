using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Finance.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIncdddfddddsdsdsddddsfd : Migration
    {
        /// <inheritdoc />
       protected override void Up(MigrationBuilder migrationBuilder)
       {
           migrationBuilder.DropColumn(
               name: "Code",
               table: "Budgets");

           // Add as nullable first
           migrationBuilder.AddColumn<Guid>(
               name: "BudgetCodeId",
               table: "Budgets",
               type: "uuid",
               nullable: true);

           // Create index (nullable columns can have indexes)
           migrationBuilder.CreateIndex(
               name: "IX_Budgets_BudgetCodeId",
               table: "Budgets",
               column: "BudgetCodeId");

           // Add foreign key (nullable foreign key is allowed)
           migrationBuilder.AddForeignKey(
               name: "FK_Budgets_BudgetCodes_BudgetCodeId",
               table: "Budgets",
               column: "BudgetCodeId",
               principalTable: "BudgetCodes",
               principalColumn: "Id",
               onDelete: ReferentialAction.Cascade);
       }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budgets_BudgetCodes_BudgetCodeId",
                table: "Budgets");

            migrationBuilder.DropIndex(
                name: "IX_Budgets_BudgetCodeId",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "BudgetCodeId",
                table: "Budgets");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Budgets",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}