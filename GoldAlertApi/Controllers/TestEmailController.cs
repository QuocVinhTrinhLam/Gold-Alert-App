using Microsoft.AspNetCore.Mvc;
using GoldAlertApi.Services;

namespace GoldAlertApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestEmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public TestEmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpGet]
    public async Task<IActionResult> TestEmail([FromQuery] string toEmail)
    {
        try
        {
            await _emailService.SendEmailAsync(
                "Gold Alert Test Email",
                "<h1>Test Email</h1><p>If you see this, email sending is working!</p>",
                toEmail
            );
            return Ok($"Email sent to {toEmail}");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error sending email: {ex.Message}");
        }
    }
}
