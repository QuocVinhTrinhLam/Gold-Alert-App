using Microsoft.AspNetCore.Mvc;
using GoldAlertApi.Services;

namespace GoldAlertApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GoldPriceController : ControllerBase
{
    private readonly IGoldPriceService _goldPriceService;

    public GoldPriceController(IGoldPriceService goldPriceService)
    {
        _goldPriceService = goldPriceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrentPrice()
    {
        var price = await _goldPriceService.GetSjcGoldSellPriceAsync();
        
        if (price.HasValue)
        {
            return Ok(new 
            { 
                price = price.Value,
                timestamp = DateTime.Now
            });
        }
        
        return NotFound("Could not fetch gold price");
    }
}
