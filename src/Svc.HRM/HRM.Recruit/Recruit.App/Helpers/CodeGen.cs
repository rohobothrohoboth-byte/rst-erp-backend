using Helpers;
using Recruit.App.Interfaces;
using Recruit.Domain.Entities;

namespace Recruit.App.Helpers;

public class CodeGen
{
    private readonly IUnitOfWork _unitOfWork;
    public CodeGen(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<string> GetPlanCode()
    {
        var validBraId = string.Empty;
        var cGen = new NumToWord().IdGenerator(10);
        var allEmp = await _unitOfWork.Repository<WorkforcePlan>().GetAll();
        if (allEmp.Any())
        {
            var validId = allEmp.FirstOrDefault(i => i.PlanCode == cGen);
            if (validId != null) { await GetPlanCode(); }
            else { validBraId = cGen; }
        }
        else
        {
            validBraId = cGen;
        }

        return validBraId;
    }

    public async Task<string> GetPostNumber()
    {
        var validBraId = string.Empty;
        var cGen = new NumToWord().IdGenerator(10);
        var allEmp = await _unitOfWork.Repository<JobPosting>().GetAll();
        if (allEmp.Any())
        {
            var validId = allEmp.FirstOrDefault(i => i.PostNumber == cGen);
            if (validId != null) { await GetPostNumber(); }
            else { validBraId = cGen; }
        }
        else
        {
            validBraId = cGen;
        }

        return validBraId;
    }




}