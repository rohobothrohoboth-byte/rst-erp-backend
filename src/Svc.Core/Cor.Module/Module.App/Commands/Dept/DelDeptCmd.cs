using Module.App.Interfaces;
using Module.Domain.Entities;
using MediatR;

namespace Module.App.Commands.Dept;

public class DelDeptCmd : IRequest { public Guid Id { get; set; } }

public class DelDeptCmdHandler : IRequestHandler<DelDeptCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public DelDeptCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(DelDeptCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<Department>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}
