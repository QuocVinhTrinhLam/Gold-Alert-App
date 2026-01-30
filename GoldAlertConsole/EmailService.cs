using System.Net;
using System.Net.Mail;


namespace GoldAlertConsole;

public class EmailService
{
    private readonly AppConfig _config;

    public EmailService(AppConfig config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string subject, string body)
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
                message.To.Add(_config.EmailTo);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = false;

                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_config.EmailFrom, _config.EmailPassword);

                    await client.SendMailAsync(message);
                    Console.WriteLine($"Email sent to {_config.EmailTo}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            if (ex.InnerException != null)
            {
                 Console.WriteLine($"Inner: {ex.InnerException.Message}");
            }
        }
    }
}
