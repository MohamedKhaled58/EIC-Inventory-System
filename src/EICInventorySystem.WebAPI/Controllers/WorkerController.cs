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
public class WorkerController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<WorkerController> _logger;

    public WorkerController(IMediator mediator, ILogger<WorkerController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    #region GET Endpoints

    /// <summary>
    /// Get workers with optional filters
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<(IEnumerable<WorkerDto> Items, int TotalCount)>> GetWorkers(
        [FromQuery] int? factoryId = null,
        [FromQuery] int? departmentId = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetWorkersQuery(factoryId, departmentId, isActive, searchTerm, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get specific worker by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<WorkerDto?>> GetWorker(int id)
    {
        var query = new GetWorkerQuery(id);
        var result = await _mediator.Send(query);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    /// <summary>
    /// Get worker by code
    /// </summary>
    [HttpGet("code/{workerCode}")]
    public async Task<ActionResult<WorkerDto?>> GetWorkerByCode(string workerCode)
    {
        var query = new GetWorkerByCodeQuery(workerCode);
        var result = await _mediator.Send(query);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    /// <summary>
    /// Search workers (for autocomplete - Zero Manual Typing)
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<WorkerDto>>> SearchWorkers(
        [FromQuery] string searchTerm,
        [FromQuery] int? factoryId = null,
        [FromQuery] int? departmentId = null,
        [FromQuery] int maxResults = 10)
    {
        var query = new SearchWorkersQuery(searchTerm, factoryId, departmentId, maxResults);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    #endregion

    #region POST/PUT Endpoints

    /// <summary>
    /// Create new worker
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<WorkerDto>> CreateWorker([FromBody] CreateWorkerDto request)
    {
        var userId = GetUserId();
        var command = new CreateWorkerCommand(request, userId);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetWorker), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update worker
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<WorkerDto>> UpdateWorker(int id, [FromBody] UpdateWorkerDto request)
    {
        var userId = GetUserId();
        var command = new UpdateWorkerCommand(request with { Id = id }, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Activate worker
    /// </summary>
    [HttpPost("{id}/activate")]
    public async Task<ActionResult<bool>> ActivateWorker(int id)
    {
        var userId = GetUserId();
        var command = new ActivateWorkerCommand(id, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deactivate worker
    /// </summary>
    [HttpPost("{id}/deactivate")]
    public async Task<ActionResult<bool>> DeactivateWorker(int id)
    {
        var userId = GetUserId();
        var command = new DeactivateWorkerCommand(id, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Transfer worker to another department
    /// </summary>
    [HttpPost("{id}/transfer")]
    public async Task<ActionResult<WorkerDto>> TransferWorker(int id, [FromBody] TransferWorkerRequestDto request)
    {
        var userId = GetUserId();
        var command = new TransferWorkerCommand(id, request.NewDepartmentId, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    #endregion

    private int GetUserId()
    {
        return int.Parse(User.FindFirst("sub")?.Value ?? "0");
    }
}

public record TransferWorkerRequestDto
{
    public int NewDepartmentId { get; init; }
}
