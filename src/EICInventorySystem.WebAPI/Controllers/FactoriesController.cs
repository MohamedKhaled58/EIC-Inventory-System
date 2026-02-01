using EICInventorySystem.Application.Commands;
using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EICInventorySystem.WebAPI.Controllers;

[ApiController]
[Route("api/factories")]
[Authorize]
public class FactoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public FactoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FactoryDto>>> GetFactories([FromQuery] bool? isActive = null)
    {
        var query = new GetFactoriesQuery(isActive);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FactoryDto>> GetFactory(int id)
    {
        var query = new GetFactoryByIdQuery(id);
        var result = await _mediator.Send(query);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "ComplexCommander,FactoryCommander")]
    public async Task<ActionResult<FactoryDto>> CreateFactory([FromBody] CreateFactoryCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetFactory), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ComplexCommander,FactoryCommander")]
    public async Task<ActionResult<FactoryDto>> UpdateFactory(int id, [FromBody] UpdateFactoryCommand command)
    {
        if (id != command.Id) return BadRequest();
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
