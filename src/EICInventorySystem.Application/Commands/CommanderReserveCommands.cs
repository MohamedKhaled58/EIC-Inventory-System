using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using MediatR;

namespace EICInventorySystem.Application.Commands;

public record CreateCommanderReserveRequestCommand(CreateCommanderReserveRequestDto Request, int UserId) : IRequest<CommanderReserveRequestDto>;

public record ApproveCommanderReserveRequestCommand(ApproveCommanderReserveRequestDto Request, int UserId) : IRequest<CommanderReserveRequestDto>;

public record RejectCommanderReserveRequestCommand(RejectCommanderReserveRequestDto Request, int UserId) : IRequest<CommanderReserveRequestDto>;

public record ReleaseCommanderReserveCommand(ReleaseCommanderReserveDto Request, int UserId) : IRequest<CommanderReserveReleaseDto>;

public record AllocateCommanderReserveCommand(int ItemId, int WarehouseId, decimal Quantity, int UserId) : IRequest<bool>;

public record AdjustCommanderReserveCommand(int ItemId, int WarehouseId, decimal NewReserveQuantity, string Reason, int UserId) : IRequest<bool>;

public class CreateCommanderReserveRequestCommandHandler : IRequestHandler<CreateCommanderReserveRequestCommand, CommanderReserveRequestDto>
{
    private readonly ICommanderReserveService _commanderReserveService;
    private readonly IAuditService _auditService;

    public CreateCommanderReserveRequestCommandHandler(ICommanderReserveService commanderReserveService, IAuditService auditService)
    {
        _commanderReserveService = commanderReserveService;
        _auditService = auditService;
    }

    public async Task<CommanderReserveRequestDto> Handle(CreateCommanderReserveRequestCommand request, CancellationToken cancellationToken)
    {
        var result = await _commanderReserveService.CreateRequestAsync(request.Request, request.UserId, cancellationToken);

        await _auditService.LogActionAsync(
            userId: request.UserId,
            action: "CommanderReserveRequested",
            entityType: "CommanderReserveRequest",
            entityId: result.Id.ToString(),
            description: $"Commander's reserve request {result.RequestNumber} created for item {result.ItemCode}",
            cancellationToken: cancellationToken);

        return result;
    }
}

public class ApproveCommanderReserveRequestCommandHandler : IRequestHandler<ApproveCommanderReserveRequestCommand, CommanderReserveRequestDto>
{
    private readonly ICommanderReserveService _commanderReserveService;
    private readonly IAuditService _auditService;

    public ApproveCommanderReserveRequestCommandHandler(ICommanderReserveService commanderReserveService, IAuditService auditService)
    {
        _commanderReserveService = commanderReserveService;
        _auditService = auditService;
    }

    public async Task<CommanderReserveRequestDto> Handle(ApproveCommanderReserveRequestCommand request, CancellationToken cancellationToken)
    {
        var result = await _commanderReserveService.ApproveRequestAsync(request.Request, request.UserId, cancellationToken);

        await _auditService.LogActionAsync(
            userId: request.UserId,
            action: "CommanderReserveApproved",
            entityType: "CommanderReserveRequest",
            entityId: result.Id.ToString(),
            description: $"Commander's reserve request {result.RequestNumber} approved",
            severity: "High",
            cancellationToken: cancellationToken);

        return result;
    }
}

public class RejectCommanderReserveRequestCommandHandler : IRequestHandler<RejectCommanderReserveRequestCommand, CommanderReserveRequestDto>
{
    private readonly ICommanderReserveService _commanderReserveService;
    private readonly IAuditService _auditService;

    public RejectCommanderReserveRequestCommandHandler(ICommanderReserveService commanderReserveService, IAuditService auditService)
    {
        _commanderReserveService = commanderReserveService;
        _auditService = auditService;
    }

