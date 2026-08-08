using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Procurement.Migrations
{
    /// <inheritdoc />
    public partial class AddLocalVendors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLocalOnly",
                table: "Vendors",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLocalOnly",
                table: "Vendors");
        }
    }
}
