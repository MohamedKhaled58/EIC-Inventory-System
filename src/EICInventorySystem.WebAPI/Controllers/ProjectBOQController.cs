using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Commands;
using EICInventorySystem.Application.Queries;
using EICInventorySystem.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EICInventorySystem.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectBOQController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProjectBOQController> _logger;

    public ProjectBOQController(IMediator mediator, ILogger<ProjectBOQController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    #region GET Endpoints

    /// <summary>
    /// Get BOQs with optional filters
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<(IEnumerable<ProjectBOQDto> Items, int TotalCount)>> GetBOQs(
        [FromQuery] int? factoryId = null,
        [FromQuery] int? projectId = null,
        [FromQuery] int? warehouseId = null,
        [FromQuery] BOQStatus? status = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetBOQsQuery(factoryId, projectId, warehouseId, status, startDate, endDate, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get specific BOQ by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectBOQDto?>> GetBOQ(int id)
    {
        var query = new GetBOQQuery(id);
        var result = await _mediator.Send(query);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    /// <summary>
    /// Get BOQ by number
    /// </summary>
    [HttpGet("number/{boqNumber}")]
    public async Task<ActionResult<ProjectBOQDto?>> GetBOQByNumber(string boqNumber)
    {
        var query = new GetBOQByNumberQuery(boqNumber);
        var result = await _mediator.Send(query);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    /// <summary>
    /// Get pending BOQs
    /// </summary>
    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<ProjectBOQDto>>> GetPendingBOQs(
        [FromQuery] int? factoryId = null)
    {
        var query = new GetPendingBOQsQuery(factoryId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get BOQs for approval (Factory Commander)
    /// </summary>
    [HttpGet("for-approval")]
    public async Task<ActionResult<IEnumerable<ProjectBOQDto>>> GetBOQsForApproval(
        [FromQuery] int? factoryId = null)
    {
        var query = new GetBOQsForApprovalQuery(factoryId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get BOQs for issuance (Storekeeper)
    /// </summary>
    [HttpGet("for-issuance")]
    public async Task<ActionResult<IEnumerable<ProjectBOQDto>>> GetBOQsForIssuance(
        [FromQuery] int? warehouseId = null)
    {
        var query = new GetBOQsForIssuanceQuery(warehouseId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get partially issued BOQs (for follow-up)
    /// </summary>
    [HttpGet("partially-issued")]
    public async Task<ActionResult<IEnumerable<ProjectBOQDto>>> GetPartiallyIssuedBOQs(
        [FromQuery] int? projectId = null)
    {
        var query = new GetPartiallyIssuedBOQsQuery(projectId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get pending items across all BOQs
    /// </summary>
    [HttpGet("pending-items")]
    public async Task<ActionResult<IEnumerable<PendingBOQItemsDto>>> GetPendingItems(
        [FromQuery] int? projectId = null,
        [FromQuery] int? factoryId = null)
    {
        var query = new GetPendingBOQItemsQuery(projectId, factoryId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get BOQ statistics
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<BOQStatisticsDto>> GetStatistics(
        [FromQuery] int? factoryId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var query = new GetBOQStatisticsQuery(factoryId, startDate, endDate);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    #endregion

    #region POST/PUT/DELETE Endpoints

    /// <summary>
    /// Create new BOQ
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProjectBOQDto>> CreateBOQ([FromBody] CreateProjectBOQDto request)
    {
        var userId = GetUserId();
        var command = new CreateBOQCommand(request, userId);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetBOQ), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update BOQ (draft only)
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ProjectBOQDto>> UpdateBOQ(int id, [FromBody] UpdateProjectBOQDto request)
    {
        var userId = GetUserId();
        var command = new UpdateBOQCommand(request with { Id = id }, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Delete BOQ (draft only)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteBOQ(int id)
    {
        var userId = GetUserId();
        var command = new DeleteBOQCommand(id, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    #endregion

    #region Workflow Endpoints

    /// <summary>
    /// Submit BOQ for approval
    /// </summary>
    [HttpPost("{id}/submit")]
    public async Task<ActionResult<ProjectBOQDto>> SubmitBOQ(int id)
    {
        var userId = GetUserId();
        var command = new SubmitBOQCommand(id, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Approve BOQ (Factory Commander)
    /// </summary>
    [HttpPost("{id}/approve")]
    public async Task<ActionResult<ProjectBOQDto>> ApproveBOQ(int id, [FromBody] ApproveBOQDto request)
    {
        var userId = GetUserId();
        var command = new ApproveBOQCommand(request with { Id = id }, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Reject BOQ
    /// </summary>
    [HttpPost("{id}/reject")]
    public async Task<ActionResult<ProjectBOQDto>> RejectBOQ(int id, [FromBody] RejectBOQDto request)
    {
        var userId = GetUserId();
        var command = new RejectBOQCommand(request with { Id = id }, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Approve Commander Reserve for BOQ
    /// </summary>
    [HttpPost("{id}/approve-reserve")]
    public async Task<ActionResult<ProjectBOQDto>> ApproveCommanderReserve(int id, [FromBody] ApproveCommanderReserveDto request)
    {
        var userId = GetUserId();
        var command = new ApproveCommanderReserveBOQCommand(request with { Id = id }, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Issue BOQ (full or partial)
    /// </summary>
    [HttpPost("{id}/issue")]
    public async Task<ActionResult<ProjectBOQDto>> IssueBOQ(int id, [FromBody] IssueBOQDto request)
    {
        var userId = GetUserId();
        var command = new IssueBOQCommand(request with { Id = id }, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Cancel BOQ
    /// </summary>
    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<bool>> CancelBOQ(int id, [FromBody] CancelBOQRequestDto request)
    {
        var userId = GetUserId();
        var command = new CancelBOQCommand(id, userId, request.Reason, request.ReasonArabic);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    #endregion

    private int GetUserId()
    {
        return int.Parse(User.FindFirst("sub")?.Value ?? "0");
    }
}

public record CancelBOQRequestDto
{
    public string? Reason { get; init; }
    public string? ReasonArabic { get; init; }
}
