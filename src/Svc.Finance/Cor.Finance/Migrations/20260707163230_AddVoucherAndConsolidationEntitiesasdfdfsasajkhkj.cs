using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Finance.Migrations
{
    /// <inheritdoc />
    public partial class AddVoucherAndConsolidationEntitiesasdfdfsasajkhkj : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EliminationEntries_ChartOfAccounts_NameId",
                table: "EliminationEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_EliminationEntries_Entities_EntityId",
                table: "EliminationEntries");

            migrationBuilder.DropIndex(
                name: "IX_EliminationEntries_EntityId",
                table: "EliminationEntries");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "EliminationEntries");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "EliminationEntries");

            migrationBuilder.RenameColumn(
                name: "NameId",
                table: "EliminationEntries",
                newName: "ToEntityId");

            migrationBuilder.RenameColumn(
                name: "DebitAmount",
                table: "EliminationEntries",
                newName: "ExchangeRate");

            migrationBuilder.RenameColumn(
                name: "CreditAmount",
                table: "EliminationEntries",
                newName: "AmountInReportingCurrency");

            migrationBuilder.RenameIndex(
                name: "IX_EliminationEntries_NameId",
                table: "EliminationEntries",
                newName: "IX_EliminationEntries_ToEntityId");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Entities",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Entities",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentEntityId",
                table: "Entities",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Entities",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationNumber",
                table: "Entities",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxId",
                table: "Entities",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Entities",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "EliminationEntries",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "EliminationEntries",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountCode",
                table: "EliminationEntries",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "EliminationEntries",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "EliminationEntries",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "EliminationEntries",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "EliminationEntries",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FromEntityId",
                table: "EliminationEntries",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "EliminationEntries",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Period",
                table: "EliminationEntries",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PostedAt",
                table: "EliminationEntries",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostedBy",
                table: "EliminationEntries",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ConsolidationReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Period = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ConsolidationGroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    Format = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    GeneratedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GeneratedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FileSize = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Summary = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    TotalRevenue = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalAssets = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalLiabilities = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalEquity = table.Column<decimal>(type: "numeric", nullable: false),
                    NetIncome = table.Column<decimal>(type: "numeric", nullable: false),
                    Adjustments = table.Column<int>(type: "integer", nullable: false),
                    Eliminations = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsolidationReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsolidationReports_ConsolidationGroups_ConsolidationGroup~",
                        column: x => x.ConsolidationGroupId,
                        principalTable: "ConsolidationGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaxReturns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TaxType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Period = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FiscalYear = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    FilingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TaxableAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "numeric", nullable: false),
                    BalanceDue = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FiledBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxReturns", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Entities_ParentEntityId",
                table: "Entities",
                column: "ParentEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EliminationEntries_FromEntityId",
                table: "EliminationEntries",
                column: "FromEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidationReports_ConsolidationGroupId",
                table: "ConsolidationReports",
                column: "ConsolidationGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_EliminationEntries_Entities_FromEntityId",
                table: "EliminationEntries",
                column: "FromEntityId",
                principalTable: "Entities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EliminationEntries_Entities_ToEntityId",
                table: "EliminationEntries",
                column: "ToEntityId",
                principalTable: "Entities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Entities_Entities_ParentEntityId",
                table: "Entities",
                column: "ParentEntityId",
                principalTable: "Entities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EliminationEntries_Entities_FromEntityId",
                table: "EliminationEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_EliminationEntries_Entities_ToEntityId",
                table: "EliminationEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_Entities_Entities_ParentEntityId",
                table: "Entities");

            migrationBuilder.DropTable(
                name: "ConsolidationReports");

            migrationBuilder.DropTable(
                name: "TaxReturns");

            migrationBuilder.DropIndex(
                name: "IX_Entities_ParentEntityId",
                table: "Entities");

            migrationBuilder.DropIndex(
                name: "IX_EliminationEntries_FromEntityId",
                table: "EliminationEntries");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "ParentEntityId",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "RegistrationNumber",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "TaxId",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "AccountCode",
                table: "EliminationEntries");

            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "EliminationEntries");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "EliminationEntries");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "EliminationEntries");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "EliminationEntries");

            migrationBuilder.DropColumn(
                name: "FromEntityId",
                table: "EliminationEntries");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "EliminationEntries");

            migrationBuilder.DropColumn(
                name: "Period",
                table: "EliminationEntries");

            migrationBuilder.DropColumn(
                name: "PostedAt",
                table: "EliminationEntries");

            migrationBuilder.DropColumn(
                name: "PostedBy",
                table: "EliminationEntries");

            migrationBuilder.RenameColumn(
                name: "ToEntityId",
                table: "EliminationEntries",
                newName: "NameId");

            migrationBuilder.RenameColumn(
                name: "ExchangeRate",
                table: "EliminationEntries",
                newName: "DebitAmount");

            migrationBuilder.RenameColumn(
                name: "AmountInReportingCurrency",
                table: "EliminationEntries",
                newName: "CreditAmount");

            migrationBuilder.RenameIndex(
                name: "IX_EliminationEntries_ToEntityId",
                table: "EliminationEntries",
                newName: "IX_EliminationEntries_NameId");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "EliminationEntries",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "EliminationEntries",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "EliminationEntries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "EntityId",
                table: "EliminationEntries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_EliminationEntries_EntityId",
                table: "EliminationEntries",
                column: "EntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_EliminationEntries_ChartOfAccounts_NameId",
                table: "EliminationEntries",
                column: "NameId",
                principalTable: "ChartOfAccounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EliminationEntries_Entities_EntityId",
                table: "EliminationEntries",
                column: "EntityId",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
