using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Finance.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SerialNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Manufacturer = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    AcquisitionCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AcquisitionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SalvageValue = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    UsefulLife = table.Column<int>(type: "integer", nullable: true),
                    DepreciationRate = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    AssetType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AssetCategory = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    AssignedTo = table.Column<Guid>(type: "uuid", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: true),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CurrentValue = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    AccumulatedDepreciation = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    LastDepreciationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    WarrantyInfo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    WarrantyExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    PeriodId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_ChartOfAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "ChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Assets_FinancialPeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "FinancialPeriods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_LocalBranches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "LocalBranches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Assets_LocalDepartments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "LocalDepartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Assets_LocalEmployees_AssignedTo",
                        column: x => x.AssignedTo,
                        principalTable: "LocalEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AccountId",
                table: "Assets",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AcquisitionDate",
                table: "Assets",
                column: "AcquisitionDate");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetCategory",
                table: "Assets",
                column: "AssetCategory");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetType",
                table: "Assets",
                column: "AssetType");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssignedTo",
                table: "Assets",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_BranchId",
                table: "Assets",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_Code",
                table: "Assets",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_DepartmentId",
                table: "Assets",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_IsActive",
                table: "Assets",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_PeriodId",
                table: "Assets",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_SerialNumber",
                table: "Assets",
                column: "SerialNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_Status",
                table: "Assets",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assets");
        }
    }
}
