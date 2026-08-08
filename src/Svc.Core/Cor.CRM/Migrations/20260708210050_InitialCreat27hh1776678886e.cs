using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.CRM.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreat27hh1776678886e : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                table: "LocalPositions");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "LocalJobGrades");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "LocalEmployees");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "LocalDepartments");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "LocalDepartments");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "LocalDepartments");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "LocalDepartments");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "LocalDepartments");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "LocalDepartments");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "LocalCompanies");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "LocalCompanies");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "LocalCompanies");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "LocalCompanies");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "LocalCompanies");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "LocalCompanies");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "LocalBranches");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "LocalBranches");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "LocalBranches");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "LocalBranches");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "LocalBranches");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "LocalBranches");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "LocalDepartments",
                newName: "DateMod");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "LocalCompanies",
                newName: "DateMod");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "LocalBranches",
                newName: "DateMod");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAdd",
                table: "LocalDepartments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAdd",
                table: "LocalCompanies",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAdd",
                table: "LocalBranches",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateAdd",
                table: "LocalDepartments");

            migrationBuilder.DropColumn(
                name: "DateAdd",
                table: "LocalCompanies");

            migrationBuilder.DropColumn(
                name: "DateAdd",
                table: "LocalBranches");

            migrationBuilder.RenameColumn(
                name: "DateMod",
                table: "LocalDepartments",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "DateMod",
                table: "LocalCompanies",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "DateMod",
                table: "LocalBranches",
                newName: "UpdatedAt");

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "LocalPositions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "LocalJobGrades",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "LocalEmployees",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "LocalDepartments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "LocalDepartments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "LocalDepartments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "LocalDepartments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "LocalDepartments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "LocalDepartments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "LocalCompanies",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "LocalCompanies",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "LocalCompanies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "LocalCompanies",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "LocalCompanies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "LocalCompanies",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "LocalBranches",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "LocalBranches",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "LocalBranches",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "LocalBranches",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "LocalBranches",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "LocalBranches",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
