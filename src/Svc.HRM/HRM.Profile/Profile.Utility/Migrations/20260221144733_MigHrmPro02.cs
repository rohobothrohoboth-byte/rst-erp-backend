using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Profile.Utility.Migrations
{
    /// <inheritdoc />
    public partial class MigHrmPro02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyContact_Employee_EmployeeId",
                table: "EmergencyContact");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpBio_Employee_EmployeeId",
                table: "EmpBio");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpFamily_Employee_EmployeeId",
                table: "EmpFamily");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpFinance_Employee_EmployeeId",
                table: "EmpFinance");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpGuarantor_Employee_EmployeeId",
                table: "EmpGuarantor");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpGuarantorFile_EmpGuarantor_EmpGuarantorId",
                table: "EmpGuarantorFile");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpGuarantorFileBlob_FileMetaData_FileMetaDataId",
                table: "EmpGuarantorFileBlob");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpPensionCard_Employee_EmployeeId",
                table: "EmpPensionCard");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpPhoto_Employee_EmployeeId",
                table: "EmpPhoto");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpPhotoBlob_FileMetaData_FileMetaDataId",
                table: "EmpPhotoBlob");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpPhotoThumbnail_FileMetaData_FileMetaDataId",
                table: "EmpPhotoThumbnail");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpSign_Employee_EmployeeId",
                table: "EmpSign");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpSignBlob_FileMetaData_FileMetaDataId",
                table: "EmpSignBlob");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpStamp_Employee_EmployeeId",
                table: "EmpStamp");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpStampBlob_FileMetaData_FileMetaDataId",
                table: "EmpStampBlob");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpState_Employee_EmployeeId",
                table: "EmpState");

            migrationBuilder.DropIndex(
                name: "IX_EmpState_EmployeeId",
                table: "EmpState");

            migrationBuilder.DropIndex(
                name: "IX_EmpStampBlob_FileMetaDataId",
                table: "EmpStampBlob");

            migrationBuilder.DropIndex(
                name: "IX_EmpStamp_EmployeeId",
                table: "EmpStamp");

            migrationBuilder.DropIndex(
                name: "IX_EmpSignBlob_FileMetaDataId",
                table: "EmpSignBlob");

            migrationBuilder.DropIndex(
                name: "IX_EmpSign_EmployeeId",
                table: "EmpSign");

            migrationBuilder.DropIndex(
                name: "IX_EmpPhotoThumbnail_FileMetaDataId",
                table: "EmpPhotoThumbnail");

            migrationBuilder.DropIndex(
                name: "IX_EmpPhotoBlob_FileMetaDataId",
                table: "EmpPhotoBlob");

            migrationBuilder.DropIndex(
                name: "IX_EmpPhoto_EmployeeId",
                table: "EmpPhoto");

            migrationBuilder.DropIndex(
                name: "IX_EmpPensionCard_EmployeeId",
                table: "EmpPensionCard");

            migrationBuilder.DropIndex(
                name: "IX_Employee_PersonId",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_EmpGuarantorFileBlob_FileMetaDataId",
                table: "EmpGuarantorFileBlob");

            migrationBuilder.DropIndex(
                name: "IX_EmpFinance_EmployeeId",
                table: "EmpFinance");

            migrationBuilder.DropIndex(
                name: "IX_EmpBio_EmployeeId",
                table: "EmpBio");

            migrationBuilder.AlterColumn<string>(
                name: "Nationality",
                table: "Person",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "MiddleNameAm",
                table: "Person",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "MiddleName",
                table: "Person",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "LastNameAm",
                table: "Person",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Person",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Person",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Person",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "FirstNameAm",
                table: "Person",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Person",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "FileMetaData",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "FileMetaData",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "FileMetaData",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "IsUnderProbation",
                table: "EmpState",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "IsTerminated",
                table: "EmpState",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "IsStandBy",
                table: "EmpState",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "IsRetired",
                table: "EmpState",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpState",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "IsApproved",
                table: "EmpState",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpStampBlob",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpStamp",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpSignBlob",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpSign",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpPhotoThumbnail",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpPhotoBlob",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpPhoto",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "IsSent",
                table: "EmpPensionCard",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "IsReceived",
                table: "EmpPensionCard",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpPensionCard",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "WorkArrangement",
                table: "Employee",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Employee",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "EmploymentType",
                table: "Employee",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "EmploymentNature",
                table: "Employee",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Employee",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpGuarantorFileBlob",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpGuarantorFile",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpGuarantor",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Tin",
                table: "EmpFinance",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "PensionNumber",
                table: "EmpFinance",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpFinance",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "BankAccountNo",
                table: "EmpFinance",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpFamily",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "MotherFullName",
                table: "EmpBio",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "MaritalStatus",
                table: "EmpBio",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpBio",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "HasMarriageCert",
                table: "EmpBio",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "HasBirthCert",
                table: "EmpBio",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "BirthLocation",
                table: "EmpBio",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmergencyContact",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Zone",
                table: "Address",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Woreda",
                table: "Address",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Website",
                table: "Address",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Telephone",
                table: "Address",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Subcity",
                table: "Address",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Region",
                table: "Address",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "PoBox",
                table: "Address",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Kebele",
                table: "Address",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Address",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "HouseNo",
                table: "Address",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Fax",
                table: "Address",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Address",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "Address",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "AddressType",
                table: "Address",
                type: "character varying(2)",
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_Person_Gender",
                table: "Person",
                column: "Gender");

            migrationBuilder.CreateIndex(
                name: "IX_Person_Id",
                table: "Person",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Person_IsDeleted",
                table: "Person",
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
                name: "IX_FileMetaData_Id",
                table: "FileMetaData",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_FileMetaData_IsDeleted",
                table: "FileMetaData",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpState_EmployeeId",
                table: "EmpState",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpState_Id",
                table: "EmpState",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EmpState_IsDeleted",
                table: "EmpState",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpStampBlob_FileMetaDataId",
                table: "EmpStampBlob",
                column: "FileMetaDataId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpStampBlob_Id",
                table: "EmpStampBlob",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EmpStampBlob_IsDeleted",
                table: "EmpStampBlob",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpStamp_EmployeeId",
                table: "EmpStamp",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpStamp_Id",
                table: "EmpStamp",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EmpStamp_IsDeleted",
                table: "EmpStamp",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpSignBlob_FileMetaDataId",
                table: "EmpSignBlob",
                column: "FileMetaDataId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpSignBlob_Id",
                table: "EmpSignBlob",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EmpSignBlob_IsDeleted",
                table: "EmpSignBlob",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpSign_EmployeeId",
                table: "EmpSign",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpSign_Id",
                table: "EmpSign",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EmpSign_IsDeleted",
                table: "EmpSign",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPhotoThumbnail_FileMetaDataId",
                table: "EmpPhotoThumbnail",
                column: "FileMetaDataId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpPhotoThumbnail_Id",
                table: "EmpPhotoThumbnail",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPhotoThumbnail_IsDeleted",
                table: "EmpPhotoThumbnail",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPhotoBlob_FileMetaDataId",
                table: "EmpPhotoBlob",
                column: "FileMetaDataId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpPhotoBlob_Id",
                table: "EmpPhotoBlob",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPhotoBlob_IsDeleted",
                table: "EmpPhotoBlob",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPhoto_EmployeeId",
                table: "EmpPhoto",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpPhoto_Id",
                table: "EmpPhoto",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPhoto_IsDeleted",
                table: "EmpPhoto",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPensionCard_EmployeeId",
                table: "EmpPensionCard",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpPensionCard_Id",
                table: "EmpPensionCard",
                column: "Id");

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
                name: "IX_Employee_Code",
                table: "Employee",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_DepartmentId",
                table: "Employee",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Id",
                table: "Employee",
                column: "Id");

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
                name: "IX_EmpGuarantorFileBlob_FileMetaDataId",
                table: "EmpGuarantorFileBlob",
                column: "FileMetaDataId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantorFileBlob_Id",
                table: "EmpGuarantorFileBlob",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantorFileBlob_IsDeleted",
                table: "EmpGuarantorFileBlob",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantorFile_EmpGuarantorId_FileMetaDataId",
                table: "EmpGuarantorFile",
                columns: new[] { "EmpGuarantorId", "FileMetaDataId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantorFile_Id",
                table: "EmpGuarantorFile",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantorFile_IsDeleted",
                table: "EmpGuarantorFile",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantor_EmployeeId_PersonId",
                table: "EmpGuarantor",
                columns: new[] { "EmployeeId", "PersonId" });

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantor_Id",
                table: "EmpGuarantor",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantor_IsDeleted",
                table: "EmpGuarantor",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantor_RelationId",
                table: "EmpGuarantor",
                column: "RelationId");

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
                name: "IX_EmpFinance_Id",
                table: "EmpFinance",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EmpFinance_IsDeleted",
                table: "EmpFinance",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpFinance_Tin",
                table: "EmpFinance",
                column: "Tin");

            migrationBuilder.CreateIndex(
                name: "IX_EmpFamily_EmployeeId_PersonId",
                table: "EmpFamily",
                columns: new[] { "EmployeeId", "PersonId" });

            migrationBuilder.CreateIndex(
                name: "IX_EmpFamily_Id",
                table: "EmpFamily",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EmpFamily_IsDeleted",
                table: "EmpFamily",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmpFamily_RelationId",
                table: "EmpFamily",
                column: "RelationId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpBio_EmployeeId",
                table: "EmpBio",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpBio_Id",
                table: "EmpBio",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EmpBio_IsDeleted",
                table: "EmpBio",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContact_EmployeeId_PersonId",
                table: "EmergencyContact",
                columns: new[] { "EmployeeId", "PersonId" });

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContact_Id",
                table: "EmergencyContact",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContact_IsDeleted",
                table: "EmergencyContact",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContact_RelationId",
                table: "EmergencyContact",
                column: "RelationId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_Country",
                table: "Address",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_Address_Email",
                table: "Address",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Address_Id",
                table: "Address",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Address_IsDeleted",
                table: "Address",
                column: "IsDeleted");

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyContact_Employee_EmployeeId",
                table: "EmergencyContact",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpBio_Employee_EmployeeId",
                table: "EmpBio",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpFamily_Employee_EmployeeId",
                table: "EmpFamily",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpFinance_Employee_EmployeeId",
                table: "EmpFinance",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpGuarantor_Employee_EmployeeId",
                table: "EmpGuarantor",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpGuarantorFile_EmpGuarantor_EmpGuarantorId",
                table: "EmpGuarantorFile",
                column: "EmpGuarantorId",
                principalTable: "EmpGuarantor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpGuarantorFileBlob_FileMetaData_FileMetaDataId",
                table: "EmpGuarantorFileBlob",
                column: "FileMetaDataId",
                principalTable: "FileMetaData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpPensionCard_Employee_EmployeeId",
                table: "EmpPensionCard",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpPhoto_Employee_EmployeeId",
                table: "EmpPhoto",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpPhotoBlob_FileMetaData_FileMetaDataId",
                table: "EmpPhotoBlob",
                column: "FileMetaDataId",
                principalTable: "FileMetaData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpPhotoThumbnail_FileMetaData_FileMetaDataId",
                table: "EmpPhotoThumbnail",
                column: "FileMetaDataId",
                principalTable: "FileMetaData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpSign_Employee_EmployeeId",
                table: "EmpSign",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpSignBlob_FileMetaData_FileMetaDataId",
                table: "EmpSignBlob",
                column: "FileMetaDataId",
                principalTable: "FileMetaData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpStamp_Employee_EmployeeId",
                table: "EmpStamp",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpStampBlob_FileMetaData_FileMetaDataId",
                table: "EmpStampBlob",
                column: "FileMetaDataId",
                principalTable: "FileMetaData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpState_Employee_EmployeeId",
                table: "EmpState",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyContact_Employee_EmployeeId",
                table: "EmergencyContact");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpBio_Employee_EmployeeId",
                table: "EmpBio");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpFamily_Employee_EmployeeId",
                table: "EmpFamily");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpFinance_Employee_EmployeeId",
                table: "EmpFinance");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpGuarantor_Employee_EmployeeId",
                table: "EmpGuarantor");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpGuarantorFile_EmpGuarantor_EmpGuarantorId",
                table: "EmpGuarantorFile");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpGuarantorFileBlob_FileMetaData_FileMetaDataId",
                table: "EmpGuarantorFileBlob");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpPensionCard_Employee_EmployeeId",
                table: "EmpPensionCard");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpPhoto_Employee_EmployeeId",
                table: "EmpPhoto");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpPhotoBlob_FileMetaData_FileMetaDataId",
                table: "EmpPhotoBlob");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpPhotoThumbnail_FileMetaData_FileMetaDataId",
                table: "EmpPhotoThumbnail");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpSign_Employee_EmployeeId",
                table: "EmpSign");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpSignBlob_FileMetaData_FileMetaDataId",
                table: "EmpSignBlob");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpStamp_Employee_EmployeeId",
                table: "EmpStamp");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpStampBlob_FileMetaData_FileMetaDataId",
                table: "EmpStampBlob");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpState_Employee_EmployeeId",
                table: "EmpState");

            migrationBuilder.DropIndex(
                name: "IX_Person_Gender",
                table: "Person");

            migrationBuilder.DropIndex(
                name: "IX_Person_Id",
                table: "Person");

            migrationBuilder.DropIndex(
                name: "IX_Person_IsDeleted",
                table: "Person");

            migrationBuilder.DropIndex(
                name: "IX_FileMetaData_ContentType",
                table: "FileMetaData");

            migrationBuilder.DropIndex(
                name: "IX_FileMetaData_FileName",
                table: "FileMetaData");

            migrationBuilder.DropIndex(
                name: "IX_FileMetaData_Id",
                table: "FileMetaData");

            migrationBuilder.DropIndex(
                name: "IX_FileMetaData_IsDeleted",
                table: "FileMetaData");

            migrationBuilder.DropIndex(
                name: "IX_EmpState_EmployeeId",
                table: "EmpState");

            migrationBuilder.DropIndex(
                name: "IX_EmpState_Id",
                table: "EmpState");

            migrationBuilder.DropIndex(
                name: "IX_EmpState_IsDeleted",
                table: "EmpState");

            migrationBuilder.DropIndex(
                name: "IX_EmpStampBlob_FileMetaDataId",
                table: "EmpStampBlob");

            migrationBuilder.DropIndex(
                name: "IX_EmpStampBlob_Id",
                table: "EmpStampBlob");

            migrationBuilder.DropIndex(
                name: "IX_EmpStampBlob_IsDeleted",
                table: "EmpStampBlob");

            migrationBuilder.DropIndex(
                name: "IX_EmpStamp_EmployeeId",
                table: "EmpStamp");

            migrationBuilder.DropIndex(
                name: "IX_EmpStamp_Id",
                table: "EmpStamp");

            migrationBuilder.DropIndex(
                name: "IX_EmpStamp_IsDeleted",
                table: "EmpStamp");

            migrationBuilder.DropIndex(
                name: "IX_EmpSignBlob_FileMetaDataId",
                table: "EmpSignBlob");

            migrationBuilder.DropIndex(
                name: "IX_EmpSignBlob_Id",
                table: "EmpSignBlob");

            migrationBuilder.DropIndex(
                name: "IX_EmpSignBlob_IsDeleted",
                table: "EmpSignBlob");

            migrationBuilder.DropIndex(
                name: "IX_EmpSign_EmployeeId",
                table: "EmpSign");

            migrationBuilder.DropIndex(
                name: "IX_EmpSign_Id",
                table: "EmpSign");

            migrationBuilder.DropIndex(
                name: "IX_EmpSign_IsDeleted",
                table: "EmpSign");

            migrationBuilder.DropIndex(
                name: "IX_EmpPhotoThumbnail_FileMetaDataId",
                table: "EmpPhotoThumbnail");

            migrationBuilder.DropIndex(
                name: "IX_EmpPhotoThumbnail_Id",
                table: "EmpPhotoThumbnail");

            migrationBuilder.DropIndex(
                name: "IX_EmpPhotoThumbnail_IsDeleted",
                table: "EmpPhotoThumbnail");

            migrationBuilder.DropIndex(
                name: "IX_EmpPhotoBlob_FileMetaDataId",
                table: "EmpPhotoBlob");

            migrationBuilder.DropIndex(
                name: "IX_EmpPhotoBlob_Id",
                table: "EmpPhotoBlob");

            migrationBuilder.DropIndex(
                name: "IX_EmpPhotoBlob_IsDeleted",
                table: "EmpPhotoBlob");

            migrationBuilder.DropIndex(
                name: "IX_EmpPhoto_EmployeeId",
                table: "EmpPhoto");

            migrationBuilder.DropIndex(
                name: "IX_EmpPhoto_Id",
                table: "EmpPhoto");

            migrationBuilder.DropIndex(
                name: "IX_EmpPhoto_IsDeleted",
                table: "EmpPhoto");

            migrationBuilder.DropIndex(
                name: "IX_EmpPensionCard_EmployeeId",
                table: "EmpPensionCard");

            migrationBuilder.DropIndex(
                name: "IX_EmpPensionCard_Id",
                table: "EmpPensionCard");

            migrationBuilder.DropIndex(
                name: "IX_EmpPensionCard_IsDeleted",
                table: "EmpPensionCard");

            migrationBuilder.DropIndex(
                name: "IX_EmpPensionCard_IsReceived",
                table: "EmpPensionCard");

            migrationBuilder.DropIndex(
                name: "IX_EmpPensionCard_IsSent",
                table: "EmpPensionCard");

            migrationBuilder.DropIndex(
                name: "IX_Employee_Code",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_DepartmentId",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_Id",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_IsDeleted",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_JobGradeId",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_PersonId",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_PositionId",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_EmpGuarantorFileBlob_FileMetaDataId",
                table: "EmpGuarantorFileBlob");

            migrationBuilder.DropIndex(
                name: "IX_EmpGuarantorFileBlob_Id",
                table: "EmpGuarantorFileBlob");

            migrationBuilder.DropIndex(
                name: "IX_EmpGuarantorFileBlob_IsDeleted",
                table: "EmpGuarantorFileBlob");

            migrationBuilder.DropIndex(
                name: "IX_EmpGuarantorFile_EmpGuarantorId_FileMetaDataId",
                table: "EmpGuarantorFile");

            migrationBuilder.DropIndex(
                name: "IX_EmpGuarantorFile_Id",
                table: "EmpGuarantorFile");

            migrationBuilder.DropIndex(
                name: "IX_EmpGuarantorFile_IsDeleted",
                table: "EmpGuarantorFile");

            migrationBuilder.DropIndex(
                name: "IX_EmpGuarantor_EmployeeId_PersonId",
                table: "EmpGuarantor");

            migrationBuilder.DropIndex(
                name: "IX_EmpGuarantor_Id",
                table: "EmpGuarantor");

            migrationBuilder.DropIndex(
                name: "IX_EmpGuarantor_IsDeleted",
                table: "EmpGuarantor");

            migrationBuilder.DropIndex(
                name: "IX_EmpGuarantor_RelationId",
                table: "EmpGuarantor");

            migrationBuilder.DropIndex(
                name: "IX_EmpFinance_BankAccountNo",
                table: "EmpFinance");

            migrationBuilder.DropIndex(
                name: "IX_EmpFinance_EmployeeId",
                table: "EmpFinance");

            migrationBuilder.DropIndex(
                name: "IX_EmpFinance_Id",
                table: "EmpFinance");

            migrationBuilder.DropIndex(
                name: "IX_EmpFinance_IsDeleted",
                table: "EmpFinance");

            migrationBuilder.DropIndex(
                name: "IX_EmpFinance_Tin",
                table: "EmpFinance");

            migrationBuilder.DropIndex(
                name: "IX_EmpFamily_EmployeeId_PersonId",
                table: "EmpFamily");

            migrationBuilder.DropIndex(
                name: "IX_EmpFamily_Id",
                table: "EmpFamily");

            migrationBuilder.DropIndex(
                name: "IX_EmpFamily_IsDeleted",
                table: "EmpFamily");

            migrationBuilder.DropIndex(
                name: "IX_EmpFamily_RelationId",
                table: "EmpFamily");

            migrationBuilder.DropIndex(
                name: "IX_EmpBio_EmployeeId",
                table: "EmpBio");

            migrationBuilder.DropIndex(
                name: "IX_EmpBio_Id",
                table: "EmpBio");

            migrationBuilder.DropIndex(
                name: "IX_EmpBio_IsDeleted",
                table: "EmpBio");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyContact_EmployeeId_PersonId",
                table: "EmergencyContact");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyContact_Id",
                table: "EmergencyContact");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyContact_IsDeleted",
                table: "EmergencyContact");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyContact_RelationId",
                table: "EmergencyContact");

            migrationBuilder.DropIndex(
                name: "IX_Address_Country",
                table: "Address");

            migrationBuilder.DropIndex(
                name: "IX_Address_Email",
                table: "Address");

            migrationBuilder.DropIndex(
                name: "IX_Address_Id",
                table: "Address");

            migrationBuilder.DropIndex(
                name: "IX_Address_IsDeleted",
                table: "Address");

            migrationBuilder.AlterColumn<string>(
                name: "Nationality",
                table: "Person",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "MiddleNameAm",
                table: "Person",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "MiddleName",
                table: "Person",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "LastNameAm",
                table: "Person",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Person",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Person",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Person",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "FirstNameAm",
                table: "Person",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Person",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "FileMetaData",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "FileMetaData",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "FileMetaData",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "IsUnderProbation",
                table: "EmpState",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "IsTerminated",
                table: "EmpState",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "IsStandBy",
                table: "EmpState",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "IsRetired",
                table: "EmpState",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpState",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "IsApproved",
                table: "EmpState",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpStampBlob",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpStamp",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpSignBlob",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpSign",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpPhotoThumbnail",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpPhotoBlob",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpPhoto",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "IsSent",
                table: "EmpPensionCard",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "IsReceived",
                table: "EmpPensionCard",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpPensionCard",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "WorkArrangement",
                table: "Employee",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Employee",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "EmploymentType",
                table: "Employee",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "EmploymentNature",
                table: "Employee",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Employee",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpGuarantorFileBlob",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpGuarantorFile",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpGuarantor",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Tin",
                table: "EmpFinance",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "PensionNumber",
                table: "EmpFinance",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpFinance",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "BankAccountNo",
                table: "EmpFinance",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpFamily",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "MotherFullName",
                table: "EmpBio",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "MaritalStatus",
                table: "EmpBio",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmpBio",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "HasMarriageCert",
                table: "EmpBio",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "HasBirthCert",
                table: "EmpBio",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "BirthLocation",
                table: "EmpBio",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmergencyContact",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Zone",
                table: "Address",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Woreda",
                table: "Address",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Website",
                table: "Address",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Telephone",
                table: "Address",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "Subcity",
                table: "Address",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Region",
                table: "Address",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "PoBox",
                table: "Address",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Kebele",
                table: "Address",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Address",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "HouseNo",
                table: "Address",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Fax",
                table: "Address",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Address",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "Address",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "AddressType",
                table: "Address",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2)",
                oldMaxLength: 2);

            migrationBuilder.CreateIndex(
                name: "IX_EmpState_EmployeeId",
                table: "EmpState",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpStampBlob_FileMetaDataId",
                table: "EmpStampBlob",
                column: "FileMetaDataId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpStamp_EmployeeId",
                table: "EmpStamp",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpSignBlob_FileMetaDataId",
                table: "EmpSignBlob",
                column: "FileMetaDataId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpSign_EmployeeId",
                table: "EmpSign",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPhotoThumbnail_FileMetaDataId",
                table: "EmpPhotoThumbnail",
                column: "FileMetaDataId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPhotoBlob_FileMetaDataId",
                table: "EmpPhotoBlob",
                column: "FileMetaDataId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPhoto_EmployeeId",
                table: "EmpPhoto",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPensionCard_EmployeeId",
                table: "EmpPensionCard",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_PersonId",
                table: "Employee",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpGuarantorFileBlob_FileMetaDataId",
                table: "EmpGuarantorFileBlob",
                column: "FileMetaDataId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpFinance_EmployeeId",
                table: "EmpFinance",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpBio_EmployeeId",
                table: "EmpBio",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyContact_Employee_EmployeeId",
                table: "EmergencyContact",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpBio_Employee_EmployeeId",
                table: "EmpBio",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpFamily_Employee_EmployeeId",
                table: "EmpFamily",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpFinance_Employee_EmployeeId",
                table: "EmpFinance",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpGuarantor_Employee_EmployeeId",
                table: "EmpGuarantor",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpGuarantorFile_EmpGuarantor_EmpGuarantorId",
                table: "EmpGuarantorFile",
                column: "EmpGuarantorId",
                principalTable: "EmpGuarantor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpGuarantorFileBlob_FileMetaData_FileMetaDataId",
                table: "EmpGuarantorFileBlob",
                column: "FileMetaDataId",
                principalTable: "FileMetaData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpPensionCard_Employee_EmployeeId",
                table: "EmpPensionCard",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpPhoto_Employee_EmployeeId",
                table: "EmpPhoto",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpPhotoBlob_FileMetaData_FileMetaDataId",
                table: "EmpPhotoBlob",
                column: "FileMetaDataId",
                principalTable: "FileMetaData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpPhotoThumbnail_FileMetaData_FileMetaDataId",
                table: "EmpPhotoThumbnail",
                column: "FileMetaDataId",
                principalTable: "FileMetaData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpSign_Employee_EmployeeId",
                table: "EmpSign",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpSignBlob_FileMetaData_FileMetaDataId",
                table: "EmpSignBlob",
                column: "FileMetaDataId",
                principalTable: "FileMetaData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpStamp_Employee_EmployeeId",
                table: "EmpStamp",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpStampBlob_FileMetaData_FileMetaDataId",
                table: "EmpStampBlob",
                column: "FileMetaDataId",
                principalTable: "FileMetaData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpState_Employee_EmployeeId",
                table: "EmpState",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
