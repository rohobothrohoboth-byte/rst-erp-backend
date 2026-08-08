
using MediatR;
using Cor.FileManagement.Models.DTOs;
namespace Cor.FileManagement.Commands;

public class RemoveShareCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid RemovedBy { get; set; }
}