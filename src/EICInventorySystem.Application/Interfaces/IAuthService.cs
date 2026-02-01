using EICInventorySystem.Application.Common.DTOs;

namespace EICInventorySystem.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
    Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default);
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto request, CancellationToken cancellationToken = default);
    Task<bool> ForgotPasswordAsync(ForgotPasswordDto request, CancellationToken cancellationToken = default);
    Task<bool> ResetPasswordAsync(ResetPasswordDto request, CancellationToken cancellationToken = default);
    Task<bool> LogoutAsync(int userId, string token, CancellationToken cancellationToken = default);
    Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default);
}
