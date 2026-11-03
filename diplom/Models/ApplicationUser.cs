namespace diplom.Models;

public class ApplicationUser
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    // Поля для профиля пользователя
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    // Токены авторизации и сброса
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? ResetTokenExpiryTime { get; set; }
}