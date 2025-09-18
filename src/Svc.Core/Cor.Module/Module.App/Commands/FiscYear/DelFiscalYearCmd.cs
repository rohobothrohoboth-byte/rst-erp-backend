using Module.App.Interfaces;
using Module.Domain.Entities;
using MediatR;

namespace Module.App.Commands.FiscYear;

public class DelFiscalYearCmd : IRequest { public Guid Id { get; set; } }

public class DelFiscalYearCmdHandler : IRequestHandler<DelFiscalYearCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public DelFiscalYearCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(DelFiscalYearCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<FiscalYear>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}
