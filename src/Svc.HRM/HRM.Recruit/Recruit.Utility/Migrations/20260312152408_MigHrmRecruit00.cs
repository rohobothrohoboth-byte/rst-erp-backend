using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recruit.Utility.Migrations
{
    /// <inheritdoc />
    public partial class MigHrmRecruit00 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1️⃣ Create the code_counters table
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS code_counters
                (
                    module_name TEXT NOT NULL,
                    year INT NULL,
                    last_number BIGINT NOT NULL,
                    PRIMARY KEY (module_name, year)
                );
            ");

            // 2️⃣ Create the universal generator function
            migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION generate_code(p_module TEXT, p_year INT DEFAULT NULL)
            RETURNS TEXT AS $$
            DECLARE
                next_number BIGINT;
                year_part TEXT := '';
            BEGIN
                -- include year part if provided
                IF p_year IS NOT NULL THEN
                    year_part := p_year::TEXT || '-';
                END IF;

                -- insert or update counter
                INSERT INTO code_counters(module_name, year, last_number)
                VALUES (p_module, p_year, 1)
                ON CONFLICT (module_name, year)
                DO UPDATE SET last_number = code_counters.last_number + 1
                RETURNING last_number INTO next_number;

                -- format code
                IF p_year IS NOT NULL THEN
                    RETURN p_module || '-' || year_part || LPAD(next_number::TEXT, 6, '0');
                ELSE
                    RETURN p_module || LPAD(next_number::TEXT, 5, '0');
                END IF;
            END;
            $$ LANGUAGE plpgsql;
            ");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,");

            migrationBuilder.CreateTable(
                name: "ApplicantAddress",
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
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantAddress", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicantContact",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    PoBox = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Fax = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AlternatePhone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantContact", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicantPerson",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FirstNameAm = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MiddleName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MiddleNameAm = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastNameAm = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Gender = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Nationality = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantPerson", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EvaluationFlow",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsGlobal = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationFlow", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EvaluationType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MaxScore = table.Column<double>(type: "double precision", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobDec",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc = table.Column<string>(type: "text", nullable: false),
                    Qualification = table.Column<string>(type: "text", nullable: false),
                    KeySkills = table.Column<string>(type: "text", nullable: false),
                    WorkLocation = table.Column<string>(type: "text", nullable: false),
                    PreGender = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ContractType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobDec", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OnboardingTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    SequenceOrder = table.Column<int>(type: "integer", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnboardingTask", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkforcePlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false, defaultValueSql: "generate_code('WFP'::text, EXTRACT(YEAR FROM CURRENT_DATE)::int)"),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Desc = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalPositions = table.Column<int>(type: "integer", nullable: false),
                    AppPositions = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    PeriodId = table.Column<Guid>(type: "uuid", nullable: true),
                    RequistionById = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkforcePlan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Applicant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegisteredDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RegisteredBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PersonId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContactId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddressId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applicant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applicant_ApplicantAddress_AddressId",
                        column: x => x.AddressId,
                        principalTable: "ApplicantAddress",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applicant_ApplicantContact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "ApplicantContact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applicant_ApplicantPerson_PersonId",
                        column: x => x.PersonId,
                        principalTable: "ApplicantPerson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EvaluationStep",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StepName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StepOrder = table.Column<int>(type: "integer", nullable: false),
                    IsFinal = table.Column<bool>(type: "boolean", nullable: false),
                    EvalTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EvaluationFlowId = table.Column<Guid>(type: "uuid", nullable: false),
                    EvaluationFlowId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationStep", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvaluationStep_EvaluationFlow_EvaluationFlowId",
                        column: x => x.EvaluationFlowId,
                        principalTable: "EvaluationFlow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EvaluationStep_EvaluationFlow_EvaluationFlowId1",
                        column: x => x.EvaluationFlowId1,
                        principalTable: "EvaluationFlow",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EvaluationStep_EvaluationType_EvalTypeId",
                        column: x => x.EvalTypeId,
                        principalTable: "EvaluationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OnboardingAssign",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsMandatory = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VerifyById = table.Column<Guid>(type: "uuid", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    OnboardingTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnboardingAssign", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnboardingAssign_OnboardingTask_OnboardingTaskId",
                        column: x => x.OnboardingTaskId,
                        principalTable: "OnboardingTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobRequisition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReqNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ReqReason = table.Column<string>(type: "text", nullable: false),
                    ReqQuantity = table.Column<int>(type: "integer", nullable: false),
                    BudgetCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    JgStepId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkforcePlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    JobDecId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobRequisition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobRequisition_JobDec_JobDecId",
                        column: x => x.JobDecId,
                        principalTable: "JobDec",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobRequisition_WorkforcePlan_WorkforcePlanId",
                        column: x => x.WorkforcePlanId,
                        principalTable: "WorkforcePlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkforcePlanReview",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    ReqPositions = table.Column<int>(type: "integer", nullable: false),
                    AppPositions = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ReviewById = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkforcePlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkforcePlanReview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkforcePlanReview_WorkforcePlan_WorkforcePlanId",
                        column: x => x.WorkforcePlanId,
                        principalTable: "WorkforcePlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EvaluationScore",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Score = table.Column<double>(type: "double precision", nullable: false),
                    IsCurrent = table.Column<bool>(type: "boolean", nullable: false),
                    Feedback = table.Column<string>(type: "text", nullable: false),
                    EvaluatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    EvalTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EvaluationStepId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationScore", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvaluationScore_EvaluationStep_EvaluationStepId",
                        column: x => x.EvaluationStepId,
                        principalTable: "EvaluationStep",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EvaluationScore_EvaluationType_EvalTypeId",
                        column: x => x.EvalTypeId,
                        principalTable: "EvaluationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobPosting",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PostNumber = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false, defaultValueSql: "generate_code('JOB'::text, EXTRACT(YEAR FROM CURRENT_DATE)::int)"),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PostType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PublishedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeadlineDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ClosedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    JobReqId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPosting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobPosting_JobRequisition_JobReqId",
                        column: x => x.JobReqId,
                        principalTable: "JobRequisition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobReqReview",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    ReqQuantity = table.Column<int>(type: "integer", nullable: false),
                    AppQuantity = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ReviewById = table.Column<Guid>(type: "uuid", nullable: false),
                    JobReqId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobReqReview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobReqReview_JobRequisition_JobReqId",
                        column: x => x.JobReqId,
                        principalTable: "JobRequisition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobApplication",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PostType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AppliedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApplicantId = table.Column<Guid>(type: "uuid", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    JobPostingId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobApplication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobApplication_JobPosting_JobPostingId",
                        column: x => x.JobPostingId,
                        principalTable: "JobPosting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobPostEvalFlow",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EvaluationFlowId = table.Column<Guid>(type: "uuid", nullable: false),
                    JobPostingId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPostEvalFlow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobPostEvalFlow_EvaluationFlow_EvaluationFlowId",
                        column: x => x.EvaluationFlowId,
                        principalTable: "EvaluationFlow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobPostEvalFlow_JobPosting_JobPostingId",
                        column: x => x.JobPostingId,
                        principalTable: "JobPosting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobPostReview",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ReviewById = table.Column<Guid>(type: "uuid", nullable: false),
                    JobPostingId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPostReview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobPostReview_JobPosting_JobPostingId",
                        column: x => x.JobPostingId,
                        principalTable: "JobPosting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationRanking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalScore = table.Column<double>(type: "double precision", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false),
                    JobAppId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationRanking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationRanking_JobApplication_JobAppId",
                        column: x => x.JobAppId,
                        principalTable: "JobApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CoverLetter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    JobAppId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoverLetter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoverLetter_JobApplication_JobAppId",
                        column: x => x.JobAppId,
                        principalTable: "JobApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobOffer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OfferNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    OfferDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OfferDocument = table.Column<string>(type: "text", nullable: false),
                    JobApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    JobPostingId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobOffer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobOffer_JobApplication_JobApplicationId",
                        column: x => x.JobApplicationId,
                        principalTable: "JobApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobOffer_JobPosting_JobPostingId",
                        column: x => x.JobPostingId,
                        principalTable: "JobPosting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Resume",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    JobAppId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resume", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resume_JobApplication_JobAppId",
                        column: x => x.JobAppId,
                        principalTable: "JobApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobOfferApproval",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StepOrder = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApprovedById = table.Column<Guid>(type: "uuid", nullable: true),
                    JobOfferId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobOfferApproval", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobOfferApproval_JobOffer_JobOfferId",
                        column: x => x.JobOfferId,
                        principalTable: "JobOffer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobOfferReview",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcceptanceDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RejectionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RejectionReason = table.Column<string>(type: "text", nullable: true),
                    ApprovalComments = table.Column<string>(type: "text", nullable: true),
                    JobOfferId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobOfferReview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobOfferReview_JobOffer_JobOfferId",
                        column: x => x.JobOfferId,
                        principalTable: "JobOffer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResumeBlob",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<byte[]>(type: "bytea", nullable: false),
                    ResumeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateAdd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateMod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeBlob", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResumeBlob_Resume_ResumeId",
                        column: x => x.ResumeId,
                        principalTable: "Resume",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applicant_AddressId",
                table: "Applicant",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Applicant_ContactId",
                table: "Applicant",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Applicant_IsDeleted",
                table: "Applicant",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Applicant_PersonId",
                table: "Applicant",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantAddress_Country",
                table: "ApplicantAddress",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantAddress_IsDeleted",
                table: "ApplicantAddress",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantContact_Email",
                table: "ApplicantContact",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantContact_IsDeleted",
                table: "ApplicantContact",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantPerson_IsDeleted",
                table: "ApplicantPerson",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationRanking_IsDeleted",
                table: "ApplicationRanking",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationRanking_JobAppId",
                table: "ApplicationRanking",
                column: "JobAppId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationRanking_TotalScore_Rank",
                table: "ApplicationRanking",
                columns: new[] { "TotalScore", "Rank" });

            migrationBuilder.CreateIndex(
                name: "IX_CoverLetter_IsDeleted",
                table: "CoverLetter",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_CoverLetter_JobAppId",
                table: "CoverLetter",
                column: "JobAppId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationFlow_IsDeleted",
                table: "EvaluationFlow",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationFlow_Name",
                table: "EvaluationFlow",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationScore_EvalTypeId_EvaluationStepId",
                table: "EvaluationScore",
                columns: new[] { "EvalTypeId", "EvaluationStepId" });

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationScore_EvaluationStepId",
                table: "EvaluationScore",
                column: "EvaluationStepId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationScore_IsDeleted",
                table: "EvaluationScore",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationStep_EvalTypeId",
                table: "EvaluationStep",
                column: "EvalTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationStep_EvaluationFlowId_StepOrder",
                table: "EvaluationStep",
                columns: new[] { "EvaluationFlowId", "StepOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationStep_EvaluationFlowId1",
                table: "EvaluationStep",
                column: "EvaluationFlowId1");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationStep_IsDeleted",
                table: "EvaluationStep",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationType_IsDeleted",
                table: "EvaluationType",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationType_Name",
                table: "EvaluationType",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobApplication_AppliedDate",
                table: "JobApplication",
                column: "AppliedDate");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplication_IsDeleted",
                table: "JobApplication",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplication_JobPostingId",
                table: "JobApplication",
                column: "JobPostingId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplication_Status",
                table: "JobApplication",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_JobDec_IsDeleted",
                table: "JobDec",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobOffer_IsDeleted",
                table: "JobOffer",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobOffer_JobApplicationId",
                table: "JobOffer",
                column: "JobApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_JobOffer_JobPostingId",
                table: "JobOffer",
                column: "JobPostingId");

            migrationBuilder.CreateIndex(
                name: "IX_JobOffer_OfferNumber",
                table: "JobOffer",
                column: "OfferNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobOffer_Status",
                table: "JobOffer",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_JobOfferApproval_IsDeleted",
                table: "JobOfferApproval",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobOfferApproval_JobOfferId",
                table: "JobOfferApproval",
                column: "JobOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_JobOfferReview_IsDeleted",
                table: "JobOfferReview",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobOfferReview_JobOfferId",
                table: "JobOfferReview",
                column: "JobOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostEvalFlow_EvaluationFlowId",
                table: "JobPostEvalFlow",
                column: "EvaluationFlowId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostEvalFlow_IsDeleted",
                table: "JobPostEvalFlow",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostEvalFlow_JobPostingId_EvaluationFlowId",
                table: "JobPostEvalFlow",
                columns: new[] { "JobPostingId", "EvaluationFlowId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobPosting_IsDeleted",
                table: "JobPosting",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobPosting_JobReqId",
                table: "JobPosting",
                column: "JobReqId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPosting_PostNumber",
                table: "JobPosting",
                column: "PostNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobPosting_Status",
                table: "JobPosting",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostReview_IsDeleted",
                table: "JobPostReview",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostReview_JobPostingId",
                table: "JobPostReview",
                column: "JobPostingId");

            migrationBuilder.CreateIndex(
                name: "IX_JobReqReview_IsDeleted",
                table: "JobReqReview",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobReqReview_JobReqId",
                table: "JobReqReview",
                column: "JobReqId");

            migrationBuilder.CreateIndex(
                name: "IX_JobRequisition_IsDeleted",
                table: "JobRequisition",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobRequisition_JobDecId",
                table: "JobRequisition",
                column: "JobDecId");

            migrationBuilder.CreateIndex(
                name: "IX_JobRequisition_WorkforcePlanId",
                table: "JobRequisition",
                column: "WorkforcePlanId");

            migrationBuilder.CreateIndex(
                name: "IX_OnboardingAssign_IsDeleted",
                table: "OnboardingAssign",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_OnboardingAssign_OnboardingTaskId",
                table: "OnboardingAssign",
                column: "OnboardingTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_OnboardingTask_IsDeleted",
                table: "OnboardingTask",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_OnboardingTask_SequenceOrder",
                table: "OnboardingTask",
                column: "SequenceOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Resume_IsDeleted",
                table: "Resume",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Resume_JobAppId",
                table: "Resume",
                column: "JobAppId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeBlob_IsDeleted",
                table: "ResumeBlob",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeBlob_ResumeId",
                table: "ResumeBlob",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkforcePlan_IsDeleted",
                table: "WorkforcePlan",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_WorkforcePlan_PlanCode",
                table: "WorkforcePlan",
                column: "PlanCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkforcePlanReview_IsDeleted",
                table: "WorkforcePlanReview",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_WorkforcePlanReview_WorkforcePlanId_Status",
                table: "WorkforcePlanReview",
                columns: new[] { "WorkforcePlanId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop function first
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS generate_code(TEXT, INT);");
            // Then drop table
            migrationBuilder.Sql("DROP TABLE IF EXISTS code_counters;");

            migrationBuilder.DropTable(
                name: "Applicant");

            migrationBuilder.DropTable(
                name: "ApplicationRanking");

            migrationBuilder.DropTable(
                name: "CoverLetter");

            migrationBuilder.DropTable(
                name: "EvaluationScore");

            migrationBuilder.DropTable(
                name: "JobOfferApproval");

            migrationBuilder.DropTable(
                name: "JobOfferReview");

            migrationBuilder.DropTable(
                name: "JobPostEvalFlow");

            migrationBuilder.DropTable(
                name: "JobPostReview");

            migrationBuilder.DropTable(
                name: "JobReqReview");

            migrationBuilder.DropTable(
                name: "OnboardingAssign");

            migrationBuilder.DropTable(
                name: "ResumeBlob");

            migrationBuilder.DropTable(
                name: "WorkforcePlanReview");

            migrationBuilder.DropTable(
                name: "ApplicantAddress");

            migrationBuilder.DropTable(
                name: "ApplicantContact");

            migrationBuilder.DropTable(
                name: "ApplicantPerson");

            migrationBuilder.DropTable(
                name: "EvaluationStep");

            migrationBuilder.DropTable(
                name: "JobOffer");

            migrationBuilder.DropTable(
                name: "OnboardingTask");

            migrationBuilder.DropTable(
                name: "Resume");

            migrationBuilder.DropTable(
                name: "EvaluationFlow");

            migrationBuilder.DropTable(
                name: "EvaluationType");

            migrationBuilder.DropTable(
                name: "JobApplication");

            migrationBuilder.DropTable(
                name: "JobPosting");

            migrationBuilder.DropTable(
                name: "JobRequisition");

            migrationBuilder.DropTable(
                name: "JobDec");

            migrationBuilder.DropTable(
                name: "WorkforcePlan");
        }
    }
}
