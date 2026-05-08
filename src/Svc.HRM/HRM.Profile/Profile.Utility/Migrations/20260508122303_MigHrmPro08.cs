using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Profile.Utility.Migrations
{
    /// <inheritdoc />
    public partial class MigHrmPro08 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmpCert",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    CertType = table.Column<string>(type: "text", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpCert", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpCert_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmpCertBirth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<byte[]>(type: "bytea", nullable: false),
                    EmpCertId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpCertBirth", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpCertBirth_EmpCert_EmpCertId",
                        column: x => x.EmpCertId,
                        principalTable: "EmpCert",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmpCertMarriage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<byte[]>(type: "bytea", nullable: false),
                    EmpCertId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpCertMarriage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpCertMarriage_EmpCert_EmpCertId",
                        column: x => x.EmpCertId,
                        principalTable: "EmpCert",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmpCert_CertType",
                table: "EmpCert",
                column: "CertType");

            migrationBuilder.CreateIndex(
                name: "IX_EmpCert_ContentType",
                table: "EmpCert",
                column: "ContentType");

            migrationBuilder.CreateIndex(
                name: "IX_EmpCert_EmployeeId",
                table: "EmpCert",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpCert_FileName",
                table: "EmpCert",
                column: "FileName");

            migrationBuilder.CreateIndex(
                name: "IX_EmpCert_IsDeleted",
                table: "EmpCert",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpCertBirth_EmpCertId",
                table: "EmpCertBirth",
                column: "EmpCertId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpCertBirth_IsDeleted",
                table: "EmpCertBirth",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpCertMarriage_EmpCertId",
                table: "EmpCertMarriage",
                column: "EmpCertId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpCertMarriage_IsDeleted",
                table: "EmpCertMarriage",
                column: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmpCertBirth");

            migrationBuilder.DropTable(
                name: "EmpCertMarriage");

            migrationBuilder.DropTable(
                name: "EmpCert");
        }
    }
}
