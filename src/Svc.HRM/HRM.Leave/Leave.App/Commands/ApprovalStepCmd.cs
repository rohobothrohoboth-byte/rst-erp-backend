using Leave.App.Helpers;
using Leave.App.Interfaces;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Commands;

public class ApprovalStepAddCmd : IRequest<ApprovalStepListDto> { public ApprovalStepAddDto AddDto { get; set; } = default!; }
public class ApprovalStepModCmd : IRequest<ApprovalStepListDto> { public ApprovalStepModDto ModDto { get; set; } = default!; }
public class ApprovalStepDelCmd : IRequest { public Guid Id { get; set; } }

public class ApprovalStepAddCmdHandler : IRequestHandler<ApprovalStepAddCmd, ApprovalStepListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public ApprovalStepAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<ApprovalStepListDto> Handle(ApprovalStepAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = new ApprovalStep
            {
                LeaveRequestId = request.AddDto.LeaveRequestId,
                StepOrder = request.AddDto.StepOrder,
                IsApproved = request.AddDto.IsApproved,
                Date = request.AddDto.Date,
                Comments = request.AddDto.Comments
            };
            await _unitOfWork.Repository<ApprovalStep>().Add(data);
            await _unitOfWork.Commit();

            var res = new ApprovalStepListDto();
            var response = await _med.Send(new ApprovalStepByIdQry { Id = data.Id }, cancellationToken);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class ApprovalStepModCmdHandler : IRequestHandler<ApprovalStepModCmd, ApprovalStepListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public ApprovalStepModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<ApprovalStepListDto> Handle(ApprovalStepModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<ApprovalStep>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"APPROVAL STEP with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();

        try
        {
            oldData.ApprovedById = request.ModDto.ApprovedById;
            oldData.LeaveRequestId = request.ModDto.LeaveRequestId;
            oldData.StepOrder = request.ModDto.StepOrder;
            oldData.IsApproved = request.ModDto.IsApproved;
            oldData.Date = request.ModDto.Date;
            oldData.Comments = request.ModDto.Comments;
            var data = await _unitOfWork.Repository<ApprovalStep>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new ApprovalStepListDto();
            var response = await _med.Send(new ApprovalStepByIdQry { Id = data.Id }, cancellationToken);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class ApprovalStepDelCmdHandler : IRequestHandler<ApprovalStepDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public ApprovalStepDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(ApprovalStepDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<ApprovalStep>().GetById(request.Id);
            if (data == null) { throw new DomainException($"APPROVAL STEP with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<ApprovalStep>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}