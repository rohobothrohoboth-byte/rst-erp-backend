using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recruit.Utility.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkforcePlanPlanDevBudgetId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PlanDevBudgetId",
                table: "WorkforcePlan",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlanDevBudgetId",
                table: "WorkforcePlan");
        }
    }
}
