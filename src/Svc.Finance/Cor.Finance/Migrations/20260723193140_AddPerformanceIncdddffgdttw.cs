using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Finance.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIncdddffgdttw : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerKey",
                table: "Customers",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerKey",
                table: "Customers");
        }
    }
}
