using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.CRM.Migrations
{
    /// <inheritdoc />
    public partial class AddContractEntitiesjhhjhddsdsdsddsddsdssdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Commissions_LocalEmployees_AgentId",
                table: "Commissions");

            migrationBuilder.DropForeignKey(
                name: "FK_SMSCampaign_Campaigns_CampaignId",
                table: "SMSCampaign");

            migrationBuilder.DropForeignKey(
                name: "FK_SocialMediaPosts_Campaigns_CampaignId",
                table: "SocialMediaPosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OpportunityProducts",
                table: "OpportunityProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SMSCampaign",
                table: "SMSCampaign");

            migrationBuilder.RenameTable(
                name: "SMSCampaign",
                newName: "SMSCampaigns");

            migrationBuilder.RenameIndex(
                name: "IX_Commissions_TransactionId",
                table: "Commissions",
                newName: "IX_Commission_TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_Commissions_AgentId",
                table: "Commissions",
                newName: "IX_Commission_AgentId");

            migrationBuilder.RenameIndex(
                name: "IX_SMSCampaign_CampaignId",
                table: "SMSCampaigns",
                newName: "IX_SMSCampaigns_CampaignId");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "TeamMembers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "TeamMembers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "TeamMembers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "TeamMembers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TeamMembers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "TeamMembers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "TeamMembers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "TeamMembers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "OpportunityProducts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OpportunityProducts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "OpportunityProducts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "OpportunityProducts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "OpportunityProducts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "OpportunityProducts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "OpportunityProducts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "OpportunityProducts",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "SyncedAt",
                table: "LocalEmployees",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "LocalEmployees",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "LocalEmployees",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "LocalEmployees",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "LocalEmployees",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "LocalEmployees",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Commissions",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "FromNumber",
                table: "SMSCampaigns",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OpportunityProducts",
                table: "OpportunityProducts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SMSCampaigns",
                table: "SMSCampaigns",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "EmailCampaigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Subject = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    HtmlContent = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecipientCount = table.Column<int>(type: "integer", nullable: false),
                    SentCount = table.Column<int>(type: "integer", nullable: false),
                    DeliveredCount = table.Column<int>(type: "integer", nullable: false),
                    OpenCount = table.Column<int>(type: "integer", nullable: false),
                    ClickCount = table.Column<int>(type: "integer", nullable: false),
                    BounceCount = table.Column<int>(type: "integer", nullable: false),
                    UnsubscribeCount = table.Column<int>(type: "integer", nullable: false),
                    OpenRate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    ClickRate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    RecipientListJson = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: true),
                    AnalyticsJson = table.Column<string>(type: "jsonb", nullable: true),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailCampaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailCampaigns_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EmailCampaigns_EmailTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "EmailTemplates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_TeamId",
                table: "TeamMembers",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialMediaPost_Platform",
                table: "SocialMediaPosts",
                column: "Platform");

            migrationBuilder.CreateIndex(
                name: "IX_SocialMediaPost_ScheduledDate",
                table: "SocialMediaPosts",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_SocialMediaPost_Status",
                table: "SocialMediaPosts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityProducts_OpportunityId",
                table: "OpportunityProducts",
                column: "OpportunityId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalEmployee_AppUserId",
                table: "LocalEmployees",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalEmployee_Code",
                table: "LocalEmployees",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_LocalEmployee_Email",
                table: "LocalEmployees",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Commission_Status",
                table: "Commissions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SMSCampaign_ScheduledDate",
                table: "SMSCampaigns",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_SMSCampaign_Status",
                table: "SMSCampaigns",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EmailCampaign_ScheduledDate",
                table: "EmailCampaigns",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_EmailCampaign_Status",
                table: "EmailCampaigns",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EmailCampaigns_CampaignId",
                table: "EmailCampaigns",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailCampaigns_TemplateId",
                table: "EmailCampaigns",
                column: "TemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Commissions_LocalEmployees_AgentId",
                table: "Commissions",
                column: "AgentId",
                principalTable: "LocalEmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SMSCampaigns_Campaigns_CampaignId",
                table: "SMSCampaigns",
                column: "CampaignId",
                principalTable: "Campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SocialMediaPosts_Campaigns_CampaignId",
                table: "SocialMediaPosts",
                column: "CampaignId",
                principalTable: "Campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Commissions_LocalEmployees_AgentId",
                table: "Commissions");

            migrationBuilder.DropForeignKey(
                name: "FK_SMSCampaigns_Campaigns_CampaignId",
                table: "SMSCampaigns");

            migrationBuilder.DropForeignKey(
                name: "FK_SocialMediaPosts_Campaigns_CampaignId",
                table: "SocialMediaPosts");

            migrationBuilder.DropTable(
                name: "EmailCampaigns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers");

            migrationBuilder.DropIndex(
                name: "IX_TeamMembers_TeamId",
                table: "TeamMembers");

            migrationBuilder.DropIndex(
                name: "IX_SocialMediaPost_Platform",
                table: "SocialMediaPosts");

            migrationBuilder.DropIndex(
                name: "IX_SocialMediaPost_ScheduledDate",
                table: "SocialMediaPosts");

            migrationBuilder.DropIndex(
                name: "IX_SocialMediaPost_Status",
                table: "SocialMediaPosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OpportunityProducts",
                table: "OpportunityProducts");

            migrationBuilder.DropIndex(
                name: "IX_OpportunityProducts_OpportunityId",
                table: "OpportunityProducts");

            migrationBuilder.DropIndex(
                name: "IX_LocalEmployee_AppUserId",
                table: "LocalEmployees");

            migrationBuilder.DropIndex(
                name: "IX_LocalEmployee_Code",
                table: "LocalEmployees");

            migrationBuilder.DropIndex(
                name: "IX_LocalEmployee_Email",
                table: "LocalEmployees");

            migrationBuilder.DropIndex(
                name: "IX_Commission_Status",
                table: "Commissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SMSCampaigns",
                table: "SMSCampaigns");

            migrationBuilder.DropIndex(
                name: "IX_SMSCampaign_ScheduledDate",
                table: "SMSCampaigns");

            migrationBuilder.DropIndex(
                name: "IX_SMSCampaign_Status",
                table: "SMSCampaigns");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "OpportunityProducts");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OpportunityProducts");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "OpportunityProducts");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "OpportunityProducts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "OpportunityProducts");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OpportunityProducts");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "OpportunityProducts");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "OpportunityProducts");

            migrationBuilder.RenameTable(
                name: "SMSCampaigns",
                newName: "SMSCampaign");

            migrationBuilder.RenameIndex(
                name: "IX_Commission_TransactionId",
                table: "Commissions",
                newName: "IX_Commissions_TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_Commission_AgentId",
                table: "Commissions",
                newName: "IX_Commissions_AgentId");

            migrationBuilder.RenameIndex(
                name: "IX_SMSCampaigns_CampaignId",
                table: "SMSCampaign",
                newName: "IX_SMSCampaign_CampaignId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SyncedAt",
                table: "LocalEmployees",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "LocalEmployees",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "LocalEmployees",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "LocalEmployees",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "LocalEmployees",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "LocalEmployees",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Commissions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "FromNumber",
                table: "SMSCampaign",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers",
                columns: new[] { "TeamId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_OpportunityProducts",
                table: "OpportunityProducts",
                columns: new[] { "OpportunityId", "ProductId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_SMSCampaign",
                table: "SMSCampaign",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Commissions_LocalEmployees_AgentId",
                table: "Commissions",
                column: "AgentId",
                principalTable: "LocalEmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SMSCampaign_Campaigns_CampaignId",
                table: "SMSCampaign",
                column: "CampaignId",
                principalTable: "Campaigns",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SocialMediaPosts_Campaigns_CampaignId",
                table: "SocialMediaPosts",
                column: "CampaignId",
                principalTable: "Campaigns",
                principalColumn: "Id");
        }
    }
}
