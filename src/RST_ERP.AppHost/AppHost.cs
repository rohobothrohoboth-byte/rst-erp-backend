using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

var builder = DistributedApplication.CreateBuilder(args);

// ✅ RabbitMQ - Make sure it starts first
var rabbitmq = builder.AddConnectionString("rabbitmq");
builder.Configuration["ConnectionStrings:rabbitmq"] = "amqp://guest:guest@localhost:5672";

// ✅ Redis - FIXED: No TLS, standard port 6379
var redisPassword = "kRrwYqqHWnnZpMhnn30way";
var redis = builder.AddRedis("redis")
    .WithRedisCommander()
    .WithEnvironment("REDIS_PASSWORD", redisPassword)
    .WithEnvironment("REDIS_ARGS", $"--requirepass {redisPassword}");

// ✅ PostgreSQL
var postgres = builder.AddPostgres("postgres")
    .WithImage("postgres:16-alpine")
    .WithEnvironment("POSTGRES_USER", "postgres")
    .WithEnvironment("POSTGRES_PASSWORD", "root")
    .WithPgAdmin()
    .WithEndpoint("tcp", endpoint =>
    {
        endpoint.Port = 5432;
        endpoint.TargetPort = 5432;
    });

// ✅ Databases
var hrmmDb = postgres.AddDatabase("hrmmDb", "core.HRMMDb");
var moduleDb = postgres.AddDatabase("moduleDb", "core.ModuleDb");
var financeDb = postgres.AddDatabase("financeDb", "core.FinanceDbCon");
var fileManagementDb = postgres.AddDatabase("fileManagementDb", "core.FileManagementDb");
var crmDb = postgres.AddDatabase("crmDb", "core.CRMDbCon");
var procurementDb = postgres.AddDatabase("procurementDb", "core.ProcurementDb");
var inventoryDb = postgres.AddDatabase("inventorydb","core.InventoryDb");
var planDevDbCon = postgres.AddDatabase("planDevDbCon","core.PlanDevDb");

// ============= SERVICES WITH FIXED PORTS =============

// ✅ Get the local IP address
var localIp = "192.168.1.7"; // Updated to match your config
var useHttps = true;

// Helper function to get service URL
string GetServiceUrl(string host, int port, bool https = true)
{
    var protocol = https ? "https" : "http";
    return $"{protocol}://{host}:{port}";
}

// ✅ Gateway
builder.AddProject<Projects.Svc_Gateway>("svc-gateway")
    .WithHttpEndpoint(port: 5000, name: "http")
    .WithHttpsEndpoint(port: 5001, name: "https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ServiceHost", localIp);

// ✅ Auth
builder.AddProject<Projects.Svc_Auth>("svc-auth")
    .WithReference(rabbitmq)
    .WithReference(redis)
    .WithHttpsEndpoint(port: 7000, name: "https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ServiceHost", localIp);

// ✅ Core HRMM
builder.AddProject<Projects.Cor_HRMM>("API-cor-hrmm")
    .WithReference(rabbitmq)
    .WithReference(hrmmDb)
    .WithHttpsEndpoint(port: 7001, name: "https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ServiceHost", localIp);

// ✅ Core Module
builder.AddProject<Projects.Cor_Module>("API-cor-module")
    .WithReference(rabbitmq)
    .WithReference(moduleDb)
    .WithHttpsEndpoint(port: 7002, name: "https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ServiceHost", localIp);

// ✅ Profile
builder.AddProject<Projects.Profile_API>("API-hrm-profile")
    .WithReference(rabbitmq)
    .WithHttpsEndpoint(port: 7004, name: "https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ServiceHost", localIp);

// ✅ Leave
builder.AddProject<Projects.Leave_API>("API-hrm-leave")
    .WithReference(rabbitmq)
    .WithHttpsEndpoint(port: 7003, name: "https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ServiceHost", localIp);

// ✅ Recruit
builder.AddProject<Projects.Recruit_API>("API-hrm-recruit")
    .WithReference(rabbitmq)
    .WithHttpsEndpoint(port: 7005, name: "https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ServiceHost", localIp);

// ✅ Task
builder.AddProject<Projects.Svc_Task_Host>("svc-task")
    .WithReference(rabbitmq)
    .WithReference(redis)
    .WithHttpsEndpoint(port: 7006, name: "https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ServiceHost", localIp);

// ✅ Notification
builder.AddProject<Projects.Svc_Notification_Host>("svc-notification")
    .WithReference(rabbitmq)
    .WithReference(redis)
    .WithHttpsEndpoint(port: 7007, name: "https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ServiceHost", localIp);

// ✅ Finance
builder.AddProject<Projects.Cor_Finance>("finance")
    .WithReference(rabbitmq)
    .WithReference(redis)
    .WithReference(financeDb)
    .WithHttpsEndpoint(port: 7008, name: "https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ServiceHost", localIp);

// ✅ File Management
builder.AddProject<Projects.Cor_FileManagement>("filemanagement")
    .WithReference(rabbitmq)
    .WithReference(redis)
    .WithReference(fileManagementDb)
    .WithHttpsEndpoint(port: 7009, name: "https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ServiceHost", localIp);

// ✅ Payroll
builder.AddProject<Projects.Svc_HRM_Payroll>("payroll")
    .WithReference(rabbitmq)
    .WithReference(redis)
    .WithHttpsEndpoint(port: 7010, name: "https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ServiceHost", localIp);

// ✅ Attendance
builder.AddProject<Projects.Svc_HRM_Attendance>("attendance")
    .WithReference(rabbitmq)
    .WithReference(redis)
    .WithHttpsEndpoint(port: 7011, name: "https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ServiceHost", localIp);

// ✅ Performance
builder.AddProject<Projects.Svc_HRM_Performance>("performance")
    .WithHttpsEndpoint(port: 7016, name: "https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ServiceHost", localIp);

// ✅ Training
builder.AddProject<Projects.Svc_HRM_Training>("training")
    .WithHttpsEndpoint(port: 7017, name: "https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ServiceHost", localIp);

// ✅ HR Reports
builder.AddProject<Projects.Svc_HRM_Reports>("hr-reports")
    .WithHttpsEndpoint(port: 7018, name: "https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ServiceHost", localIp);

// ✅ CRM
builder.AddProject<Projects.Cor_CRM>("crm")
    .WithReference(rabbitmq)
    .WithReference(redis)
    .WithReference(crmDb)
    .WithHttpsEndpoint(port: 7012, name: "https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ServiceHost", localIp);

// ✅ ✅ PROCUREMENT (New Service)
builder.AddProject<Projects.Cor_Procurement>("procurement")
    .WithReference(rabbitmq)
    .WithReference(redis)
    .WithReference(procurementDb)
    .WithHttpsEndpoint(port: 7013, name: "https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ServiceHost", localIp)
    .WithEnvironment("AuthUrl", $"https://{localIp}:7000");

builder.AddProject<Projects.Cor_Inventory>("inventory")
    .WithReference(rabbitmq)
    .WithReference(redis)
    .WithReference(inventoryDb)
    .WithHttpsEndpoint(port: 7014, name: "https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ServiceHost", localIp)
    .WithEnvironment("AuthUrl", $"https://{localIp}:7000");

    // Plan & Development Service
    builder.AddProject<Projects.Cor_PlanDev>("planDev")
        .WithReference(rabbitmq)
        .WithReference(redis)
        .WithReference(planDevDbCon)
        .WithHttpsEndpoint(port: 7015, name: "https")
        .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
        .WithEnvironment("ServiceHost", localIp)
        .WithEnvironment("AuthUrl", $"https://{localIp}:7000");

builder.Build().Run();