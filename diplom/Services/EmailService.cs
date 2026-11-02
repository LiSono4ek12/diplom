using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Api.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken)
    {
        var senderName = _config["SmtpSettings:SenderName"] ?? "Auth Web API";
        var senderEmail = _config["SmtpSettings:SenderEmail"] ?? "noreply@authapi.com";
        var host = _config["SmtpSettings:Host"] ?? "sandbox.smtp.mailtrap.io";
        var port = int.Parse(_config["SmtpSettings:Port"] ?? "2525");
        var username = _config["SmtpSettings:Username"] ?? string.Empty;
        var password = _config["SmtpSettings:Password"] ?? string.Empty;

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(senderName, senderEmail));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = "Инструкция по сбросу пароля";

        var resetLink = $"https://localhost:7186/api/account/reset-password?email={Uri.EscapeDataString(toEmail)}&token={Uri.EscapeDataString(resetToken)}";

        email.Body = new TextPart("html")
        {
            Text = $@"
                <h2>Сброс пароля</h2>
                <p>Вы запросили сброс пароля на сайте.</p>
                <p>Скопируйте токен восстановления для Swagger:</p>
                <p><b>{resetToken}</b></p>
                <p>Или перейдите по ссылке: <a href='{resetLink}'>{resetLink}</a></p>"
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(username, password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}