using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using MediatR;

namespace EICInventorySystem.Application.Commands;

public record LoginCommand(LoginRequestDto Request, string IpAddress) : IRequest<LoginResponseDto>;

public record RefreshTokenCommand(RefreshTokenRequestDto Request) : IRequest<LoginResponseDto>;

public record ChangePasswordCommand(ChangePasswordDto Request, int UserId) : IRequest<bool>;

public record ForgotPasswordCommand(ForgotPasswordDto Request) : IRequest<bool>;

public record ResetPasswordCommand(ResetPasswordDto Request) : IRequest<bool>;

public record LogoutCommand(int UserId, string Token) : IRequest<bool>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IAuthService _authService;
    private readonly IAuditService _auditService;

    public LoginCommandHandler(IAuthService authService, IAuditService auditService)
    {
        _authService = authService;
        _auditService = auditService;
    }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request.Request, cancellationToken);

        // Log successful login
        await _auditService.LogActionAsync(
            userId: result.User.Id,
            action: "Login",
            entityType: "User",
            entityId: result.User.Id.ToString(),
            description: $"User {result.User.Username} logged in successfully",
            ipAddress: request.IpAddress,
            cancellationToken: cancellationToken);

        return result;
    }
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResponseDto>
{
    private readonly IAuthService _authService;

    public RefreshTokenCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<LoginResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await _authService.RefreshTokenAsync(request.Request, cancellationToken);
    }
}

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
{
    private readonly IAuthService _authService;
    private readonly IAuditService _auditService;

    public ChangePasswordCommandHandler(IAuthService authService, IAuditService auditService)
    {
        _authService = authService;
        _auditService = auditService;
    }

    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.ChangePasswordAsync(request.UserId, request.Request, cancellationToken);

        if (result)
        {
            await _auditService.LogActionAsync(
                userId: request.UserId,
                action: "PasswordChange",
                entityType: "User",
                entityId: request.UserId.ToString(),
                description: "User changed password",
                cancellationToken: cancellationToken);
        }

        return result;
    }
}

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, bool>
{
    private readonly IAuthService _authService;

    public ForgotPasswordCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        return await _authService.ForgotPasswordAsync(request.Request, cancellationToken);
    }
}

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, bool>
{
    private readonly IAuthService _authService;

    public ResetPasswordCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        return await _authService.ResetPasswordAsync(request.Request, cancellationToken);
    }
}

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
{
    private readonly IAuthService _authService;
    private readonly IAuditService _auditService;

    public LogoutCommandHandler(IAuthService authService, IAuditService auditService)
    {
        _authService = authService;
        _auditService = auditService;
    }

    public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.LogoutAsync(request.UserId, request.Token, cancellationToken);

        if (result)
        {
            await _auditService.LogActionAsync(
                userId: request.UserId,
                action: "Logout",
                entityType: "User",
                entityId: request.UserId.ToString(),
                description: "User logged out",
                cancellationToken: cancellationToken);
        }

        return result;
    }
}

public record GetMeQuery(int UserId) : IRequest<UserDto?>;

public class GetMeQueryHandler : IRequestHandler<GetMeQuery, UserDto?>
{
    private readonly IAuthService _authService;

    public GetMeQueryHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<UserDto?> Handle(GetMeQuery request, CancellationToken cancellationToken)
    {
        return await _authService.GetUserByIdAsync(request.UserId, cancellationToken);
    }
}
