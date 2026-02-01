using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EICInventorySystem.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly ISecurityService _securityService;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, ISecurityService securityService, IConfiguration configuration)
    {
        _context = context;
        _securityService = securityService;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.Factory)
            .Include(u => u.Department)
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive, cancellationToken);

        if (user == null || !_securityService.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        var permissions = await GetUserPermissionsAsync(user.Id, cancellationToken);
        var token = _securityService.GenerateJwtToken(user.Id, user.Username, user.Role, permissions);
        var refreshToken = _securityService.GenerateRefreshToken();

        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        user.RecordLogin();
        await _context.SaveChangesAsync(cancellationToken);

        return new LoginResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            User = MapToUserDto(user, permissions)
        };
    }

    public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default)
    {
        var (isValid, userId, username) = _securityService.ValidateJwtToken(request.Token);
        if (!isValid)
        {
            throw new UnauthorizedAccessException("Invalid token");
        }

        var user = await _context.Users
            .Include(u => u.Factory)
            .Include(u => u.Department)
            .FirstOrDefaultAsync(u => u.Id == userId && u.RefreshToken == request.RefreshToken && u.IsActive, cancellationToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        var permissions = await GetUserPermissionsAsync(user.Id, cancellationToken);
        var newToken = _securityService.GenerateJwtToken(user.Id, user.Username, user.Role, permissions);
        var newRefreshToken = _securityService.GenerateRefreshToken();

        user.SetRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
        await _context.SaveChangesAsync(cancellationToken);

        return new LoginResponseDto
        {
            Token = newToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            User = MapToUserDto(user, permissions)
        };
    }

    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto request, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        if (!_securityService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            throw new InvalidOperationException("Current password is incorrect");
        }

        if (request.NewPassword != request.ConfirmPassword)
        {
            throw new InvalidOperationException("New password and confirmation do not match");
        }

        user.UpdatePassword(_securityService.HashPassword(request.NewPassword), user.Id);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto request, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive, cancellationToken);
        if (user == null)
        {
            // Don't reveal that the user doesn't exist
            return true;
        }

        user.SetPasswordResetToken(_securityService.GeneratePasswordResetToken(), DateTime.UtcNow.AddHours(24));
        await _context.SaveChangesAsync(cancellationToken);

        // TODO: Send password reset email
        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordDto request, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException("Invalid request");
        }

        if (!_securityService.ValidatePasswordResetToken(request.Token, user.PasswordResetToken ?? "", user.PasswordResetTokenExpiryTime ?? DateTime.MinValue))
        {
            throw new InvalidOperationException("Invalid or expired reset token");
        }

        if (request.NewPassword != request.ConfirmPassword)
        {
            throw new InvalidOperationException("New password and confirmation do not match");
        }

        user.UpdatePassword(_securityService.HashPassword(request.NewPassword), user.Id);
        user.ClearPasswordResetToken();
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> LogoutAsync(int userId, string token, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user != null)
        {
            user.RevokeRefreshToken();
            await _context.SaveChangesAsync(cancellationToken);
        }
        return true;
    }

    public async Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var (isValid, _, _) = _securityService.ValidateJwtToken(token);
        return await Task.FromResult(isValid);
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.Factory)
            .Include(u => u.Department)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null) return null;

        var permissions = await GetUserPermissionsAsync(userId, cancellationToken);
        return MapToUserDto(user, permissions);
    }

    private async Task<List<string>> GetUserPermissionsAsync(int userId, CancellationToken cancellationToken)
    {
        // TODO: Implement actual permission retrieval from database
        return await Task.FromResult(new List<string>());
    }

    private static UserDto MapToUserDto(Domain.Entities.User user, List<string> permissions)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            FullNameArabic = user.FullNameArabic,
            Role = user.Role,
            RoleArabic = GetRoleArabic(user.Role),
            FactoryId = user.FactoryId,
            FactoryName = user.Factory?.Name,
            FactoryNameArabic = user.Factory?.NameArabic,
            DepartmentId = user.DepartmentId?.ToString(),
            DepartmentName = user.Department?.Name,
            DepartmentNameArabic = user.Department?.NameArabic,
            IsActive = user.IsActive,
            LastLoginAt = user.LastLoginAt,
            ProfileImageUrl = user.ProfileImageUrl,
            Permissions = permissions
        };
    }

    private static string GetRoleArabic(string role) => role switch
    {
        "Admin" => "مدير النظام",
        "Commander" => "القائد",
        "WarehouseManager" => "مدير المخزن",
        "WarehouseStaff" => "موظف المخزن",
        "Requester" => "طالب الصرف",
        _ => role
    };
}
