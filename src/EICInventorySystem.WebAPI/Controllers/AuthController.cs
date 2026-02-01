using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Commands;
using EICInventorySystem.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EICInventorySystem.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get current user details
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetMe()
    {
        var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
        var query = new GetMeQuery(userId);
        var result = await _mediator.Send(query);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    /// <summary>
    /// User login endpoint
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var command = new LoginCommand(request, ipAddress);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var command = new RefreshTokenCommand(request);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Change user password
    /// </summary>
    [Authorize]
    [HttpPost("change-password")]
    public async Task<ActionResult<bool>> ChangePassword([FromBody] ChangePasswordDto request)
    {
        var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
        var command = new ChangePasswordCommand(request, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Request password reset
    /// </summary>
    [HttpPost("forgot-password")]
    public async Task<ActionResult<bool>> ForgotPassword([FromBody] ForgotPasswordDto request)
    {
        var command = new ForgotPasswordCommand(request);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Reset password with token
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<ActionResult<bool>> ResetPassword([FromBody] ResetPasswordDto request)
    {
        var command = new ResetPasswordCommand(request);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// User logout
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult<bool>> Logout()
    {
        var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var command = new LogoutCommand(userId, token);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
