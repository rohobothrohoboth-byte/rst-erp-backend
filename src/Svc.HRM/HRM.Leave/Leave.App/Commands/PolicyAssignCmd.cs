using Common;
using Leave.App.Interfaces;
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

    public async Task<int> Handle(PolicyAssignCmd request, CancellationToken cancellationToken)
    {
        var assignedCount = 0;
        var empL = await _hrmPro.GetListEmp(cancellationToken);
        if (empL.Res.Count > 0)
        {

        }


        return assignedCount;
    }
}