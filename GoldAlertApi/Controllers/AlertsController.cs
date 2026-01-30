using GoldAlertApi.Models;
using GoldAlertApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoldAlertApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertsController : ControllerBase
{
    private readonly IAlertStore _store;

    public AlertsController(IAlertStore store)
    {
        _store = store;
    }

    [HttpGet]
    public IActionResult GetAlerts()
    {
        return Ok(_store.GetAll());
    }

    [HttpPost]
    public IActionResult CreateAlert([FromBody] CreateAlertRequest request)
    {
        if (string.IsNullOrEmpty(request.Condition) || request.TargetPrice <= 0)
        {
            return BadRequest(new { message = "Invalid input" });
        }

        var alert = _store.Add(request);
        return CreatedAtAction(nameof(GetAlerts), new { id = alert.Id }, alert);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteAlert(int id)
    {
        _store.Delete(id);
        return NoContent();
    }
}
