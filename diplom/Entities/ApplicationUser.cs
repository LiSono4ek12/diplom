namespace Api.Entities;

public class ApplicationUser
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    // Поля для Refresh Token
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    // Поля для сброса пароля через Email
    public string? PasswordResetToken { get; set; }
    public DateTime? ResetTokenExpiryTime { get; set; }
}