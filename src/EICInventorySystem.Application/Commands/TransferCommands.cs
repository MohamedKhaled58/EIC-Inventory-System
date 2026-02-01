using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using MediatR;

namespace EICInventorySystem.Application.Commands;

// Transfer Commands
public record CreateTransferCommand(CreateTransferDto Request, int UserId) : IRequest<TransferDto>;
public record ApproveTransferCommand(int Id, string? Notes, int UserId) : IRequest<TransferDto>;
public record RejectTransferCommand(int Id, string Reason, int UserId) : IRequest<TransferDto>;
public record ShipTransferCommand(int Id, int UserId) : IRequest<TransferDto>;
public record ReceiveTransferCommand(int Id, string? Notes, int UserId) : IRequest<TransferDto>;

// Transfer Command Handlers
public class CreateTransferCommandHandler : IRequestHandler<CreateTransferCommand, TransferDto>
{
    private readonly ITransferService _transferService;

    public CreateTransferCommandHandler(ITransferService transferService)
    {
        _transferService = transferService;
    }

    public async Task<TransferDto> Handle(CreateTransferCommand request, CancellationToken cancellationToken)
    {
        return await _transferService.CreateTransferAsync(request.Request, request.UserId, cancellationToken);
    }
}

public class ApproveTransferCommandHandler : IRequestHandler<ApproveTransferCommand, TransferDto>
{
    private readonly ITransferService _transferService;

    public ApproveTransferCommandHandler(ITransferService transferService)
    {
        _transferService = transferService;
    }

    public async Task<TransferDto> Handle(ApproveTransferCommand request, CancellationToken cancellationToken)
    {
        var dto = new ApproveTransferDto
        {
            Id = request.Id,
            Notes = request.Notes
        };
        return await _transferService.ApproveTransferAsync(dto, request.UserId, cancellationToken);
    }
}

public class RejectTransferCommandHandler : IRequestHandler<RejectTransferCommand, TransferDto>
{
    private readonly ITransferService _transferService;

    public RejectTransferCommandHandler(ITransferService transferService)
    {
        _transferService = transferService;
    }

    public async Task<TransferDto> Handle(RejectTransferCommand request, CancellationToken cancellationToken)
    {
        var dto = new RejectTransferDto
        {
            Id = request.Id,
            RejectionReason = request.Reason
        };
        return await _transferService.RejectTransferAsync(dto, request.UserId, cancellationToken);
    }
}

public class ShipTransferCommandHandler : IRequestHandler<ShipTransferCommand, TransferDto>
{
    private readonly ITransferService _transferService;

    public ShipTransferCommandHandler(ITransferService transferService)
    {
        _transferService = transferService;
    }

    public async Task<TransferDto> Handle(ShipTransferCommand request, CancellationToken cancellationToken)
    {
        var dto = new ShipTransferDto
        {
            Id = request.Id
        };
        return await _transferService.ShipTransferAsync(dto, request.UserId, cancellationToken);
    }
}

public class ReceiveTransferCommandHandler : IRequestHandler<ReceiveTransferCommand, TransferDto>
{
    private readonly ITransferService _transferService;

    public ReceiveTransferCommandHandler(ITransferService transferService)
    {
        _transferService = transferService;
    }

    public async Task<TransferDto> Handle(ReceiveTransferCommand request, CancellationToken cancellationToken)
    {
        var dto = new ReceiveTransferDto
        {
            Id = request.Id,
            Notes = request.Notes
        };
        return await _transferService.ReceiveTransferAsync(dto, request.UserId, cancellationToken);
    }
}
