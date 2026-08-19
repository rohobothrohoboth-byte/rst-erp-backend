using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Docker;

var builder = DistributedApplication.CreateBuilder(args);

// ============================================================
// INFRASTRUCTURE SERVICES
// ============================================================

// RABBITMQ - Use local instance
var rabbitmq = builder.AddConnectionString("rabbitmq");
builder.Configuration["ConnectionStrings:rabbitmq"] = "amqp://guest:guest@localhost:5672";

// Redis
var redisPassword = builder.AddParameter("redisPassword", secret: true);
var redis = builder.AddRedis("redis")
    .WithPassword(redisPassword)
    .WithRedisCommander()
    .WithEndpoint(port: 6379, targetPort: 6379);

var postgres = builder.AddPostgres("postgres")
    .WithImage("postgres:16-alpine")
    .WithEnvironment("POSTGRES_USER", "postgres")
    .WithEnvironment("POSTGRES_PASSWORD", "root")
    .WithEnvironment("POSTGRES_DB", "postgres")
    .WithEnvironment("PGDATA", "/var/lib/postgresql/data/pgdata")
    .WithEnvironment("POSTGRES_HOST_AUTH_METHOD", "trust")
    .WithPgAdmin(pgAdmin =>
    {
        pgAdmin.WithEnvironment("PGADMIN_DEFAULT_EMAIL", "admin@admin.com")
               .WithEnvironment("PGADMIN_DEFAULT_PASSWORD", "root")
               .WithEnvironment("PGADMIN_CONFIG_SERVER_MODE", "False")
               .WithEnvironment("PGADMIN_CONFIG_MASTER_PASSWORD_REQUIRED", "False")
               .WithEnvironment("PGADMIN_CONFIG_ENHANCED_COOKIE_PROTECTION", "False")
               .WithEnvironment("PGADMIN_CONFIG_CONSOLE_LOG_LEVEL", "10")
               .ExcludeFromManifest(); // Use this instead of WithHealthCheck
    })
    .WithDataVolume("postgres-data");
// ============================================================
// DATABASES
// ============================================================

var authDb = postgres.AddDatabase("Auth-Mgr");
var hrmMDbCon = postgres.AddDatabase("core-HRMM");
var hrmmDb = postgres.AddDatabase("core-HRMMDb");
var moduleDb = postgres.AddDatabase("core-ModuleDb");
var financeDb = postgres.AddDatabase("core-FinanceDbCon");
var fileManagementDb = postgres.AddDatabase("core-FileManagementDb");
var crmDb = postgres.AddDatabase("core-CRMDbCon");
var procurementDb = postgres.AddDatabase("core-ProcurementDb");
var inventoryDb = postgres.AddDatabase("core-InventoryDb");
var planDevCon = postgres.AddDatabase("core-PlanDevDb");
var trainingDb = postgres.AddDatabase("HRM-TrainingDb");
var performanceDb = postgres.AddDatabase("HRM-PerformanceDb");
var hrmProCon = postgres.AddDatabase("HRM-Pro");
var projectManagementDb = postgres.AddDatabase("core-ProjectManagementDb");
var leaveDb = postgres.AddDatabase("HRM-Leave");
var recruitDb = postgres.AddDatabase("HRM-Recruit");
var taskDb = postgres.AddDatabase("Task-Mgr");
var notificationDb = postgres.AddDatabase("Notification-Mgr");
var payrollDb = postgres.AddDatabase("Payroll");
var attendanceDb = postgres.AddDatabase("Attendance");

// ============================================================
// SERVICE CONFIGURATION
// ============================================================

var localIp = "192.168.1.2";

// ============================================================
// MICROSERVICES - FIXED PORTS
// ============================================================

// Gateway - Port 5000
var gateway = builder.AddProject<Projects.Svc_Gateway>("svc-gateway")
    .WithHttpEndpoint(port: 5000, name: "http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:5000")  // ✅ FIXED
    .WithEnvironment("ServiceHost", localIp)
    .WithEnvironment("ServiceUrls__NotificationApi", "http://localhost:7007")
    .WithEnvironment("ServiceUrls__TaskApi", "http://localhost:7006");

