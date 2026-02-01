using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Domain.Enums;
using MediatR;

namespace EICInventorySystem.Application.Queries;

#region Worker Queries

public record GetWorkerQuery(int Id) : IRequest<WorkerDto?>;
public record GetWorkerByCodeQuery(string WorkerCode) : IRequest<WorkerDto?>;

public record GetWorkersQuery(
    int? FactoryId,
    int? DepartmentId,
    bool? IsActive,
    string? SearchTerm,
    int PageNumber,
    int PageSize) : IRequest<(IEnumerable<WorkerDto> Items, int TotalCount)>;

public record SearchWorkersQuery(
    string SearchTerm,
    int? FactoryId,
    int? DepartmentId,
    int MaxResults = 10) : IRequest<IEnumerable<WorkerDto>>;

#endregion

#region Custody Queries

public record GetCustodyQuery(int Id) : IRequest<OperationalCustodyDto?>;
public record GetCustodyByNumberQuery(string CustodyNumber) : IRequest<OperationalCustodyDto?>;

public record GetCustodiesQuery(
    int? WorkerId,
    int? ItemId,
    int? FactoryId,
    int? DepartmentId,
    int? WarehouseId,
    CustodyStatus? Status,
    bool? IsOverdue,
    DateTime? StartDate,
    DateTime? EndDate,
    int PageNumber,
    int PageSize) : IRequest<(IEnumerable<OperationalCustodyDto> Items, int TotalCount)>;

public record GetActiveCustodiesByWorkerQuery(int WorkerId) : IRequest<IEnumerable<OperationalCustodyDto>>;
public record GetOverdueCustodiesQuery(int MaxDays, int? FactoryId) : IRequest<IEnumerable<OperationalCustodyDto>>;
public record GetCustodyAgingReportQuery(int WorkerId) : IRequest<CustodyAgingReportDto>;
public record GetCustodyStatisticsQuery(int? FactoryId, int? DepartmentId) : IRequest<CustodyStatisticsDto>;
public record GetCustodiesByWorkerQuery(int? DepartmentId, int? FactoryId) : IRequest<IEnumerable<CustodyByWorkerDto>>;

#endregion

#region Worker Query Handlers

public class GetWorkerQueryHandler : IRequestHandler<GetWorkerQuery, WorkerDto?>
{
    private readonly IWorkerService _workerService;

    public GetWorkerQueryHandler(IWorkerService workerService)
    {
        _workerService = workerService;
    }

    public async Task<WorkerDto?> Handle(GetWorkerQuery request, CancellationToken cancellationToken)
    {
        try
        {
            return await _workerService.GetWorkerByIdAsync(request.Id, cancellationToken);
        }
        catch
        {
            return null;
        }
    }
}

public class GetWorkerByCodeQueryHandler : IRequestHandler<GetWorkerByCodeQuery, WorkerDto?>
{
    private readonly IWorkerService _workerService;

    public GetWorkerByCodeQueryHandler(IWorkerService workerService)
    {
        _workerService = workerService;
    }

    public async Task<WorkerDto?> Handle(GetWorkerByCodeQuery request, CancellationToken cancellationToken)
    {
        return await _workerService.GetWorkerByCodeAsync(request.WorkerCode, cancellationToken);
    }
}

public class GetWorkersQueryHandler : IRequestHandler<GetWorkersQuery, (IEnumerable<WorkerDto> Items, int TotalCount)>
{
    private readonly IWorkerService _workerService;

    public GetWorkersQueryHandler(IWorkerService workerService)
    {
        _workerService = workerService;
    }

