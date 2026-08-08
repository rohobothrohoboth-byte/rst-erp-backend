using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Finance.Migrations
{
    /// <inheritdoc />
    public partial class AddVoucherAndConsolidationdsd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Vendors",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Rating",
                table: "Vendors",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationNumber",
                table: "Vendors",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalSpent",
                table: "Vendors",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalTransactions",
                table: "Vendors",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Vendors",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "RegistrationNumber",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "TotalSpent",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "TotalTransactions",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Vendors");
        }
    }
}
