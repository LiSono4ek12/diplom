namespace Api.DTOs;

public record RegisterDto(string Email, string Password);

public record LoginDto(string Email, string Password);

public record AuthResponseDto(string AccessToken, string RefreshToken, DateTime Expiration);

public record RefreshTokenDto(string AccessToken, string RefreshToken);

public record ForgotPasswordDto(string Email);

public record ResetPasswordDto(string Email, string Token, string NewPassword);