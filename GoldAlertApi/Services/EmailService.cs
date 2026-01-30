using System.Net;
using System.Net.Mail;
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
            Console.WriteLine("Email credentials not configured. Skipping email.");
            return;
        }

        try
        {
            using (var message = new MailMessage())
            {
                message.From = new MailAddress(_config.EmailFrom);
                message.To.Add(toAddress);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = false;

                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_config.EmailFrom, _config.EmailPassword);

                    await client.SendMailAsync(message);
                    Console.WriteLine($"Email sent to {toAddress}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }
}
