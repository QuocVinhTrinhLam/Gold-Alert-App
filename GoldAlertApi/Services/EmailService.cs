using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;

namespace GoldAlertApi.Services;

public interface IEmailService
{
    Task SendEmailAsync(string subject, string body, string toAddress);
}

public class EmailService : IEmailService
{
    private readonly AppConfig _config;

    public EmailService(IOptions<AppConfig> config)
    {
        _config = config.Value;
    }

    public async Task SendEmailAsync(string subject, string body, string toAddress)
    {
        if (string.IsNullOrEmpty(_config.EmailFrom) || string.IsNullOrEmpty(_config.EmailPassword))
        {
             throw new Exception("Email credentials not configured in appsettings.json");
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Gold Alert System", _config.EmailFrom));
        message.To.Add(new MailboxAddress("", toAddress));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = body
        };
        message.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            // For testing purposes, we can accept all certificates if needed, 
            // but standard Gmail usually has valid certs.
            // client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_config.EmailFrom, _config.EmailPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            
            Console.WriteLine($"Email sent to {toAddress} via MailKit");
        }
    }
}
