using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Finance.Migrations
{
    /// <inheritdoc />
    public partial class AddPeriodIdToJournalEntriesee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntries_FinancialPeriods_FinancialPeriodId",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_FinancialPeriodId",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "FinancialPeriodId",
                table: "JournalEntries");

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "PurchaseOrders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "PurchaseOrderLines",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "Payments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "JournalLines",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "Invoices",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "InvoiceLines",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "Expenses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "Budgets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "BudgetLines",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "BankTransactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_PeriodId",
                table: "PurchaseOrders",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_PeriodId",
                table: "PurchaseOrderLines",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PeriodId",
                table: "Payments",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalLines_PeriodId",
                table: "JournalLines",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_PeriodId",
                table: "JournalEntries",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_PeriodId",
                table: "Invoices",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLines_PeriodId",
                table: "InvoiceLines",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_PeriodId",
                table: "Expenses",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_PeriodId",
                table: "Budgets",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetLines_PeriodId",
                table: "BudgetLines",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_PeriodId",
                table: "BankTransactions",
                column: "PeriodId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankTransactions_FinancialPeriods_PeriodId",
                table: "BankTransactions",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetLines_FinancialPeriods_PeriodId",
                table: "BudgetLines",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Budgets_FinancialPeriods_PeriodId",
                table: "Budgets",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_FinancialPeriods_PeriodId",
                table: "Expenses",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceLines_FinancialPeriods_PeriodId",
                table: "InvoiceLines",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_FinancialPeriods_PeriodId",
                table: "Invoices",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntries_FinancialPeriods_PeriodId",
                table: "JournalEntries",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalLines_FinancialPeriods_PeriodId",
                table: "JournalLines",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_FinancialPeriods_PeriodId",
                table: "Payments",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderLines_FinancialPeriods_PeriodId",
                table: "PurchaseOrderLines",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_FinancialPeriods_PeriodId",
                table: "PurchaseOrders",
                column: "PeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankTransactions_FinancialPeriods_PeriodId",
                table: "BankTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetLines_FinancialPeriods_PeriodId",
                table: "BudgetLines");

            migrationBuilder.DropForeignKey(
                name: "FK_Budgets_FinancialPeriods_PeriodId",
                table: "Budgets");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_FinancialPeriods_PeriodId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceLines_FinancialPeriods_PeriodId",
                table: "InvoiceLines");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_FinancialPeriods_PeriodId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntries_FinancialPeriods_PeriodId",
                table: "JournalEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalLines_FinancialPeriods_PeriodId",
                table: "JournalLines");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_FinancialPeriods_PeriodId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderLines_FinancialPeriods_PeriodId",
                table: "PurchaseOrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_FinancialPeriods_PeriodId",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_PeriodId",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrderLines_PeriodId",
                table: "PurchaseOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PeriodId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_JournalLines_PeriodId",
                table: "JournalLines");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_PeriodId",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_PeriodId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceLines_PeriodId",
                table: "InvoiceLines");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_PeriodId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Budgets_PeriodId",
                table: "Budgets");

            migrationBuilder.DropIndex(
                name: "IX_BudgetLines_PeriodId",
                table: "BudgetLines");

            migrationBuilder.DropIndex(
                name: "IX_BankTransactions_PeriodId",
                table: "BankTransactions");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "PurchaseOrderLines");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "JournalLines");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "BudgetLines");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "BankTransactions");

            migrationBuilder.AddColumn<Guid>(
                name: "FinancialPeriodId",
                table: "JournalEntries",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_FinancialPeriodId",
                table: "JournalEntries",
                column: "FinancialPeriodId");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntries_FinancialPeriods_FinancialPeriodId",
                table: "JournalEntries",
                column: "FinancialPeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");
        }
    }
}
