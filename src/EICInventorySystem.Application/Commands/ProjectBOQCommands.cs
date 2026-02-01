using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using MediatR;

namespace EICInventorySystem.Application.Commands;

#region BOQ Commands

public record CreateBOQCommand(CreateProjectBOQDto Request, int UserId) : IRequest<ProjectBOQDto>;
public record UpdateBOQCommand(UpdateProjectBOQDto Request, int UserId) : IRequest<ProjectBOQDto>;
public record SubmitBOQCommand(int Id, int UserId) : IRequest<ProjectBOQDto>;
public record ApproveBOQCommand(ApproveBOQDto Request, int UserId) : IRequest<ProjectBOQDto>;
public record RejectBOQCommand(RejectBOQDto Request, int UserId) : IRequest<ProjectBOQDto>;
public record ApproveCommanderReserveBOQCommand(ApproveCommanderReserveDto Request, int UserId) : IRequest<ProjectBOQDto>;
public record IssueBOQCommand(IssueBOQDto Request, int UserId) : IRequest<ProjectBOQDto>;
public record CancelBOQCommand(int Id, int UserId, string? Reason, string? ReasonArabic) : IRequest<bool>;
public record DeleteBOQCommand(int Id, int UserId) : IRequest<bool>;

#endregion

#region Command Handlers

public class CreateBOQCommandHandler : IRequestHandler<CreateBOQCommand, ProjectBOQDto>
{
    private readonly IProjectBOQService _boqService;
    private readonly IAuditService _auditService;

    public CreateBOQCommandHandler(IProjectBOQService boqService, IAuditService auditService)
    {
        _boqService = boqService;
        _auditService = auditService;
    }

    public async Task<ProjectBOQDto> Handle(CreateBOQCommand request, CancellationToken cancellationToken)
    {
        var result = await _boqService.CreateBOQAsync(request.Request, request.UserId, cancellationToken);

        await _auditService.LogActionAsync(
            userId: request.UserId,
            action: "BOQCreated",
            entityType: "ProjectBOQ",
            entityId: result.Id.ToString(),
            description: $"BOQ {result.BOQNumber} created for project {result.ProjectName}",
            cancellationToken: cancellationToken);

        return result;
    }
}

public class UpdateBOQCommandHandler : IRequestHandler<UpdateBOQCommand, ProjectBOQDto>
{
    private readonly IProjectBOQService _boqService;
    private readonly IAuditService _auditService;

    public UpdateBOQCommandHandler(IProjectBOQService boqService, IAuditService auditService)
    {
        _boqService = boqService;
        _auditService = auditService;
    }

    public async Task<ProjectBOQDto> Handle(UpdateBOQCommand request, CancellationToken cancellationToken)
    {
        var result = await _boqService.UpdateBOQAsync(request.Request, request.UserId, cancellationToken);

        await _auditService.LogActionAsync(
            userId: request.UserId,
            action: "BOQUpdated",
            entityType: "ProjectBOQ",
            entityId: result.Id.ToString(),
            description: $"BOQ {result.BOQNumber} updated",
            cancellationToken: cancellationToken);

        return result;
    }
}

public class SubmitBOQCommandHandler : IRequestHandler<SubmitBOQCommand, ProjectBOQDto>
{
    private readonly IProjectBOQService _boqService;
    private readonly IAuditService _auditService;

    public SubmitBOQCommandHandler(IProjectBOQService boqService, IAuditService auditService)
    {
        _boqService = boqService;
        _auditService = auditService;
    }

    public async Task<ProjectBOQDto> Handle(SubmitBOQCommand request, CancellationToken cancellationToken)
    {
        var result = await _boqService.SubmitBOQAsync(request.Id, request.UserId, cancellationToken);

        await _auditService.LogActionAsync(
            userId: request.UserId,
            action: "BOQSubmitted",
            entityType: "ProjectBOQ",
            entityId: result.Id.ToString(),
            description: $"BOQ {result.BOQNumber} submitted for approval",
            cancellationToken: cancellationToken);

        return result;
    }
}

public class ApproveBOQCommandHandler : IRequestHandler<ApproveBOQCommand, ProjectBOQDto>
{
    private readonly IProjectBOQService _boqService;
    private readonly IAuditService _auditService;

    public ApproveBOQCommandHandler(IProjectBOQService boqService, IAuditService auditService)
    {
        _boqService = boqService;
        _auditService = auditService;
    }

    public async Task<ProjectBOQDto> Handle(ApproveBOQCommand request, CancellationToken cancellationToken)
    {
        var result = await _boqService.ApproveBOQAsync(request.Request, request.UserId, cancellationToken);

        await _auditService.LogActionAsync(
            userId: request.UserId,
            action: "BOQApproved",
            entityType: "ProjectBOQ",
            entityId: result.Id.ToString(),
            description: $"BOQ {result.BOQNumber} approved by Factory Commander",
            cancellationToken: cancellationToken);

        return result;
    }
}

