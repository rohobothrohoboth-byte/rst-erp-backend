using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.Finance.Migrations
{
    public partial class AddVoucherAndConsolidationdsdjhh : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "LocalEmployees"
                ALTER COLUMN "AppUserId"
                TYPE uuid
                USING NULLIF("AppUserId", '')::uuid;
            """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "LocalEmployees"
                ALTER COLUMN "AppUserId"
                TYPE text
                USING "AppUserId"::text;
            """);
        }
    }
}