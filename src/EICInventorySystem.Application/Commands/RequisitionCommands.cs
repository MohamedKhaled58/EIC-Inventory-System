using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using MediatR;

namespace EICInventorySystem.Application.Commands;

public record CreateRequisitionCommand(CreateRequisitionDto Request, int UserId) : IRequest<RequisitionDto>;

public record UpdateRequisitionCommand(UpdateRequisitionDto Request, int UserId) : IRequest<RequisitionDto>;

public record ApproveRequisitionCommand(ApproveRequisitionDto Request, int UserId) : IRequest<RequisitionDto>;

public record RejectRequisitionCommand(RejectRequisitionDto Request, int UserId) : IRequest<RequisitionDto>;

public record IssueRequisitionCommand(IssueRequisitionDto Request, int UserId) : IRequest<RequisitionDto>;

public record ReceiveRequisitionCommand(ReceiveRequisitionDto Request, int UserId) : IRequest<RequisitionDto>;

public record CancelRequisitionCommand(int Id, int UserId, string Reason) : IRequest<bool>;

public class CreateRequisitionCommandHandler : IRequestHandler<CreateRequisitionCommand, RequisitionDto>
{
    private readonly IRequisitionService _requisitionService;
    private readonly IAuditService _auditService;

    public CreateRequisitionCommandHandler(IRequisitionService requisitionService, IAuditService auditService)
    {
        _requisitionService = requisitionService;
        _auditService = auditService;
    }

    public async Task<RequisitionDto> Handle(CreateRequisitionCommand request, CancellationToken cancellationToken)
    {
        var result = await _requisitionService.CreateRequisitionAsync(request.Request, request.UserId, cancellationToken);

        await _auditService.LogActionAsync(
            userId: request.UserId,
            action: "RequisitionCreated",
            entityType: "Requisition",
            entityId: result.Id.ToString(),
            description: $"Requisition {result.RequisitionNumber} created",
            cancellationToken: cancellationToken);

        return result;
    }
}

public class UpdateRequisitionCommandHandler : IRequestHandler<UpdateRequisitionCommand, RequisitionDto>
{
    private readonly IRequisitionService _requisitionService;
    private readonly IAuditService _auditService;

    public UpdateRequisitionCommandHandler(IRequisitionService requisitionService, IAuditService auditService)
    {
        _requisitionService = requisitionService;
        _auditService = auditService;
    }

    public async Task<RequisitionDto> Handle(UpdateRequisitionCommand request, CancellationToken cancellationToken)
    {
        var result = await _requisitionService.UpdateRequisitionAsync(request.Request, request.UserId, cancellationToken);

        await _auditService.LogActionAsync(
            userId: request.UserId,
            action: "RequisitionUpdated",
            entityType: "Requisition",
            entityId: result.Id.ToString(),
            description: $"Requisition {result.RequisitionNumber} updated",
            cancellationToken: cancellationToken);

        return result;
    }
}

public class ApproveRequisitionCommandHandler : IRequestHandler<ApproveRequisitionCommand, RequisitionDto>
{
    private readonly IRequisitionService _requisitionService;
    private readonly IAuditService _auditService;

    public ApproveRequisitionCommandHandler(IRequisitionService requisitionService, IAuditService auditService)
    {
        _requisitionService = requisitionService;
        _auditService = auditService;
    }

    public async Task<RequisitionDto> Handle(ApproveRequisitionCommand request, CancellationToken cancellationToken)
    {
        var result = await _requisitionService.ApproveRequisitionAsync(request.Request, request.UserId, cancellationToken);

        await _auditService.LogActionAsync(
            userId: request.UserId,
            action: "RequisitionApproved",
            entityType: "Requisition",
            entityId: result.Id.ToString(),
            description: $"Requisition {result.RequisitionNumber} approved",
            cancellationToken: cancellationToken);

        return result;
    }
}

public class RejectRequisitionCommandHandler : IRequestHandler<RejectRequisitionCommand, RequisitionDto>
{
    private readonly IRequisitionService _requisitionService;
    private readonly IAuditService _auditService;

    public RejectRequisitionCommandHandler(IRequisitionService requisitionService, IAuditService auditService)
    {
        _requisitionService = requisitionService;
        _auditService = auditService;
    }

    public async Task<RequisitionDto> Handle(RejectRequisitionCommand request, CancellationToken cancellationToken)
    {
        var result = await _requisitionService.RejectRequisitionAsync(request.Request, request.UserId, cancellationToken);

        await _auditService.LogActionAsync(
            userId: request.UserId,
            action: "RequisitionRejected",
            entityType: "Requisition",
            entityId: result.Id.ToString(),
            description: $"Requisition {result.RequisitionNumber} rejected: {request.Request.RejectionReason}",
            cancellationToken: cancellationToken);

        return result;
    }
}

public class IssueRequisitionCommandHandler : IRequestHandler<IssueRequisitionCommand, RequisitionDto>
{
    private readonly IRequisitionService _requisitionService;
    private readonly IAuditService _auditService;

    public IssueRequisitionCommandHandler(IRequisitionService requisitionService, IAuditService auditService)
    {
        _requisitionService = requisitionService;
        _auditService = auditService;
    }

    public async Task<RequisitionDto> Handle(IssueRequisitionCommand request, CancellationToken cancellationToken)
    {
        var result = await _requisitionService.IssueRequisitionAsync(request.Request, request.UserId, cancellationToken);

        await _auditService.LogActionAsync(
            userId: request.UserId,
            action: "RequisitionIssued",
            entityType: "Requisition",
            entityId: result.Id.ToString(),
            description: $"Requisition {result.RequisitionNumber} issued",
            cancellationToken: cancellationToken);

        return result;
    }
}

public class ReceiveRequisitionCommandHandler : IRequestHandler<ReceiveRequisitionCommand, RequisitionDto>
{
    private readonly IRequisitionService _requisitionService;
    private readonly IAuditService _auditService;

    public ReceiveRequisitionCommandHandler(IRequisitionService requisitionService, IAuditService auditService)
    {
        _requisitionService = requisitionService;
        _auditService = auditService;
    }

    public async Task<RequisitionDto> Handle(ReceiveRequisitionCommand request, CancellationToken cancellationToken)
    {
        var result = await _requisitionService.ReceiveRequisitionAsync(request.Request, request.UserId, cancellationToken);

        await _auditService.LogActionAsync(
            userId: request.UserId,
            action: "RequisitionReceived",
            entityType: "Requisition",
            entityId: result.Id.ToString(),
            description: $"Requisition {result.RequisitionNumber} received",
            cancellationToken: cancellationToken);

        return result;
    }
}

public class CancelRequisitionCommandHandler : IRequestHandler<CancelRequisitionCommand, bool>
{
    private readonly IRequisitionService _requisitionService;
    private readonly IAuditService _auditService;

    public CancelRequisitionCommandHandler(IRequisitionService requisitionService, IAuditService auditService)
    {
        _requisitionService = requisitionService;
        _auditService = auditService;
    }

    public async Task<bool> Handle(CancelRequisitionCommand request, CancellationToken cancellationToken)
    {
        var result = await _requisitionService.CancelRequisitionAsync(request.Id, request.UserId, request.Reason, cancellationToken);

        if (result)
        {
            await _auditService.LogActionAsync(
                userId: request.UserId,
                action: "RequisitionCancelled",
                entityType: "Requisition",
                entityId: request.Id.ToString(),
                description: $"Requisition {request.Id} cancelled: {request.Reason}",
                cancellationToken: cancellationToken);
        }

        return result;
    }
}
