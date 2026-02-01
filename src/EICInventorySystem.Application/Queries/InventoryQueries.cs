using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using MediatR;

namespace EICInventorySystem.Application.Queries;

public record GetInventoryRecordsQuery(int? WarehouseId = null, int? ItemId = null, string? Status = null) : IRequest<IEnumerable<InventoryRecordDto>>;

public record GetInventoryRecordQuery(int WarehouseId, int ItemId) : IRequest<InventoryRecordDto?>;

public record GetInventorySummaryQuery(int? WarehouseId = null) : IRequest<IEnumerable<InventorySummaryDto>>;

public record GetItemTransactionHistoryQuery(int ItemId, int? WarehouseId = null, DateTime? StartDate = null, DateTime? EndDate = null) : IRequest<IEnumerable<ItemTransactionHistoryDto>>;

public record GetLowStockAlertsQuery(int? WarehouseId = null) : IRequest<IEnumerable<LowStockAlertDto>>;

public record GetReserveAlertsQuery(int? WarehouseId = null) : IRequest<IEnumerable<ReserveAlertDto>>;

public record GetCommanderReserveQuery(int? WarehouseId = null, int? ItemId = null) : IRequest<IEnumerable<CommanderReserveDto>>;

public record GetCommanderReserveSummaryQuery(int? WarehouseId = null) : IRequest<IEnumerable<CommanderReserveSummaryDto>>;

public class GetInventoryRecordsQueryHandler : IRequestHandler<GetInventoryRecordsQuery, IEnumerable<InventoryRecordDto>>
{
    private readonly IInventoryService _inventoryService;

    public GetInventoryRecordsQueryHandler(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public async Task<IEnumerable<InventoryRecordDto>> Handle(GetInventoryRecordsQuery request, CancellationToken cancellationToken)
    {
        return await _inventoryService.GetInventoryRecordsAsync(request.WarehouseId, request.ItemId, request.Status, cancellationToken);
    }
}

public class GetInventoryRecordQueryHandler : IRequestHandler<GetInventoryRecordQuery, InventoryRecordDto?>
{
    private readonly IInventoryService _inventoryService;

    public GetInventoryRecordQueryHandler(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public async Task<InventoryRecordDto?> Handle(GetInventoryRecordQuery request, CancellationToken cancellationToken)
    {
        return await _inventoryService.GetInventoryRecordAsync(request.WarehouseId, request.ItemId, cancellationToken);
    }
}

public class GetInventorySummaryQueryHandler : IRequestHandler<GetInventorySummaryQuery, IEnumerable<InventorySummaryDto>>
{
    private readonly IInventoryService _inventoryService;

    public GetInventorySummaryQueryHandler(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public async Task<IEnumerable<InventorySummaryDto>> Handle(GetInventorySummaryQuery request, CancellationToken cancellationToken)
    {
        return await _inventoryService.GetInventorySummaryAsync(request.WarehouseId, cancellationToken);
    }
}

public class GetItemTransactionHistoryQueryHandler : IRequestHandler<GetItemTransactionHistoryQuery, IEnumerable<ItemTransactionHistoryDto>>
{
    private readonly IInventoryService _inventoryService;

    public GetItemTransactionHistoryQueryHandler(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public async Task<IEnumerable<ItemTransactionHistoryDto>> Handle(GetItemTransactionHistoryQuery request, CancellationToken cancellationToken)
    {
        return await _inventoryService.GetItemTransactionHistoryAsync(request.ItemId, request.WarehouseId, request.StartDate, request.EndDate, cancellationToken);
    }
}

public class GetLowStockAlertsQueryHandler : IRequestHandler<GetLowStockAlertsQuery, IEnumerable<LowStockAlertDto>>
{
    private readonly IInventoryService _inventoryService;

    public GetLowStockAlertsQueryHandler(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public async Task<IEnumerable<LowStockAlertDto>> Handle(GetLowStockAlertsQuery request, CancellationToken cancellationToken)
    {
        return await _inventoryService.GetLowStockAlertsAsync(request.WarehouseId, cancellationToken);
    }
}

public class GetReserveAlertsQueryHandler : IRequestHandler<GetReserveAlertsQuery, IEnumerable<ReserveAlertDto>>
{
    private readonly IInventoryService _inventoryService;

    public GetReserveAlertsQueryHandler(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public async Task<IEnumerable<ReserveAlertDto>> Handle(GetReserveAlertsQuery request, CancellationToken cancellationToken)
    {
        return await _inventoryService.GetReserveAlertsAsync(request.WarehouseId, cancellationToken);
    }
}

public class GetCommanderReserveQueryHandler : IRequestHandler<GetCommanderReserveQuery, IEnumerable<CommanderReserveDto>>
{
    private readonly ICommanderReserveService _commanderReserveService;

    public GetCommanderReserveQueryHandler(ICommanderReserveService commanderReserveService)
    {
        _commanderReserveService = commanderReserveService;
    }

    public async Task<IEnumerable<CommanderReserveDto>> Handle(GetCommanderReserveQuery request, CancellationToken cancellationToken)
    {
        return await _commanderReserveService.GetCommanderReserveAsync(request.WarehouseId, request.ItemId, cancellationToken);
    }
}

public class GetCommanderReserveSummaryQueryHandler : IRequestHandler<GetCommanderReserveSummaryQuery, IEnumerable<CommanderReserveSummaryDto>>
{
    private readonly ICommanderReserveService _commanderReserveService;

    public GetCommanderReserveSummaryQueryHandler(ICommanderReserveService commanderReserveService)
    {
        _commanderReserveService = commanderReserveService;
    }

    public async Task<IEnumerable<CommanderReserveSummaryDto>> Handle(GetCommanderReserveSummaryQuery request, CancellationToken cancellationToken)
    {
        return await _commanderReserveService.GetCommanderReserveSummaryAsync(request.WarehouseId, cancellationToken);
    }
}
