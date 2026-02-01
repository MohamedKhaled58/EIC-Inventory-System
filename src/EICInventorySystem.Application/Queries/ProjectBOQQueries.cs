using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Domain.Enums;
using MediatR;

namespace EICInventorySystem.Application.Queries;

#region BOQ Queries

public record GetBOQQuery(int Id) : IRequest<ProjectBOQDto?>;
public record GetBOQByNumberQuery(string BOQNumber) : IRequest<ProjectBOQDto?>;

public record GetBOQsQuery(
    int? FactoryId,
    int? ProjectId,
    int? WarehouseId,
    BOQStatus? Status,
    DateTime? StartDate,
    DateTime? EndDate,
    int PageNumber,
    int PageSize) : IRequest<(IEnumerable<ProjectBOQDto> Items, int TotalCount)>;

public record GetPendingBOQsQuery(int? FactoryId) : IRequest<IEnumerable<ProjectBOQDto>>;
public record GetBOQsForApprovalQuery(int? FactoryId) : IRequest<IEnumerable<ProjectBOQDto>>;
public record GetBOQsForIssuanceQuery(int? WarehouseId) : IRequest<IEnumerable<ProjectBOQDto>>;
public record GetPartiallyIssuedBOQsQuery(int? ProjectId) : IRequest<IEnumerable<ProjectBOQDto>>;
public record GetPendingBOQItemsQuery(int? ProjectId, int? FactoryId) : IRequest<IEnumerable<PendingBOQItemsDto>>;
public record GetBOQStatisticsQuery(int? FactoryId, DateTime? StartDate, DateTime? EndDate) : IRequest<BOQStatisticsDto>;

#endregion

#region BOQ Query Handlers

public class GetBOQQueryHandler : IRequestHandler<GetBOQQuery, ProjectBOQDto?>
{
    private readonly IProjectBOQService _boqService;

    public GetBOQQueryHandler(IProjectBOQService boqService)
    {
        _boqService = boqService;
    }

    public async Task<ProjectBOQDto?> Handle(GetBOQQuery request, CancellationToken cancellationToken)
    {
        try
        {
            return await _boqService.GetBOQByIdAsync(request.Id, cancellationToken);
        }
        catch
        {
            return null;
        }
    }
}

public class GetBOQByNumberQueryHandler : IRequestHandler<GetBOQByNumberQuery, ProjectBOQDto?>
{
    private readonly IProjectBOQService _boqService;

    public GetBOQByNumberQueryHandler(IProjectBOQService boqService)
    {
        _boqService = boqService;
    }

    public async Task<ProjectBOQDto?> Handle(GetBOQByNumberQuery request, CancellationToken cancellationToken)
    {
        return await _boqService.GetBOQByNumberAsync(request.BOQNumber, cancellationToken);
    }
}

public class GetBOQsQueryHandler : IRequestHandler<GetBOQsQuery, (IEnumerable<ProjectBOQDto> Items, int TotalCount)>
{
    private readonly IProjectBOQService _boqService;

    public GetBOQsQueryHandler(IProjectBOQService boqService)
    {
        _boqService = boqService;
    }

    public async Task<(IEnumerable<ProjectBOQDto> Items, int TotalCount)> Handle(GetBOQsQuery request, CancellationToken cancellationToken)
    {
        return await _boqService.GetBOQsAsync(
            request.FactoryId,
            request.ProjectId,
            request.WarehouseId,
            request.Status,
            request.StartDate,
            request.EndDate,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}

public class GetPendingBOQsQueryHandler : IRequestHandler<GetPendingBOQsQuery, IEnumerable<ProjectBOQDto>>
{
    private readonly IProjectBOQService _boqService;

    public GetPendingBOQsQueryHandler(IProjectBOQService boqService)
    {
        _boqService = boqService;
    }

    public async Task<IEnumerable<ProjectBOQDto>> Handle(GetPendingBOQsQuery request, CancellationToken cancellationToken)
    {
        return await _boqService.GetPendingBOQsAsync(request.FactoryId, cancellationToken);
    }
}

public class GetBOQsForApprovalQueryHandler : IRequestHandler<GetBOQsForApprovalQuery, IEnumerable<ProjectBOQDto>>
{
    private readonly IProjectBOQService _boqService;

    public GetBOQsForApprovalQueryHandler(IProjectBOQService boqService)
    {
        _boqService = boqService;
    }

    public async Task<IEnumerable<ProjectBOQDto>> Handle(GetBOQsForApprovalQuery request, CancellationToken cancellationToken)
    {
        return await _boqService.GetBOQsForApprovalAsync(request.FactoryId, cancellationToken);
    }
}

public class GetBOQsForIssuanceQueryHandler : IRequestHandler<GetBOQsForIssuanceQuery, IEnumerable<ProjectBOQDto>>
{
    private readonly IProjectBOQService _boqService;

    public GetBOQsForIssuanceQueryHandler(IProjectBOQService boqService)
    {
        _boqService = boqService;
    }

    public async Task<IEnumerable<ProjectBOQDto>> Handle(GetBOQsForIssuanceQuery request, CancellationToken cancellationToken)
    {
        return await _boqService.GetBOQsForIssuanceAsync(request.WarehouseId, cancellationToken);
    }
}

public class GetPartiallyIssuedBOQsQueryHandler : IRequestHandler<GetPartiallyIssuedBOQsQuery, IEnumerable<ProjectBOQDto>>
{
    private readonly IProjectBOQService _boqService;

    public GetPartiallyIssuedBOQsQueryHandler(IProjectBOQService boqService)
    {
        _boqService = boqService;
    }

    public async Task<IEnumerable<ProjectBOQDto>> Handle(GetPartiallyIssuedBOQsQuery request, CancellationToken cancellationToken)
    {
        return await _boqService.GetPartiallyIssuedBOQsAsync(request.ProjectId, cancellationToken);
    }
}

public class GetPendingBOQItemsQueryHandler : IRequestHandler<GetPendingBOQItemsQuery, IEnumerable<PendingBOQItemsDto>>
{
    private readonly IProjectBOQService _boqService;

    public GetPendingBOQItemsQueryHandler(IProjectBOQService boqService)
    {
        _boqService = boqService;
    }

    public async Task<IEnumerable<PendingBOQItemsDto>> Handle(GetPendingBOQItemsQuery request, CancellationToken cancellationToken)
    {
        return await _boqService.GetPendingItemsAsync(request.ProjectId, request.FactoryId, cancellationToken);
    }
}

public class GetBOQStatisticsQueryHandler : IRequestHandler<GetBOQStatisticsQuery, BOQStatisticsDto>
{
    private readonly IProjectBOQService _boqService;

    public GetBOQStatisticsQueryHandler(IProjectBOQService boqService)
    {
        _boqService = boqService;
    }

    public async Task<BOQStatisticsDto> Handle(GetBOQStatisticsQuery request, CancellationToken cancellationToken)
    {
        return await _boqService.GetBOQStatisticsAsync(request.FactoryId, request.StartDate, request.EndDate, cancellationToken);
    }
}

#endregion
