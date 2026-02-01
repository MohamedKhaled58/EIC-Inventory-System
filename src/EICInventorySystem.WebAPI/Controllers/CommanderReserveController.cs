using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Commands;
using EICInventorySystem.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EICInventorySystem.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommanderReserveController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CommanderReserveController> _logger;

    public CommanderReserveController(IMediator mediator, ILogger<CommanderReserveController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get Commander's Reserve information
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommanderReserveDto>>> GetCommanderReserve(
        [FromQuery] int? warehouseId = null,
        [FromQuery] int? itemId = null)
    {
        var query = new GetCommanderReserveQuery(warehouseId, itemId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get Commander's Reserve items
    /// </summary>
    [HttpGet("items")]
    public async Task<ActionResult<IEnumerable<CommanderReserveDto>>> GetCommanderReserveItems(
        [FromQuery] int? warehouseId = null)
    {
        try
        {
            var query = new GetCommanderReserveQuery(warehouseId, null);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting commander reserve items");
            return Ok(new List<CommanderReserveDto>());
        }
    }

    /// <summary>
    /// Get pending Commander's Reserve requests
    /// </summary>
    [HttpGet("requests/pending")]
    public async Task<ActionResult<IEnumerable<CommanderReserveRequestDto>>> GetPendingRequests()
    {
        try
        {
            var query = new GetPendingCommanderReserveRequestsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending commander reserve requests");
            return Ok(new List<CommanderReserveRequestDto>());
        }
    }

    /// <summary>
    /// Get Commander's Reserve transactions
    /// </summary>
    [HttpGet("transactions")]
    public async Task<ActionResult<IEnumerable<object>>> GetTransactions(
        [FromQuery] int? warehouseId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            // Return empty list for now - transactions would need a specific query
            return Ok(new List<object>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting commander reserve transactions");
            return Ok(new List<object>());
        }
    }

    /// <summary>
    /// Get Commander's Reserve summary
    /// </summary>
    [HttpGet("summary")]
    public async Task<ActionResult<IEnumerable<CommanderReserveSummaryDto>>> GetCommanderReserveSummary(
        [FromQuery] int? warehouseId = null)
    {
        var query = new GetCommanderReserveSummaryQuery(warehouseId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create Commander's Reserve request
    /// </summary>
    [HttpPost("requests")]
    public async Task<ActionResult<CommanderReserveRequestDto>> CreateCommanderReserveRequest(
        [FromBody] CreateCommanderReserveRequestDto request)
    {
        var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
        var command = new CreateCommanderReserveRequestCommand(request, userId);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetCommanderReserve), new { id = result.Id }, result);
    }

    /// <summary>
    /// Approve Commander's Reserve request
    /// </summary>
    [HttpPost("requests/{id}/approve")]
    [Authorize(Policy = "AccessCommanderReserve")]
    public async Task<ActionResult<CommanderReserveRequestDto>> ApproveCommanderReserveRequest(
        int id,
        [FromBody] ApproveCommanderReserveRequestDto request)
    {
        var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
        var command = new ApproveCommanderReserveRequestCommand(request with { Id = id }, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Reject Commander's Reserve request
    /// </summary>
    [HttpPost("requests/{id}/reject")]
    [Authorize(Policy = "AccessCommanderReserve")]
    public async Task<ActionResult<CommanderReserveRequestDto>> RejectCommanderReserveRequest(
        int id,
        [FromBody] RejectCommanderReserveRequestDto request)
    {
        var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
        var command = new RejectCommanderReserveRequestCommand(request with { Id = id }, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Release Commander's Reserve
    /// </summary>
    [HttpPost("release")]
    [Authorize(Policy = "AccessCommanderReserve")]
    public async Task<ActionResult<CommanderReserveReleaseDto>> ReleaseCommanderReserve(
        [FromBody] ReleaseCommanderReserveDto request)
    {
        var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
        var command = new ReleaseCommanderReserveCommand(request, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Allocate Commander's Reserve
    /// </summary>
    [HttpPost("allocate")]
    [Authorize(Policy = "AccessCommanderReserve")]
    public async Task<ActionResult<bool>> AllocateCommanderReserve(
        [FromBody] AllocateCommanderReserveRequestDto request)
    {
        var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
        var command = new AllocateCommanderReserveCommand(request.ItemId, request.WarehouseId, request.Quantity, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Adjust Commander's Reserve
    /// </summary>
    [HttpPost("adjust")]
    [Authorize(Policy = "AccessCommanderReserve")]
    public async Task<ActionResult<bool>> AdjustCommanderReserve(
        [FromBody] AdjustCommanderReserveRequestDto request)
    {
        var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
        var command = new AdjustCommanderReserveCommand(request.ItemId, request.WarehouseId, request.NewReserveQuantity, request.Reason, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}

public record AllocateCommanderReserveRequestDto
{
    public int ItemId { get; init; }
    public int WarehouseId { get; init; }
    public decimal Quantity { get; init; }
}

public record AdjustCommanderReserveRequestDto
{
    public int ItemId { get; init; }
    public int WarehouseId { get; init; }
    public decimal NewReserveQuantity { get; init; }
    public string Reason { get; init; } = string.Empty;
}
