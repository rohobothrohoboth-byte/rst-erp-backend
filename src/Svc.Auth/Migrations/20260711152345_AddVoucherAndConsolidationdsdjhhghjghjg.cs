using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Svc.Auth.Migrations
{
    public partial class AddVoucherAndConsolidationdsdjhhghjghjg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "Employees"
                ALTER COLUMN "AppUserId"
                TYPE uuid
                USING NULLIF("AppUserId", '')::uuid;
            """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "Employees"
                ALTER COLUMN "AppUserId"
                TYPE character varying(50)
                USING "AppUserId"::text;
            """);
        }
    }
}