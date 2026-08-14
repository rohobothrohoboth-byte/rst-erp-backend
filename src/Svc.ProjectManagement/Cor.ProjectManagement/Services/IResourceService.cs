// Services/IResourceService.cs
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Services
{
    public interface IResourceService
    {
        Task<ProjectResourceDto> AllocateResourceAsync(AllocateResourceDto dto);
        Task<ProjectResourceDto> UpdateResourceAllocationAsync(Guid id, UpdateResourceAllocationDto dto);
        Task<bool> ReleaseResourceAsync(Guid id, ReleaseResourceDto dto);
        Task<List<ProjectResourceDto>> GetResourcesByProjectAsync(Guid projectId, ResourceAllocationStatus? status);
        Task<PaginatedResponse<ProjectResourceDto>> GetResourceAllocationsAsync(ResourceFilterDto filter);
    }

    public class ResourceService : IResourceService
    {
        public Task<ProjectResourceDto> AllocateResourceAsync(AllocateResourceDto dto) => throw new NotImplementedException();
        public Task<ProjectResourceDto> UpdateResourceAllocationAsync(Guid id, UpdateResourceAllocationDto dto) => throw new NotImplementedException();
        public Task<bool> ReleaseResourceAsync(Guid id, ReleaseResourceDto dto) => throw new NotImplementedException();
        public Task<List<ProjectResourceDto>> GetResourcesByProjectAsync(Guid projectId, ResourceAllocationStatus? status) => throw new NotImplementedException();
        public Task<PaginatedResponse<ProjectResourceDto>> GetResourceAllocationsAsync(ResourceFilterDto filter) => throw new NotImplementedException();
    }
}