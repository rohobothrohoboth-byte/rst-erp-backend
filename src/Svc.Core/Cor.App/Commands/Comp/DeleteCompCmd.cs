using Cor.App.Interfaces;
using Cor.Domain.Entities;
using MassTransit;
using MediatR;
using RST.Cont;

namespace Cor.App.Commands.Comp;

public class DeleteCompCmd : IRequest
{
    public Guid Id { get; set; }
}

public class DeleteCompCmdHandler : IRequestHandler<DeleteCompCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _iPubEndpoint;

    public DeleteCompCmdHandler(IUnitOfWork unitOfWork, IPublishEndpoint iPubEndpoint)
    {
        _unitOfWork = unitOfWork;
        _iPubEndpoint = iPubEndpoint;
    }

    public async Task Handle(DeleteCompCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<Company>().Delete(request.Id);
            var puComp = new CompDeleted
            {
                Id = request.Id
            };
            await _iPubEndpoint.Publish(puComp, cancellationToken);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}
