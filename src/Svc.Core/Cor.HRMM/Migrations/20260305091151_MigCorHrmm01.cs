using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.HRMM.Migrations
{
    /// <inheritdoc />
    public partial class MigCorHrmm01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JgStep_JobGrade_JobGradeId",
                table: "JgStep");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionBenefit_BenefitSetting_BenefitSettingId",
                table: "PositionBenefit");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionBenefit_Position_PositionId",
                table: "PositionBenefit");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionEducation_EducationQual_EducationQualId",
                table: "PositionEducation");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionEducation_Position_PositionId",
                table: "PositionEducation");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionExp_Position_PositionId",
                table: "PositionExp");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionReq_Position_PositionId",
                table: "PositionReq");

            migrationBuilder.DropIndex(
                name: "IX_PositionEducation_PositionId",
                table: "PositionEducation");

            migrationBuilder.DropIndex(
                name: "IX_PositionBenefit_PositionId",
                table: "PositionBenefit");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "PositionReq");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "PositionExp");

            migrationBuilder.DropColumn(
                name: "EducationLevelId",
                table: "PositionEducation");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "PositionEducation");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "PositionBenefit");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Position");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "JobGrade");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "JgStep");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "EducationQual");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "BenefitSetting");

            migrationBuilder.AlterColumn<string>(
                name: "SundayWorkOption",
                table: "PositionReq",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "SaturdayWorkOption",
                table: "PositionReq",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ProfessionType",
                table: "PositionReq",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "PositionReq",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "PositionReq",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "PositionReq",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "PositionExp",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "PositionExp",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "PositionEducation",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<string>(
                name: "EducationLevel",
                table: "PositionEducation",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "PositionEducation",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "PositionBenefit",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "PositionBenefit",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AlterColumn<string>(
                name: "NameAm",
                table: "Position",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Position",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "IsVacant",
                table: "Position",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Position",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Position",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "JobGrade",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "JobGrade",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "JobGrade",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "JgStep",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "JgStep",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "JgStep",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EducationQual",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EducationQual",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "EducationQual",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AlterColumn<string>(
                name: "Per",
                table: "BenefitSetting",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BenefitSetting",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "BenefitSetting",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "BenefitSetting",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.CreateIndex(
                name: "IX_PositionReq_IsDeleted",
                table: "PositionReq",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_PositionExp_IsDeleted",
                table: "PositionExp",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_PositionEducation_IsDeleted",
                table: "PositionEducation",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_PositionEducation_PositionId_EducationQualId",
                table: "PositionEducation",
                columns: new[] { "PositionId", "EducationQualId" });

            migrationBuilder.CreateIndex(
                name: "IX_PositionBenefit_IsDeleted",
                table: "PositionBenefit",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_PositionBenefit_PositionId_BenefitSettingId",
                table: "PositionBenefit",
                columns: new[] { "PositionId", "BenefitSettingId" });

            migrationBuilder.CreateIndex(
                name: "IX_Position_DepartmentId_IsVacant",
                table: "Position",
                columns: new[] { "DepartmentId", "IsVacant" });

            migrationBuilder.CreateIndex(
                name: "IX_Position_IsDeleted",
                table: "Position",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobGrade_IsDeleted",
                table: "JobGrade",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobGrade_Name",
                table: "JobGrade",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JgStep_IsDeleted",
                table: "JgStep",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JgStep_Name",
                table: "JgStep",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EducationQual_IsDeleted",
                table: "EducationQual",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EducationQual_Name",
                table: "EducationQual",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BenefitSetting_IsDeleted",
                table: "BenefitSetting",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_BenefitSetting_Name",
                table: "BenefitSetting",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BenefitSetting_Per",
                table: "BenefitSetting",
                column: "Per");

            migrationBuilder.AddForeignKey(
                name: "FK_JgStep_JobGrade_JobGradeId",
                table: "JgStep",
                column: "JobGradeId",
                principalTable: "JobGrade",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionBenefit_BenefitSetting_BenefitSettingId",
                table: "PositionBenefit",
                column: "BenefitSettingId",
                principalTable: "BenefitSetting",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionBenefit_Position_PositionId",
                table: "PositionBenefit",
                column: "PositionId",
                principalTable: "Position",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionEducation_EducationQual_EducationQualId",
                table: "PositionEducation",
                column: "EducationQualId",
                principalTable: "EducationQual",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionEducation_Position_PositionId",
                table: "PositionEducation",
                column: "PositionId",
                principalTable: "Position",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionExp_Position_PositionId",
                table: "PositionExp",
                column: "PositionId",
                principalTable: "Position",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionReq_Position_PositionId",
                table: "PositionReq",
                column: "PositionId",
                principalTable: "Position",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JgStep_JobGrade_JobGradeId",
                table: "JgStep");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionBenefit_BenefitSetting_BenefitSettingId",
                table: "PositionBenefit");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionBenefit_Position_PositionId",
                table: "PositionBenefit");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionEducation_EducationQual_EducationQualId",
                table: "PositionEducation");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionEducation_Position_PositionId",
                table: "PositionEducation");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionExp_Position_PositionId",
                table: "PositionExp");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionReq_Position_PositionId",
                table: "PositionReq");

            migrationBuilder.DropIndex(
                name: "IX_PositionReq_IsDeleted",
                table: "PositionReq");

            migrationBuilder.DropIndex(
                name: "IX_PositionExp_IsDeleted",
                table: "PositionExp");

            migrationBuilder.DropIndex(
                name: "IX_PositionEducation_IsDeleted",
                table: "PositionEducation");

            migrationBuilder.DropIndex(
                name: "IX_PositionEducation_PositionId_EducationQualId",
                table: "PositionEducation");

            migrationBuilder.DropIndex(
                name: "IX_PositionBenefit_IsDeleted",
                table: "PositionBenefit");

            migrationBuilder.DropIndex(
                name: "IX_PositionBenefit_PositionId_BenefitSettingId",
                table: "PositionBenefit");

            migrationBuilder.DropIndex(
                name: "IX_Position_DepartmentId_IsVacant",
                table: "Position");

            migrationBuilder.DropIndex(
                name: "IX_Position_IsDeleted",
                table: "Position");

            migrationBuilder.DropIndex(
                name: "IX_JobGrade_IsDeleted",
                table: "JobGrade");

            migrationBuilder.DropIndex(
                name: "IX_JobGrade_Name",
                table: "JobGrade");

            migrationBuilder.DropIndex(
                name: "IX_JgStep_IsDeleted",
                table: "JgStep");

            migrationBuilder.DropIndex(
                name: "IX_JgStep_Name",
                table: "JgStep");

            migrationBuilder.DropIndex(
                name: "IX_EducationQual_IsDeleted",
                table: "EducationQual");

            migrationBuilder.DropIndex(
                name: "IX_EducationQual_Name",
                table: "EducationQual");

            migrationBuilder.DropIndex(
                name: "IX_BenefitSetting_IsDeleted",
                table: "BenefitSetting");

            migrationBuilder.DropIndex(
                name: "IX_BenefitSetting_Name",
                table: "BenefitSetting");

            migrationBuilder.DropIndex(
                name: "IX_BenefitSetting_Per",
                table: "BenefitSetting");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "PositionReq");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "PositionExp");

            migrationBuilder.DropColumn(
                name: "EducationLevel",
                table: "PositionEducation");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "PositionEducation");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "PositionBenefit");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Position");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "JobGrade");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "JgStep");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "EducationQual");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "BenefitSetting");

            migrationBuilder.AlterColumn<string>(
                name: "SundayWorkOption",
                table: "PositionReq",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "SaturdayWorkOption",
                table: "PositionReq",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "ProfessionType",
                table: "PositionReq",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "PositionReq",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "PositionReq",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "PositionReq",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "PositionExp",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "PositionExp",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "PositionEducation",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "EducationLevelId",
                table: "PositionEducation",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "PositionEducation",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "PositionBenefit",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "PositionBenefit",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<string>(
                name: "NameAm",
                table: "Position",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Position",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "IsVacant",
                table: "Position",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Position",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Position",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "JobGrade",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "JobGrade",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "JobGrade",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "JgStep",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "JgStep",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "JgStep",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EducationQual",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EducationQual",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "EducationQual",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<string>(
                name: "Per",
                table: "BenefitSetting",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BenefitSetting",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "BenefitSetting",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "BenefitSetting",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateIndex(
                name: "IX_PositionEducation_PositionId",
                table: "PositionEducation",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionBenefit_PositionId",
                table: "PositionBenefit",
                column: "PositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_JgStep_JobGrade_JobGradeId",
                table: "JgStep",
                column: "JobGradeId",
                principalTable: "JobGrade",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionBenefit_BenefitSetting_BenefitSettingId",
                table: "PositionBenefit",
                column: "BenefitSettingId",
                principalTable: "BenefitSetting",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionBenefit_Position_PositionId",
                table: "PositionBenefit",
                column: "PositionId",
                principalTable: "Position",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionEducation_EducationQual_EducationQualId",
                table: "PositionEducation",
                column: "EducationQualId",
                principalTable: "EducationQual",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionEducation_Position_PositionId",
                table: "PositionEducation",
                column: "PositionId",
                principalTable: "Position",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionExp_Position_PositionId",
                table: "PositionExp",
                column: "PositionId",
                principalTable: "Position",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionReq_Position_PositionId",
                table: "PositionReq",
                column: "PositionId",
                principalTable: "Position",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
