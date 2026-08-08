using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.HRMM.Migrations
{
    /// <inheritdoc />
    public partial class AddVoucherAnd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiKeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ApiKeyValue = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ClientType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "External"),
                    BaseUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AllowedEndpoints = table.Column<string>(type: "jsonb", nullable: false, defaultValue: "[]"),
                    RateLimitPerMinute = table.Column<int>(type: "integer", nullable: false, defaultValue: 60),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastUsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RequestCount = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiKeyLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApiKeyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Endpoint = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Method = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Success = table.Column<bool>(type: "boolean", nullable: false),
                    StatusCode = table.Column<int>(type: "integer", nullable: true),
                    ClientIp = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeyLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiKeyLogs_ApiKeys_ApiKeyId",
                        column: x => x.ApiKeyId,
                        principalTable: "ApiKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeyLogs_ApiKeyId",
                table: "ApiKeyLogs",
                column: "ApiKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeyLogs_Success",
                table: "ApiKeyLogs",
                column: "Success");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeyLogs_Timestamp",
                table: "ApiKeyLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_ApiKey",
                table: "ApiKeys",
                column: "ApiKeyValue",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_CreatedAt",
                table: "ApiKeys",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_ExpiresAt",
                table: "ApiKeys",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_IsActive",
                table: "ApiKeys",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiKeyLogs");

            migrationBuilder.DropTable(
                name: "ApiKeys");
        }
    }
}
