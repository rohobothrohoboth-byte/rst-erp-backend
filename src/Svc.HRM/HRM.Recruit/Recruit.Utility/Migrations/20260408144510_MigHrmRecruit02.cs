using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recruit.Utility.Migrations
{
    /// <inheritdoc />
    public partial class MigHrmRecruit02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "JobDec",
                newName: "KeyRespo");

            migrationBuilder.RenameColumn(
                name: "Qualification",
                table: "JobDec",
                newName: "ReqQual");

            migrationBuilder.RenameColumn(
                name: "ContractType",
                table: "JobDec",
                newName: "WorkArr");

            migrationBuilder.AddColumn<string>(
                name: "EmpNature",
                table: "JobDec",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmpNature",
                table: "JobDec");

            migrationBuilder.RenameColumn(
                name: "WorkArr",
                table: "JobDec",
                newName: "ContractType");

            migrationBuilder.RenameColumn(
                name: "ReqQual",
                table: "JobDec",
                newName: "Qualification");

            migrationBuilder.RenameColumn(
                name: "KeyRespo",
                table: "JobDec",
                newName: "Title");
        }
    }
}
