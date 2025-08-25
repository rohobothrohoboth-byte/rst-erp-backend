using Cor.App.Interfaces;
using Cor.Domain.Entities;
using MediatR;

namespace Cor.App.Commands.Comp;

public class DelCompCmd : IRequest { public Guid Id { get; set; } }

public class DelCompCmdHandler : IRequestHandler<DelCompCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public DelCompCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(DelCompCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<Company>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}