using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Module.Migrations
{
    /// <inheritdoc />
    public partial class MigCorMod02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branch_Company_CompId",
                table: "Branch");

            migrationBuilder.DropForeignKey(
                name: "FK_Department_Branch_BranchId",
                table: "Department");

            migrationBuilder.DropForeignKey(
                name: "FK_Holiday_FiscalYear_FiscalYearId",
                table: "Holiday");

            migrationBuilder.DropForeignKey(
                name: "FK_Period_FiscalYear_FiscalYearId",
                table: "Period");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Period");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Holiday");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "FiscalYear");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Branch");

            migrationBuilder.CreateSequence(
                name: "bra_code_seq",
                maxValue: 9999999L);

            migrationBuilder.AlterColumn<string>(
                name: "Quarter",
                table: "Period",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Period",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Period",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "IsActive",
                table: "Period",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Period",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Holiday",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Holiday",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Holiday",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FiscalYear",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "FiscalYear",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "IsActive",
                table: "FiscalYear",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "FiscalYear",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AlterColumn<string>(
                name: "NameAm",
                table: "Department",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Department",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Department",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "DeptStat",
                table: "Department",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Department",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AlterColumn<string>(
                name: "NameAm",
                table: "Company",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Company",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Company",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Company",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AlterColumn<string>(
                name: "NameAm",
                table: "Branch",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Branch",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Branch",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Branch",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Branch",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValueSql: "'BR-' || LPAD(nextval('bra_code_seq')::text, 7, '0')",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "BranchType",
                table: "Branch",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "BranchStat",
                table: "Branch",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Branch",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.CreateIndex(
                name: "IX_Period_FiscalYearId_Name",
                table: "Period",
                columns: new[] { "FiscalYearId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Period_IsActive",
                table: "Period",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Period_IsDeleted",
                table: "Period",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Holiday_Date",
                table: "Holiday",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Holiday_FiscalYearId_Date",
                table: "Holiday",
                columns: new[] { "FiscalYearId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Holiday_IsDeleted",
                table: "Holiday",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_FiscalYear_IsActive",
                table: "FiscalYear",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_FiscalYear_IsDeleted",
                table: "FiscalYear",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_FiscalYear_Name",
                table: "FiscalYear",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Department_BranchId_Name",
                table: "Department",
                columns: new[] { "BranchId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Department_IsDeleted",
                table: "Department",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Company_IsDeleted",
                table: "Company",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Company_Name",
                table: "Company",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branch_Code",
                table: "Branch",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branch_CompId_Name",
                table: "Branch",
                columns: new[] { "CompId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branch_IsDeleted",
                table: "Branch",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Branch_Name",
                table: "Branch",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Branch_Company_CompId",
                table: "Branch",
                column: "CompId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Department_Branch_BranchId",
                table: "Department",
                column: "BranchId",
                principalTable: "Branch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Holiday_FiscalYear_FiscalYearId",
                table: "Holiday",
                column: "FiscalYearId",
                principalTable: "FiscalYear",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Period_FiscalYear_FiscalYearId",
                table: "Period",
                column: "FiscalYearId",
                principalTable: "FiscalYear",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branch_Company_CompId",
                table: "Branch");

            migrationBuilder.DropForeignKey(
                name: "FK_Department_Branch_BranchId",
                table: "Department");

            migrationBuilder.DropForeignKey(
                name: "FK_Holiday_FiscalYear_FiscalYearId",
                table: "Holiday");

            migrationBuilder.DropForeignKey(
                name: "FK_Period_FiscalYear_FiscalYearId",
                table: "Period");

            migrationBuilder.DropIndex(
                name: "IX_Period_FiscalYearId_Name",
                table: "Period");

            migrationBuilder.DropIndex(
                name: "IX_Period_IsActive",
                table: "Period");

            migrationBuilder.DropIndex(
                name: "IX_Period_IsDeleted",
                table: "Period");

            migrationBuilder.DropIndex(
                name: "IX_Holiday_Date",
                table: "Holiday");

            migrationBuilder.DropIndex(
                name: "IX_Holiday_FiscalYearId_Date",
                table: "Holiday");

            migrationBuilder.DropIndex(
                name: "IX_Holiday_IsDeleted",
                table: "Holiday");

            migrationBuilder.DropIndex(
                name: "IX_FiscalYear_IsActive",
                table: "FiscalYear");

            migrationBuilder.DropIndex(
                name: "IX_FiscalYear_IsDeleted",
                table: "FiscalYear");

            migrationBuilder.DropIndex(
                name: "IX_FiscalYear_Name",
                table: "FiscalYear");

            migrationBuilder.DropIndex(
                name: "IX_Department_BranchId_Name",
                table: "Department");

            migrationBuilder.DropIndex(
                name: "IX_Department_IsDeleted",
                table: "Department");

            migrationBuilder.DropIndex(
                name: "IX_Company_IsDeleted",
                table: "Company");

            migrationBuilder.DropIndex(
                name: "IX_Company_Name",
                table: "Company");

            migrationBuilder.DropIndex(
                name: "IX_Branch_Code",
                table: "Branch");

            migrationBuilder.DropIndex(
                name: "IX_Branch_CompId_Name",
                table: "Branch");

            migrationBuilder.DropIndex(
                name: "IX_Branch_IsDeleted",
                table: "Branch");

            migrationBuilder.DropIndex(
                name: "IX_Branch_Name",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Period");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Holiday");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "FiscalYear");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Branch");

            migrationBuilder.DropSequence(
                name: "bra_code_seq");

            migrationBuilder.AlterColumn<string>(
                name: "Quarter",
                table: "Period",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Period",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Period",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "IsActive",
                table: "Period",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Period",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Holiday",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Holiday",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Holiday",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FiscalYear",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "FiscalYear",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "IsActive",
                table: "FiscalYear",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "FiscalYear",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<string>(
                name: "NameAm",
                table: "Department",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Department",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Department",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "DeptStat",
                table: "Department",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Department",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<string>(
                name: "NameAm",
                table: "Company",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Company",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Company",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Company",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<string>(
                name: "NameAm",
                table: "Branch",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Branch",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Branch",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Branch",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Branch",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldDefaultValueSql: "'BR-' || LPAD(nextval('bra_code_seq')::text, 7, '0')");

            migrationBuilder.AlterColumn<string>(
                name: "BranchType",
                table: "Branch",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "BranchStat",
                table: "Branch",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Branch",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddForeignKey(
                name: "FK_Branch_Company_CompId",
                table: "Branch",
                column: "CompId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Department_Branch_BranchId",
                table: "Department",
                column: "BranchId",
                principalTable: "Branch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Holiday_FiscalYear_FiscalYearId",
                table: "Holiday",
                column: "FiscalYearId",
                principalTable: "FiscalYear",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Period_FiscalYear_FiscalYearId",
                table: "Period",
                column: "FiscalYearId",
                principalTable: "FiscalYear",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
