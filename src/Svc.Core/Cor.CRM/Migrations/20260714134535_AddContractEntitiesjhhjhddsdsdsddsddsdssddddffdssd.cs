using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.CRM.Migrations
{
    /// <inheritdoc />
    public partial class AddContractEntitiesjhhjhddsdsdsddsddsdssddddffdssd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, handle any NULL values or invalid data
            // Then alter the column with explicit USING clause
            migrationBuilder.Sql(
                @"ALTER TABLE ""SocialMediaAccounts""
                  ALTER COLUMN ""CreatedByUserId"" TYPE uuid
                  USING ""CreatedByUserId""::uuid;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUserId",
                table: "SocialMediaAccounts",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}