public class RejectBOQCommandHandler : IRequestHandler<RejectBOQCommand, ProjectBOQDto>
{
    private readonly IProjectBOQService _boqService;
    private readonly IAuditService _auditService;

    public RejectBOQCommandHandler(IProjectBOQService boqService, IAuditService auditService)
    {
        _boqService = boqService;
        _auditService = auditService;
    }

    public async Task<ProjectBOQDto> Handle(RejectBOQCommand request, CancellationToken cancellationToken)
    {
        var result = await _boqService.RejectBOQAsync(request.Request, request.UserId, cancellationToken);

        await _auditService.LogActionAsync(
            userId: request.UserId,
            action: "BOQRejected",
            entityType: "ProjectBOQ",
            entityId: result.Id.ToString(),
            description: $"BOQ {result.BOQNumber} rejected: {request.Request.RejectionReason}",
            cancellationToken: cancellationToken);

        return result;
    }
}

public class ApproveCommanderReserveBOQCommandHandler : IRequestHandler<ApproveCommanderReserveBOQCommand, ProjectBOQDto>
{
    private readonly IProjectBOQService _boqService;
    private readonly IAuditService _auditService;

    public ApproveCommanderReserveBOQCommandHandler(IProjectBOQService boqService, IAuditService auditService)
    {
        _boqService = boqService;
        _auditService = auditService;
    }

    public async Task<ProjectBOQDto> Handle(ApproveCommanderReserveBOQCommand request, CancellationToken cancellationToken)
    {
        var result = await _boqService.ApproveCommanderReserveAsync(request.Request, request.UserId, cancellationToken);

        await _auditService.LogActionAsync(
            userId: request.UserId,
            action: "BOQCommanderReserveApproved",
            entityType: "ProjectBOQ",
            entityId: result.Id.ToString(),
            description: $"Commander Reserve approved for BOQ {result.BOQNumber}",
            cancellationToken: cancellationToken);

        return result;
    }
}

public class IssueBOQCommandHandler : IRequestHandler<IssueBOQCommand, ProjectBOQDto>
{
    private readonly IProjectBOQService _boqService;
    private readonly IAuditService _auditService;

    public IssueBOQCommandHandler(IProjectBOQService boqService, IAuditService auditService)
    {
        _boqService = boqService;
        _auditService = auditService;
    }

    public async Task<ProjectBOQDto> Handle(IssueBOQCommand request, CancellationToken cancellationToken)
    {
        var result = await _boqService.IssueBOQAsync(request.Request, request.UserId, cancellationToken);

        var issueType = result.Status == Domain.Enums.BOQStatus.FullyIssued ? "fully" : "partially";
        await _auditService.LogActionAsync(
            userId: request.UserId,
            action: result.Status == Domain.Enums.BOQStatus.FullyIssued ? "BOQFullyIssued" : "BOQPartiallyIssued",
            entityType: "ProjectBOQ",
            entityId: result.Id.ToString(),
            description: $"BOQ {result.BOQNumber} {issueType} issued. Issued: {result.IssuedQuantity}, Remaining: {result.RemainingQuantity}",
            cancellationToken: cancellationToken);

        return result;
    }
}

public class CancelBOQCommandHandler : IRequestHandler<CancelBOQCommand, bool>
{
    private readonly IProjectBOQService _boqService;
    private readonly IAuditService _auditService;

    public CancelBOQCommandHandler(IProjectBOQService boqService, IAuditService auditService)
    {
        _boqService = boqService;
        _auditService = auditService;
    }

    public async Task<bool> Handle(CancelBOQCommand request, CancellationToken cancellationToken)
    {
        var result = await _boqService.CancelBOQAsync(request.Id, request.UserId, request.Reason, request.ReasonArabic, cancellationToken);

        if (result)
        {
            await _auditService.LogActionAsync(
                userId: request.UserId,
                action: "BOQCancelled",
                entityType: "ProjectBOQ",
                entityId: request.Id.ToString(),
                description: $"BOQ {request.Id} cancelled: {request.Reason}",
                cancellationToken: cancellationToken);
        }

        return result;
    }
}

public class DeleteBOQCommandHandler : IRequestHandler<DeleteBOQCommand, bool>
{
    private readonly IProjectBOQService _boqService;
    private readonly IAuditService _auditService;

    public DeleteBOQCommandHandler(IProjectBOQService boqService, IAuditService auditService)
    {
        _boqService = boqService;
        _auditService = auditService;
    }

    public async Task<bool> Handle(DeleteBOQCommand request, CancellationToken cancellationToken)
    {
        var result = await _boqService.DeleteBOQAsync(request.Id, request.UserId, cancellationToken);

        if (result)
        {
            await _auditService.LogActionAsync(
                userId: request.UserId,
                action: "BOQDeleted",
                entityType: "ProjectBOQ",
                entityId: request.Id.ToString(),
                description: $"BOQ {request.Id} deleted",
                cancellationToken: cancellationToken);
        }

        return result;
    }
}

#endregion