// Auth - Port 7000
var auth = builder.AddProject<Projects.Svc_Auth>("svc-auth")
    .WithReference(rabbitmq)
    .WithReference(redis)
    .WithReference(authDb)
    .WithHttpEndpoint(port: 7000, name: "http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:7000")  // ✅ FIXED
    .WithEnvironment("ServiceHost", localIp)
    .WithEnvironment("RabbitMQ__Host", "192.168.1.2")
    .WithEnvironment("RabbitMQ__Port", "5672")
    .WithEnvironment("RabbitMQ__Username", "guest")
    .WithEnvironment("RabbitMQ__Password", "guest")
    .WithEnvironment("ServiceUrls__CoreModuleApi", "http://localhost:7002")
    .WithEnvironment("ServiceUrls__CoreHRMMApi", "http://localhost:7001")
    .WithEnvironment("ServiceUrls__HRMProApi", "http://localhost:7004")
   // .WithEnvironment("ConnectionStrings__authMgrCon", "Host=localhost;Port=5432;Database=Auth.Mgr;Username=postgres;Password=root")
    .WaitFor(postgres);

// Core HRMM - Port 7001
var coreHrmm = builder.AddProject<Projects.Cor_HRMM>("API-cor-hrmm")
    .WithReference(rabbitmq)
    .WithReference(hrmMDbCon)
    .WithHttpEndpoint(port: 7001, name: "http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:7001")  // ✅ FIXED
    .WithEnvironment("ServiceHost", localIp)
    .WithEnvironment("RabbitMQ__Host", "localhost")
    .WithEnvironment("RabbitMQ__Port", "5672")
    .WithEnvironment("RabbitMQ__Username", "guest")
    .WithEnvironment("RabbitMQ__Password", "guest")
    .WithEnvironment("ServiceUrls__CoreModuleApi", "http://localhost:7002")
    .WithEnvironment("ServiceUrls__AuthApi", "http://localhost:7000")
    .WithEnvironment("ServiceUrls__HRMProApi", "http://localhost:7004")
  //  .WithEnvironment("ConnectionStrings__coreHRMMDbCon", "Host=localhost;Port=5432;Database=core.HRMM;Username=postgres;Password=root")
    .WaitFor(postgres);

// Core Module - Port 7002
var coreModule = builder.AddProject<Projects.Cor_Module>("API-cor-module")
    .WithReference(rabbitmq)
    .WithReference(moduleDb)
    .WithHttpEndpoint(port: 7002, name: "http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:7002")  // ✅ FIXED
    .WithEnvironment("ServiceHost", localIp)
    .WithEnvironment("RabbitMQ__Host", "localhost")
    .WithEnvironment("RabbitMQ__Port", "5672")
    .WithEnvironment("RabbitMQ__Username", "guest")
    .WithEnvironment("RabbitMQ__Password", "guest")
    .WithEnvironment("ServiceUrls__CoreHRMMApi", "http://localhost:7001")
    .WithEnvironment("ServiceUrls__AuthApi", "http://localhost:7000")
    .WithEnvironment("ServiceUrls__HRMProApi", "http://localhost:7004")
   // .WithEnvironment("ConnectionStrings__CorModuleDbCon", "Host=localhost;Port=5432;Database=core.Module;Username=postgres;Password=root")
    .WaitFor(postgres);

// HRM Profile - Port 7004
var hrmProfile = builder.AddProject<Projects.Profile_API>("API-hrm-profile")
    .WithReference(rabbitmq)
    .WithReference(hrmProCon)
    .WithReference(redis)
    .WithHttpEndpoint(port: 7004, name: "http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:7004")  // ✅ FIXED
    .WithEnvironment("ServiceHost", localIp)
    .WithEnvironment("RabbitMQ__Host", "localhost")
    .WithEnvironment("RabbitMQ__Port", "5672")
    .WithEnvironment("RabbitMQ__Username", "guest")
    .WithEnvironment("RabbitMQ__Password", "guest")
    .WithEnvironment("ServiceUrls__CoreModuleApi", "http://localhost:7002")
    .WithEnvironment("ServiceUrls__CoreHRMMApi", "http://localhost:7001")
    .WithEnvironment("ServiceUrls__AuthApi", "http://localhost:7000")
  //  .WithEnvironment("ConnectionStrings__HRMProDbCon", "Host=localhost;Port=5432;Database=HRM.Pro;Username=postgres;Password=root")
    .WaitFor(postgres)
    .WaitFor(coreModule);

// HRM Leave - Port 7003
builder.AddProject<Projects.Leave_API>("API-hrm-leave")
    .WithReference(rabbitmq)
    .WithReference(leaveDb)
    .WithHttpEndpoint(port: 7003, name: "http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:7003")  // ✅ FIXED
    .WithEnvironment("ServiceHost", localIp)
    .WithEnvironment("RabbitMQ__Host", "localhost")
    .WithEnvironment("RabbitMQ__Port", "5672")
    .WithEnvironment("RabbitMQ__Username", "guest")
    .WithEnvironment("RabbitMQ__Password", "guest")
    .WithEnvironment("ServiceUrls__CoreModuleApi", "http://localhost:7002")
    .WithEnvironment("ServiceUrls__CoreHRMMApi", "http://localhost:7001")
    .WithEnvironment("ServiceUrls__HRMProApi", "http://localhost:7004")
   // .WithEnvironment("ConnectionStrings__HrmLeaveDbCon", "Host=localhost;Port=5432;Database=HRM.Leave;Username=postgres;Password=root")
    .WaitFor(postgres);

