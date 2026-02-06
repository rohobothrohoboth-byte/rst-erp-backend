using Cor.Module.Interfaces;
using Cor.Module.Models.Entities;
using Helpers;

namespace Cor.Module.Helpers;

public class BraCode
{
    private readonly IUnitOfWork _unitOfWork;

    public BraCode(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<string> GetBraCode()
    {
        var validBraId = string.Empty;
        var gen = new NumToWord();
        var cGen = $"BR-{gen.NumGenerator(5)}";
        var allEmp = await _unitOfWork.Repository<Branch>().GetAll();
        if (allEmp.Any())
        {
            var validId = allEmp.FirstOrDefault(i => i.Code == cGen);
            if (validId != null) { await GetBraCode(); }
            else { validBraId = cGen; }
        }
        else
        {
            validBraId = cGen;
        }

        return validBraId;
    }




}
