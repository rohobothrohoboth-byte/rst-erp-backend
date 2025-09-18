using Svc.Shared.DTOs;

namespace Svc.Shared.Services;

public interface IRegionClient { Task<LupListDto?> GetRegion(Guid id); }
