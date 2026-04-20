using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recruit.Utility.Migrations
{
    /// <inheritdoc />
    public partial class MigHrmRecruit06 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobAppEvalProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    JobAppId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentStepId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobAppEvalProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobAppEvalProgress_EvaluationStep_CurrentStepId",
                        column: x => x.CurrentStepId,
                        principalTable: "EvaluationStep",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobAppEvalProgress_JobApplication_JobAppId",
                        column: x => x.JobAppId,
                        principalTable: "JobApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobAppEvalProgress_CurrentStepId",
                table: "JobAppEvalProgress",
                column: "CurrentStepId");

            migrationBuilder.CreateIndex(
                name: "IX_JobAppEvalProgress_IsCompleted",
                table: "JobAppEvalProgress",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobAppEvalProgress_IsDeleted",
                table: "JobAppEvalProgress",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobAppEvalProgress_JobAppId",
                table: "JobAppEvalProgress",
                column: "JobAppId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobAppEvalProgress");
        }
    }
}
