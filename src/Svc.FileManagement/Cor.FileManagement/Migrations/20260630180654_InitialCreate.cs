using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.FileManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileFolders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FolderType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    IsShared = table.Column<bool>(type: "boolean", nullable: false),
                    SharingLevel = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RowVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileFolders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileFolders_FileFolders_ParentId",
                        column: x => x.ParentId,
                        principalTable: "FileFolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FileDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    OriginalFileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    FileType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FileExtension = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ThumbnailPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    StorageProvider = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Module = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uuid", nullable: true),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DocumentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    IsShared = table.Column<bool>(type: "boolean", nullable: false),
                    SharingLevel = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ArchivedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    FolderId = table.Column<Guid>(type: "uuid", nullable: true),
                    UploadedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UploadedByName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RowVersion = table.Column<long>(type: "bigint", nullable: false),
                    Hash = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileDocuments_FileDocuments_ParentId",
                        column: x => x.ParentId,
                        principalTable: "FileDocuments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FileDocuments_FileFolders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "FileFolders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FileAccessLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: true),
                    FolderId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ActionType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IPAddress = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    AccessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RowVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAccessLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileAccessLogs_FileDocuments_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "FileDocuments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FileAccessLogs_FileFolders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "FileFolders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FileShares",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: true),
                    FolderId = table.Column<Guid>(type: "uuid", nullable: true),
                    SharedWithId = table.Column<Guid>(type: "uuid", nullable: false),
                    SharedWithType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Permission = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CanDownload = table.Column<bool>(type: "boolean", nullable: false),
                    CanDelete = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SharedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    SharedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RowVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileShares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileShares_FileDocuments_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "FileDocuments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FileShares_FileFolders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "FileFolders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileAccessLogs_AccessedAt",
                table: "FileAccessLogs",
                column: "AccessedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FileAccessLogs_Action",
                table: "FileAccessLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_FileAccessLogs_DocumentId",
                table: "FileAccessLogs",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAccessLogs_FolderId",
                table: "FileAccessLogs",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAccessLogs_UserId",
                table: "FileAccessLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FileDocuments_Category",
                table: "FileDocuments",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_FileDocuments_FolderId",
                table: "FileDocuments",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_FileDocuments_IsArchived",
                table: "FileDocuments",
                column: "IsArchived");

            migrationBuilder.CreateIndex(
                name: "IX_FileDocuments_IsDeleted",
                table: "FileDocuments",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_FileDocuments_Module",
                table: "FileDocuments",
                column: "Module");

            migrationBuilder.CreateIndex(
                name: "IX_FileDocuments_Module_ReferenceId_Category",
                table: "FileDocuments",
                columns: new[] { "Module", "ReferenceId", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_FileDocuments_ParentId",
                table: "FileDocuments",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_FileDocuments_ReferenceId",
                table: "FileDocuments",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_FileDocuments_UploadedBy",
                table: "FileDocuments",
                column: "UploadedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FileFolders_FolderType",
                table: "FileFolders",
                column: "FolderType");

            migrationBuilder.CreateIndex(
                name: "IX_FileFolders_IsArchived",
                table: "FileFolders",
                column: "IsArchived");

            migrationBuilder.CreateIndex(
                name: "IX_FileFolders_IsDeleted",
                table: "FileFolders",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_FileFolders_OwnerId",
                table: "FileFolders",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_FileFolders_ParentId",
                table: "FileFolders",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_FileShares_DocumentId",
                table: "FileShares",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_FileShares_ExpiresAt",
                table: "FileShares",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_FileShares_FolderId",
                table: "FileShares",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_FileShares_IsActive",
                table: "FileShares",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_FileShares_SharedWithId",
                table: "FileShares",
                column: "SharedWithId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileAccessLogs");

            migrationBuilder.DropTable(
                name: "FileShares");

            migrationBuilder.DropTable(
                name: "FileDocuments");

            migrationBuilder.DropTable(
                name: "FileFolders");
        }
    }
}