// HRM Recruit - Port 7005
builder.AddProject<Projects.Recruit_API>("API-hrm-recruit")
    .WithReference(rabbitmq)
    .WithReference(recruitDb)
    .WithHttpEndpoint(port: 7005, name: "http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:7005")  // ✅ FIXED
    .WithEnvironment("ServiceHost", localIp)
    .WithEnvironment("RabbitMQ__Host", "localhost")
    .WithEnvironment("RabbitMQ__Port", "5672")
    .WithEnvironment("RabbitMQ__Username", "guest")
    .WithEnvironment("RabbitMQ__Password", "guest")
    .WithEnvironment("ServiceUrls__CoreModuleApi", "http://localhost:7002")
    .WithEnvironment("ServiceUrls__CoreHRMMApi", "http://localhost:7001")
    .WithEnvironment("ServiceUrls__HRMProApi", "http://localhost:7004")
   // .WithEnvironment("ConnectionStrings__HrmRecruitDbCon", "Host=localhost;Port=5432;Database=HRM.Recruit;Username=postgres;Password=root")
    .WaitFor(postgres);

// Task - Port 7006
builder.AddProject<Projects.Svc_Task_Host>("svc-task")
    .WithReference(rabbitmq)
    .WithReference(redis)
    .WithReference(taskDb)
    .WithHttpEndpoint(port: 7006, name: "http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:7006")  // ✅ FIXED
    .WithEnvironment("ServiceHost", localIp)
    .WithEnvironment("RabbitMQ__Host", "localhost")
    .WithEnvironment("RabbitMQ__Port", "5672")
    .WithEnvironment("RabbitMQ__Username", "guest")
    .WithEnvironment("RabbitMQ__Password", "guest")
   // .WithEnvironment("ConnectionStrings__TaskDbCon", "Host=localhost;Port=5432;Database=Task;Username=postgres;Password=root")
    .WaitFor(postgres);

// Notification - Port 7007
builder.AddProject<Projects.Svc_Notification_Host>("svc-notification")
    .WithReference(rabbitmq)
    .WithReference(redis)
    .WithReference(notificationDb)
    .WithHttpEndpoint(port: 7007, name: "http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:7007")  // ✅ FIXED
    .WithEnvironment("ServiceHost", localIp)
    .WithEnvironment("RabbitMQ__Host", "localhost")
    .WithEnvironment("RabbitMQ__Port", "5672")
    .WithEnvironment("RabbitMQ__Username", "guest")
    .WithEnvironment("RabbitMQ__Password", "guest")
   // .WithEnvironment("ConnectionStrings__NotificationDbCon", "Host=localhost;Port=5432;Database=Notification;Username=postgres;Password=root")
    .WaitFor(postgres);

// Finance - Port 7008
builder.AddProject<Projects.Cor_Finance>("svc-finance")
    .WithReference(rabbitmq)
    .WithReference(redis)
    .WithReference(financeDb)
    .WithHttpEndpoint(port: 7008, name: "http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:7008")  // ✅ FIXED
    .WithEnvironment("ServiceHost", localIp)
    .WithEnvironment("RabbitMQ__Host", "localhost")
    .WithEnvironment("RabbitMQ__Port", "5672")
    .WithEnvironment("RabbitMQ__Username", "guest")
    .WithEnvironment("RabbitMQ__Password", "guest")
    .WithEnvironment("ServiceUrls__AuthApi", "http://localhost:7000")
    .WithEnvironment("ConnectionStrings__coreFinanceDbCon", "Host=localhost;Port=5432;Database=core.Finance;Username=postgres;Password=root;Include Error Detail=true")
  //  .WithEnvironment("ConnectionStrings__WarehouseDbCon", "Host=localhost;Port=5432;Database=core.FinanceWarehouse;Username=postgres;Password=root")
    .WaitFor(postgres);

