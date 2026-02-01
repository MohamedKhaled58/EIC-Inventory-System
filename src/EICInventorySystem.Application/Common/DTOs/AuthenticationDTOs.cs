namespace EICInventorySystem.Application.Common.DTOs;

public record LoginRequestDto
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string? DeviceInfo { get; init; }
    public string? IpAddress { get; init; }
}

public record LoginResponseDto
{
    public string Token { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public UserDto User { get; init; } = null!;
}

public record RefreshTokenRequestDto
{
    public string Token { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
}

public record ChangePasswordDto
{
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
}

public record ForgotPasswordDto
{
    public string Email { get; init; } = string.Empty;
}

public record ResetPasswordDto
{
    public string Email { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
}

public record UserDto
{
    public int Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string FullNameArabic { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string RoleArabic { get; init; } = string.Empty;
    public int? FactoryId { get; init; }
    public string? FactoryName { get; init; }
    public string? FactoryNameArabic { get; init; }
    public string? DepartmentId { get; init; }
    public string? DepartmentName { get; init; }
    public string? DepartmentNameArabic { get; init; }
    public bool IsActive { get; init; }
    public DateTime? LastLoginAt { get; init; }
    public string? ProfileImageUrl { get; init; }
    public List<string> Permissions { get; init; } = new();
}
