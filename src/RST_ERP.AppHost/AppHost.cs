var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Svc_Lup>("svc-lup");

builder.AddProject<Projects.Svc_Gateway>("svc-gateway");

builder.AddProject<Projects.Cor_HRMM>("cor-hrmm");

builder.AddProject<Projects.Cor_Module>("cor-module");

builder.AddProject<Projects.Leave_API>("leave-api");

builder.AddProject<Projects.Profile_API>("profile-api");

builder.AddProject<Projects.Svc_Auth>("svc-auth");

builder.Build().Run();
