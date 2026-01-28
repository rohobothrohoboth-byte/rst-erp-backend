using Common;
using Leave.App.Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using Leave.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leave.App.Commands;

public class PolicyAssignCmd : IRequest<int> { }

public class PolicyAssignHandler : IRequestHandler<PolicyAssignCmd, int>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHrmProfileClient _hrmPro;
    public PolicyAssignHandler(IUnitOfWork unitOfWork, IHrmProfileClient hrmPro)
    {
        _unitOfWork = unitOfWork;
        _hrmPro = hrmPro;
    }


    private async Task<List<EmpPolicyCtx>> GetCondList()
    {
        var empList = new List<EmpPolicyCtx>();
        var stat = BoolToStr.EnumToString(PolicyStatus.Active);
        var poL = (await _unitOfWork.Repository<LeavePolicy>().Find(p => p.Status == stat)).Select(p => p.Id).ToList();
        if (poL.Count > 0)
        {
            var pAss = (await _unitOfWork.Repository<PolicyAssignmentRule>().Find(p => poL.Contains(p.LeavePolicyId) && p.IsActive == true)).ToList();
            if (pAss.Count > 0)
            {
                foreach (var po in pAss)
                {
                    var condL = (await _unitOfWork.Repository<PolicyRuleCondition>().Find(p => p.PolicyAssignmentRuleId == po.Id)).ToList();
                    if (condL.Count > 0)
                    {
                        foreach (var cond in condL)
                        {
                            
                        }
                    }
                }
            }
        }

        return empList;
    }


    private async Task<List<EmpPolicyCtx>> GetEmpList(CancellationToken cancellationToken)
    {
        var empList = new List<EmpPolicyCtx>();
        var empL = await _hrmPro.GetListEmpPolicy(cancellationToken);
        if (empL.Res.Count > 0)
        {
            foreach (var emp in empL.Res)
            {
                var vm = new EmpPolicyCtx
                {
                    EmployeeId = Guid.Parse(emp.Id),
                    Name = emp.Name,
                    SerYear = double.Parse(emp.SerYear),
                    EmpType = emp.EmpType,
                    WorkAr = emp.WorkAr,
                    Gender = emp.Gender,
                    Jg = emp.Jg,
                };
                empList.Add(vm);
            }
        }

        return empList;
    }

    public async Task<int> Handle(PolicyAssignCmd request, CancellationToken cancellationToken)
    {
        var assignedCount = 0;
        var empL = await GetEmpList(cancellationToken);

        //var empL = await _hrmPro.GetListEmpPolicy(cancellationToken);
        //if (empL.Res.Count > 0)
        //{
        //    var empList = new List<EmpPolicyCtx>();
        //    foreach (var emp in empL.Res)
        //    {
        //        var vm = new EmpPolicyCtx
        //        {
        //            EmployeeId = Guid.Parse(emp.Id),
        //            Name = emp.Name,
        //            SerYear = double.Parse(emp.SerYear),
        //            EmpType = emp.EmpType,
        //            WorkAr = emp.WorkAr,
        //            Gender = emp.Gender,
        //            Jg = emp.Jg,
        //        };
        //        empList.Add(vm);
        //    }
        //}


        return assignedCount;
    }
}