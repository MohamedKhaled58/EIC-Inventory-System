namespace EICInventorySystem.Application.Interfaces;

public interface ISecurityService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
    string GenerateJwtToken(int userId, string username, string role, IEnumerable<string> permissions);
    string GenerateRefreshToken();
    (bool IsValid, int UserId, string Username) ValidateJwtToken(string token);
    string GeneratePasswordResetToken();
    bool ValidatePasswordResetToken(string token, string storedToken, DateTime expiryDate);
    string EncryptSensitiveData(string data);
    string DecryptSensitiveData(string encryptedData);
}
