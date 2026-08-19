using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Profile.Utility.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Region = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Subcity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Zone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Woreda = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Kebele = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    HouseNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Telephone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    PoBox = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Fax = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Website = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
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
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NameAm = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OpenDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BranchType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BranchStat = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CompId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NameAm = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TaxId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    LogoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NameAm = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DeptStat = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
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
                name: "JgStep",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Salary = table.Column<double>(type: "numeric(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    SalaryPayFreq = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    JobGradeId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JgStep", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobGrades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StartSalary = table.Column<double>(type: "numeric(18,2)", nullable: false),
                    MaxSalary = table.Column<double>(type: "numeric(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobGrades", x => x.Id);
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
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NameAm = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NoOfPosition = table.Column<int>(type: "integer", nullable: false),
                    IsVacant = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    JobGradeId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
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
                    EmpState = table.Column<string>(type: "text", nullable: false),
                    EmploymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PersonId = table.Column<Guid>(type: "uuid", nullable: false),
                    JobGradeId = table.Column<Guid>(type: "uuid", nullable: false),
                    PositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportsToId = table.Column<Guid>(type: "uuid", nullable: true),
                    AppUserId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Nationality = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Relation = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddressId = table.Column<Guid>(type: "uuid", nullable: false),
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
                name: "EmpEducation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Institution = table.Column<string>(type: "text", nullable: false),
                    FieldOfStudy = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GPA = table.Column<double>(type: "double precision", nullable: true),
                    EduLevel = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpEducation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpEducation_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmpExperience",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Company = table.Column<string>(type: "text", nullable: false),
                    PosTitle = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Respo = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpExperience", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpExperience_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmpFamily",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Nationality = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Relation = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Nationality = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Relation = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddressId = table.Column<Guid>(type: "uuid", nullable: false),
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeContract",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContractType = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Salary = table.Column<decimal>(type: "numeric", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeContract", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeContract_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeePromotion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromPosition = table.Column<string>(type: "text", nullable: true),
                    ToPosition = table.Column<string>(type: "text", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ApprovedBy = table.Column<string>(type: "text", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeePromotion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeePromotion_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeTransfer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromBranch = table.Column<string>(type: "text", nullable: true),
                    ToBranch = table.Column<string>(type: "text", nullable: true),
                    FromDepartment = table.Column<string>(type: "text", nullable: true),
                    ToDepartment = table.Column<string>(type: "text", nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeTransfer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeTransfer_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
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
                name: "EmpSalary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BaseSalary = table.Column<double>(type: "double precision", nullable: false),
                    Currency = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    SalaryPayFreq = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    JgStepId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpSalary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpSalary_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_Branches_Code",
                table: "Branches",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branches_CompId",
                table: "Branches",
                column: "CompId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_IsDeleted",
                table: "Branches",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_IsDeleted",
                table: "Companies",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_BranchId",
                table: "Departments",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_IsDeleted",
                table: "Departments",
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
                name: "IX_EmergencyContact_Id_AddressId",
                table: "EmergencyContact",
                columns: new[] { "Id", "AddressId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContact_Id_EmployeeId",
                table: "EmergencyContact",
                columns: new[] { "Id", "EmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContact_IsDeleted",
                table: "EmergencyContact",
                column: "IsDeleted");

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

            migrationBuilder.CreateIndex(
                name: "IX_EmpEducation_EmployeeId",
                table: "EmpEducation",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpExperience_EmployeeId",
                table: "EmpExperience",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpFamily_EmployeeId",
                table: "EmpFamily",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpFamily_IsDeleted",
                table: "EmpFamily",
                column: "IsDeleted");

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
                name: "IX_EmpGuarantor_Id_AddressId",
                table: "EmpGuarantor",
                columns: new[] { "Id", "AddressId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantor_Id_EmployeeId",
                table: "EmpGuarantor",
                columns: new[] { "Id", "EmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantor_IsDeleted",
                table: "EmpGuarantor",
                column: "IsDeleted");

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
                name: "IX_EmployeeContract_EmployeeId",
                table: "EmployeeContract",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePromotion_EmployeeId",
                table: "EmployeePromotion",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTransfer_EmployeeId",
                table: "EmployeeTransfer",
                column: "EmployeeId");

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
                name: "IX_EmpSalary_EffectiveFrom",
                table: "EmpSalary",
                column: "EffectiveFrom");

            migrationBuilder.CreateIndex(
                name: "IX_EmpSalary_EmployeeId",
                table: "EmpSalary",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpSalary_IsDeleted",
                table: "EmpSalary",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpSalary_JgStepId",
                table: "EmpSalary",
                column: "JgStepId");

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
                name: "IX_JgSteps_IsDeleted",
                table: "JgStep",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JgSteps_JobGradeId",
                table: "JgStep",
                column: "JobGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_JobGrades_IsDeleted",
                table: "JobGrades",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Person_Gender",
                table: "Person",
                column: "Gender");

            migrationBuilder.CreateIndex(
                name: "IX_Person_IsDeleted",
                table: "Person",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_DepartmentId",
                table: "Positions",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_IsDeleted",
                table: "Positions",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_JobGradeId",
                table: "Positions",
                column: "JobGradeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "EmergencyContact");

            migrationBuilder.DropTable(
                name: "EmpBio");

            migrationBuilder.DropTable(
                name: "EmpCertBirth");

            migrationBuilder.DropTable(
                name: "EmpCertMarriage");

            migrationBuilder.DropTable(
                name: "EmpEducation");

            migrationBuilder.DropTable(
                name: "EmpExperience");

            migrationBuilder.DropTable(
                name: "EmpFamily");

            migrationBuilder.DropTable(
                name: "EmpFinance");

            migrationBuilder.DropTable(
                name: "EmpGuarantorFile");

            migrationBuilder.DropTable(
                name: "EmpGuarantorFileBlob");

            migrationBuilder.DropTable(
                name: "EmployeeContract");

            migrationBuilder.DropTable(
                name: "EmployeePromotion");

            migrationBuilder.DropTable(
                name: "EmployeeTransfer");

            migrationBuilder.DropTable(
                name: "EmpPensionCard");

            migrationBuilder.DropTable(
                name: "EmpPhoto");

            migrationBuilder.DropTable(
                name: "EmpPhotoBlob");

            migrationBuilder.DropTable(
                name: "EmpPhotoThumbnail");

            migrationBuilder.DropTable(
                name: "EmpSalary");

            migrationBuilder.DropTable(
                name: "EmpSign");

            migrationBuilder.DropTable(
                name: "EmpSignBlob");

            migrationBuilder.DropTable(
                name: "EmpStamp");

            migrationBuilder.DropTable(
                name: "EmpStampBlob");

            migrationBuilder.DropTable(
                name: "JgStep");

            migrationBuilder.DropTable(
                name: "JobGrades");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "EmpCert");

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
