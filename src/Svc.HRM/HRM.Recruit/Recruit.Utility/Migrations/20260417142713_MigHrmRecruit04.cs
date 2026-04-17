using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recruit.Utility.Migrations
{
    /// <inheritdoc />
    public partial class MigHrmRecruit04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EvaluationScore_EvaluationType_EvalTypeId",
                table: "EvaluationScore");

            migrationBuilder.RenameColumn(
                name: "EvalTypeId",
                table: "EvaluationScore",
                newName: "JobAppId");

            migrationBuilder.RenameIndex(
                name: "IX_EvaluationScore_EvalTypeId_EvaluationStepId",
                table: "EvaluationScore",
                newName: "IX_EvaluationScore_JobAppId_EvaluationStepId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationScore_JobAppId",
                table: "EvaluationScore",
                column: "JobAppId");

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluationScore_JobApplication_JobAppId",
                table: "EvaluationScore",
                column: "JobAppId",
                principalTable: "JobApplication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EvaluationScore_JobApplication_JobAppId",
                table: "EvaluationScore");

            migrationBuilder.DropIndex(
                name: "IX_EvaluationScore_JobAppId",
                table: "EvaluationScore");

            migrationBuilder.RenameColumn(
                name: "JobAppId",
                table: "EvaluationScore",
                newName: "EvalTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_EvaluationScore_JobAppId_EvaluationStepId",
                table: "EvaluationScore",
                newName: "IX_EvaluationScore_EvalTypeId_EvaluationStepId");

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluationScore_EvaluationType_EvalTypeId",
                table: "EvaluationScore",
                column: "EvalTypeId",
                principalTable: "EvaluationType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
