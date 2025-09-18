using Module.App.Interfaces;
using Module.Domain.Entities;
using MediatR;

namespace Module.App.Commands.BranchOff;

public class DelBranchCmd : IRequest { public Guid Id { get; set; } }

public class DelBranchCmdHandler : IRequestHandler<DelBranchCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public DelBranchCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(DelBranchCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<Branch>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}