// File Management - Port 7009
builder.AddProject<Projects.Cor_FileManagement>("filemanagement")
    .WithReference(rabbitmq)
    .WithReference(redis)
    .WithReference(fileManagementDb)
    .WithHttpEndpoint(port: 7009, name: "http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:7009")  // ✅ FIXED
    .WithEnvironment("ServiceHost", localIp)
    .WithEnvironment("RabbitMQ__Host", "localhost")
    .WithEnvironment("RabbitMQ__Port", "5672")
    .WithEnvironment("RabbitMQ__Username", "guest")
    .WithEnvironment("RabbitMQ__Password", "guest")
  //  .WithEnvironment("ConnectionStrings__FileManagementDbCon", "Host=localhost;Port=5432;Database=FileManagement;Username=postgres;Password=root")
    .WaitFor(postgres);

// Payroll - Port 7010
builder.AddProject<Projects.Svc_HRM_Payroll>("svc-payroll")
    .WithReference(rabbitmq)
    .WithReference(redis)
    .WithReference(payrollDb)
    .WithHttpEndpoint(port: 7010, name: "http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:7010")  // ✅ FIXED
    .WithEnvironment("ServiceHost", localIp)
    .WithEnvironment("RabbitMQ__Host", "localhost")
    .WithEnvironment("RabbitMQ__Port", "5672")
    .WithEnvironment("RabbitMQ__Username", "guest")
    .WithEnvironment("RabbitMQ__Password", "guest")
    .WithEnvironment("ServiceUrls__AuthApi", "http://localhost:7000")
   // .WithEnvironment("ConnectionStrings__PayrollDbCon", "Host=localhost;Port=5432;Database=Payroll;Username=postgres;Password=root")
    .WaitFor(postgres);

// Attendance - Port 7011
builder.AddProject<Projects.Svc_HRM_Attendance>("svc-attendance")
    .WithReference(rabbitmq)
    .WithReference(redis)
    .WithReference(attendanceDb)
    .WithHttpEndpoint(port: 7011, name: "http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:7011")  // ✅ FIXED
    .WithEnvironment("ServiceHost", localIp)
    .WithEnvironment("RabbitMQ__Host", "localhost")
    .WithEnvironment("RabbitMQ__Port", "5672")
    .WithEnvironment("RabbitMQ__Username", "guest")
    .WithEnvironment("RabbitMQ__Password", "guest")
    .WithEnvironment("ServiceUrls__AuthApi", "http://localhost:7000")
    .WithEnvironment("ServiceUrls__CoreModuleApi", "http://localhost:7002")
    .WithEnvironment("ServiceUrls__CoreHRMMApi", "http://localhost:7001")
    .WithEnvironment("ServiceUrls__HRMProApi", "http://localhost:7004")
  //  .WithEnvironment("ConnectionStrings__AttendanceDbCon", "Host=localhost;Port=5432;Database=Attendance;Username=postgres;Password=root")
    .WaitFor(postgres);

// Training - Port 5007
builder.AddProject<Projects.Svc_HRM_Training>("training")
    .WithReference(trainingDb)
    .WithHttpEndpoint(port: 5007, name: "http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:5007")  // ✅ FIXED
    .WithEnvironment("ServiceHost", localIp)
   // .WithEnvironment("ConnectionStrings__HrmTrainingDbCon", "Host=localhost;Port=5432;Database=HRM.Training;Username=postgres;Password=root")
    .WaitFor(postgres);

// Performance - Port 5005
builder.AddProject<Projects.Svc_HRM_Performance>("performance")
    .WithReference(performanceDb)
    .WithHttpEndpoint(port: 5005, name: "http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:5005")  // ✅ FIXED
    .WithEnvironment("ServiceHost", localIp)
  //  .WithEnvironment("ConnectionStrings__HrmPerformanceDbCon", "Host=localhost;Port=5432;Database=HRM.Performance;Username=postgres;Password=root")
    .WaitFor(postgres);

// CRM - Port 7012
builder.AddProject<Projects.Cor_CRM>("svc-crm")
    .WithReference(rabbitmq)
    .WithReference(redis)
    .WithReference(crmDb)
    .WithHttpEndpoint(port: 7012, name: "http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:7012")  // ✅ FIXED
    .WithEnvironment("ServiceHost", localIp)
    .WithEnvironment("RabbitMQ__Host", "localhost")
    .WithEnvironment("RabbitMQ__Port", "5672")
    .WithEnvironment("RabbitMQ__Username", "guest")
    .WithEnvironment("RabbitMQ__Password", "guest")
    .WithEnvironment("ServiceUrls__AuthApi", "http://localhost:7000")
    .WithEnvironment("ServiceUrls__CoreModuleApi", "http://localhost:7002")
    .WithEnvironment("ServiceUrls__CoreHRMMApi", "http://localhost:7001")
    .WithEnvironment("ServiceUrls__HRMProApi", "http://localhost:7004")
    //.WithEnvironment("ConnectionStrings__CrmDbCon", "Host=localhost;Port=5432;Database=CRM;Username=postgres;Password=root")
    .WaitFor(postgres);

