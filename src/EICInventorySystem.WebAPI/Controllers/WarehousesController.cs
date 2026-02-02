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
public class WarehousesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<WarehousesController> _logger;

    public WarehousesController(IMediator mediator, ILogger<WarehousesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all warehouses
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WarehouseDto>>> GetWarehouses(
        [FromQuery] int? factoryId = null,
        [FromQuery] string? type = null,
        [FromQuery] bool? isActive = null)
    {
        try
        {
            var query = new GetWarehousesQuery(factoryId, type, isActive);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting warehouses");
            return StatusCode(500, new { message = "Error fetching warehouses", error = ex.Message });
        }
    }

    /// <summary>
    /// Get specific warehouse by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<WarehouseDto>> GetWarehouse(int id)
    {
        try
        {
            var query = new GetWarehouseByIdQuery(id);
            var result = await _mediator.Send(query);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting warehouse {Id}", id);
            return StatusCode(500, new { message = "Error fetching warehouse", error = ex.Message });
        }
    }

    /// <summary>
    /// Get central warehouses only
    /// </summary>
    [HttpGet("central")]
    public async Task<ActionResult<IEnumerable<WarehouseDto>>> GetCentralWarehouses()
    {
        try
        {
            var query = new GetWarehousesQuery(null, "Central", true);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting central warehouses");
            return StatusCode(500, new { message = "Error fetching central warehouses", error = ex.Message });
        }
    }

    /// <summary>
    /// Get factory warehouses
    /// </summary>
    [HttpGet("factory/{factoryId}")]
    public async Task<ActionResult<IEnumerable<WarehouseDto>>> GetFactoryWarehouses(int factoryId)
    {
        try
        {
            var query = new GetWarehousesQuery(factoryId, "Factory", true);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting factory warehouses for factory {FactoryId}", factoryId);
            return StatusCode(500, new { message = "Error fetching factory warehouses", error = ex.Message });
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<WarehouseDto>>> SearchWarehouses(
        [FromQuery] string? query = null,
        [FromQuery] int? factoryId = null)
    {
        try
        {
            var searchQuery = new SearchWarehousesQuery(query, factoryId);
            var result = await _mediator.Send(searchQuery);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching warehouses");
            return StatusCode(500, new { message = "Error searching warehouses", error = ex.Message });
        }
    }

    /// <summary>
    /// Create a new warehouse
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "ComplexCommander,FactoryCommander")]
    public async Task<ActionResult<WarehouseDto>> CreateWarehouse([FromBody] CreateWarehouseCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetWarehouse), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating warehouse");
            return StatusCode(500, new { message = "Error creating warehouse", error = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing warehouse
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "ComplexCommander,FactoryCommander")]
    public async Task<ActionResult<WarehouseDto>> UpdateWarehouse(int id, [FromBody] UpdateWarehouseCommand command)
    {
        if (id != command.Id) return BadRequest();

        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating warehouse {Id}", id);
            return StatusCode(500, new { message = "Error updating warehouse", error = ex.Message });
        }
    }
}
