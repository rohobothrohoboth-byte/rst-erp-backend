using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.HRMM.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PositionReq_PositionId",
                table: "PositionReq");

            migrationBuilder.DropIndex(
                name: "IX_PositionExp_PositionId",
                table: "PositionExp");

            migrationBuilder.DropIndex(
                name: "IX_PositionEducation_PositionId_EducationQualId",
                table: "PositionEducation");

            migrationBuilder.DropIndex(
                name: "IX_PositionBenefit_PositionId_BenefitSettingId",
                table: "PositionBenefit");

            migrationBuilder.DropIndex(
                name: "IX_Position_DepartmentId_Name",
                table: "Position");

            migrationBuilder.DropIndex(
                name: "IX_JobGrade_Name",
                table: "JobGrade");

            migrationBuilder.DropIndex(
                name: "IX_JgStep_JobGradeId_Name",
                table: "JgStep");

            migrationBuilder.DropIndex(
                name: "IX_EducationQual_Name",
                table: "EducationQual");

            migrationBuilder.DropIndex(
                name: "IX_BenefitSetting_Name",
                table: "BenefitSetting");

            migrationBuilder.AddColumn<Guid>(
                name: "JobGradeId",
                table: "Position",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PositionReq_PositionId",
                table: "PositionReq",
                column: "PositionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PositionExp_PositionId",
                table: "PositionExp",
                column: "PositionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PositionEducation_PositionId_EducationQualId",
                table: "PositionEducation",
                columns: new[] { "PositionId", "EducationQualId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PositionBenefit_PositionId_BenefitSettingId",
                table: "PositionBenefit",
                columns: new[] { "PositionId", "BenefitSettingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Position_DepartmentId_Name",
                table: "Position",
                columns: new[] { "DepartmentId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Position_JobGradeId",
                table: "Position",
                column: "JobGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_JobGrade_Name",
                table: "JobGrade",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JgStep_JobGradeId_Name",
                table: "JgStep",
                columns: new[] { "JobGradeId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EducationQual_Name",
                table: "EducationQual",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BenefitSetting_Name",
                table: "BenefitSetting",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Position_JobGrade_JobGradeId",
                table: "Position",
                column: "JobGradeId",
                principalTable: "JobGrade",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Position_JobGrade_JobGradeId",
                table: "Position");

            migrationBuilder.DropIndex(
                name: "IX_PositionReq_PositionId",
                table: "PositionReq");

            migrationBuilder.DropIndex(
                name: "IX_PositionExp_PositionId",
                table: "PositionExp");

            migrationBuilder.DropIndex(
                name: "IX_PositionEducation_PositionId_EducationQualId",
                table: "PositionEducation");

            migrationBuilder.DropIndex(
                name: "IX_PositionBenefit_PositionId_BenefitSettingId",
                table: "PositionBenefit");

            migrationBuilder.DropIndex(
                name: "IX_Position_DepartmentId_Name",
                table: "Position");

            migrationBuilder.DropIndex(
                name: "IX_Position_JobGradeId",
                table: "Position");

            migrationBuilder.DropIndex(
                name: "IX_JobGrade_Name",
                table: "JobGrade");

            migrationBuilder.DropIndex(
                name: "IX_JgStep_JobGradeId_Name",
                table: "JgStep");

            migrationBuilder.DropIndex(
                name: "IX_EducationQual_Name",
                table: "EducationQual");

            migrationBuilder.DropIndex(
                name: "IX_BenefitSetting_Name",
                table: "BenefitSetting");

            migrationBuilder.DropColumn(
                name: "JobGradeId",
                table: "Position");

            migrationBuilder.CreateIndex(
                name: "IX_PositionReq_PositionId",
                table: "PositionReq",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionExp_PositionId",
                table: "PositionExp",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionEducation_PositionId_EducationQualId",
                table: "PositionEducation",
                columns: new[] { "PositionId", "EducationQualId" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_PositionBenefit_PositionId_BenefitSettingId",
                table: "PositionBenefit",
                columns: new[] { "PositionId", "BenefitSettingId" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Position_DepartmentId_Name",
                table: "Position",
                columns: new[] { "DepartmentId", "Name" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_JobGrade_Name",
                table: "JobGrade",
                column: "Name",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_JgStep_JobGradeId_Name",
                table: "JgStep",
                columns: new[] { "JobGradeId", "Name" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_EducationQual_Name",
                table: "EducationQual",
                column: "Name",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_BenefitSetting_Name",
                table: "BenefitSetting",
                column: "Name",
                unique: true,
                filter: "\"IsDeleted\" = false");
        }
    }
}

