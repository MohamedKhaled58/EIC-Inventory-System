using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Commands;
using EICInventorySystem.Application.Queries;
using EICInventorySystem.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EICInventorySystem.WebAPI.Controllers;

[ApiController]
[Route("api/requisitions")]
[Authorize]
public class RequisitionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<RequisitionController> _logger;

    public RequisitionController(IMediator mediator, ILogger<RequisitionController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get requisitions with optional filters
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<(IEnumerable<RequisitionDto> Items, int TotalCount)>> GetRequisitions(
        [FromQuery] int? factoryId = null,
        [FromQuery] int? warehouseId = null,
        [FromQuery] int? departmentId = null,
        [FromQuery] int? projectId = null,
        [FromQuery] RequisitionStatus? status = null,
        [FromQuery] RequisitionType? type = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetRequisitionsQuery(factoryId, warehouseId, departmentId, projectId, status, type, startDate, endDate, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        
        return Ok(new
        {
            Items = result.Items,
            TotalCount = result.TotalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize),
            HasPreviousPage = pageNumber > 1,
            HasNextPage = pageNumber < (int)Math.Ceiling(result.TotalCount / (double)pageSize)
        });
    }

    /// <summary>
    /// Get specific requisition by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<RequisitionDto?>> GetRequisition(int id)
    {
        var query = new GetRequisitionQuery(id);
        var result = await _mediator.Send(query);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    /// <summary>
    /// Get requisition by number
    /// </summary>
    [HttpGet("number/{requisitionNumber}")]
    public async Task<ActionResult<RequisitionDto?>> GetRequisitionByNumber(string requisitionNumber)
    {
        var query = new GetRequisitionByNumberQuery(requisitionNumber);
        var result = await _mediator.Send(query);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    /// <summary>
    /// Get pending requisitions
    /// </summary>
    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<RequisitionDto>>> GetPendingRequisitions(
        [FromQuery] int? warehouseId = null)
    {
        var query = new GetPendingRequisitionsQuery(warehouseId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get requisitions for approval
    /// </summary>
    [HttpGet("for-approval")]
    public async Task<ActionResult<IEnumerable<RequisitionDto>>> GetRequisitionsForApproval(
        [FromQuery] int? warehouseId = null)
    {
        var query = new GetRequisitionsForApprovalQuery(warehouseId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get requisitions for issuance
    /// </summary>
    [HttpGet("for-issuance")]
    public async Task<ActionResult<IEnumerable<RequisitionDto>>> GetRequisitionsForIssuance(
        [FromQuery] int? warehouseId = null)
    {
        var query = new GetRequisitionsForIssuanceQuery(warehouseId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get requisitions for receiving
    /// </summary>
    [HttpGet("for-receiving")]
    public async Task<ActionResult<IEnumerable<RequisitionDto>>> GetRequisitionsForReceiving(
        [FromQuery] int? departmentId = null,
        [FromQuery] int? projectId = null)
    {
        var query = new GetRequisitionsForReceivingQuery(departmentId, projectId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get requisition statistics
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<RequisitionStatisticsDto>> GetRequisitionStatistics(
        [FromQuery] int? factoryId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var query = new GetRequisitionStatisticsQuery(factoryId, startDate, endDate);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create new requisition
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<RequisitionDto>> CreateRequisition([FromBody] CreateRequisitionDto request)
    {
        var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
        var command = new CreateRequisitionCommand(request, userId);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetRequisition), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update requisition
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<RequisitionDto>> UpdateRequisition(int id, [FromBody] UpdateRequisitionDto request)
    {
        var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
        var command = new UpdateRequisitionCommand(request with { Id = id }, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Approve requisition
    /// </summary>
    [HttpPost("{id}/approve")]
    public async Task<ActionResult<RequisitionDto>> ApproveRequisition(int id, [FromBody] ApproveRequisitionDto request)
    {
        var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
        var command = new ApproveRequisitionCommand(request with { Id = id }, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Reject requisition
    /// </summary>
    [HttpPost("{id}/reject")]
    public async Task<ActionResult<RequisitionDto>> RejectRequisition(int id, [FromBody] RejectRequisitionDto request)
    {
        var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
        var command = new RejectRequisitionCommand(request with { Id = id }, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Issue requisition
    /// </summary>
    [HttpPost("{id}/issue")]
    public async Task<ActionResult<RequisitionDto>> IssueRequisition(int id, [FromBody] IssueRequisitionDto request)
    {
        var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
        var command = new IssueRequisitionCommand(request with { Id = id }, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Receive requisition
    /// </summary>
    [HttpPost("{id}/receive")]
    public async Task<ActionResult<RequisitionDto>> ReceiveRequisition(int id, [FromBody] ReceiveRequisitionDto request)
    {
        var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
        var command = new ReceiveRequisitionCommand(request with { Id = id }, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Cancel requisition
    /// </summary>
    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<bool>> CancelRequisition(int id, [FromBody] CancelRequisitionRequestDto request)
    {
        var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
        var command = new CancelRequisitionCommand(id, userId, request.Reason);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}

public record CancelRequisitionRequestDto
{
    public string Reason { get; init; } = string.Empty;
}