// Procurement - Port 7013
builder.AddProject<Projects.Cor_Procurement>("procurement")
    .WithReference(rabbitmq)
    .WithReference(redis)
    .WithReference(procurementDb)
    .WithHttpEndpoint(port: 7013, name: "http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:7013")  // ✅ FIXED
    .WithEnvironment("ServiceHost", localIp)
    .WithEnvironment("RabbitMQ__Host", "localhost")
    .WithEnvironment("RabbitMQ__Port", "5672")
    .WithEnvironment("RabbitMQ__Username", "guest")
    .WithEnvironment("RabbitMQ__Password", "guest")
    .WithEnvironment("AuthUrl", $"http://{localIp}:7000")
   // .WithEnvironment("ConnectionStrings__ProcurementDbCon", "Host=localhost;Port=5432;Database=Procurement;Username=postgres;Password=root")
    .WaitFor(postgres);

// Inventory - Port 7014
builder.AddProject<Projects.Cor_Inventory>("svc-inventory")
    .WithReference(rabbitmq)
    .WithReference(redis)
    .WithReference(inventoryDb)
    .WithHttpEndpoint(port: 7014, name: "http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:7014")  // ✅ FIXED
    .WithEnvironment("ServiceHost", localIp)
    .WithEnvironment("RabbitMQ__Host", "localhost")
    .WithEnvironment("RabbitMQ__Port", "5672")
    .WithEnvironment("RabbitMQ__Username", "guest")
    .WithEnvironment("RabbitMQ__Password", "guest")
    .WithEnvironment("AuthUrl", $"http://{localIp}:7000")
   // .WithEnvironment("ConnectionStrings__InventoryDbCon", "Host=localhost;Port=5432;Database=Inventory;Username=postgres;Password=root")
    .WaitFor(postgres);

// Plan & Development - Port 7015
builder.AddProject<Projects.Cor_PlanDev>("planDev")
    .WithReference(rabbitmq)
    .WithReference(redis)
    .WithReference(planDevCon)
    .WithHttpEndpoint(port: 7015, name: "http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:7015")  // ✅ FIXED
    .WithEnvironment("ServiceHost", localIp)
    .WithEnvironment("RabbitMQ__Host", "localhost")
    .WithEnvironment("RabbitMQ__Port", "5672")
    .WithEnvironment("RabbitMQ__Username", "guest")
    .WithEnvironment("RabbitMQ__Password", "guest")
    .WithEnvironment("AuthUrl", $"http://{localIp}:7000")
   // .WithEnvironment("ConnectionStrings__PlanDevDbCon", "Host=localhost;Port=5432;Database=PlanDev;Username=postgres;Password=root")
    .WaitFor(postgres);

// Project Management - Port 7016
builder.AddProject<Projects.Cor_ProjectManagement>("projectmanagement")
    .WithReference(rabbitmq)
    .WithReference(redis)
    .WithReference(projectManagementDb)
    .WithHttpEndpoint(port: 7016, name: "http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:7016")  // ✅ FIXED
    .WithEnvironment("ServiceHost", localIp)
    .WithEnvironment("RabbitMQ__Host", "localhost")
    .WithEnvironment("RabbitMQ__Port", "5672")
    .WithEnvironment("RabbitMQ__Username", "guest")
    .WithEnvironment("RabbitMQ__Password", "guest")
    .WithEnvironment("AuthUrl", $"http://{localIp}:7000")
    .WithEnvironment("FinanceApiUrl", $"http://{localIp}:7008")
    .WithEnvironment("CoreApiUrl", $"http://{localIp}:7002")
    .WithEnvironment("HrmApiUrl", $"http://{localIp}:7001")
    .WithEnvironment("CrmApiUrl", $"http://{localIp}:7012")
    .WithEnvironment("InventoryApiUrl", $"http://{localIp}:7014")
    .WithEnvironment("ProcurementApiUrl", $"http://{localIp}:7013")
   // .WithEnvironment("ConnectionStrings__ProjectManagementDbCon", "Host=localhost;Port=5432;Database=ProjectManagement;Username=postgres;Password=root")
    .WaitFor(postgres)
    .WaitFor(coreModule);

// ============================================================
// DOCKER COMPOSE GENERATION
// ============================================================

if (builder.ExecutionContext.IsPublishMode)
{
    builder.AddDockerComposeEnvironment("prod-env");
}

builder.Build().Run();