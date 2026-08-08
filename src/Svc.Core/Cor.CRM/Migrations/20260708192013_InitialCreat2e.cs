using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.CRM.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreat2e : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "Teams",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "Tasks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "SmsLogs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "Quotes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "QuoteLines",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "Products",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "Payments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "Opportunities",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "Notifications",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "Notes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "LeadScoreRules",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "Leads",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "LeadRoutingRules",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "Invoices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "InvoiceLines",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "Integrations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "EmailTemplates",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "EmailLogs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "Documents",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "Departments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "CustomFieldDefinitions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "Customers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "Contacts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "Campaigns",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "ActivityLogs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                table: "Activities",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "SmsLogs");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "QuoteLines");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "LeadScoreRules");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "LeadRoutingRules");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Integrations");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "EmailTemplates");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "EmailLogs");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "CustomFieldDefinitions");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "ActivityLogs");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Activities");
        }
    }
}