    public async Task<(IEnumerable<WorkerDto> Items, int TotalCount)> Handle(GetWorkersQuery request, CancellationToken cancellationToken)
    {
        return await _workerService.GetWorkersAsync(
            request.FactoryId,
            request.DepartmentId,
            request.IsActive,
            request.SearchTerm,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}

public class SearchWorkersQueryHandler : IRequestHandler<SearchWorkersQuery, IEnumerable<WorkerDto>>
{
    private readonly IWorkerService _workerService;

    public SearchWorkersQueryHandler(IWorkerService workerService)
    {
        _workerService = workerService;
    }

    public async Task<IEnumerable<WorkerDto>> Handle(SearchWorkersQuery request, CancellationToken cancellationToken)
    {
        return await _workerService.SearchWorkersAsync(
            request.SearchTerm,
            request.FactoryId,
            request.DepartmentId,
            request.MaxResults,
            cancellationToken);
    }
}

#endregion

#region Custody Query Handlers

public class GetCustodyQueryHandler : IRequestHandler<GetCustodyQuery, OperationalCustodyDto?>
{
    private readonly IOperationalCustodyService _custodyService;

    public GetCustodyQueryHandler(IOperationalCustodyService custodyService)
    {
        _custodyService = custodyService;
    }

    public async Task<OperationalCustodyDto?> Handle(GetCustodyQuery request, CancellationToken cancellationToken)
    {
        try
        {
            return await _custodyService.GetCustodyByIdAsync(request.Id, cancellationToken);
        }
        catch
        {
            return null;
        }
    }
}

public class GetCustodyByNumberQueryHandler : IRequestHandler<GetCustodyByNumberQuery, OperationalCustodyDto?>
{
    private readonly IOperationalCustodyService _custodyService;

    public GetCustodyByNumberQueryHandler(IOperationalCustodyService custodyService)
    {
        _custodyService = custodyService;
    }

    public async Task<OperationalCustodyDto?> Handle(GetCustodyByNumberQuery request, CancellationToken cancellationToken)
    {
        return await _custodyService.GetCustodyByNumberAsync(request.CustodyNumber, cancellationToken);
    }
}

public class GetCustodiesQueryHandler : IRequestHandler<GetCustodiesQuery, (IEnumerable<OperationalCustodyDto> Items, int TotalCount)>
{
    private readonly IOperationalCustodyService _custodyService;

    public GetCustodiesQueryHandler(IOperationalCustodyService custodyService)
    {
        _custodyService = custodyService;
    }

    public async Task<(IEnumerable<OperationalCustodyDto> Items, int TotalCount)> Handle(GetCustodiesQuery request, CancellationToken cancellationToken)
    {
        return await _custodyService.GetCustodiesAsync(
            request.WorkerId,
            request.ItemId,
            request.FactoryId,
            request.DepartmentId,
            request.WarehouseId,
            request.Status,
            request.IsOverdue,
            request.StartDate,
            request.EndDate,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}

public class GetActiveCustodiesByWorkerQueryHandler : IRequestHandler<GetActiveCustodiesByWorkerQuery, IEnumerable<OperationalCustodyDto>>
{
    private readonly IOperationalCustodyService _custodyService;

    public GetActiveCustodiesByWorkerQueryHandler(IOperationalCustodyService custodyService)
    {
        _custodyService = custodyService;
    }

    public async Task<IEnumerable<OperationalCustodyDto>> Handle(GetActiveCustodiesByWorkerQuery request, CancellationToken cancellationToken)
    {
        return await _custodyService.GetActiveCustodiesByWorkerAsync(request.WorkerId, cancellationToken);
    }
}

public class GetOverdueCustodiesQueryHandler : IRequestHandler<GetOverdueCustodiesQuery, IEnumerable<OperationalCustodyDto>>
{
    private readonly IOperationalCustodyService _custodyService;

    public GetOverdueCustodiesQueryHandler(IOperationalCustodyService custodyService)
    {
        _custodyService = custodyService;
    }

    public async Task<IEnumerable<OperationalCustodyDto>> Handle(GetOverdueCustodiesQuery request, CancellationToken cancellationToken)
    {
        return await _custodyService.GetOverdueCustodiesAsync(request.MaxDays, request.FactoryId, cancellationToken);
    }
}

public class GetCustodyAgingReportQueryHandler : IRequestHandler<GetCustodyAgingReportQuery, CustodyAgingReportDto>
{
    private readonly IOperationalCustodyService _custodyService;

    public GetCustodyAgingReportQueryHandler(IOperationalCustodyService custodyService)
    {
        _custodyService = custodyService;
    }

    public async Task<CustodyAgingReportDto> Handle(GetCustodyAgingReportQuery request, CancellationToken cancellationToken)
    {
        return await _custodyService.GetCustodyAgingReportAsync(request.WorkerId, cancellationToken);
    }
}

public class GetCustodyStatisticsQueryHandler : IRequestHandler<GetCustodyStatisticsQuery, CustodyStatisticsDto>
{
    private readonly IOperationalCustodyService _custodyService;

    public GetCustodyStatisticsQueryHandler(IOperationalCustodyService custodyService)
    {
        _custodyService = custodyService;
    }

    public async Task<CustodyStatisticsDto> Handle(GetCustodyStatisticsQuery request, CancellationToken cancellationToken)
    {
        return await _custodyService.GetCustodyStatisticsAsync(request.FactoryId, request.DepartmentId, cancellationToken);
    }
}

public class GetCustodiesByWorkerQueryHandler : IRequestHandler<GetCustodiesByWorkerQuery, IEnumerable<CustodyByWorkerDto>>
{
    private readonly IOperationalCustodyService _custodyService;

    public GetCustodiesByWorkerQueryHandler(IOperationalCustodyService custodyService)
    {
        _custodyService = custodyService;
    }

    public async Task<IEnumerable<CustodyByWorkerDto>> Handle(GetCustodiesByWorkerQuery request, CancellationToken cancellationToken)
    {
        return await _custodyService.GetCustodiesByWorkerAsync(request.DepartmentId, request.FactoryId, cancellationToken);
    }
}

#endregion
