// gRPCService/ProjectGrpcService.cs - Minimal version
using Grpc.Core;
using Cor.ProjectManagement.Services;
using Cor.ProjectManagement.Models.DTOs;

namespace Cor.ProjectManagement.gRPCService
{
    // Define a simple service class without proto dependency
    public class ProjectGrpcService
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<ProjectGrpcService> _logger;

        public ProjectGrpcService(
            IProjectService projectService,
            ILogger<ProjectGrpcService> logger)
        {
            _projectService = projectService;
            _logger = logger;
        }

        // Simple methods without gRPC proto
        public async Task<object> GetProjectAsync(string id)
        {
            try
            {
                var project = await _projectService.GetProjectByIdAsync(Guid.Parse(id));
                return new
                {
                    Id = project.Id.ToString(),
                    Name = project.Name,
                    Code = project.Code,
                    Status = project.Status.ToString(),
                    StartDate = project.StartDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    Budget = project.Budget,
                    CompletionPercentage = project.CompletionPercentage
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in gRPC GetProject for ID: {ProjectId}", id);
                throw;
            }
        }
    }
}