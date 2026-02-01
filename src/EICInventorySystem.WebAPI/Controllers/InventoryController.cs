using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EICInventorySystem.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(IMediator mediator, ILogger<InventoryController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get inventory records with optional filters
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryRecordDto>>> GetInventoryRecords(
        [FromQuery] int? warehouseId = null,
        [FromQuery] int? itemId = null,
        [FromQuery] string? status = null)
    {
        var query = new GetInventoryRecordsQuery(warehouseId, itemId, status);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get specific inventory record
    /// </summary>
    [HttpGet("{warehouseId}/{itemId}")]
    public async Task<ActionResult<InventoryRecordDto?>> GetInventoryRecord(int warehouseId, int itemId)
    {
        var query = new GetInventoryRecordQuery(warehouseId, itemId);
        var result = await _mediator.Send(query);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    /// <summary>
    /// Get inventory summary
    /// </summary>
    [HttpGet("summary")]
    public async Task<ActionResult<IEnumerable<InventorySummaryDto>>> GetInventorySummary(
        [FromQuery] int? warehouseId = null)
    {
        var query = new GetInventorySummaryQuery(warehouseId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get item transaction history
    /// </summary>
    [HttpGet("transactions/{itemId}")]
    public async Task<ActionResult<IEnumerable<ItemTransactionHistoryDto>>> GetItemTransactionHistory(
        int itemId,
        [FromQuery] int? warehouseId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var query = new GetItemTransactionHistoryQuery(itemId, warehouseId, startDate, endDate);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get low stock alerts
    /// </summary>
    [HttpGet("alerts/low-stock")]
    public async Task<ActionResult<IEnumerable<LowStockAlertDto>>> GetLowStockAlerts(
        [FromQuery] int? warehouseId = null)
    {
        var query = new GetLowStockAlertsQuery(warehouseId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get reserve alerts
    /// </summary>
    [HttpGet("alerts/reserve")]
    public async Task<ActionResult<IEnumerable<ReserveAlertDto>>> GetReserveAlerts(
        [FromQuery] int? warehouseId = null)
    {
        var query = new GetReserveAlertsQuery(warehouseId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get Commander's Reserve information
    /// </summary>
    [HttpGet("commander-reserve")]
    public async Task<ActionResult<IEnumerable<CommanderReserveDto>>> GetCommanderReserve(
        [FromQuery] int? warehouseId = null,
        [FromQuery] int? itemId = null)
    {
        var query = new GetCommanderReserveQuery(warehouseId, itemId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get Commander's Reserve summary
    /// </summary>
    [HttpGet("commander-reserve/summary")]
    public async Task<ActionResult<IEnumerable<CommanderReserveSummaryDto>>> GetCommanderReserveSummary(
        [FromQuery] int? warehouseId = null)
    {
        var query = new GetCommanderReserveSummaryQuery(warehouseId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
