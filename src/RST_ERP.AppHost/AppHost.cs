var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Svc_Lup>("API-lup");

builder.AddProject<Projects.Svc_Gateway>("svc-gateway");

builder.AddProject<Projects.Cor_HRMM>("API-cor-hrmm");

builder.AddProject<Projects.Cor_Module>("API-cor-module");

builder.AddProject<Projects.Leave_API>("API-hrm-leave");

builder.AddProject<Projects.Profile_API>("API-hrm-profile");

builder.AddProject<Projects.Svc_Auth>("svc-auth");

builder.Build().Run();
