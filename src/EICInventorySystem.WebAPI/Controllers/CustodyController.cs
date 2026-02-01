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
public class CustodyController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CustodyController> _logger;

    public CustodyController(IMediator mediator, ILogger<CustodyController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    #region GET Endpoints

    /// <summary>
    /// Get custodies with optional filters
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<(IEnumerable<OperationalCustodyDto> Items, int TotalCount)>> GetCustodies(
        [FromQuery] int? workerId = null,
        [FromQuery] int? itemId = null,
        [FromQuery] int? factoryId = null,
        [FromQuery] int? departmentId = null,
        [FromQuery] int? warehouseId = null,
        [FromQuery] CustodyStatus? status = null,
        [FromQuery] bool? isOverdue = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetCustodiesQuery(workerId, itemId, factoryId, departmentId, warehouseId, status, isOverdue, startDate, endDate, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get specific custody by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<OperationalCustodyDto?>> GetCustody(int id)
    {
        var query = new GetCustodyQuery(id);
        var result = await _mediator.Send(query);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    /// <summary>
    /// Get custody by number
    /// </summary>
    [HttpGet("number/{custodyNumber}")]
    public async Task<ActionResult<OperationalCustodyDto?>> GetCustodyByNumber(string custodyNumber)
    {
        var query = new GetCustodyByNumberQuery(custodyNumber);
        var result = await _mediator.Send(query);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    /// <summary>
    /// Get active custodies for a worker
    /// </summary>
    [HttpGet("worker/{workerId}")]
    public async Task<ActionResult<IEnumerable<OperationalCustodyDto>>> GetWorkerCustodies(int workerId)
    {
        var query = new GetActiveCustodiesByWorkerQuery(workerId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get overdue custodies
    /// </summary>
    [HttpGet("overdue")]
    public async Task<ActionResult<IEnumerable<OperationalCustodyDto>>> GetOverdueCustodies(
        [FromQuery] int maxDays = 30,
        [FromQuery] int? factoryId = null)
    {
        var query = new GetOverdueCustodiesQuery(maxDays, factoryId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get custody aging report for a worker
    /// </summary>
    [HttpGet("aging/{workerId}")]
    public async Task<ActionResult<CustodyAgingReportDto>> GetCustodyAgingReport(int workerId)
    {
        var query = new GetCustodyAgingReportQuery(workerId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get custody statistics
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<CustodyStatisticsDto>> GetStatistics(
        [FromQuery] int? factoryId = null,
        [FromQuery] int? departmentId = null)
    {
        var query = new GetCustodyStatisticsQuery(factoryId, departmentId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get custodies grouped by worker
    /// </summary>
    [HttpGet("by-worker")]
    public async Task<ActionResult<IEnumerable<CustodyByWorkerDto>>> GetCustodiesByWorker(
        [FromQuery] int? departmentId = null,
        [FromQuery] int? factoryId = null)
    {
        var query = new GetCustodiesByWorkerQuery(departmentId, factoryId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    #endregion

    #region POST Endpoints

    /// <summary>
    /// Issue custody to worker
    /// </summary>
    [HttpPost("issue")]
    public async Task<ActionResult<OperationalCustodyDto>> IssueCustody([FromBody] CreateCustodyDto request)
    {
        var userId = GetUserId();
        var command = new IssueCustodyCommand(request, userId);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetCustody), new { id = result.Id }, result);
    }

    /// <summary>
    /// Return custody (full or partial)
    /// </summary>
    [HttpPost("{id}/return")]
    public async Task<ActionResult<OperationalCustodyDto>> ReturnCustody(int id, [FromBody] ReturnCustodyDto request)
    {
        var userId = GetUserId();
        var command = new ReturnCustodyCommand(request with { Id = id }, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Consume custody (item used/consumed)
    /// </summary>
    [HttpPost("{id}/consume")]
    public async Task<ActionResult<OperationalCustodyDto>> ConsumeCustody(int id, [FromBody] ConsumeCustodyDto request)
    {
        var userId = GetUserId();
        var command = new ConsumeCustodyCommand(request with { Id = id }, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Transfer custody to another worker
    /// </summary>
    [HttpPost("{id}/transfer")]
    public async Task<ActionResult<OperationalCustodyDto>> TransferCustody(int id, [FromBody] TransferCustodyDto request)
    {
        var userId = GetUserId();
        var command = new TransferCustodyCommand(request with { Id = id }, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    #endregion

    private int GetUserId()
    {
        return int.Parse(User.FindFirst("sub")?.Value ?? "0");
    }
}
