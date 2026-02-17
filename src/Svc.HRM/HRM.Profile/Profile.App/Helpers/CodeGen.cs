using Helpers;
using Profile.App.Interfaces;
using Profile.Domain.Entities;

namespace Profile.App.Helpers;

public class CodeGen
{
    private readonly IUnitOfWork _unitOfWork;
    public CodeGen(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<string> GetEmpCode()
    {
        var validBraId = string.Empty;
        var cGen = new NumToWord().IdGenerator(10);
        var allEmp = await _unitOfWork.Repository<Employee>().GetAll();
        if (allEmp.Any())
        {
            var validId = allEmp.FirstOrDefault(i => i.Code == cGen);
            if (validId != null) { await GetEmpCode(); }
            else { validBraId = cGen; }
        }
        else
        {
            validBraId = cGen;
        }

        return validBraId;
    }




}