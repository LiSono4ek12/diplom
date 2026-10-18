namespace Api.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendPasswordResetEmailAsync(string toEmail, string resetToken)
    {
        var resetLink = $"https://localhost:7000/reset-password?email={Uri.EscapeDataString(toEmail)}&token={Uri.EscapeDataString(resetToken)}";

        _logger.LogInformation("=========================================");
        _logger.LogInformation("Ссылка для сброса пароля для {Email}:", toEmail);
        _logger.LogInformation("{Link}", resetLink);
        _logger.LogInformation("=========================================");

        return Task.CompletedTask;
    }
}