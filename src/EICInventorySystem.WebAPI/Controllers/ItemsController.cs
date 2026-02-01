using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EICInventorySystem.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ItemsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ItemsController> _logger;

    public ItemsController(IMediator mediator, ILogger<ItemsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all items with optional filters
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetItems(
        [FromQuery] int? categoryId = null,
        [FromQuery] string? type = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 100)
    {
        try
        {
            var query = new GetItemsQuery(categoryId, type, isActive, pageNumber, pageSize);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting items");
            return StatusCode(500, new { message = "Error fetching items", error = ex.Message });
        }
    }

    /// <summary>
    /// Get specific item by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> GetItem(int id)
    {
        try
        {
            var query = new GetItemByIdQuery(id);
            var result = await _mediator.Send(query);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting item {Id}", id);
            return StatusCode(500, new { message = "Error fetching item", error = ex.Message });
        }
    }

    /// <summary>
    /// Search items
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ItemDto>>> SearchItems(
        [FromQuery] string? query = null,
        [FromQuery] int? warehouseId = null,
        [FromQuery] int? categoryId = null)
    {
        try
        {
            var searchQuery = new SearchItemsQuery(query, warehouseId, categoryId);
            var result = await _mediator.Send(searchQuery);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching items");
            return StatusCode(500, new { message = "Error searching items", error = ex.Message });
        }
    }

    /// <summary>
    /// Get item categories
    /// </summary>
    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<ItemCategoryDto>>> GetCategories()
    {
        try
        {
            var query = new GetItemCategoriesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting item categories");
            return StatusCode(500, new { message = "Error fetching categories", error = ex.Message });
        }
    }

    /// <summary>
    /// Get items with inventory in specific warehouse
    /// </summary>
    [HttpGet("warehouse/{warehouseId}")]
    public async Task<ActionResult<IEnumerable<ItemWithInventoryDto>>> GetItemsWithInventory(int warehouseId)
    {
        try
        {
            var query = new GetItemsWithInventoryQuery(warehouseId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting items with inventory for warehouse {WarehouseId}", warehouseId);
            return StatusCode(500, new { message = "Error fetching items with inventory", error = ex.Message });
        }
    }
}
