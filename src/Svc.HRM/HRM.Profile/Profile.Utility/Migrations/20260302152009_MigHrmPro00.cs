using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Profile.Utility.Migrations
{
    /// <inheritdoc />
    public partial class MigHrmPro00 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,");

            migrationBuilder.CreateSequence(
                name: "emp_code_seq",
                maxValue: 9999999L);

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AddressType = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Region = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Subcity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Zone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Woreda = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Kebele = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    HouseNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Telephone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    PoBox = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Fax = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Website = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileMetaData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileMetaData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FirstNameAm = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MiddleNameAm = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastNameAm = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Nationality = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmpGuarantorFileBlob",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<byte[]>(type: "bytea", nullable: false),
                    FileMetaDataId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpGuarantorFileBlob", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpGuarantorFileBlob_FileMetaData_FileMetaDataId",
                        column: x => x.FileMetaDataId,
                        principalTable: "FileMetaData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmpPhotoBlob",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<byte[]>(type: "bytea", nullable: false),
                    FileMetaDataId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpPhotoBlob", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpPhotoBlob_FileMetaData_FileMetaDataId",
                        column: x => x.FileMetaDataId,
                        principalTable: "FileMetaData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmpPhotoThumbnail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<byte[]>(type: "bytea", nullable: false),
                    FileMetaDataId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpPhotoThumbnail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpPhotoThumbnail_FileMetaData_FileMetaDataId",
                        column: x => x.FileMetaDataId,
                        principalTable: "FileMetaData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmpSignBlob",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<byte[]>(type: "bytea", nullable: false),
                    FileMetaDataId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpSignBlob", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpSignBlob_FileMetaData_FileMetaDataId",
                        column: x => x.FileMetaDataId,
                        principalTable: "FileMetaData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmpStampBlob",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<byte[]>(type: "bytea", nullable: false),
                    FileMetaDataId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpStampBlob", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpStampBlob_FileMetaData_FileMetaDataId",
                        column: x => x.FileMetaDataId,
                        principalTable: "FileMetaData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValueSql: "'EMP' || LPAD(nextval('emp_code_seq')::text, 7, '0')"),
                    EmploymentType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EmploymentNature = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    WorkArrangement = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EmploymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PersonId = table.Column<Guid>(type: "uuid", nullable: false),
                    JobGradeId = table.Column<Guid>(type: "uuid", nullable: false),
                    PositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employee_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmergencyContact",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AddressId = table.Column<Guid>(type: "uuid", nullable: false),
                    Relation = table.Column<string>(type: "text", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyContact", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmergencyContact_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmergencyContact_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmergencyContact_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmpBio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BirthLocation = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    MotherFullName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    HasBirthCert = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    HasMarriageCert = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    MaritalStatus = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    AddressId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpBio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpBio_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpBio_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmpFamily",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Relation = table.Column<string>(type: "text", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpFamily", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpFamily_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmpFamily_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmpFinance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Tin = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BankAccountNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PensionNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpFinance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpFinance_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmpGuarantor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AddressId = table.Column<Guid>(type: "uuid", nullable: false),
                    Relation = table.Column<string>(type: "text", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpGuarantor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpGuarantor_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpGuarantor_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmpGuarantor_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmpPensionCard",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsReceived = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IsSent = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpPensionCard", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpPensionCard_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmpPhoto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileMetaDataId = table.Column<Guid>(type: "uuid", nullable: false),
                    ThumbnailId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpPhoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpPhoto_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmpPhoto_FileMetaData_FileMetaDataId",
                        column: x => x.FileMetaDataId,
                        principalTable: "FileMetaData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpPhoto_FileMetaData_ThumbnailId",
                        column: x => x.ThumbnailId,
                        principalTable: "FileMetaData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmpSign",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileMetaDataId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpSign", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpSign_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmpSign_FileMetaData_FileMetaDataId",
                        column: x => x.FileMetaDataId,
                        principalTable: "FileMetaData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmpStamp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileMetaDataId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpStamp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpStamp_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmpStamp_FileMetaData_FileMetaDataId",
                        column: x => x.FileMetaDataId,
                        principalTable: "FileMetaData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmpState",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsTerminated = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IsApproved = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IsStandBy = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IsRetired = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IsUnderProbation = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpState", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpState_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmpGuarantorFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpGuarantorId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileMetaDataId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpGuarantorFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpGuarantorFile_EmpGuarantor_EmpGuarantorId",
                        column: x => x.EmpGuarantorId,
                        principalTable: "EmpGuarantor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmpGuarantorFile_FileMetaData_FileMetaDataId",
                        column: x => x.FileMetaDataId,
                        principalTable: "FileMetaData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_Country",
                table: "Address",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_Address_Email",
                table: "Address",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Address_IsDeleted",
                table: "Address",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContact_AddressId",
                table: "EmergencyContact",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContact_EmployeeId",
                table: "EmergencyContact",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContact_EmployeeId_PersonId",
                table: "EmergencyContact",
                columns: new[] { "EmployeeId", "PersonId" });

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContact_IsDeleted",
                table: "EmergencyContact",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContact_PersonId",
                table: "EmergencyContact",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpBio_AddressId",
                table: "EmpBio",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpBio_EmployeeId",
                table: "EmpBio",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpBio_IsDeleted",
                table: "EmpBio",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpFamily_EmployeeId",
                table: "EmpFamily",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpFamily_EmployeeId_PersonId",
                table: "EmpFamily",
                columns: new[] { "EmployeeId", "PersonId" });

            migrationBuilder.CreateIndex(
                name: "IX_EmpFamily_IsDeleted",
                table: "EmpFamily",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpFamily_PersonId",
                table: "EmpFamily",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpFinance_BankAccountNo",
                table: "EmpFinance",
                column: "BankAccountNo");

            migrationBuilder.CreateIndex(
                name: "IX_EmpFinance_EmployeeId",
                table: "EmpFinance",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpFinance_IsDeleted",
                table: "EmpFinance",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpFinance_Tin",
                table: "EmpFinance",
                column: "Tin");

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantor_AddressId",
                table: "EmpGuarantor",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantor_EmployeeId",
                table: "EmpGuarantor",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantor_EmployeeId_PersonId",
                table: "EmpGuarantor",
                columns: new[] { "EmployeeId", "PersonId" });

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantor_IsDeleted",
                table: "EmpGuarantor",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantor_PersonId",
                table: "EmpGuarantor",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantorFile_EmpGuarantorId",
                table: "EmpGuarantorFile",
                column: "EmpGuarantorId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantorFile_EmpGuarantorId_FileMetaDataId",
                table: "EmpGuarantorFile",
                columns: new[] { "EmpGuarantorId", "FileMetaDataId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantorFile_FileMetaDataId",
                table: "EmpGuarantorFile",
                column: "FileMetaDataId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantorFile_IsDeleted",
                table: "EmpGuarantorFile",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantorFileBlob_FileMetaDataId",
                table: "EmpGuarantorFileBlob",
                column: "FileMetaDataId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantorFileBlob_IsDeleted",
                table: "EmpGuarantorFileBlob",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Code",
                table: "Employee",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_DepartmentId",
                table: "Employee",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_IsDeleted",
                table: "Employee",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_JobGradeId",
                table: "Employee",
                column: "JobGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_PersonId",
                table: "Employee",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_PositionId",
                table: "Employee",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPensionCard_EmployeeId",
                table: "EmpPensionCard",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpPensionCard_IsDeleted",
                table: "EmpPensionCard",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPensionCard_IsReceived",
                table: "EmpPensionCard",
                column: "IsReceived");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPensionCard_IsSent",
                table: "EmpPensionCard",
                column: "IsSent");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPhoto_EmployeeId",
                table: "EmpPhoto",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpPhoto_FileMetaDataId",
                table: "EmpPhoto",
                column: "FileMetaDataId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPhoto_IsDeleted",
                table: "EmpPhoto",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPhoto_ThumbnailId",
                table: "EmpPhoto",
                column: "ThumbnailId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPhotoBlob_FileMetaDataId",
                table: "EmpPhotoBlob",
                column: "FileMetaDataId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpPhotoBlob_IsDeleted",
                table: "EmpPhotoBlob",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPhotoThumbnail_FileMetaDataId",
                table: "EmpPhotoThumbnail",
                column: "FileMetaDataId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpPhotoThumbnail_IsDeleted",
                table: "EmpPhotoThumbnail",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpSign_EmployeeId",
                table: "EmpSign",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpSign_FileMetaDataId",
                table: "EmpSign",
                column: "FileMetaDataId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpSign_IsDeleted",
                table: "EmpSign",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpSignBlob_FileMetaDataId",
                table: "EmpSignBlob",
                column: "FileMetaDataId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpSignBlob_IsDeleted",
                table: "EmpSignBlob",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpStamp_EmployeeId",
                table: "EmpStamp",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpStamp_FileMetaDataId",
                table: "EmpStamp",
                column: "FileMetaDataId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpStamp_IsDeleted",
                table: "EmpStamp",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpStampBlob_FileMetaDataId",
                table: "EmpStampBlob",
                column: "FileMetaDataId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpStampBlob_IsDeleted",
                table: "EmpStampBlob",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpState_EmployeeId",
                table: "EmpState",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpState_IsDeleted",
                table: "EmpState",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_FileMetaData_ContentType",
                table: "FileMetaData",
                column: "ContentType");

            migrationBuilder.CreateIndex(
                name: "IX_FileMetaData_FileName",
                table: "FileMetaData",
                column: "FileName");

            migrationBuilder.CreateIndex(
                name: "IX_FileMetaData_IsDeleted",
                table: "FileMetaData",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Person_Gender",
                table: "Person",
                column: "Gender");

            migrationBuilder.CreateIndex(
                name: "IX_Person_IsDeleted",
                table: "Person",
                column: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmergencyContact");

            migrationBuilder.DropTable(
                name: "EmpBio");

            migrationBuilder.DropTable(
                name: "EmpFamily");

            migrationBuilder.DropTable(
                name: "EmpFinance");

            migrationBuilder.DropTable(
                name: "EmpGuarantorFile");

            migrationBuilder.DropTable(
                name: "EmpGuarantorFileBlob");

            migrationBuilder.DropTable(
                name: "EmpPensionCard");

            migrationBuilder.DropTable(
                name: "EmpPhoto");

            migrationBuilder.DropTable(
                name: "EmpPhotoBlob");

            migrationBuilder.DropTable(
                name: "EmpPhotoThumbnail");

            migrationBuilder.DropTable(
                name: "EmpSign");

            migrationBuilder.DropTable(
                name: "EmpSignBlob");

            migrationBuilder.DropTable(
                name: "EmpStamp");

            migrationBuilder.DropTable(
                name: "EmpStampBlob");

            migrationBuilder.DropTable(
                name: "EmpState");

            migrationBuilder.DropTable(
                name: "EmpGuarantor");

            migrationBuilder.DropTable(
                name: "FileMetaData");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropSequence(
                name: "emp_code_seq");
        }
    }
}
