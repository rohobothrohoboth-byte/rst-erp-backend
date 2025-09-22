namespace Module.App.Services;

public interface ILupClient
{
    Task<LupListDto?> GetQuarter(Guid id);

}
