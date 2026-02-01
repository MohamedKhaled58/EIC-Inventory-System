using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using MediatR;

namespace EICInventorySystem.Application.Commands;

#region Worker Commands

public record CreateWorkerCommand(CreateWorkerDto Request, int UserId) : IRequest<WorkerDto>;
public record UpdateWorkerCommand(UpdateWorkerDto Request, int UserId) : IRequest<WorkerDto>;
public record ActivateWorkerCommand(int Id, int UserId) : IRequest<bool>;
public record DeactivateWorkerCommand(int Id, int UserId) : IRequest<bool>;
public record TransferWorkerCommand(int WorkerId, int NewDepartmentId, int UserId) : IRequest<WorkerDto>;

#endregion

#region Custody Commands

public record IssueCustodyCommand(CreateCustodyDto Request, int UserId) : IRequest<OperationalCustodyDto>;
public record ReturnCustodyCommand(ReturnCustodyDto Request, int UserId) : IRequest<OperationalCustodyDto>;
public record ConsumeCustodyCommand(ConsumeCustodyDto Request, int UserId) : IRequest<OperationalCustodyDto>;
public record TransferCustodyCommand(TransferCustodyDto Request, int UserId) : IRequest<OperationalCustodyDto>;

#endregion

#region Worker Command Handlers

public class CreateWorkerCommandHandler : IRequestHandler<CreateWorkerCommand, WorkerDto>
{
    private readonly IWorkerService _workerService;
    private readonly IAuditService _auditService;

    public CreateWorkerCommandHandler(IWorkerService workerService, IAuditService auditService)
    {
        _workerService = workerService;
        _auditService = auditService;
    }

    public async Task<WorkerDto> Handle(CreateWorkerCommand request, CancellationToken cancellationToken)
    {
        var result = await _workerService.CreateWorkerAsync(request.Request, request.UserId, cancellationToken);

        await _auditService.LogActionAsync(
            userId: request.UserId,
            action: "WorkerCreated",
            entityType: "Worker",
            entityId: result.Id.ToString(),
            description: $"Worker {result.WorkerCode} - {result.Name} created",
            cancellationToken: cancellationToken);

        return result;
    }
}

public class UpdateWorkerCommandHandler : IRequestHandler<UpdateWorkerCommand, WorkerDto>
{
    private readonly IWorkerService _workerService;
    private readonly IAuditService _auditService;

    public UpdateWorkerCommandHandler(IWorkerService workerService, IAuditService auditService)
    {
        _workerService = workerService;
        _auditService = auditService;
    }

    public async Task<WorkerDto> Handle(UpdateWorkerCommand request, CancellationToken cancellationToken)
    {
        var result = await _workerService.UpdateWorkerAsync(request.Request, request.UserId, cancellationToken);

        await _auditService.LogActionAsync(
            userId: request.UserId,
            action: "WorkerUpdated",
            entityType: "Worker",
            entityId: result.Id.ToString(),
            description: $"Worker {result.WorkerCode} updated",
            cancellationToken: cancellationToken);

        return result;
    }
}

public class ActivateWorkerCommandHandler : IRequestHandler<ActivateWorkerCommand, bool>
{
    private readonly IWorkerService _workerService;
    private readonly IAuditService _auditService;

    public ActivateWorkerCommandHandler(IWorkerService workerService, IAuditService auditService)
    {
        _workerService = workerService;
        _auditService = auditService;
    }

    public async Task<bool> Handle(ActivateWorkerCommand request, CancellationToken cancellationToken)
    {
        var result = await _workerService.ActivateWorkerAsync(request.Id, request.UserId, cancellationToken);

        if (result)
        {
            await _auditService.LogActionAsync(
                userId: request.UserId,
                action: "WorkerActivated",
                entityType: "Worker",
                entityId: request.Id.ToString(),
                description: $"Worker {request.Id} activated",
                cancellationToken: cancellationToken);
        }

        return result;
    }
}

public class DeactivateWorkerCommandHandler : IRequestHandler<DeactivateWorkerCommand, bool>
{
    private readonly IWorkerService _workerService;
    private readonly IAuditService _auditService;

    public DeactivateWorkerCommandHandler(IWorkerService workerService, IAuditService auditService)
    {
        _workerService = workerService;
        _auditService = auditService;
    }

    public async Task<bool> Handle(DeactivateWorkerCommand request, CancellationToken cancellationToken)
    {
        var result = await _workerService.DeactivateWorkerAsync(request.Id, request.UserId, cancellationToken);

        if (result)
        {
            await _auditService.LogActionAsync(
                userId: request.UserId,
                action: "WorkerDeactivated",
                entityType: "Worker",
                entityId: request.Id.ToString(),
                description: $"Worker {request.Id} deactivated",
                cancellationToken: cancellationToken);
        }

        return result;
    }
}

public class TransferWorkerCommandHandler : IRequestHandler<TransferWorkerCommand, WorkerDto>
{
    private readonly IWorkerService _workerService;
    private readonly IAuditService _auditService;

