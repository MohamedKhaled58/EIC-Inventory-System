using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Queries;
using EICInventorySystem.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EICInventorySystem.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransfersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TransfersController> _logger;

    public TransfersController(IMediator mediator, ILogger<TransfersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all transfers with optional filters
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransferDto>>> GetTransfers(
        [FromQuery] int? fromWarehouseId = null,
        [FromQuery] int? toWarehouseId = null,
        [FromQuery] string? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetTransfersQuery(fromWarehouseId, toWarehouseId, status, pageNumber, pageSize);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transfers");
            return StatusCode(500, new { message = "Error fetching transfers", error = ex.Message });
        }
    }

    /// <summary>
    /// Get specific transfer by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TransferDto>> GetTransfer(int id)
    {
        try
        {
            var query = new GetTransferByIdQuery(id);
            var result = await _mediator.Send(query);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transfer {Id}", id);
            return StatusCode(500, new { message = "Error fetching transfer", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new transfer request
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TransferDto>> CreateTransfer([FromBody] CreateTransferDto request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            var command = new CreateTransferCommand(request, userId);
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetTransfer), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating transfer");
            return StatusCode(500, new { message = "Error creating transfer", error = ex.Message });
        }
    }

    /// <summary>
    /// Approve transfer
    /// </summary>
    [HttpPost("{id}/approve")]
    public async Task<ActionResult<TransferDto>> ApproveTransfer(int id, [FromBody] ApproveTransferDto? request = null)
    {
        try
        {
            var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            var command = new ApproveTransferCommand(id, request?.Notes, userId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving transfer {Id}", id);
            return StatusCode(500, new { message = "Error approving transfer", error = ex.Message });
        }
    }

    /// <summary>
    /// Reject transfer
    /// </summary>
    [HttpPost("{id}/reject")]
    public async Task<ActionResult<TransferDto>> RejectTransfer(int id, [FromBody] RejectTransferDto request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            var command = new RejectTransferCommand(id, request.Reason, userId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting transfer {Id}", id);
            return StatusCode(500, new { message = "Error rejecting transfer", error = ex.Message });
        }
    }

    /// <summary>
    /// Ship transfer (start transit)
    /// </summary>
    [HttpPost("{id}/ship")]
    public async Task<ActionResult<TransferDto>> ShipTransfer(int id)
    {
        try
        {
            var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            var command = new ShipTransferCommand(id, userId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error shipping transfer {Id}", id);
            return StatusCode(500, new { message = "Error shipping transfer", error = ex.Message });
        }
    }

    /// <summary>
    /// Complete transfer (receive items)
    /// </summary>
    [HttpPost("{id}/receive")]
    public async Task<ActionResult<TransferDto>> ReceiveTransfer(int id, [FromBody] ReceiveTransferDto? request = null)
    {
        try
        {
            var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            var command = new ReceiveTransferCommand(id, request?.Notes, userId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error receiving transfer {Id}", id);
            return StatusCode(500, new { message = "Error receiving transfer", error = ex.Message });
        }
    }
}

public record ApproveTransferDto
{
    public string? Notes { get; init; }
}

public record RejectTransferDto
{
    public string Reason { get; init; } = string.Empty;
}

public record ReceiveTransferDto
{
    public string? Notes { get; init; }
}
