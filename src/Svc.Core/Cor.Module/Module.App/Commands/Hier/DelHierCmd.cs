using Module.App.Interfaces;
using Module.Domain.Entities;
using MediatR;

namespace Module.App.Commands.Hier;

public class DelHierCmd : IRequest { public Guid Id { get; set; } }

public class DelHierCmdHandler : IRequestHandler<DelHierCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public DelHierCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(DelHierCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<Hierarchy>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}