    public TransferWorkerCommandHandler(IWorkerService workerService, IAuditService auditService)
    {
        _workerService = workerService;
        _auditService = auditService;
    }

    public async Task<WorkerDto> Handle(TransferWorkerCommand request, CancellationToken cancellationToken)
    {
        var result = await _workerService.TransferWorkerToDepartmentAsync(request.WorkerId, request.NewDepartmentId, request.UserId, cancellationToken);

        await _auditService.LogActionAsync(
            userId: request.UserId,
            action: "WorkerTransferred",
            entityType: "Worker",
            entityId: result.Id.ToString(),
            description: $"Worker {result.WorkerCode} transferred to department {result.DepartmentName}",
            cancellationToken: cancellationToken);

        return result;
    }
}

#endregion

#region Custody Command Handlers

public class IssueCustodyCommandHandler : IRequestHandler<IssueCustodyCommand, OperationalCustodyDto>
{
    private readonly IOperationalCustodyService _custodyService;
    private readonly IAuditService _auditService;

    public IssueCustodyCommandHandler(IOperationalCustodyService custodyService, IAuditService auditService)
    {
        _custodyService = custodyService;
        _auditService = auditService;
    }

    public async Task<OperationalCustodyDto> Handle(IssueCustodyCommand request, CancellationToken cancellationToken)
    {
        var result = await _custodyService.IssueCustodyAsync(request.Request, request.UserId, cancellationToken);

        await _auditService.LogActionAsync(
            userId: request.UserId,
            action: "CustodyIssued",
            entityType: "OperationalCustody",
            entityId: result.Id.ToString(),
            description: $"Custody {result.CustodyNumber}: {result.Quantity} {result.Unit} of {result.ItemCode} issued to {result.WorkerName}",
            cancellationToken: cancellationToken);

        return result;
    }
}

public class ReturnCustodyCommandHandler : IRequestHandler<ReturnCustodyCommand, OperationalCustodyDto>
{
    private readonly IOperationalCustodyService _custodyService;
    private readonly IAuditService _auditService;

    public ReturnCustodyCommandHandler(IOperationalCustodyService custodyService, IAuditService auditService)
    {
        _custodyService = custodyService;
        _auditService = auditService;
    }

    public async Task<OperationalCustodyDto> Handle(ReturnCustodyCommand request, CancellationToken cancellationToken)
    {
        var result = await _custodyService.ReturnCustodyAsync(request.Request, request.UserId, cancellationToken);

        await _auditService.LogActionAsync(
            userId: request.UserId,
            action: "CustodyReturned",
            entityType: "OperationalCustody",
            entityId: result.Id.ToString(),
            description: $"Custody {result.CustodyNumber}: {request.Request.ReturnQuantity} {result.Unit} returned. Remaining: {result.RemainingQuantity}",
            cancellationToken: cancellationToken);

        return result;
    }
}

public class ConsumeCustodyCommandHandler : IRequestHandler<ConsumeCustodyCommand, OperationalCustodyDto>
{
    private readonly IOperationalCustodyService _custodyService;
    private readonly IAuditService _auditService;

    public ConsumeCustodyCommandHandler(IOperationalCustodyService custodyService, IAuditService auditService)
    {
        _custodyService = custodyService;
        _auditService = auditService;
    }

    public async Task<OperationalCustodyDto> Handle(ConsumeCustodyCommand request, CancellationToken cancellationToken)
    {
        var result = await _custodyService.ConsumeCustodyAsync(request.Request, request.UserId, cancellationToken);

        await _auditService.LogActionAsync(
            userId: request.UserId,
            action: "CustodyConsumed",
            entityType: "OperationalCustody",
            entityId: result.Id.ToString(),
            description: $"Custody {result.CustodyNumber}: {request.Request.ConsumeQuantity} {result.Unit} consumed. Remaining: {result.RemainingQuantity}",
            cancellationToken: cancellationToken);

        return result;
    }
}

public class TransferCustodyCommandHandler : IRequestHandler<TransferCustodyCommand, OperationalCustodyDto>
{
    private readonly IOperationalCustodyService _custodyService;
    private readonly IAuditService _auditService;

    public TransferCustodyCommandHandler(IOperationalCustodyService custodyService, IAuditService auditService)
    {
        _custodyService = custodyService;
        _auditService = auditService;
    }

    public async Task<OperationalCustodyDto> Handle(TransferCustodyCommand request, CancellationToken cancellationToken)
    {
        var result = await _custodyService.TransferCustodyAsync(request.Request, request.UserId, cancellationToken);

        await _auditService.LogActionAsync(
            userId: request.UserId,
            action: "CustodyTransferred",
            entityType: "OperationalCustody",
            entityId: result.Id.ToString(),
            description: $"Custody {result.CustodyNumber} transferred to worker {result.WorkerName}",
            cancellationToken: cancellationToken);

        return result;
    }
}

#endregion