    public async Task<CommanderReserveRequestDto> Handle(RejectCommanderReserveRequestCommand request, CancellationToken cancellationToken)
    {
        var result = await _commanderReserveService.RejectRequestAsync(request.Request, request.UserId, cancellationToken);

        await _auditService.LogActionAsync(
            userId: request.UserId,
            action: "CommanderReserveRejected",
            entityType: "CommanderReserveRequest",
            entityId: result.Id.ToString(),
            description: $"Commander's reserve request {result.RequestNumber} rejected: {request.Request.RejectionReason}",
            severity: "Medium",
            cancellationToken: cancellationToken);

        return result;
    }
}

public class ReleaseCommanderReserveCommandHandler : IRequestHandler<ReleaseCommanderReserveCommand, CommanderReserveReleaseDto>
{
    private readonly ICommanderReserveService _commanderReserveService;
    private readonly IAuditService _auditService;

    public ReleaseCommanderReserveCommandHandler(ICommanderReserveService commanderReserveService, IAuditService auditService)
    {
        _commanderReserveService = commanderReserveService;
        _auditService = auditService;
    }

    public async Task<CommanderReserveReleaseDto> Handle(ReleaseCommanderReserveCommand request, CancellationToken cancellationToken)
    {
        var result = await _commanderReserveService.ReleaseReserveAsync(request.Request, request.UserId, cancellationToken);

        await _auditService.LogActionAsync(
            userId: request.UserId,
            action: "CommanderReserveReleased",
            entityType: "CommanderReserveRelease",
            entityId: result.Id.ToString(),
            description: $"Commander's reserve released: {result.Quantity} {result.Unit} of {result.ItemCode}",
            severity: "High",
            cancellationToken: cancellationToken);

        return result;
    }
}

public class AllocateCommanderReserveCommandHandler : IRequestHandler<AllocateCommanderReserveCommand, bool>
{
    private readonly ICommanderReserveService _commanderReserveService;
    private readonly IAuditService _auditService;

    public AllocateCommanderReserveCommandHandler(ICommanderReserveService commanderReserveService, IAuditService auditService)
    {
        _commanderReserveService = commanderReserveService;
        _auditService = auditService;
    }

    public async Task<bool> Handle(AllocateCommanderReserveCommand request, CancellationToken cancellationToken)
    {
        var result = await _commanderReserveService.AllocateReserveAsync(request.ItemId, request.WarehouseId, request.Quantity, request.UserId, cancellationToken);

        if (result)
        {
            await _auditService.LogActionAsync(
                userId: request.UserId,
                action: "CommanderReserveAllocated",
                entityType: "InventoryRecord",
                entityId: $"{request.WarehouseId}-{request.ItemId}",
                description: $"Commander's reserve allocated: {request.Quantity} units for item {request.ItemId} in warehouse {request.WarehouseId}",
                severity: "Medium",
                cancellationToken: cancellationToken);
        }

        return result;
    }
}

public class AdjustCommanderReserveCommandHandler : IRequestHandler<AdjustCommanderReserveCommand, bool>
{
    private readonly ICommanderReserveService _commanderReserveService;
    private readonly IAuditService _auditService;

    public AdjustCommanderReserveCommandHandler(ICommanderReserveService commanderReserveService, IAuditService auditService)
    {
        _commanderReserveService = commanderReserveService;
        _auditService = auditService;
    }

    public async Task<bool> Handle(AdjustCommanderReserveCommand request, CancellationToken cancellationToken)
    {
        var result = await _commanderReserveService.AdjustReserveAsync(request.ItemId, request.WarehouseId, request.NewReserveQuantity, request.Reason, request.UserId, cancellationToken);

        if (result)
        {
            await _auditService.LogActionAsync(
                userId: request.UserId,
                action: "CommanderReserveAdjusted",
                entityType: "InventoryRecord",
                entityId: $"{request.WarehouseId}-{request.ItemId}",
                description: $"Commander's reserve adjusted to {request.NewReserveQuantity} units for item {request.ItemId} in warehouse {request.WarehouseId}. Reason: {request.Reason}",
                severity: "High",
                cancellationToken: cancellationToken);
        }

        return result;
    }
}
