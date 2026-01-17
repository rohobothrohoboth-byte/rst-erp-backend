using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Leave.Utility.Migrations
{
    /// <inheritdoc />
    public partial class MigHrmLeave04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeavePolicyConfig_LeaveAppChain_LeaveAppChainId",
                table: "LeavePolicyConfig");

            migrationBuilder.DropIndex(
                name: "IX_LeavePolicyConfig_FiscalYearId_LeavePolicyId_LeaveAppChainId",
                table: "LeavePolicyConfig");

            migrationBuilder.DropIndex(
                name: "IX_LeavePolicyConfig_LeaveAppChainId",
                table: "LeavePolicyConfig");

            migrationBuilder.DropColumn(
                name: "LeaveAppChainId",
                table: "LeavePolicyConfig");

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicyConfig_FiscalYearId_LeavePolicyId",
                table: "LeavePolicyConfig",
                columns: new[] { "FiscalYearId", "LeavePolicyId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LeavePolicyConfig_FiscalYearId_LeavePolicyId",
                table: "LeavePolicyConfig");

            migrationBuilder.AddColumn<Guid>(
                name: "LeaveAppChainId",
                table: "LeavePolicyConfig",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicyConfig_FiscalYearId_LeavePolicyId_LeaveAppChainId",
                table: "LeavePolicyConfig",
                columns: new[] { "FiscalYearId", "LeavePolicyId", "LeaveAppChainId" });

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicyConfig_LeaveAppChainId",
                table: "LeavePolicyConfig",
                column: "LeaveAppChainId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeavePolicyConfig_LeaveAppChain_LeaveAppChainId",
                table: "LeavePolicyConfig",
                column: "LeaveAppChainId",
                principalTable: "LeaveAppChain",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
