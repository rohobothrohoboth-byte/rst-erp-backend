using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Module.App.Interfaces;
using Module.Domain.Entities;

namespace Module.App.Helpers;


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
