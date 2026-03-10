using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.HRMM.Migrations
{
    /// <inheritdoc />
    public partial class MigCorHrmm02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JgStep_JobGrade_JobGradeId",
                table: "JgStep");

            migrationBuilder.DropIndex(
                name: "IX_PositionEducation_PositionId_EducationQualId",
                table: "PositionEducation");

            migrationBuilder.DropIndex(
                name: "IX_PositionBenefit_PositionId_BenefitSettingId",
                table: "PositionBenefit");

            migrationBuilder.DropIndex(
                name: "IX_Position_DepartmentId_IsVacant",
                table: "Position");

            migrationBuilder.DropIndex(
                name: "IX_JgStep_Name",
                table: "JgStep");

            migrationBuilder.AlterColumn<string>(
                name: "Per",
                table: "BenefitSetting",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.CreateIndex(
                name: "IX_PositionEducation_PositionId",
                table: "PositionEducation",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionEducation_PositionId_EducationQualId",
                table: "PositionEducation",
                columns: new[] { "PositionId", "EducationQualId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PositionBenefit_PositionId",
                table: "PositionBenefit",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionBenefit_PositionId_BenefitSettingId",
                table: "PositionBenefit",
                columns: new[] { "PositionId", "BenefitSettingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Position_DepartmentId",
                table: "Position",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Position_DepartmentId_Name",
                table: "Position",
                columns: new[] { "DepartmentId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Position_Name",
                table: "Position",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_JgStep_JobGradeId_Name",
                table: "JgStep",
                columns: new[] { "JobGradeId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JgStep_JobGrade_JobGradeId",
                table: "JgStep",
                column: "JobGradeId",
                principalTable: "JobGrade",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JgStep_JobGrade_JobGradeId",
                table: "JgStep");

            migrationBuilder.DropIndex(
                name: "IX_PositionEducation_PositionId",
                table: "PositionEducation");

            migrationBuilder.DropIndex(
                name: "IX_PositionEducation_PositionId_EducationQualId",
                table: "PositionEducation");

            migrationBuilder.DropIndex(
                name: "IX_PositionBenefit_PositionId",
                table: "PositionBenefit");

            migrationBuilder.DropIndex(
                name: "IX_PositionBenefit_PositionId_BenefitSettingId",
                table: "PositionBenefit");

            migrationBuilder.DropIndex(
                name: "IX_Position_DepartmentId",
                table: "Position");

            migrationBuilder.DropIndex(
                name: "IX_Position_DepartmentId_Name",
                table: "Position");

            migrationBuilder.DropIndex(
                name: "IX_Position_Name",
                table: "Position");

            migrationBuilder.DropIndex(
                name: "IX_JgStep_JobGradeId_Name",
                table: "JgStep");

            migrationBuilder.AlterColumn<string>(
                name: "Per",
                table: "BenefitSetting",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_PositionEducation_PositionId_EducationQualId",
                table: "PositionEducation",
                columns: new[] { "PositionId", "EducationQualId" });

            migrationBuilder.CreateIndex(
                name: "IX_PositionBenefit_PositionId_BenefitSettingId",
                table: "PositionBenefit",
                columns: new[] { "PositionId", "BenefitSettingId" });

            migrationBuilder.CreateIndex(
                name: "IX_Position_DepartmentId_IsVacant",
                table: "Position",
                columns: new[] { "DepartmentId", "IsVacant" });

            migrationBuilder.CreateIndex(
                name: "IX_JgStep_Name",
                table: "JgStep",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JgStep_JobGrade_JobGradeId",
                table: "JgStep",
                column: "JobGradeId",
                principalTable: "JobGrade",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
