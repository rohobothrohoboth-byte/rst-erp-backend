using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Profile.Utility.Migrations
{
    /// <inheritdoc />
    public partial class MigHrmPro05 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyContact_Employee_EmployeeId",
                table: "EmergencyContact");

            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyContact_Person_PersonId",
                table: "EmergencyContact");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpFamily_Employee_EmployeeId",
                table: "EmpFamily");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpFamily_Person_PersonId",
                table: "EmpFamily");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpGuarantor_Employee_EmployeeId",
                table: "EmpGuarantor");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpGuarantor_Person_PersonId",
                table: "EmpGuarantor");

            migrationBuilder.DropIndex(
                name: "IX_EmpGuarantor_EmployeeId_PersonId",
                table: "EmpGuarantor");

            migrationBuilder.DropIndex(
                name: "IX_EmpGuarantor_PersonId",
                table: "EmpGuarantor");

            migrationBuilder.DropIndex(
                name: "IX_EmpFamily_EmployeeId_PersonId",
                table: "EmpFamily");

            migrationBuilder.DropIndex(
                name: "IX_EmpFamily_PersonId",
                table: "EmpFamily");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyContact_EmployeeId_PersonId",
                table: "EmergencyContact");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyContact_PersonId",
                table: "EmergencyContact");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "EmpGuarantor");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "EmpFamily");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "EmergencyContact");

            migrationBuilder.AlterColumn<string>(
                name: "Relation",
                table: "EmpGuarantor",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "EmpGuarantor",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "EmpGuarantor",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "EmpGuarantor",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "EmpGuarantor",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "EmpGuarantor",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Relation",
                table: "EmpFamily",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "EmpFamily",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "EmpFamily",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "EmpFamily",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "EmpFamily",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "EmpFamily",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Relation",
                table: "EmergencyContact",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "EmergencyContact",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "EmergencyContact",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "EmergencyContact",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "EmergencyContact",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "EmergencyContact",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantor_Id_AddressId",
                table: "EmpGuarantor",
                columns: new[] { "Id", "AddressId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantor_Id_EmployeeId",
                table: "EmpGuarantor",
                columns: new[] { "Id", "EmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContact_Id_AddressId",
                table: "EmergencyContact",
                columns: new[] { "Id", "AddressId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContact_Id_EmployeeId",
                table: "EmergencyContact",
                columns: new[] { "Id", "EmployeeId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyContact_Employee_EmployeeId",
                table: "EmergencyContact",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpFamily_Employee_EmployeeId",
                table: "EmpFamily",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpGuarantor_Employee_EmployeeId",
                table: "EmpGuarantor",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyContact_Employee_EmployeeId",
                table: "EmergencyContact");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpFamily_Employee_EmployeeId",
                table: "EmpFamily");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpGuarantor_Employee_EmployeeId",
                table: "EmpGuarantor");

            migrationBuilder.DropIndex(
                name: "IX_EmpGuarantor_Id_AddressId",
                table: "EmpGuarantor");

            migrationBuilder.DropIndex(
                name: "IX_EmpGuarantor_Id_EmployeeId",
                table: "EmpGuarantor");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyContact_Id_AddressId",
                table: "EmergencyContact");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyContact_Id_EmployeeId",
                table: "EmergencyContact");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "EmpGuarantor");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "EmpGuarantor");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "EmpGuarantor");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "EmpGuarantor");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "EmpGuarantor");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "EmpFamily");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "EmpFamily");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "EmpFamily");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "EmpFamily");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "EmpFamily");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "EmergencyContact");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "EmergencyContact");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "EmergencyContact");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "EmergencyContact");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "EmergencyContact");

            migrationBuilder.AlterColumn<string>(
                name: "Relation",
                table: "EmpGuarantor",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<Guid>(
                name: "PersonId",
                table: "EmpGuarantor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Relation",
                table: "EmpFamily",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<Guid>(
                name: "PersonId",
                table: "EmpFamily",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Relation",
                table: "EmergencyContact",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<Guid>(
                name: "PersonId",
                table: "EmergencyContact",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantor_EmployeeId_PersonId",
                table: "EmpGuarantor",
                columns: new[] { "EmployeeId", "PersonId" });

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantor_PersonId",
                table: "EmpGuarantor",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpFamily_EmployeeId_PersonId",
                table: "EmpFamily",
                columns: new[] { "EmployeeId", "PersonId" });

            migrationBuilder.CreateIndex(
                name: "IX_EmpFamily_PersonId",
                table: "EmpFamily",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContact_EmployeeId_PersonId",
                table: "EmergencyContact",
                columns: new[] { "EmployeeId", "PersonId" });

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContact_PersonId",
                table: "EmergencyContact",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyContact_Employee_EmployeeId",
                table: "EmergencyContact",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyContact_Person_PersonId",
                table: "EmergencyContact",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpFamily_Employee_EmployeeId",
                table: "EmpFamily",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpFamily_Person_PersonId",
                table: "EmpFamily",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpGuarantor_Employee_EmployeeId",
                table: "EmpGuarantor",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpGuarantor_Person_PersonId",
                table: "EmpGuarantor",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
