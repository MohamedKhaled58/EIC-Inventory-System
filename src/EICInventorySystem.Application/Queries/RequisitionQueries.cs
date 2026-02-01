using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using MediatR;

namespace EICInventorySystem.Application.Queries;

public record GetRequisitionsQuery(
    int? FactoryId = null,
    int? WarehouseId = null,
    int? DepartmentId = null,
    int? ProjectId = null,
    RequisitionStatus? Status = null,
    RequisitionType? Type = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<(IEnumerable<RequisitionDto> Items, int TotalCount)>;

public record GetRequisitionQuery(int Id) : IRequest<RequisitionDto?>;

public record GetRequisitionByNumberQuery(string RequisitionNumber) : IRequest<RequisitionDto?>;

public record GetPendingRequisitionsQuery(int? WarehouseId = null) : IRequest<IEnumerable<RequisitionDto>>;

public record GetRequisitionsForApprovalQuery(int? WarehouseId = null) : IRequest<IEnumerable<RequisitionDto>>;

public record GetRequisitionsForIssuanceQuery(int? WarehouseId = null) : IRequest<IEnumerable<RequisitionDto>>;

public record GetRequisitionsForReceivingQuery(int? DepartmentId = null, int? ProjectId = null) : IRequest<IEnumerable<RequisitionDto>>;

public record GetRequisitionStatisticsQuery(int? FactoryId = null, DateTime? StartDate = null, DateTime? EndDate = null) : IRequest<RequisitionStatisticsDto>;

public record RequisitionStatisticsDto
{
    public int TotalRequisitions { get; init; }
    public int PendingRequisitions { get; init; }
    public int ApprovedRequisitions { get; init; }
    public int RejectedRequisitions { get; init; }
    public int IssuedRequisitions { get; init; }
    public int ReceivedRequisitions { get; init; }
    public decimal TotalRequestedQuantity { get; init; }
    public decimal TotalApprovedQuantity { get; init; }
    public decimal TotalIssuedQuantity { get; init; }
    public int RequisitionsRequiringReserve { get; init; }
    public int ReserveRequisitionsApproved { get; init; }
}

public class GetRequisitionsQueryHandler : IRequestHandler<GetRequisitionsQuery, (IEnumerable<RequisitionDto> Items, int TotalCount)>
{
    private readonly IRequisitionService _requisitionService;

    public GetRequisitionsQueryHandler(IRequisitionService requisitionService)
    {
        _requisitionService = requisitionService;
    }

    public async Task<(IEnumerable<RequisitionDto> Items, int TotalCount)> Handle(GetRequisitionsQuery request, CancellationToken cancellationToken)
    {
        return await _requisitionService.GetRequisitionsAsync(
            request.FactoryId,
            request.WarehouseId,
            request.DepartmentId,
            request.ProjectId,
            request.Status,
            request.Type,
            request.StartDate,
            request.EndDate,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}

public class GetRequisitionQueryHandler : IRequestHandler<GetRequisitionQuery, RequisitionDto?>
{
    private readonly IRequisitionService _requisitionService;

    public GetRequisitionQueryHandler(IRequisitionService requisitionService)
    {
        _requisitionService = requisitionService;
    }

    public async Task<RequisitionDto?> Handle(GetRequisitionQuery request, CancellationToken cancellationToken)
    {
        return await _requisitionService.GetRequisitionAsync(request.Id, cancellationToken);
    }
}

public class GetRequisitionByNumberQueryHandler : IRequestHandler<GetRequisitionByNumberQuery, RequisitionDto?>
{
    private readonly IRequisitionService _requisitionService;

    public GetRequisitionByNumberQueryHandler(IRequisitionService requisitionService)
    {
        _requisitionService = requisitionService;
    }

    public async Task<RequisitionDto?> Handle(GetRequisitionByNumberQuery request, CancellationToken cancellationToken)
    {
        return await _requisitionService.GetRequisitionByNumberAsync(request.RequisitionNumber, cancellationToken);
    }
}

public class GetPendingRequisitionsQueryHandler : IRequestHandler<GetPendingRequisitionsQuery, IEnumerable<RequisitionDto>>
{
    private readonly IRequisitionService _requisitionService;

    public GetPendingRequisitionsQueryHandler(IRequisitionService requisitionService)
    {
        _requisitionService = requisitionService;
    }

    public async Task<IEnumerable<RequisitionDto>> Handle(GetPendingRequisitionsQuery request, CancellationToken cancellationToken)
    {
        return await _requisitionService.GetPendingRequisitionsAsync(request.WarehouseId, cancellationToken);
    }
}

public class GetRequisitionsForApprovalQueryHandler : IRequestHandler<GetRequisitionsForApprovalQuery, IEnumerable<RequisitionDto>>
{
    private readonly IRequisitionService _requisitionService;

    public GetRequisitionsForApprovalQueryHandler(IRequisitionService requisitionService)
    {
        _requisitionService = requisitionService;
    }

    public async Task<IEnumerable<RequisitionDto>> Handle(GetRequisitionsForApprovalQuery request, CancellationToken cancellationToken)
    {
        return await _requisitionService.GetRequisitionsForApprovalAsync(request.WarehouseId, cancellationToken);
    }
}

public class GetRequisitionsForIssuanceQueryHandler : IRequestHandler<GetRequisitionsForIssuanceQuery, IEnumerable<RequisitionDto>>
{
    private readonly IRequisitionService _requisitionService;

    public GetRequisitionsForIssuanceQueryHandler(IRequisitionService requisitionService)
    {
        _requisitionService = requisitionService;
    }

    public async Task<IEnumerable<RequisitionDto>> Handle(GetRequisitionsForIssuanceQuery request, CancellationToken cancellationToken)
    {
        return await _requisitionService.GetRequisitionsForIssuanceAsync(request.WarehouseId, cancellationToken);
    }
}

public class GetRequisitionsForReceivingQueryHandler : IRequestHandler<GetRequisitionsForReceivingQuery, IEnumerable<RequisitionDto>>
{
    private readonly IRequisitionService _requisitionService;

    public GetRequisitionsForReceivingQueryHandler(IRequisitionService requisitionService)
    {
        _requisitionService = requisitionService;
    }

    public async Task<IEnumerable<RequisitionDto>> Handle(GetRequisitionsForReceivingQuery request, CancellationToken cancellationToken)
    {
        return await _requisitionService.GetRequisitionsForReceivingAsync(request.DepartmentId, request.ProjectId, cancellationToken);
    }
}

public class GetRequisitionStatisticsQueryHandler : IRequestHandler<GetRequisitionStatisticsQuery, RequisitionStatisticsDto>
{
    private readonly IRequisitionService _requisitionService;

    public GetRequisitionStatisticsQueryHandler(IRequisitionService requisitionService)
    {
        _requisitionService = requisitionService;
    }

    public async Task<RequisitionStatisticsDto> Handle(GetRequisitionStatisticsQuery request, CancellationToken cancellationToken)
    {
        return await _requisitionService.GetRequisitionStatisticsAsync(request.FactoryId, request.StartDate, request.EndDate, cancellationToken);
    }
}
