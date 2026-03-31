using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recruit.Utility.Migrations
{
    /// <inheritdoc />
    public partial class MigHrmRecruit01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobOffer_OfferNumber",
                table: "JobOffer");

            migrationBuilder.AlterColumn<string>(
                name: "ReqNumber",
                table: "JobRequisition",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                defaultValueSql: "generate_code('JR'::text, EXTRACT(YEAR FROM CURRENT_DATE)::int)",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "OfferNumber",
                table: "JobOffer",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                defaultValueSql: "generate_code('JO'::text, EXTRACT(YEAR FROM CURRENT_DATE)::int)",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_JobOffer_OfferNumber",
                table: "JobOffer",
                column: "OfferNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobOffer_OfferNumber",
                table: "JobOffer");

            migrationBuilder.AlterColumn<string>(
                name: "ReqNumber",
                table: "JobRequisition",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(15)",
                oldMaxLength: 15,
                oldDefaultValueSql: "generate_code('JR'::text, EXTRACT(YEAR FROM CURRENT_DATE)::int)");

            migrationBuilder.AlterColumn<string>(
                name: "OfferNumber",
                table: "JobOffer",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(15)",
                oldMaxLength: 15,
                oldDefaultValueSql: "generate_code('JO'::text, EXTRACT(YEAR FROM CURRENT_DATE)::int)");

            migrationBuilder.CreateIndex(
                name: "IX_JobOffer_OfferNumber",
                table: "JobOffer",
                column: "OfferNumber",
                unique: true);
        }
    }
}
